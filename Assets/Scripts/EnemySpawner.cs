using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnpoints;
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Enemy 1")]
    [SerializeField] private float enemy1StartSpawnsPerSecond = 0.1f;
    [SerializeField] private float enemy1SpawnRateIncrease = 0.1f;
    [SerializeField] private float enemy1RateIncreaseInterval = 10f;
    [SerializeField] private float enemy1MaxSpawnsPerSecond = 2f;

    [Header("Enemy 2")]
    [SerializeField] private float enemy2StartInterval = 20f;
    [SerializeField] private float enemy2IntervalReduction = 2.5f;
    [SerializeField] private float enemy2AccelerationInterval = 30f;
    [SerializeField] private float enemy2MinInterval = 10f;

    [Header("Enemy 3 / 4")]
    [SerializeField] private float enemy3SpawnInterval = 30f;
    [SerializeField] private float enemy4SpawnInterval = 20f;

    [Header("Boss Health")]
    [SerializeField] private float boss1HealthMultiplier = 3f;
    [SerializeField] private float boss2HealthMultiplier = 3.5f;
    [SerializeField] private float boss3HealthMultiplier = 3f;
    [SerializeField] private float boss4HealthMultiplier = 2.5f;

    private float elapsedTime;
    private float enemy1Timer;
    private float enemy2Timer;
    private float enemy3Timer;
    private float enemy4Timer;

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        HandleEnemy1Spawns();
        HandleEnemy2Spawns();
        HandleEnemy3Spawns();
        HandleEnemy4Spawns();
    }

    public void SpawnBoss(int bossNumber, int enemyIndex)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No enemy prefabs configured");
            return;
        }

        int prefabIndex = Mathf.Clamp(enemyIndex, 0, enemyPrefabs.Length - 1);
        GameObject boss = SpawnEnemy(prefabIndex);
        if (boss == null)
        {
            return;
        }

        Enemy enemyComponent = boss.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.SetBoss(bossNumber, prefabIndex, GetBossHealthMultiplier(bossNumber));
        }
    }

    public void ResetSpawner()
    {
        elapsedTime = 0f;
        enemy1Timer = 0f;
        enemy2Timer = 0f;
        enemy3Timer = 0f;
        enemy4Timer = 0f;
    }

    private void HandleEnemy1Spawns()
    {
        enemy1Timer += Time.deltaTime;
        float interval = 1f / GetEnemy1SpawnRate();
        SpawnWhileReady(ref enemy1Timer, interval, 0);
    }

    private void HandleEnemy2Spawns()
    {
        if (Mastermind.instance == null || !Mastermind.instance.IsEnemyTypeUnlocked(1))
        {
            return;
        }

        enemy2Timer += Time.deltaTime;
        SpawnWhileReady(ref enemy2Timer, GetEnemy2SpawnInterval(), 1);
    }

    private void HandleEnemy3Spawns()
    {
        if (Mastermind.instance == null || !Mastermind.instance.IsEnemyTypeUnlocked(2))
        {
            return;
        }

        enemy3Timer += Time.deltaTime;
        SpawnWhileReady(ref enemy3Timer, enemy3SpawnInterval, 2);
    }

    private void HandleEnemy4Spawns()
    {
        if (Mastermind.instance == null || !Mastermind.instance.IsEnemyTypeUnlocked(3))
        {
            return;
        }

        enemy4Timer += Time.deltaTime;
        SpawnWhileReady(ref enemy4Timer, enemy4SpawnInterval, 3);
    }

    private void SpawnWhileReady(ref float timer, float interval, int enemyIndex)
    {
        if (interval <= 0f)
        {
            return;
        }

        while (timer >= interval)
        {
            timer -= interval;
            if (SpawnEnemy(enemyIndex) == null)
            {
                return;
            }
        }
    }

    private float GetEnemy1SpawnRate()
    {
        float increaseSteps = Mathf.Floor(elapsedTime / enemy1RateIncreaseInterval);
        float currentRate = enemy1StartSpawnsPerSecond + (increaseSteps * enemy1SpawnRateIncrease);
        return Mathf.Min(currentRate, enemy1MaxSpawnsPerSecond);
    }

    private float GetEnemy2SpawnInterval()
    {
        float unlockTime = 60f;
        float timeSinceUnlock = Mathf.Max(0f, elapsedTime - unlockTime);
        float reductionSteps = Mathf.Floor(timeSinceUnlock / enemy2AccelerationInterval);
        float currentInterval = enemy2StartInterval - (reductionSteps * enemy2IntervalReduction);
        return Mathf.Max(currentInterval, enemy2MinInterval);
    }

    private float GetBossHealthMultiplier(int bossNumber)
    {
        return bossNumber switch
        {
            1 => boss1HealthMultiplier,
            2 => boss2HealthMultiplier,
            3 => boss3HealthMultiplier,
            4 => boss4HealthMultiplier,
            _ => 1f
        };
    }

    private GameObject SpawnEnemy(int enemyIndex)
    {
        if (spawnpoints == null || spawnpoints.Length == 0)
        {
            Debug.LogWarning("No spawnpoints configured");
            return null;
        }

        if (enemyPrefabs == null || enemyIndex < 0 || enemyIndex >= enemyPrefabs.Length || enemyPrefabs[enemyIndex] == null)
        {
            Debug.LogWarning($"Enemy prefab {enemyIndex} is missing");
            return null;
        }

        int spawnIndex = Random.Range(0, spawnpoints.Length);
        GameObject spawnPoint = spawnpoints[spawnIndex];
        return Instantiate(enemyPrefabs[enemyIndex], spawnPoint.transform);
    }
}
