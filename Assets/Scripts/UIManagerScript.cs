using UnityEngine;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour
{
    // Constants for game numbers so we can switch them if necessary
    public const int ESC_GAME = 1;
    public const int UNDERTALE_GAME = 2;

    private Color dark_gray = new Color(0.2f, 0.2f, 0.2f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void cleanup()
    {
        
    }

    public void setScene(int sceneNumber)
    {
        cleanup();
        switch (sceneNumber)
        {
            case(ESC_GAME):
                // do stuff
                break;
            case(UNDERTALE_GAME):
                UndertaleSceneSetup();
                break;
        }
    }

    public void UndertaleSceneSetup() {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            // lazy instantiation!
            GameObject cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
        }
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 5f;
        mainCamera.transform.position = new Vector3(0, 0, -10);
        mainCamera.backgroundColor = new Color(.87f, .87f, .87f);

        // Create Arena (Background)
        GameObject arena = new GameObject("BattleArena");
        SpriteRenderer arenaSpriteRenderer = arena.AddComponent<SpriteRenderer>();
        // arena has no sprite currently; keep color and put it behind other sprites
        arenaSpriteRenderer.color = Color.black;
        arenaSpriteRenderer.sortingOrder = -1;
        BoxCollider2D arenacollider = arena.AddComponent<BoxCollider2D>();
        arenacollider.size = new Vector2(16, 9);
        arenacollider.isTrigger = false;

        // Create Player Character
        GameObject player = new GameObject("Player");
        player.transform.position = Vector3.zero;
        
        SpriteRenderer playerSprite = player.AddComponent<SpriteRenderer>();
        playerSprite.sprite = CreatePlayerSprite();
        playerSprite.sortingOrder = 3;
        
        BoxCollider2D playerCollider = player.AddComponent<BoxCollider2D>();
        playerCollider.size = new Vector2(0.5f, 0.5f);
        playerCollider.isTrigger = true;

        Rigidbody2D playerRb = player.AddComponent<Rigidbody2D>();
        playerRb.gravityScale = 0;
        playerRb.isKinematic = true;
        
        CharacterScript characterScript = player.AddComponent<CharacterScript>();
        characterScript.setSpeed(8f);
        player.tag = "Player";

        GameObject bulletPrefab = new GameObject("Bullet");
        bulletPrefab.SetActive(false);
        SpriteRenderer bulletSprite = bulletPrefab.AddComponent<SpriteRenderer>();
        bulletSprite.sprite = CreateBulletSprite();
        bulletSprite.sortingOrder = 2;
        bulletPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        BoxCollider2D bulletCollider = bulletPrefab.AddComponent<BoxCollider2D>();
        bulletCollider.size = new Vector2(0.3f, 0.3f);
        bulletCollider.isTrigger = true;

        Rigidbody2D bulletRb = bulletPrefab.AddComponent<Rigidbody2D>();
        bulletRb.gravityScale = 0;
        bulletRb.isKinematic = true;

        BulletScript bulletScript = bulletPrefab.AddComponent<BulletScript>();
        bulletScript.SetSpeed(1.5f);

        GameObject pagePrefab = new GameObject("Page");
        pagePrefab.SetActive(false); // Keep inactive in scene for instantiation
        SpriteRenderer pageSprite = pagePrefab.AddComponent<SpriteRenderer>();
        pageSprite.sprite = CreatePageSprite();
        pageSprite.color = Color.white;
        pageSprite.sortingOrder = 2;
        
        BoxCollider2D pageCollider = pagePrefab.AddComponent<BoxCollider2D>();
        pageCollider.size = new Vector2(0.4f, 0.4f);
        pageCollider.isTrigger = true;

        Rigidbody2D pageRb = pagePrefab.AddComponent<Rigidbody2D>();
        pageRb.gravityScale = 0;
        pageRb.isKinematic = true;
        
        CollectablePageScript pageScript = pagePrefab.AddComponent<CollectablePageScript>();
        pageScript.SetSpeed(2f);

        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Create ProjectileGroups for managing bullets and pages using composite pattern
        GameObject bulletGroupObj = new GameObject("BulletGroup");
        ProjectileGroup bulletGroup = bulletGroupObj.AddComponent<ProjectileGroup>();

        GameObject pageGroupObj = new GameObject("PageGroup");
        ProjectileGroup pageGroup = pageGroupObj.AddComponent<ProjectileGroup>();

        // Create Progress Bar Background
        GameObject progressBarBg = new GameObject("ProgressBarBg");
        progressBarBg.transform.parent = canvasObj.transform;
        RectTransform progressBarBgRect = progressBarBg.AddComponent<RectTransform>();
        progressBarBgRect.anchorMin = new Vector2(0.5f, 1f);
        progressBarBgRect.anchorMax = new Vector2(0.5f, 1f);
        progressBarBgRect.sizeDelta = new Vector2(400, 40);
        progressBarBgRect.anchoredPosition = new Vector2(0, -400);

        Image progressBarBgImage = progressBarBg.AddComponent<Image>();
        // Use builtin UI sprite and sliced type so background and fill match visually
        progressBarBgImage.type = Image.Type.Sliced;
        progressBarBgImage.color = Color.gray;

        // Loading text above progress bar (top-middle)
        GameObject loadingTextObj = new GameObject("LoadingText");
        loadingTextObj.transform.parent = canvasObj.transform;
        RectTransform loadingTextRect = loadingTextObj.AddComponent<RectTransform>();
        loadingTextRect.anchorMin = new Vector2(0.5f, .75f);
        loadingTextRect.anchorMax = new Vector2(0.5f, .75f);
        loadingTextRect.sizeDelta = new Vector2(400, 60);
        loadingTextRect.anchoredPosition = new Vector2(0, -40);
        Text loadingText = loadingTextObj.AddComponent<Text>();
        loadingText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        loadingText.text = "Loading . . .";
        loadingText.alignment = TextAnchor.MiddleCenter;
        loadingText.fontSize = 48;
        loadingText.color = dark_gray;

        // Create Progress Bar Fill (as filled image)
        GameObject progressBarFill = new GameObject("ProgressBarFill");
        progressBarFill.transform.parent = progressBarBg.transform;
        RectTransform progressBarFillRect = progressBarFill.AddComponent<RectTransform>();
        // Left-anchored fill: start with zero width and expand to the right
        progressBarFillRect.anchorMin = new Vector2(0f, 0f);
        progressBarFillRect.anchorMax = new Vector2(0f, 1f);
        progressBarFillRect.pivot = new Vector2(0f, 0.5f);
        progressBarFillRect.anchoredPosition = Vector2.zero;
        progressBarFillRect.sizeDelta = new Vector2(0f, progressBarBgRect.sizeDelta.y);

        Image progressBarFillImage = progressBarFill.AddComponent<Image>();
        progressBarFillImage.type = Image.Type.Sliced;
        progressBarFillImage.color = Color.green;

        GameObject gameManagerObj = new GameObject("FightSceneManager");
        FightSceneManager fightManager = gameManagerObj.AddComponent<FightSceneManager>();
        fightManager.battleAreaCollider = arenacollider;
        fightManager.bulletPrefab = bulletPrefab;
        fightManager.pagePrefab = pagePrefab;
        pagePrefab.transform.parent = gameManagerObj.transform; // Parent to manager for organization
        fightManager.bulletGroup = bulletGroup;
        fightManager.pageGroup = pageGroup;

        fightManager.progressBar = progressBarFillImage;
        fightManager.loadingText = loadingText;
        fightManager.progressBarFillRect = progressBarFillRect;
        fightManager.progressBarWidth = progressBarBgRect.sizeDelta.x;
        fightManager.bulletSpawnRate = 2f;           // Spawn bullets every 2 seconds (slower)
        fightManager.pageSpawnRate = 3f;             // Spawn pages every 3 seconds

    }

    private static Sprite CreatePlayerSprite()
    {
        // Debug.Log(Resources.Load<Sprite>("file-not-found-sprite"));
        return Resources.Load<Sprite>("file-not-found-sprite");

    }

    private static Sprite CreateBulletSprite()
    {
        Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        
        Color[] pixels = new Color[16 * 16];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;

        // Draw a small circle
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), Vector2.one * 7.5f);
                if (dist < 7.5f)
                    pixels[y * 16 + x] = Color.white;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 16, 16), Vector2.one * 0.5f, 16);
    }

    private static Sprite CreatePageSprite()
    {
        return Resources.Load<Sprite>("download-sprite");
    }
}
