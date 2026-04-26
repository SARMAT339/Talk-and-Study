using UnityEngine;
using UnityEngine.InputSystem;

public class BellController : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Target")]
    public Transform targetArea;
    public float force = 10f;
    public float randomOffset = 0.2f;

    void Start()
    {
        rb.isKinematic = true;
        rb.gravityScale = 1f;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                Throw();
            }
        }
    }

    void Throw()
    {
        rb.isKinematic = false;

        // точка ВНУТРИ области вокруг targetArea
        Vector2 randomTarget = (Vector2)targetArea.position + new Vector2(
            Random.Range(-randomOffset, randomOffset),
            Random.Range(-randomOffset, randomOffset)
        );

        // направление
        Vector2 direction = (randomTarget - (Vector2)transform.position);

        // нормализуем НЕ обязательно (важно!)
        // если хочешь стабильную силу — оставь normalized
        direction = direction.normalized;

        // лёгкая дуга
        direction += Vector2.up * 0.5f;

        rb.linearVelocity = direction * force;
    }
}