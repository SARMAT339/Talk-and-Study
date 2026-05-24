using UnityEngine;
using UnityEngine.InputSystem;

public class BellController : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Target")]
    public Transform targetArea;
    public float force = 10f;
    public float randomOffset = 0.2f;

    [HideInInspector]
    public BellSpawner spawner;

    private bool wasThrown = false;

    void Start()
    {
        rb.isKinematic = true;
        rb.gravityScale = 1f;
    }

    void Update()
    {
        if (wasThrown)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(
                Mouse.current.position.ReadValue()
            );

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                Throw();
            }
        }
    }

    void Throw()
    {
        wasThrown = true;

        rb.isKinematic = false;

        Vector2 randomTarget = (Vector2)targetArea.position + new Vector2(
            Random.Range(-randomOffset, randomOffset),
            Random.Range(-randomOffset, randomOffset)
        );

        Vector2 direction =
            (randomTarget - (Vector2)transform.position).normalized;

        direction += Vector2.up * 0.5f;

        rb.linearVelocity = direction * force;

        // сообщаем спавнеру что был бросок
        if (spawner != null)
        {
            spawner.OnBellThrown();
        }
    }
}