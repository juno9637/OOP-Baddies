using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class FightSceneManager : MonoBehaviour, IObservable
{
    public BoxCollider2D battleAreaCollider;
    private Bounds battleBounds;

    public GameObject bulletPrefab;
    public GameObject pagePrefab;
    public ProjectileGroup bulletGroup;
    public ProjectileGroup pageGroup;
    public float bulletSpawnRate = 1.5f;
    public float pageSpawnRate = 3f;
    public int bulletsPerWave = 3;

    public Image progressBar;
    public Text winText;
    public Text loadingText;
    public RectTransform progressBarFillRect;
    public float progressBarWidth = 400f;

    private int pagesCollected = 0;
    private int maxPages = 4;
    private float bulletSpawnTimer = 0f;
    private float pageSpawnTimer = 0f;
    private bool gameWon = false;
    private bool gameLost = false;

    private List<IObserver> observers = new List<IObserver>();

    void Start()
    {
        // Get battle bounds from the collider
        if (battleAreaCollider != null)
        {
            battleBounds = battleAreaCollider.bounds;
            Debug.Log("Battle bounds: center=" + battleBounds.center + ", size=" + battleBounds.size + ", extents=" + battleBounds.extents);
        }
        else
        {
            Debug.LogError("battleAreaCollider is NULL!");
            battleBounds = new Bounds(Vector3.zero, new Vector3(16, 9, 1));
            Debug.Log("Using default bounds: " + battleBounds);
        }

        // Initialize UI
        if (progressBar != null)
        {
            progressBar.fillAmount = 0f;
            Debug.Log("FightSceneManager: progressBar assigned at Start, initial fillAmount=" + progressBar.fillAmount);
        }

        if (winText != null)
        {
            winText.text = "";
        }

        pagesCollected = 0;
        gameWon = false;
        gameLost = false;
    }

    void Update()
    {
        if (gameWon || gameLost)
            return;

        // Spawn bullets continuously until game is won
        bulletSpawnTimer += Time.deltaTime;
        if (bulletSpawnTimer >= bulletSpawnRate)
        {
            SpawnBullet();
            bulletSpawnTimer = 0f;
        }

        // Spawn pages (up to 4 total)
        pageSpawnTimer += Time.deltaTime;
        if (pageSpawnTimer >= pageSpawnRate)
        {
            SpawnPage();
            pageSpawnTimer = 0f;
        }
    }

    private void SpawnBullet()
    {
        if (bulletPrefab == null || bulletGroup == null)
            return;

        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject bulletObj = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        bulletObj.SetActive(true);
        Debug.Log("Bullet spawned at " + spawnPos);
        
        SpriteRenderer sr = bulletObj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = true;
            sr.color = new Color(0.2f, 0.2f, 0.2f);
            sr.sortingOrder = 2;
        }
        
        BulletScript bulletScript = bulletObj.GetComponent<BulletScript>();
        bulletScript.SetSpeed(3f);
        bulletGroup.AddProjectile(bulletScript);
    }

    private void SpawnPage()
    {
        if (pagePrefab == null || pageGroup == null || pagesCollected >= maxPages)
        {
            Debug.Log("SpawnPage early return: pagePrefab=" + (pagePrefab != null) + ", pageGroup=" + (pageGroup != null) + ", pagesCollected=" + pagesCollected + ", maxPages=" + maxPages);
            return;
        }

        // Spawn pages above the top edge so they start offscreen and fall down
        Vector3 center = battleBounds.center;
        Vector3 extents = battleBounds.extents;
        Debug.Log("SpawnPage: center=" + center + ", extents=" + extents);
        float pageSpawnMargin = Mathf.Max(extents.x, extents.y) * 0.25f;
        float spawnX = Random.Range(center.x - extents.x, center.x + extents.x);
        float spawnY = center.y + extents.y + pageSpawnMargin;
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);
        Debug.Log("Page attempting to spawn at: " + spawnPos);
        
        GameObject pageObj = Instantiate(pagePrefab, spawnPos, Quaternion.identity);
        pageObj.SetActive(true);
        Debug.Log("Page spawned at " + spawnPos);
        
        // Ensure sprite renderer is visible
        SpriteRenderer sr = pageObj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = true;
            sr.color = Color.white;
            sr.sortingOrder = 2;
        }
        
        CollectablePageScript pageScript = pageObj.GetComponent<CollectablePageScript>();
        if (pageScript != null)
        {
            pageScript.SetSpeed(2f);
            pageGroup.AddProjectile(pageScript);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Spawn from edges of the battle area
        Vector3 center = battleBounds.center;
        Vector3 extents = battleBounds.extents;
        float spawnMargin = Mathf.Max(extents.x, extents.y) * 0.25f; // how far outside the arena to start

        Vector3 spawnPos = new Vector3(Random.Range(center.x - extents.x, center.x + extents.x), center.y + extents.y + spawnMargin, 0);
        
        return spawnPos;
    }

    public void CollectPage()
    {
        if (gameWon || gameLost)
            return;

        pagesCollected++;
        Debug.Log("Page collected! Total: " + pagesCollected + " / " + maxPages);

        // Update progress bar UI: prefer resizing the fill Rect if provided
        float progress = (float)pagesCollected / maxPages;
        if (progressBarFillRect != null)
        {
            Vector2 sd = progressBarFillRect.sizeDelta;
            sd.x = progressBarWidth * progress;
            progressBarFillRect.sizeDelta = sd;
            Debug.Log("Progress bar rect width set to: " + sd.x);
        }
        else if (progressBar != null)
        {
            progressBar.fillAmount = progress;
            Debug.Log("Progress bar fillAmount: " + progressBar.fillAmount);
        }

        // Check win condition
        if (pagesCollected >= maxPages)
        {
            WinGame();
        }
    }

    public void RestartScene()
    {
        gameLost = true;
        Invoke("ReloadScene", 0.5f);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void WinGame()
    {
        gameWon = true;

        // Update loading text to show completion
        if (loadingText != null)
            loadingText.text = "Complete!";

        // Destroy all projectiles using the composite groups
        if (bulletGroup != null)
            bulletGroup.DestroyAll();
        if (pageGroup != null)
            pageGroup.DestroyAll();

        Notify("Downloaded a file");

    }

    public Bounds GetBattleBounds()
    {
        return battleBounds;
    }

    public void Attach(IObserver observer)
    {
        if (observer == null || observers.Contains(observer)) {
            return;
        }
        observers.Add(observer);
    }

    public void Notify(string message)
    {
        foreach (IObserver observer in observers)
            observer.OnNotify(message);
    }



}
