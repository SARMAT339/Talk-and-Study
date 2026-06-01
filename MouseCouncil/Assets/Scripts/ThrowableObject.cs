using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    private BellController bellController;

    public void SetBellController(BellController controller)
    {
        bellController = controller;
    }

    void Awake()
    {
        if (bellController == null)
            bellController = GetComponent<BellController>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (bellController == null || !bellController.WasThrown)
            return;

        if (collision.gameObject.CompareTag("CAT"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnBellHitCat(bellController);
            return;
        }

        if (collision.gameObject.CompareTag("Disable"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnBellMissed(bellController);
        }
    }
}
