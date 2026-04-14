using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]private GameObject [] spawnpoints;
    [SerializeField]private GameObject [] enemyPrefabs;
    private GameObject spawn;
    private GameObject enemy;

    private float timer;
    private float spawntime;
    private int stage;

    private void Update()
    {
        stage = Mastermind.instance.getStageCount();
        spawntime = 1/ Mastermind.instance.getSpawnSpeed();

        timer += Time.deltaTime;

        if(timer >= spawntime)
        {
            var x = Random.Range(0, spawnpoints.Length);
            spawn = spawnpoints[x];
            /*if(stage > enemyPrefabs.Length)
            {
                stage = enemyPrefabs.Length;
            }*/
            enemy = enemyPrefabs[0];
            Instantiate(enemy, spawn.transform);

            timer = 0;
        }
    }
    public void SpawnBoss()
    {
       int count = Mastermind.instance.getStageCount();
       if(count > enemyPrefabs.Length -1)
        {
            count = enemyPrefabs.Length -1;
        }
        var x = Random.Range(0, spawnpoints.Length);
            spawn = spawnpoints[x];
            enemy = enemyPrefabs[count];
            Instantiate(enemy, spawn.transform);

    }
}
