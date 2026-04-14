using System;
using UnityEngine;

public class Mastermind : MonoBehaviour
{
    public static Mastermind instance;
   
    private float hitspeed;
    private float hitdmg;

    private void Start()
{
    hitspeed = 5f;
    hitdmg = 1f;
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
    }
    private void Victory()
    {
        Debug.Log("Victory");
    }
}
