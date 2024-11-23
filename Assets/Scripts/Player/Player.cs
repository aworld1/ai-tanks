// Player.cs (Modified)
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Player : MonoBehaviour
{
    // Movement Parameters
    [Header("Movement Settings")]
    public float moveSpeed = 4f;
    public float rotationSpeed = 360f;

    // Health Parameters
    [Header("Health Settings")]
    public int maxHealth = 100;
    [HideInInspector]
    public int currentHealth;

    // Input Keys
    [Header("Control Settings")]
    public KeyCode turnLeftKey = KeyCode.A;
    public KeyCode turnRightKey = KeyCode.D;
    public KeyCode moveForwardKey = KeyCode.W;

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

    private float shootTimer = 0f;

    // Events
    public event Action OnHealthChanged;
    public event Action OnDeath;

    // Components
    protected Rigidbody2D rb;

    // Initialization
    protected virtual void Start()
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

    // Physics-based Movement
    protected virtual void FixedUpdate()
    {
        HandleMovement();

        // Handle shooting timer
        if (shootTimer > 0f)
        {
            shootTimer -= Time.fixedDeltaTime;
        }

        HandleShooting();
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

    // Handle Player Movement
    protected virtual void HandleMovement()
    {
        float turnInput = 0f;
        if (Input.GetKey(turnLeftKey))
            turnInput = -1f; // Turn left
        if (Input.GetKey(turnRightKey))
            turnInput = 1f; // Turn right

        float moveInput = 0f;
        if (Input.GetKey(moveForwardKey))
            moveInput = 1f; // Move forward

        // Rotate the player
        float rotationAmount = -turnInput * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotationAmount);

        // Move the player forward
        Vector2 direction = transform.up; // Assuming the 'up' direction is the forward direction
        Vector2 newPosition = rb.position + direction * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    // Method to Inflict Damage
    public void TakeDamage(int damage)
    {
        if (damage < 0)
        {
            Debug.LogWarning("Damage value cannot be negative.");
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Clamp to 0

        // Trigger the health changed event
        OnHealthChanged?.Invoke();

        // Check for death
        if (currentHealth <= 0)
        {
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
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Clamp to maxHealth

        // Trigger the health changed event
        OnHealthChanged?.Invoke();
    }

    // Handle Player Death
    protected virtual void Die()
    {

        // Trigger the death event
        OnDeath?.Invoke();

        // Optional: Play death animation, disable controls, etc.
        // For now, simply destroy the player object
        Destroy(gameObject);
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
            // Instantiate the bullet at the shooting point's position and rotation
            GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);

            // Set the bullet's team to match the player's team
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.bulletTeam = this.playerTeam;
                bulletScript.damage = this.bulletDamage;
                bulletScript.speed = this.bulletSpeed;
            }

            // color the bullet based on the player's team
            SpriteRenderer bulletRenderer = bullet.GetComponent<SpriteRenderer>();
            if (bulletRenderer != null)
            {
                bulletRenderer.color = playerTeam == Team.Red ? Color.red : Color.blue;
            }

            // Reset the shoot timer
            shootTimer = shootCooldown;
        }
    }


    // Abstract method to handle shooting input; to be implemented in derived classes
    protected abstract void HandleShooting();
}
