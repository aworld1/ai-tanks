// ShortRangeShooter.cs
using UnityEngine;

public class ShortRangeShooter : Player
{
    public override void Initialize()
    {
        base.Initialize();
        moveSpeed = 4f; // Faster movement
        rotationSpeed = 360f; // Faster rotation
        maxHealth = 150; // More health
        currentHealth = maxHealth;

        // Specific shooting settings
        shootCooldown = 0.5f; // 0.5-second cooldown
        bulletSpeed = 12f; // Slower bullets compared to LongRangeShooter
        bulletDamage = 20; // 20 damage
    }
}
