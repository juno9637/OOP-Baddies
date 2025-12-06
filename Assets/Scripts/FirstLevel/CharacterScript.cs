using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    private float moveSpeed = 5f;
    public float boundaryPadding = 0.5f;
    
    private Vector3 moveDirection = Vector3.zero;
    private Bounds battleBounds;
    private FightSceneManager fightManager;

    void Start()
    {
        fightManager = FindFirstObjectByType<FightSceneManager>();
        if (fightManager != null)
        {
            battleBounds = fightManager.GetBattleBounds();
        }
    }

    void Update()
    {
        HandleInput();
        Move();
    }

    private void HandleInput()
    {
        moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            moveDirection += Vector3.up;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            moveDirection += Vector3.down;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            moveDirection += Vector3.right;

        moveDirection = moveDirection.normalized;
    }

    private void Move()
    {
        Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;

        // Clamp to battle bounds
        if (battleBounds.size != Vector3.zero)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, 
                battleBounds.min.x + boundaryPadding, 
                battleBounds.max.x - boundaryPadding);
            newPosition.y = Mathf.Clamp(newPosition.y, 
                battleBounds.min.y + boundaryPadding, 
                battleBounds.max.y - boundaryPadding);
        }

        transform.position = newPosition;
    }

    public void TakeDamage()
    {
        if (fightManager != null)
        {
            fightManager.RestartScene();
        }
    }

    public void setSpeed(float speed) {
        moveSpeed = speed;
    }
}
