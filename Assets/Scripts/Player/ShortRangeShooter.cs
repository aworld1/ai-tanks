// ShortRangeShooter.cs
using UnityEngine;

public class ShortRangeShooter : Player
{
    new void Start()
    {
        base.Start();
        moveSpeed = 4f; // Faster movement
        rotationSpeed = 360f; // Faster rotation
        maxHealth = 150; // More health
        currentHealth = maxHealth;

        // Specific shooting settings
        shootCooldown = 0.5f; // 0.5-second cooldown
        bulletSpeed = 12f; // Slower bullets compared to LongRangeShooter
        bulletDamage = 20; // 20 damage
    }

    protected override void HandleShooting()
    {
        // Example: check if the space key is pressed
        if (Input.GetKey(KeyCode.Space))
        {
            FireBullet();
        }
    }
}
