using System.Collections;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveDuration = 2f;
    public Vector3 startPosition = new Vector3(-19.2f, 0f, -10f);
    public Vector3 gamePosition = new Vector3(0f, 0f, -10f);

    private bool isMoving;

    void Awake()
    {
        transform.position = startPosition;
    }

    public void SnapToStart()
    {
        StopAllCoroutines();
        isMoving = false;
        transform.position = startPosition;
    }

    public void MoveToGame()
    {
        if (!isMoving)
            StartCoroutine(MoveRoutine(gamePosition));
    }

    public IEnumerator MoveToStartRoutine()
    {
        yield return MoveRoutine(startPosition);
    }

    IEnumerator MoveRoutine(Vector3 target)
    {
        isMoving = true;

        Vector3 from = transform.position;
        float time = 0f;

        while (time < moveDuration)
        {
            float t = time / moveDuration;
            t = Mathf.SmoothStep(0f, 1f, t);
            transform.position = Vector3.Lerp(from, target, t);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }
}
