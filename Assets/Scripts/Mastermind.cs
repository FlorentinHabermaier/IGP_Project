using System;
using UnityEngine;

public class Mastermind : MonoBehaviour
{
    public static Mastermind instance;
   
   private float time;
   private int stageCount = 0;
    private float hitspeed;
    private float hitdmg;
    private float spawnspeed;

    private void Update()
    {
        time += Time.deltaTime;
        if(time > 60)
        {
            time = 0;
            stageCount ++;
        }
        if(stageCount == 4)
        {
            Victory();
        }
    }
    private void Start()
{
    hitspeed = 5f;
    hitdmg = 5f;
    spawnspeed = 1;
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
    }
}
