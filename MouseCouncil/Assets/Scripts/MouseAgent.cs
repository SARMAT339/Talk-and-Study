using System;
using UnityEngine;

public class MouseAgent : MonoBehaviour
{
    public GameObject idleObject;
    public GameObject runObject;
    public float speed = 3f;

    private Vector3 homePosition;
    private bool isMoving;
    private Vector3 moveTarget;
    private Action onArrived;

    public Vector3 HomePosition => homePosition;
    public bool IsMoving => isMoving;

    public void CaptureHomePosition()
    {
        homePosition = transform.position;
    }

    public void SnapToHome()
    {
        StopMovement();
        transform.position = homePosition;
        SetIdleVisual();
    }

    public void MoveTo(Vector3 worldPosition, Action arrived = null)
    {
        moveTarget = worldPosition;
        onArrived = arrived;
        isMoving = true;
    }

    public void ReturnHome(Action arrived = null)
    {
        MoveTo(homePosition, arrived);
    }

    public void StopMovement()
    {
        isMoving = false;
        onArrived = null;
    }

    public void SetIdleAtCurrentPosition()
    {
        StopMovement();
        SetIdleVisual();
    }

    void Update()
    {
        if (!isMoving)
            return;

        UpdateFacing();

        transform.position = Vector2.MoveTowards(
            transform.position,
            moveTarget,
            speed * Time.deltaTime
        );

        float distance = Vector2.Distance(transform.position, moveTarget);
        if (distance > 0.1f)
        {
            SetRunVisual();
            return;
        }

        transform.position = moveTarget;
        isMoving = false;
        SetIdleVisual();

        Action callback = onArrived;
        onArrived = null;
        callback?.Invoke();
    }

    void SetIdleVisual()
    {
        if (idleObject != null)
            idleObject.SetActive(true);
        if (runObject != null)
            runObject.SetActive(false);
    }

    void SetRunVisual()
    {
        if (idleObject != null)
            idleObject.SetActive(false);
        if (runObject != null)
            runObject.SetActive(true);
    }

    void UpdateFacing()
    {
        float deltaX = moveTarget.x - transform.position.x;
        if (Mathf.Abs(deltaX) < 0.01f)
            return;

        bool movingRight = deltaX > 0f;
        SetSpriteFacing(idleObject, movingRight);
        SetSpriteFacing(runObject, movingRight);
    }

    static void SetSpriteFacing(GameObject visual, bool movingRight)
    {
        if (visual == null)
            return;

        SpriteRenderer sprite = visual.GetComponent<SpriteRenderer>();
        if (sprite != null)
            sprite.flipX = !movingRight;
    }
}
