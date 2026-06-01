using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSwitcher : MonoBehaviour
{
    public GameObject idleObject;
    public GameObject runObject;

    public Transform target;
    public float speed = 3f;

    private bool isStarted = false;

    void Awake()
    {
        if (FindFirstObjectByType<GameManager>() != null)
            enabled = false;
    }

    void Update()
    {
        if (!enabled)
            return;

        if (!isStarted)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                isStarted = true;
            }

            if (Touchscreen.current != null &&
                Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                isStarted = true;
            }

            idleObject.SetActive(true);
            runObject.SetActive(false);
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > 0.1f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );

            idleObject.SetActive(false);
            runObject.SetActive(true);
        }
        else
        {
            idleObject.SetActive(true);
            runObject.SetActive(false);
        }
    }
}