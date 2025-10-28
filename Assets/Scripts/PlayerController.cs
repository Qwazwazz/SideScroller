using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Combat")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private PlayerState currentState;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        ChangeState(new IdleState());
    }

    void Update()
    {
        // Skip input when paused
        if (GameManager.Instance != null && GameManager.Instance.IsPaused())
            return;

        currentState?.UpdateState(this);
    }

    public void ChangeState(PlayerState newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
        EventManager.TriggerEvent("OnPlayerStateChanged", currentState.GetStateName());
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void Fire()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            // Determine direction based on sprite flip
            int direction = spriteRenderer.flipX ? -1 : 1;

            // Tell the bullet which direction to move
            animator.Play("Attack");
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
                bulletScript.SetDirection(direction);

            AudioManager.Instance.PlayShootSound();
            animator.Play("Idle");
        }
    }

    public void TakeDamage()
    {
        GameManager.Instance.PlayerDied();
        Respawn();
    }

    void Respawn()
    {
        transform.position = GameManager.Instance.spawnPoint;
        ChangeState(new IdleState());
    }

    public string GetCurrentStateName()
    {
        return currentState != null ? currentState.GetStateName() : "None";
    }
}
