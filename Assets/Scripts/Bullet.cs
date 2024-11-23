// Bullet.cs
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public Team bulletTeam = Team.Red; // Default team; set upon instantiation
    [HideInInspector]
    public int damage = 20; // Set based on shooter
    [HideInInspector]
    public float speed = 10f; // Set based on shooter

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Move the bullet forward based on its rotation
        Vector2 direction = transform.up;
        rb.linearVelocity = direction * speed;

        // Destroy the bullet after 5 seconds to prevent clutter
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is a player
        if (collision.CompareTag("Player"))
        {
            Player hitPlayer = collision.GetComponent<Player>();
            if (hitPlayer != null && hitPlayer.playerTeam != bulletTeam)
            {
                // Apply damage to the opposing player
                hitPlayer.TakeDamage(damage);

                // Destroy the bullet after hitting
                Destroy(gameObject);
            }
        }
    }
}
