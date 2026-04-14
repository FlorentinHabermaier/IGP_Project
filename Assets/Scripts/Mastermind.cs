using System;
using UnityEngine;

public class Mastermind : MonoBehaviour
{
    public static Mastermind instance;
   
   private float time;
   private float spawnspeedtime;
   private int stageCount = 0;
    private float hitspeed;
    private float hitdmg;
    private EnemySpawner spawner;
    [SerializeField]private float spawnspeed;

    private void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
         hitspeed = 5f;
    hitdmg = 5f;
    spawnspeed = .1f;
    }
    private void Update()
    {
        time += Time.deltaTime;
        spawnspeedtime += Time.deltaTime;
        if(spawnspeedtime > 10)
        {
            spawnspeed += .1f;
            spawnspeedtime = 0;
        }
        if(time > 60)
        {
            time = 0;
            stageCount ++;
            spawner.SpawnBoss();
        }
        if(stageCount == 5)
        {
            Victory();
        }
    }
  
public int getStageCount()
    {
        return stageCount;
    }
public float getSpawnSpeed()
    {
        return spawnspeed;
    }
    public float getHitSpeed()
    {
        return hitspeed;
    }
    public float getHitDmg()
    {
        return hitdmg;
    }
    public void setHitSpeed(float speed)
    {
        hitspeed = speed;
    }
    public void setHitDmg(float dmg)
    {
        hitdmg = dmg;
    }
    private void Awake()
    {
        instance = this;
    }
    public void GameOver()
    {
        Debug.Log("Game Over");
        Time.timeScale = 0f;
    }
    public void Victory()
    {
        Debug.Log("Victory");
        Time.timeScale = 0f;
    }
}
