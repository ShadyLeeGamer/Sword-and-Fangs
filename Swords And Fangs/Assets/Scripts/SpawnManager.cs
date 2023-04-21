using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] EnemyMovement[] enemyPrefabs;
    [SerializeField] Color[] enemyColours;
    [SerializeField] float spawnRangeMin, spawnRangeMax;
    [SerializeField] float spawnStartDelay;
    [SerializeField] Wave[] waves;

    Wave currentWave;
    public int currentWaveIndex;

    int enemiesLeftToSpawn;
    public int EnemiesRemaining { get; private set; }
    float timeToSpawn;

    bool canSpawn = true;
    public static SpawnManager Instance { get; private set; }

    ObjectPooler objectPooler;
    GameManager gameManager;
    GameScreen gameScreen;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        gameManager = GameManager.Instance;
        gameScreen = GameScreen.Instance;
    }

    void Update()
    {
        if (canSpawn)
            if (enemiesLeftToSpawn > 0 && Time.time > timeToSpawn)
            {
                enemiesLeftToSpawn--;
                timeToSpawn = Time.time + currentWave.delayBetweenSpawns;

                StartCoroutine("SpawnEnemy");
            }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.N))
        {
            StopCoroutine("SpawnEnemy");

            StartNextWave();
        }
#endif
    }

    GameObject currentEnemyHolder;

    IEnumerator SpawnEnemy()
    {
        Vector3 spawnPos = GetRandomOpenPosition(spawnRangeMin, spawnRangeMax);

        float spawnTimer = 0;
        while (spawnTimer < spawnStartDelay)
        {
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        EnemyMovement randomEnemyPrefab = null;
        Color randomColour = currentWave.spawnChancesGradient.Evaluate(UnityEngine.Random.value);
        for (int i = 0; i < enemyColours.Length; i++)
            if (randomColour == enemyColours[i])
                randomEnemyPrefab = enemyPrefabs[i];
        EnemyMovement newEnemy =
            objectPooler.GetEnemy(randomEnemyPrefab.id,
                                  spawnPos, Quaternion.identity,
                                  new ObjectData(randomEnemyPrefab.maxHealth));
        newEnemy.transform.SetParent(currentEnemyHolder.transform);
    }

    Vector3 GetRandomOpenPosition(float minRadius, float maxRadius)
    {
        var finalOpenPos = Vector3.zero;

        Vector3 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
        randomDirection = new Vector3(randomDirection.x, 0f, randomDirection.y);
        float randomDistance = UnityEngine.Random.Range(minRadius, maxRadius);

        Vector3 randomPos = Vector3.zero + randomDirection * randomDistance;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 100f, 1))
            finalOpenPos = hit.position;
        return finalOpenPos + (Vector3.up * 2f);
    }

    public void RecordPlayerDeath()
    {
        canSpawn = false;
    }

    public void RecordEnemyDeath()
    {
        gameScreen.gameGUI.RefreshKnightsLeftCounter(EnemiesRemaining -= 1);
        if (EnemiesRemaining == 0)
            gameManager.EndWave();
    }

    public void StartNextWave()
    {
        currentWaveIndex++;
        Destroy(currentEnemyHolder);
        currentEnemyHolder = new GameObject("EnemyHolder");

        if (currentWaveIndex - 1 < waves.Length)
        {
            currentWave = waves[currentWaveIndex - 1];

            enemiesLeftToSpawn = currentWave.enemyCount;
            EnemiesRemaining = enemiesLeftToSpawn;
        }
    }

    [Serializable]
    public struct Wave
    {
        public int enemyCount;
        public Gradient spawnChancesGradient;
        public float delayBetweenSpawns;
    }
}