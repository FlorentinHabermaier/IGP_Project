using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Mastermind : MonoBehaviour
{
    public static Mastermind instance;
   
   private float time;
   private float spawnspeedtime;
   private int stageCount = 0;
    private float hitspeed;
    private float hitdmg;
    [SerializeField]private TextMeshProUGUI bosstext;
    [SerializeField]private TextMeshProUGUI timeText;
    private float timeTextTimer;
    private EnemySpawner spawner;
    [SerializeField]private float spawnspeed;

    private void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
         hitspeed = 5f;
        hitdmg = 5f;
        spawnspeed = .1f;
        bosstext.text = "";
    }
    private void Update()
    {
        timeTextTimer += Time.deltaTime;

        int minutes = Mathf.FloorToInt(timeTextTimer / 60f);
        int seconds = Mathf.FloorToInt(timeTextTimer % 60f);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        time += Time.deltaTime;
        spawnspeedtime += Time.deltaTime;
        if(spawnspeedtime > 10)
        {
            spawnspeed += .1f;
            spawnspeedtime = 0;
        }
        if(time > 10)
        {
            time = 0;
            stageCount ++;
            if(stageCount == 5)
        {
            Victory();
        }
            bosstext.text = "Boss spawnt";
            spawner.SpawnBoss();
            StartCoroutine(Hide());
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

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(5);
        bosstext.enabled = false;
    }
}
