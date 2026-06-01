using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class BellController : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Target")]
    public Transform targetArea;
    public float force = 10f;
    public float randomOffset = 0.2f;

    [HideInInspector]
    public BellSpawner spawner;

    [HideInInspector]
    public GameManager gameManager;

    private bool canThrow;
    private bool wasThrown;

    public bool WasThrown => wasThrown;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    public void PrepareForSpawn()
    {
        wasThrown = false;
        canThrow = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
        rb.gravityScale = 1f;
    }

    public void SetThrowEnabled(bool enabled)
    {
        canThrow = enabled && !wasThrown;
    }

    void Update()
    {
        if (!canThrow || wasThrown)
            return;

        if (!WasScreenPressed())
            return;

        Vector2 screenPos = GetScreenPosition();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
            Throw();
    }

    void Throw()
    {
        wasThrown = true;
        canThrow = false;
        rb.isKinematic = false;

        Vector2 randomTarget = (Vector2)targetArea.position + new Vector2(
            Random.Range(-randomOffset, randomOffset),
            Random.Range(-randomOffset, randomOffset)
        );

        Vector2 direction = (randomTarget - (Vector2)transform.position).normalized;
        direction += Vector2.up * 0.5f;
        rb.linearVelocity = direction * force;

        if (gameManager != null)
            gameManager.OnBellThrown(this);
    }

    static bool WasScreenPressed()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;

        return false;
    }

    static Vector2 GetScreenPosition()
    {
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed)
            return Touchscreen.current.primaryTouch.position.ReadValue();

        if (Mouse.current != null)
            return Mouse.current.position.ReadValue();

        return Vector2.zero;
    }
}
