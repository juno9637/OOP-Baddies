using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    protected Vector3 moveDirection = Vector3.zero;
    protected float moveSpeed = 5f;
    protected CharacterScript playerCharacter;
    private Vector3 startPosition;

    // Public properties for accessing protected members (used by composite groups)
    public Vector3 MoveDirection { get => moveDirection; }
    public float MoveSpeed { get => moveSpeed; }

    protected virtual void Start() {
        playerCharacter = FindFirstObjectByType<CharacterScript>();
        startPosition = transform.position;
    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move() {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        // Destroy if too far from start position
        if (Vector3.Distance(transform.position, startPosition) > 50f)
        {
            Destroy(gameObject);
        }
    }

    public virtual void SetDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    public virtual void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }
}
