// Player.cs (Modified)
using UnityEngine;
using System;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.IO;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Player : Agent
{
    private string logFilePath;

    private void Start()
    {
        logFilePath = Path.Combine(Application.persistentDataPath, "agent_log.txt");
        damageZone = FindFirstObjectByType<DamageZone>();
    }

    private void LogToFile(string message)
    {
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine(message);
        }
    }

    // Movement Parameters
    [Header("Movement Settings")]
    public float moveSpeed = 4f;
    public float rotationSpeed = 360f;

    // Health Parameters
    [Header("Health Settings")]
    public int maxHealth = 100;
    [HideInInspector]
    public int currentHealth;

    [HideInInspector]
    public DamageZone damageZone;

    // Team Settings
    [Header("Team Settings")]
    public Team playerTeam = Team.Red; // Default to Red

    // Shooting Settings
    [Header("Shooting Settings")]
    public GameObject bulletPrefab; // Assign via Inspector
    public Transform shootingPoint; // Assign via Inspector
    public float shootCooldown = 1f; // Default cooldown
    public float bulletSpeed = 10f; // Default bullet speed
    public int bulletDamage = 20; // Default bullet damage

    // Ray Perception Settings
    [Header("Ray Perception Settings")]
    public float rayDistance = 20f;
    public float rayAngle = 90f;
    public int rayCount = 8;

    private float shootTimer = 0f;

    // Events
    public event Action OnHealthChanged;
    public event Action OnDeath;

    // Components
    protected Rigidbody2D rb;

    // Initialization
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        // Assign layer based on team
        if (playerTeam == Team.Red)
        {
            gameObject.layer = LayerMask.NameToLayer("RedTeam");
        }
        else if (playerTeam == Team.Blue)
        {   
            gameObject.layer = LayerMask.NameToLayer("BlueTeam");
        }

        // Find ShootingPoint if not assigned
        if (shootingPoint == null)
        {
            shootingPoint = transform.Find("ShootingPoint");
            if (shootingPoint == null)
            {
                Debug.LogError($"{gameObject.name}: No ShootingPoint found.");
            }
        }
    }

    public override void OnEpisodeBegin()
    {
        currentHealth = maxHealth;
        shootTimer = 0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        LogToFile($"CollectObservations called on {gameObject.name}");
        // Base observations
        sensor.AddObservation(transform.position); // 2 values (x,y)
        sensor.AddObservation(transform.rotation.eulerAngles.z); // 1 value
        sensor.AddObservation(currentHealth / (float)maxHealth); // 1 value
        sensor.AddObservation(shootTimer <= 0f ? 1.0f : 0.0f); // 1 value
        sensor.AddObservation(damageZone.currentRadius);

        // Ray-based observations
        float startAngle = -rayAngle / 2f;
        float angleIncrement = rayAngle / (rayCount - 1);
        int enemyLayerMask = playerTeam == Team.Red ? 
            LayerMask.GetMask("BlueTeam") : 
            LayerMask.GetMask("RedTeam");
        int wallLayerMask = LayerMask.GetMask("Walls");
        int combinedMask = enemyLayerMask | wallLayerMask;

        for (int i = 0; i < rayCount; i++)
        {
            float currentAngle = startAngle + (angleIncrement * i);
            Vector2 direction = Quaternion.Euler(0, 0, currentAngle) * transform.up;
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayDistance, combinedMask);
            
            // Draw debug rays - Green for no hit, Red for hit
            Color rayColor = hit.collider != null ? Color.red : Color.green;
            Debug.DrawRay(transform.position, direction * rayDistance, rayColor);
            
            // If we hit an enemy, add their health and direction
            if (hit.collider != null)
            {
                LogToFile($"{gameObject.name} sees {hit.collider.gameObject.layer}");
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Walls"))
                {
                    sensor.AddObservation((Vector2)hit.collider.transform.position); // location
                    sensor.AddObservation(new Vector3(1.0f, 0.0f, 0.0f)); // wall
                }
                else
                {
                    Player enemyPlayer = hit.collider.GetComponent<Player>();
                    if (enemyPlayer != null)
                    {
                        sensor.AddObservation((Vector2)hit.collider.transform.position); // location
                        sensor.AddObservation(new Vector3(0.0f, 0.0f, 1.0f)); // enemy
                    }
                    else
                    {
                        sensor.AddObservation(Vector2.zero); // location
                        sensor.AddObservation(new Vector3(0.0f, 1.0f, 0.0f)); // nothing
                    }
                }
            }
            else
            {
                // no hit
                sensor.AddObservation(Vector2.zero); // location
                sensor.AddObservation(new Vector3(0.0f, 1.0f, 0.0f)); // nothing
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Get discrete actions
        int moveAction = actions.DiscreteActions[0];
        int turnAction = actions.DiscreteActions[1];

        // Handle movement
        float moveInput = 0f;
        if (moveAction == 1)
        {
            moveInput = 1f; // Move forward
        }

        // Handle turning
        float rotationAmount = 0f;
        if (turnAction == 1)
        {
            rotationAmount = rotationSpeed * Time.fixedDeltaTime; // Turn right
        }
        else if (turnAction == 2)
        {
            rotationAmount = -rotationSpeed * Time.fixedDeltaTime; // Turn left
        }

        rb.MoveRotation(rb.rotation + rotationAmount);

        Vector2 direction = transform.up;
        Vector2 newPosition = rb.position + direction * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

        // Handle shooting (discrete action)
        if (actions.DiscreteActions[2] == 1 && shootTimer <= 0f)
        {
            FireBullet();
        }

        // Update shooting timer
        if (shootTimer > 0f)
        {
            shootTimer -= Time.fixedDeltaTime;
        }
    }

    public void AssignRedTeam()
    {
        playerTeam = Team.Red;
        gameObject.layer = LayerMask.NameToLayer("RedTeam");

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }

        Transform barrel = transform.Find("Barrel");
        if (barrel != null)
        {
            SpriteRenderer barrelRenderer = barrel.GetComponent<SpriteRenderer>();
            if (barrelRenderer != null)
            {
                barrelRenderer.color = Color.red;
            }
        }
    }

    public void AssignBlueTeam()
    {
        playerTeam = Team.Blue;
        gameObject.layer = LayerMask.NameToLayer("BlueTeam");

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.blue;
        }

        Transform barrel = transform.Find("Barrel");
        if (barrel != null)
        {
            SpriteRenderer barrelRenderer = barrel.GetComponent<SpriteRenderer>();
            if (barrelRenderer != null)
            {
                barrelRenderer.color = Color.blue;
            }
        }
    }

    // Method to Inflict Damage
    public void TakeDamage(int damage, Player shooter = null)
    {
        if (damage < 0)
        {
            Debug.LogWarning("Damage value cannot be negative.");
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        // Add negative reward for taking damage
        AddReward(-0.5f);
        if (shooter == null)
        {
            AddReward(-0.1f); //Storm
        }

        OnHealthChanged?.Invoke();

        if (currentHealth <= 0)
        {
            if (shooter != null)
            {
                shooter.OnEnemyEliminated();
            }
            Die();
        }
    }

    // Method to Heal (Optional)
    public void Heal(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Heal amount cannot be negative.");
            return;
        }

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        // Add positive reward for healing
        AddReward(0.05f);

        OnHealthChanged?.Invoke();
    }

    // Handle Player Death
    protected virtual void Die()
    {
        // Add large negative reward for dying
        AddReward(-1.0f);
        
        OnDeath?.Invoke();
        EndEpisode();
        
        gameObject.SetActive(false);
    }

    // Method to Fire a Bullet
    protected void FireBullet()
    {
        if (bulletPrefab == null || shootingPoint == null)
        {
            Debug.LogWarning($"{gameObject.name}: BulletPrefab or ShootingPoint not assigned.");
            return;
        }

        if (shootTimer <= 0f)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.bulletTeam = this.playerTeam;
                bulletScript.damage = this.bulletDamage;
                bulletScript.speed = this.bulletSpeed;
                bulletScript.shooter = this;
            }

            SpriteRenderer bulletRenderer = bullet.GetComponent<SpriteRenderer>();
            if (bulletRenderer != null)
            {
                bulletRenderer.color = playerTeam == Team.Red ? Color.red : Color.blue;
            }

            shootTimer = shootCooldown;
        }
    }

    // Method to handle successful hit on enemy
    public void OnBulletHitEnemy()
    {
        // Add positive reward for successful hit
        AddReward(0.5f);
    }

    // Method to handle enemy elimination
    public void OnEnemyEliminated()
    {
        // Add large positive reward for eliminating an enemy
        AddReward(1.0f);
    }
}
