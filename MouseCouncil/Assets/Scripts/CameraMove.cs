using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    public float moveDuration = 2f;

    private bool isMoving = false;
    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = new Vector3(0f, 0f, -10f);
    }

    void Update()
    {
        // Проверка клика мыши
        if (Mouse.current.leftButton.wasPressedThisFrame && !isMoving)
        {
            StartCoroutine(MoveCamera());
        }

        // Проверка тапа (мобильные)
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame &&
            !isMoving)
        {
            StartCoroutine(MoveCamera());
        }
    }

    IEnumerator MoveCamera()
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        float time = 0f;

        while (time < moveDuration)
        {
            float t = time / moveDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPos, targetPosition, t);

            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }
}