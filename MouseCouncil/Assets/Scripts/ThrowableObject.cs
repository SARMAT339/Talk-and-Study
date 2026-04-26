using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Если столкнулся с объектом с тегом "CAT"
        if (collision.gameObject.CompareTag("CAT"))
        {
            Debug.Log("Столкновение с LogObject");
        }

        // Если столкнулся с объектом с тегом "Disable"
        if (collision.gameObject.CompareTag("Disable"))
        {
            Debug.Log("Столкновение с DisableObject, отключение через 2 секунды");
            StartCoroutine(DisableAfterDelay(2f));
        }
    }

    private System.Collections.IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
