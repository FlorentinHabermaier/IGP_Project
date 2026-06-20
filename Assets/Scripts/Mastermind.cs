using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Mastermind : MonoBehaviour
{
    public static Mastermind instance;
    private static readonly float[] BossSpawnTimes = { 60f, 120f, 180f, 300f };
    private static readonly int[] BossEnemyIndices = { 1, 2, 3, 4 };
    private const int FinalBossEnemyIndex = 4;
    private const float DefaultTowerHP = 100f;
    private const float DefaultHitSpeed = 5f;
    private const float DefaultHitDamage = 5f;
    private const float DefaultTowerUpgradeCost = 20f;
    private const float DefaultHitSpeedUpgradeCost = 20f;
    private const float DefaultHitDamageUpgradeCost = 50f;
    private const float DefaultLifestealCost = 100f;
    private const float DefaultSpikeCost = 30f;
    private const float DefaultRankenDamage = 9f;

    private int stageCount = 0;
    private int nextBossSpawnIndex;
    private readonly bool[] unlockedBossEnemyTypes = new bool[FinalBossEnemyIndex + 1];
    private float hitspeed;
    private float hitdmg;
    private bool lifesteal = false;
    private bool isGameOver;
    private bool isPaused;
    private bool isShopOpen;

    private float TowerHP;
    private float MaxTowerHP;

    [SerializeField]private TextMeshProUGUI bosstext;
    [SerializeField]private TextMeshProUGUI timeText;
    [SerializeField]private TextMeshProUGUI HPText;
    [SerializeField]private TextMeshProUGUI DmgText;
    [SerializeField]private TextMeshProUGUI GoldText;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private TextMeshProUGUI victoryDeathCountText;
    [SerializeField] private bool playtestTimeSpeedX4;
    [SerializeField] private int deathCount;
    [SerializeField] private float bossSpawnShakeDuration = 0.45f;
    [SerializeField] private float bossSpawnShakeStrength = 0.3f;
    [SerializeField] private float damageFlashDuration = 0.72f;
    [SerializeField] private float damageFlashAlpha = 0.14f;
    private float TowerUpgradeCost = DefaultTowerUpgradeCost;
    private float HitSpeedUpgradeCost = DefaultHitSpeedUpgradeCost;
    private float HitDmgUpgradeCost = DefaultHitDamageUpgradeCost;
    private float LifestealCost = DefaultLifestealCost;
    private float SpikeCost = DefaultSpikeCost;
    private float Gold;
    private int rankenCount;
    private float rankenDamage = 9f;
    private float rankenDamageIncrease = 5f;
    private float timeTextTimer;
    private EnemySpawner spawner;
    [SerializeField] private GameObject[] rankenPrefabs;
    private GameObject[] activeRanken;
    private Transform mainCameraTransform;
    private Vector3 mainCameraBaseLocalPosition;
    private Image damageFlashOverlay;
    private Coroutine cameraShakeCoroutine;
    private Coroutine damageFlashCoroutine;

    [SerializeField] private ArduinoInput arduinoInput;

    private void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
        InitializeScreenEffects();
        CreateActiveRankenArray();
        ResetPersistentProgress();
        bosstext.text = "";
        bosstext.enabled = false;
        HideAllScreens();
        HPText.text = TowerHP.ToString();
        DmgText.text = "DPS: " + hitdmg * hitspeed;
        GoldText.text = "Gold " + Gold.ToString();
        UpdateTimeText();
        UpdateVictoryDeathCountText();

        UpdateLedProgress();
        ApplyTimeScale();
    }
    private void Update()
    {
        if (isGameOver)
        {
            return;
        }

        if (isPaused || isShopOpen)
        {
            return;
        }

        DmgText.text = "DPS: " + hitdmg * hitspeed;
        GoldText.text = "Gold " + Gold.ToString();
        timeTextTimer += Time.deltaTime;

        UpdateTimeText();

        if (nextBossSpawnIndex < BossSpawnTimes.Length && timeTextTimer >= BossSpawnTimes[nextBossSpawnIndex])
        {
            stageCount ++;
            UpdateLedProgress();
            bosstext.enabled = true;
            bosstext.text = "Boss " + stageCount + " spawnt";
            spawner.SpawnBoss(stageCount, BossEnemyIndices[nextBossSpawnIndex]);
            TriggerBossSpawnEffect();
            nextBossSpawnIndex++;
            StartCoroutine(Hide());
        }
    }
    public void TakeDamage(float dmg)
    {
        if (isGameOver)
        {
            return;
        }

        TowerHP -= dmg;
        HPText.text = TowerHP.ToString();
        TriggerDamageTakenEffect();
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
    public bool IsEnemyTypeUnlocked(int enemyIndex)
    {
        if (enemyIndex <= 0)
        {
            return true;
        }

        if (enemyIndex >= unlockedBossEnemyTypes.Length)
        {
            return false;
        }

        return unlockedBossEnemyTypes[enemyIndex];
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

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            ApplyTimeScale();
        }
    }

    public void GameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;
        isPaused = false;
        isShopOpen = false;
        deathCount++;
        Debug.Log("Game Over");
        bosstext.enabled = false;
        UpdateVictoryDeathCountText();
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(false);
        }
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }
        ApplyTimeScale();
    }
    public void Victory()
    {
        isGameOver = true;
        isPaused = false;
        isShopOpen = false;
        Debug.Log("Victory");
        UpdateVictoryDeathCountText();
        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true);
        }
        ApplyTimeScale();
    }

    public void BossDefeated(int bossEnemyIndex)
    {
        UnlockBossEnemyType(bossEnemyIndex);

        if (bossEnemyIndex >= FinalBossEnemyIndex)
        {
            Victory();
        }
    }

    public void TryAgain()
    {
        ResetRun(false);
    }

    public void RestartFromVictory()
    {
        ResetRun(true);
    }

    public void TogglePauseMenu()
    {
        if (isGameOver || deathScreen != null && deathScreen.activeSelf)
        {
            return;
        }

        isPaused = !isPaused;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isPaused);
        }

        ApplyTimeScale();
    }

    public void NotifyShopOpened()
    {
        if (isGameOver)
        {
            return;
        }

        isShopOpen = true;
        ApplyTimeScale();
    }

    public void NotifyShopClosed()
    {
        isShopOpen = false;
        ApplyTimeScale();
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

    private void UpdateTimeText()
    {
        int minutes = Mathf.FloorToInt(timeTextTimer / 60f);
        int seconds = Mathf.FloorToInt(timeTextTimer % 60f);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void ClearEnemies()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            enemy.gameObject.SetActive(false);
            Destroy(enemy.gameObject);
        }
    }

    private void ResetRun(bool resetProgress)
    {
        StopAllCoroutines();
        ClearEnemies();
        ResetScreenEffects();

        if (resetProgress)
        {
            ResetPersistentProgress();
        }

        ResetBossEnemyUnlocks();

        stageCount = 0;
        nextBossSpawnIndex = 0;
        timeTextTimer = 0f;
        isGameOver = false;
        isPaused = false;
        isShopOpen = false;

        if (!resetProgress)
        {
            TowerHP = MaxTowerHP;
        }

        if (spawner != null)
        {
            spawner.ResetSpawner();
        }

        bosstext.text = "";
        bosstext.enabled = false;
        HPText.text = TowerHP.ToString();
        DmgText.text = "DPS: " + hitdmg * hitspeed;
        GoldText.text = "Gold " + Gold.ToString();
        UpdateTimeText();
        UpdateVictoryDeathCountText();
        UpdateLedProgress();
        HideAllScreens();
        ApplyTimeScale();
    }

    private void ResetPersistentProgress()
    {
        ClearRanken();
        CreateActiveRankenArray();
        ResetBossEnemyUnlocks();

        TowerHP = DefaultTowerHP;
        MaxTowerHP = DefaultTowerHP;
        hitspeed = DefaultHitSpeed;
        hitdmg = DefaultHitDamage;
        Gold = 0f;
        lifesteal = false;
        TowerUpgradeCost = DefaultTowerUpgradeCost;
        HitSpeedUpgradeCost = DefaultHitSpeedUpgradeCost;
        HitDmgUpgradeCost = DefaultHitDamageUpgradeCost;
        LifestealCost = DefaultLifestealCost;
        SpikeCost = DefaultSpikeCost;
        rankenCount = 0;
        rankenDamage = DefaultRankenDamage;
        deathCount = 0;
    }

    private void ResetBossEnemyUnlocks()
    {
        for (int i = 0; i < unlockedBossEnemyTypes.Length; i++)
        {
            unlockedBossEnemyTypes[i] = false;
        }
    }

    private void UnlockBossEnemyType(int enemyIndex)
    {
        if (enemyIndex <= 0 || enemyIndex >= unlockedBossEnemyTypes.Length)
        {
            return;
        }

        unlockedBossEnemyTypes[enemyIndex] = true;
    }

    private void ClearRanken()
    {
        if (activeRanken == null)
        {
            return;
        }

        foreach (GameObject ranke in activeRanken)
        {
            if (ranke == null)
            {
                continue;
            }

            ranke.SetActive(false);
            Destroy(ranke);
        }
    }

    private void CreateActiveRankenArray()
    {
        int rankenSlots = rankenPrefabs != null && rankenPrefabs.Length > 0 ? rankenPrefabs.Length : 4;
        activeRanken = new GameObject[rankenSlots];
    }

    private void InitializeScreenEffects()
    {
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
            mainCameraBaseLocalPosition = mainCameraTransform.localPosition;
        }

        EnsureDamageFlashOverlay();
        SetDamageFlashAlpha(0f);
    }

    private void HideAllScreens()
    {
        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(false);
        }
    }

    private void TriggerBossSpawnEffect()
    {
        if (mainCameraTransform == null || bossSpawnShakeDuration <= 0f || bossSpawnShakeStrength <= 0f)
        {
            return;
        }

        if (cameraShakeCoroutine != null)
        {
            StopCoroutine(cameraShakeCoroutine);
        }

        mainCameraTransform.localPosition = mainCameraBaseLocalPosition;
        cameraShakeCoroutine = StartCoroutine(ShakeCamera());
    }

    private void TriggerDamageTakenEffect()
    {
        EnsureDamageFlashOverlay();
        if (damageFlashOverlay == null || damageFlashDuration <= 0f || damageFlashAlpha <= 0f)
        {
            return;
        }

        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
        }

        damageFlashCoroutine = StartCoroutine(FlashDamageOverlay());
    }

    private IEnumerator ShakeCamera()
    {
        float timer = 0f;

        while (timer < bossSpawnShakeDuration)
        {
            timer += Time.unscaledDeltaTime;
            Vector2 offset = UnityEngine.Random.insideUnitCircle * bossSpawnShakeStrength;
            mainCameraTransform.localPosition = mainCameraBaseLocalPosition + new Vector3(offset.x, offset.y, 0f);
            yield return null;
        }

        mainCameraTransform.localPosition = mainCameraBaseLocalPosition;
        cameraShakeCoroutine = null;
    }

    private IEnumerator FlashDamageOverlay()
    {
        float timer = 0f;
        SetDamageFlashAlpha(damageFlashAlpha);

        while (timer < damageFlashDuration)
        {
            timer += Time.unscaledDeltaTime;
            float normalizedTime = Mathf.Clamp01(timer / damageFlashDuration);
            SetDamageFlashAlpha(Mathf.Lerp(damageFlashAlpha, 0f, normalizedTime));
            yield return null;
        }

        SetDamageFlashAlpha(0f);
        damageFlashCoroutine = null;
    }

    private void EnsureDamageFlashOverlay()
    {
        if (damageFlashOverlay != null)
        {
            return;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        GameObject overlayObject = new GameObject("DamageFlashOverlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        overlayObject.transform.SetParent(canvas.transform, false);
        overlayObject.transform.SetAsLastSibling();

        RectTransform overlayTransform = overlayObject.GetComponent<RectTransform>();
        overlayTransform.anchorMin = Vector2.zero;
        overlayTransform.anchorMax = Vector2.one;
        overlayTransform.offsetMin = Vector2.zero;
        overlayTransform.offsetMax = Vector2.zero;

        damageFlashOverlay = overlayObject.GetComponent<Image>();
        damageFlashOverlay.raycastTarget = false;
        SetDamageFlashAlpha(0f);
    }

    private void ResetScreenEffects()
    {
        if (cameraShakeCoroutine != null)
        {
            StopCoroutine(cameraShakeCoroutine);
            cameraShakeCoroutine = null;
        }

        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
            damageFlashCoroutine = null;
        }

        if (mainCameraTransform != null)
        {
            mainCameraTransform.localPosition = mainCameraBaseLocalPosition;
        }

        SetDamageFlashAlpha(0f);
    }

    private void SetDamageFlashAlpha(float alpha)
    {
        if (damageFlashOverlay == null)
        {
            return;
        }

        damageFlashOverlay.color = new Color(0.8f, 0.1f, 0.1f, alpha);
    }

    private void UpdateVictoryDeathCountText()
    {
        if (victoryDeathCountText == null)
        {
            return;
        }

        victoryDeathCountText.text = $"Deaths: {deathCount}";
    }

    private void ApplyTimeScale()
    {
        if (isGameOver || isPaused || isShopOpen)
        {
            Time.timeScale = 0f;
            return;
        }

        Time.timeScale = playtestTimeSpeedX4 ? 4f : 1f;
    }
}
