using UnityEngine;

public class BellSpawner : MonoBehaviour
{
    [Header("Bell")]
    public GameObject bellPrefab;

    [Header("Spawn")]
    public Transform spawnPoint;
    public Transform targetArea;

    private GameManager gameManager;

    public void Initialize(GameManager manager)
    {
        gameManager = manager;

        if (bellPrefab == null)
        {
            GameObject sceneBell = GameObject.Find("Bell_0");
            if (sceneBell != null)
                bellPrefab = sceneBell;
        }

        if (spawnPoint == null && bellPrefab != null)
            spawnPoint = bellPrefab.transform;
    }

    public BellController SpawnBellForTurn()
    {
        if (bellPrefab == null || spawnPoint == null)
            return null;

        Vector3 position = spawnPoint.position;
        GameObject newBell = Instantiate(bellPrefab, position, Quaternion.identity);
        newBell.SetActive(true);

        BellController bell = newBell.GetComponent<BellController>();
        if (bell == null)
            return null;

        if (targetArea != null)
            bell.targetArea = targetArea;
        else if (gameManager != null && gameManager.catTarget != null)
            bell.targetArea = gameManager.catTarget;

        bell.spawner = this;
        bell.gameManager = gameManager;
        bell.PrepareForSpawn();

        ThrowableObject throwable = newBell.GetComponent<ThrowableObject>();
        if (throwable != null)
            throwable.SetBellController(bell);

        return bell;
    }
}
