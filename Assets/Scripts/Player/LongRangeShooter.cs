// LongRangeShooter.cs
using UnityEngine;

public class LongRangeShooter : Player
{
    public override void Initialize()
    {
        base.Initialize();
        moveSpeed = 2f; // Slower movement
        rotationSpeed = 180f; // Slower rotation
        maxHealth = 100; // Less health
        currentHealth = maxHealth;

        // Specific shooting settings
        shootCooldown = 1f; // 1-second cooldown
        bulletSpeed = 15f; // Faster bullets
        bulletDamage = 30; // 30 damage
    }
}
