using UnityEngine;

public class BulletScript : ProjectileScript
{
    private float lifeTime = 3f; // seconds before the bullet expires
    private float lifeTimer = 0f;

    protected override void Update()
    {
        base.Update();
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    protected override void Move()
    {
        if (playerCharacter != null)
        {
            // Calculate direction toward player
            Vector3 directionToPlayer = (playerCharacter.transform.position - transform.position).normalized;
            SetDirection(directionToPlayer);
        }
        base.Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CharacterScript character = collision.GetComponent<CharacterScript>();
            if (character != null)
            {
                character.TakeDamage();
            }
            Destroy(gameObject);
        }
    }
}
