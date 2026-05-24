using System.Collections;
using UnityEngine;

public class HideNPC : MonoBehaviour
{
    [Header("Точка укрытия")]
    public Transform hidePoint;

    [Header("Скорость")]
    public float moveSpeed = 2f;

    [Header("Ожидание")]
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;

    [Header("Время в укрытии")]
    public float hideTime = 1f;

    private Vector3 startPosition;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        startPosition = transform.position;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(BehaviourLoop());
    }

    IEnumerator BehaviourLoop()
    {
        while (true)
        {
            animator.SetBool("isRunning", false);

            float wait = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(wait);

            // Бежим в укрытие
            yield return StartCoroutine(MoveToPosition(hidePoint.position));

            animator.SetBool("isRunning", false);

            yield return new WaitForSeconds(hideTime);

            // Возвращаемся назад
            yield return StartCoroutine(MoveToPosition(startPosition));
        }
    }

    IEnumerator MoveToPosition(Vector3 target)
    {
        animator.SetBool("isRunning", true);

        // Определяем направление
        if (target.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = target;

        animator.SetBool("isRunning", false);
    }
}