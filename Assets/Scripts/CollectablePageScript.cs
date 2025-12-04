using UnityEngine;

public class CollectablePageScript : ProjectileScript
{
    private float lifeTime = 5f; // seconds before the page expires
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
        SetDirection(Vector3.down);
        base.Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Page collision detected!");
            FightSceneManager fightManager = FindFirstObjectByType<FightSceneManager>();
            if (fightManager != null)
            {
                Debug.Log("Calling CollectPage...");
                fightManager.CollectPage();
            }
            else
            {
                Debug.LogError("FightSceneManager not found!");
            }
            Destroy(gameObject);
        }
    }
}