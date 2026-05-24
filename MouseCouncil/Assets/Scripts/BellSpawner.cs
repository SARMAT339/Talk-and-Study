using System.Collections;
using UnityEngine;

public class BellSpawner : MonoBehaviour
{
    [Header("Bell")]
    public GameObject bellPrefab;

    [Header("Spawn")]
    public Transform spawnPoint;
    public Transform targetArea;

    [Header("Settings")]
    public int maxBells = 3;
    public float spawnDelay = 1.5f;

    private int currentBell = 0;

    void Start()
    {
        SpawnBell();
    }

    public void OnBellThrown()
    {
        currentBell++;

        if (currentBell < maxBells)
        {
            StartCoroutine(SpawnWithDelay());
        }
    }

    IEnumerator SpawnWithDelay()
    {
        yield return new WaitForSeconds(spawnDelay);

        SpawnBell();
    }

    void SpawnBell()
    {
        GameObject newBell = Instantiate(
            bellPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        BellController bell = newBell.GetComponent<BellController>();

        bell.targetArea = targetArea;
        bell.spawner = this;
    }
}