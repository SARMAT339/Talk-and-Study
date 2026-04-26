using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    [Header("Main")]
    public Camera cam;
    public SpriteRenderer background;

    [Header("Objects")]
    public List<Transform> objects = new List<Transform>();

    private Dictionary<Transform, Vector2> relativePositions = new Dictionary<Transform, Vector2>();

    private Vector2 originalBgSize;

    void Start()
    {
        if (cam == null) cam = Camera.main;

        if (background != null)
            originalBgSize = background.sprite.bounds.size;

        CacheRelativePositions();
        Adapt();
    }

    void Update()
    {
        Adapt();
    }

    void CacheRelativePositions()
    {
        relativePositions.Clear();

        Vector2 size = background.bounds.size;
        Vector2 min = background.bounds.min;

        foreach (var obj in objects)
        {
            if (obj == null) continue;

            float xPercent = (obj.position.x - min.x) / size.x;
            float yPercent = (obj.position.y - min.y) / size.y;

            relativePositions[obj] = new Vector2(xPercent, yPercent);
        }
    }

    void Adapt()
    {
        if (background == null || cam == null) return;

        ScaleBackgroundToCover();
        UpdateObjects();
    }

    void ScaleBackgroundToCover()
    {
        float screenHeight = cam.orthographicSize * 2f;
        float screenWidth = screenHeight * cam.aspect;

        float bgWidth = originalBgSize.x;
        float bgHeight = originalBgSize.y;

        float scaleX = screenWidth / bgWidth;
        float scaleY = screenHeight / bgHeight;

        // COVER — берём максимальный масштаб
        float scale = Mathf.Max(scaleX, scaleY);

        background.transform.localScale = new Vector3(scale, scale, 1f);
    }

    void UpdateObjects()
    {
        Vector2 size = background.bounds.size;
        Vector2 min = background.bounds.min;

        foreach (var pair in relativePositions)
        {
            Transform obj = pair.Key;
            Vector2 percent = pair.Value;

            if (obj == null) continue;

            float x = min.x + size.x * percent.x;
            float y = min.y + size.y * percent.y;

            obj.position = new Vector3(x, y, obj.position.z);
        }
    }
}