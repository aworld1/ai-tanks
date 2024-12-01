// DamageZone.cs
using UnityEngine;
using System.Collections.Generic;

public class DamageZone : MonoBehaviour
{
    [Header("Shrinking Settings")]
    public float shrinkSpeed = 1f; // Units per second
    public float minRadius = 5f; // Minimum safe zone radius

    [Header("Damage Settings")]
    public float initialDamagePerSecond = 2f;
    public float damageIncreaseTime1 = 30f; // Time in seconds to increase damage to first level
    public float damageIncreaseTime2 = 60f; // Time in seconds to increase damage to second level
    public float damageLevel1 = 5f;
    public float damageLevel2 = 10f;

    public float tickTime = 1f; // Time in seconds between damage ticks

    [Header("Arena Center")]
    public Transform arenaCenter; // Reference to the center of the arena

    private CircleCollider2D circleCollider;
    public float currentRadius;
    private float elapsedTime = 0f;
    private float currentDamagePerSecond;

    private float nextTickTime = 0f;

    // To keep track of players
    private List<Player> players = new List<Player>();

    void Start()
    {
        // Get the CircleCollider2D component
        circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider == null)
        {
            Debug.LogError("DamageZone: No CircleCollider2D found.");
            enabled = false;
            return;
        }

        // Initialize current radius
        currentRadius = circleCollider.radius;
        currentDamagePerSecond = initialDamagePerSecond;

        // Find all players in the scene (assuming they have the tag "Player")
        Invoke("FindPlayers", 0.1f);

        // If arenaCenter is not assigned, default to DamageZone's position
        if (arenaCenter == null)
        {
            arenaCenter = this.transform;
        }
    }

    void FindPlayers()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject obj in playerObjects)
        {
            Player player = obj.GetComponent<Player>();
            if (player != null)
            {
                players.Add(player);
            }
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // Update damage per second based on elapsed time
        if (elapsedTime >= damageIncreaseTime2)
        {
            currentDamagePerSecond = damageLevel2;
        }
        else if (elapsedTime >= damageIncreaseTime1)
        {
            currentDamagePerSecond = damageLevel1;
        }
        else
        {
            currentDamagePerSecond = initialDamagePerSecond;
        }

        // Shrink the damage zone
        if (currentRadius > minRadius)
        {
            transform.localScale -= Vector3.one * shrinkSpeed * Time.deltaTime;
            float minScale = minRadius / currentRadius;
            transform.localScale = Vector3.Max(transform.localScale, Vector3.one * minScale);
        }

        if (Time.time >= nextTickTime)
        {
            ApplyDamage();
            nextTickTime = Time.time + tickTime;
        }
    }

    private void ApplyDamage()
    {
        // Apply damage to players inside the safe zone
        foreach (Player player in players)
        {
            if (player != null && !IsPlayerInside(player))
            {
                player.TakeDamage((int)currentDamagePerSecond);
            }
        }
    }

    // Method to check if a player is inside the safe zone
    private bool IsPlayerInside(Player player) {
        float distance = Vector2.Distance(arenaCenter.position, player.transform.position);
        float scaledRadius = currentRadius * transform.localScale.x; // Assuming uniform scaling
        return distance <= scaledRadius;
    }

    // Optional: Visual feedback when damage is applied
    /*
    void OnDrawGizmos()
    {
        // Draw the safe zone
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(arenaCenter.position, currentRadius);
    }
    */
}
