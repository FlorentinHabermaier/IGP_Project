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
    private bool lifesteal = false;

    private float TowerHP;
    private float MaxTowerHP;

    [SerializeField]private TextMeshProUGUI bosstext;
    [SerializeField]private TextMeshProUGUI timeText;
    [SerializeField]private TextMeshProUGUI HPText;
    [SerializeField]private TextMeshProUGUI DmgText;
    [SerializeField]private TextMeshProUGUI GoldText;
    private float TowerUpgradeCost = 20;
    private float HitSpeedUpgradeCost = 20;
    private float HitDmgUpgradeCost = 50;
    private float LifestealCost = 100;
    private float SpikeCost = 30;
    private float Gold;
    private int rankenCount;
    private float rankenDamage = 9f;
    private float rankenDamageIncrease = 5f;
    private float timeTextTimer;
    private EnemySpawner spawner;
    [SerializeField]private float spawnspeed;
    [SerializeField] private GameObject[] rankenPrefabs;
    private GameObject[] activeRanken;

    [SerializeField] private ArduinoInput arduinoInput;

    private void Start()
    {
        TowerHP = 100f;
        MaxTowerHP = TowerHP;
        spawner = FindFirstObjectByType<EnemySpawner>();
        activeRanken = new GameObject[4];
         hitspeed = 5f;
        hitdmg = 5f;
        spawnspeed = .1f;
        bosstext.text = "";
        HPText.text = TowerHP.ToString();
        DmgText.text = "DPS: " + hitdmg * hitspeed;
        GoldText.text = Gold.ToString();

        UpdateLedProgress();
    }
    private void Update()
    {
        DmgText.text = "DPS: " + hitdmg * hitspeed;
        GoldText.text = "Gold " + Gold.ToString();
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
        if(time > 60)
        {
            time = 0;
            stageCount ++;
            UpdateLedProgress();
            if(stageCount == 6)
        {
            Victory();
        }
            bosstext.text = "Boss spawnt";
            spawner.SpawnBoss();
            StartCoroutine(Hide());
        }
    }
    public void TakeDamage(float dmg)
    {
        TowerHP -= dmg;
        HPText.text = TowerHP.ToString();
        if(TowerHP <= 0)
        {
           GameOver();
        }
    }
    public float getTowerHP()
    {
        return TowerHP;
    }
  public void setTowerHP(float hp)
    {
        this.TowerHP = hp;
    }
    public float getMaxTowerHP()
    {
        return MaxTowerHP;
    }
    public void setMaxTowerHP(float hp)
    {
        this.MaxTowerHP = hp;
    }
    public void setGold(float gold)
    {
        this.Gold = gold;
    }
    public float getGold()
    {
        return Gold;
    }
    public void GoldGained(float profit)
    {
        Gold += profit;
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
    public float getTowerUgpradeCost()
    {
        return TowerUpgradeCost;
    }
    public float getHitSpeedUpgradeCost()
    {
        return HitSpeedUpgradeCost;
    }
    public float getHitDmgUpgradeCost()
    {
        return HitDmgUpgradeCost;
    }
    public float getSpikeCost()
    {
        return SpikeCost;
    }
    public float getLifestealCost()
    {
        return LifestealCost;
    }
    public int getRankenCount()
    {
        return rankenCount;
    }
    public float getRankenDamage()
    {
        return rankenDamage;
    }
    public bool AreAllRankenUnlocked()
    {
        return rankenCount >= 4;
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
    public bool getLifesteal()
    {
        return lifesteal;
    }


    IEnumerator Hide()
    {
        yield return new WaitForSeconds(5);
        bosstext.enabled = false;
    }

    //Upgrades
    public void TowerUpgrade()
    {
        MaxTowerHP += 50;
        TowerHP = Mathf.Min(TowerHP + 50, MaxTowerHP);
        HPText.text = TowerHP.ToString();
        TowerUpgradeCost *= 2;
    }
    public void HitSpeedUpgrade()
    {
        hitspeed += 2.5f;
        HitSpeedUpgradeCost *= 2;
    }
    public void HitDmgUpgrade()
    {
        hitdmg += 2.5f;
        HitDmgUpgradeCost *= 2;
    }
    public void UnlockLifesteal()
    {
        lifesteal = true;
    }
    public bool SpikeUpgrade()
    {
        if (rankenCount < 4)
        {
            if (!UnlockNextRanke())
            {
                return false;
            }
        }
        else
        {
            rankenDamage += rankenDamageIncrease;
        }

        SpikeCost *= 2;
        return true;
    }

    private bool UnlockNextRanke()
    {
        if (rankenPrefabs == null || rankenCount >= rankenPrefabs.Length || rankenPrefabs[rankenCount] == null)
        {
            Debug.LogWarning("Ranken prefab is missing");
            return false;
        }

        Tower tower = FindFirstObjectByType<Tower>();
        Vector3 spawnPosition = rankenPrefabs[rankenCount].transform.localPosition;
        if (tower != null)
        {
            spawnPosition += tower.transform.position;
        }

        activeRanken[rankenCount] = Instantiate(rankenPrefabs[rankenCount], spawnPosition, rankenPrefabs[rankenCount].transform.rotation);
        rankenCount++;
        return true;
    }

    //für livesteal
    public void healing()
    {
        if(TowerHP >= MaxTowerHP)
        {
            return;
        }
        TowerHP = Mathf.Min(TowerHP + 1, MaxTowerHP);
        HPText.text = TowerHP.ToString();
    }

    // für arduino
    private void UpdateLedProgress()
    {
        if (arduinoInput == null)
        {
            arduinoInput = FindFirstObjectByType<ArduinoInput>();
        }

        if (arduinoInput == null)
            return;

        arduinoInput.SendLedCount(stageCount);
    }
}
