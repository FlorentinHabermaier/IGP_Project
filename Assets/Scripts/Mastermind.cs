using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Mastermind : MonoBehaviour
{
    public static Mastermind instance;
    private const string MainMenuSceneName = "MainMenu";
    private const string RuntimeMenuCardName = "RuntimeMenuCard";
    private const string RuntimeMainMenuButtonName = "MainMenuButton";
    private static readonly Vector2 MenuCardSize = new Vector2(430f, 360f);
    private static readonly Vector2 MenuButtonSize = new Vector2(220f, 70f);
    private static readonly Vector2 HudButtonSize = new Vector2(150f, 48f);
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
    [Header("UI Art")]
    [SerializeField] private Sprite menuPanelSprite;
    [SerializeField] private Sprite titleBannerSprite;
    [SerializeField] private Sprite primaryButtonSprite;
    [SerializeField] private Sprite primaryButtonPressedSprite;
    [SerializeField] private Sprite secondaryButtonSprite;
    [SerializeField] private Sprite secondaryButtonPressedSprite;
    [SerializeField] private Sprite buttonHoverSprite;
    [SerializeField] private Sprite buttonDisabledSprite;
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
        ConfigureGameplayButtons();
        ConfigureMenuScreens();
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
            int bossEnemyIndex = BossEnemyIndices[nextBossSpawnIndex];
            bool isFinalBossSpawn = bossEnemyIndex >= FinalBossEnemyIndex;
            bosstext.enabled = true;
            bosstext.text = isFinalBossSpawn
                ? "Boss " + stageCount + " spawnt\nkill the final boss"
                : "Boss " + stageCount + " spawnt";
            spawner.SpawnBoss(stageCount, bossEnemyIndex);
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
        ResetScreenEffects();
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
        ResetScreenEffects();
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

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenuSceneName);
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

    private void ConfigureMenuScreens()
    {
        ConfigureMenuScreen(
            pauseMenu,
            "PAUSE",
            "Weiter",
            true,
            new Color(0.025f, 0.035f, 0.045f, 0.84f),
            new Color(0.13f, 0.10f, 0.075f, 0.96f),
            new Color(1f, 0.86f, 0.46f, 1f),
            new Color(0.26f, 0.36f, 0.48f, 1f));

        ConfigureMenuScreen(
            deathScreen,
            "GAME OVER",
            "Nochmal",
            true,
            new Color(0.08f, 0.025f, 0.025f, 0.86f),
            new Color(0.15f, 0.085f, 0.07f, 0.96f),
            new Color(1f, 0.42f, 0.32f, 1f),
            new Color(0.54f, 0.20f, 0.18f, 1f));

        ConfigureMenuScreen(
            victoryScreen,
            "VICTORY",
            "Neu starten",
            true,
            new Color(0.08f, 0.06f, 0.02f, 0.82f),
            new Color(0.15f, 0.105f, 0.06f, 0.96f),
            new Color(1f, 0.88f, 0.36f, 1f),
            new Color(0.35f, 0.45f, 0.30f, 1f));
    }

    private void ConfigureMenuScreen(
        GameObject screen,
        string titleText,
        string primaryButtonText,
        bool addMainMenuButton,
        Color overlayColor,
        Color panelColor,
        Color accentColor,
        Color buttonColor)
    {
        if (screen == null)
        {
            return;
        }

        Image overlayImage = screen.GetComponent<Image>();
        if (overlayImage != null)
        {
            overlayImage.color = overlayColor;
            overlayImage.raycastTarget = true;
        }

        EnsureMenuCard(screen, panelColor, accentColor);
        EnsureTitleBanner(screen);

        if (addMainMenuButton)
        {
            EnsureMainMenuButton(screen);
        }

        StyleMenuTexts(screen, titleText, accentColor);
        StyleMenuButtons(screen, primaryButtonText, buttonColor, accentColor);
        LayoutMenuButtons(screen);
    }

    private void ConfigureGameplayButtons()
    {
        foreach (Button button in FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            if (IsInsideScreen(button.transform))
            {
                continue;
            }

            StyleButton(button, false, new Color(1f, 0.95f, 0.80f, 1f));

            RectTransform rectTransform = button.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = HudButtonSize;
            }
        }
    }

    private void EnsureMenuCard(GameObject screen, Color panelColor, Color accentColor)
    {
        Transform cardTransform = screen.transform.Find(RuntimeMenuCardName);
        GameObject cardObject = cardTransform != null ? cardTransform.gameObject : null;

        if (cardObject == null)
        {
            cardObject = new GameObject(RuntimeMenuCardName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            cardObject.layer = screen.layer;
            cardObject.transform.SetParent(screen.transform, false);
        }

        cardObject.transform.SetAsFirstSibling();

        RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = MenuCardSize;

        Image image = cardObject.GetComponent<Image>();
        if (menuPanelSprite != null)
        {
            image.sprite = menuPanelSprite;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
        }
        else
        {
            image.color = panelColor;
        }

        image.raycastTarget = false;

        Shadow shadow = GetOrAddShadow(cardObject);
        shadow.effectColor = new Color(0f, 0f, 0f, 0.55f);
        shadow.effectDistance = new Vector2(8f, -8f);
        shadow.useGraphicAlpha = false;

        Outline outline = GetOrAddComponent<Outline>(cardObject);
        outline.effectColor = accentColor;
        outline.effectDistance = new Vector2(3f, -3f);
        outline.useGraphicAlpha = false;
    }

    private void EnsureTitleBanner(GameObject screen)
    {
        if (titleBannerSprite == null)
        {
            return;
        }

        const string titleBannerName = "RuntimeTitleBanner";
        Transform bannerTransform = screen.transform.Find(titleBannerName);
        GameObject bannerObject = bannerTransform != null ? bannerTransform.gameObject : null;

        if (bannerObject == null)
        {
            bannerObject = new GameObject(titleBannerName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            bannerObject.layer = screen.layer;
            bannerObject.transform.SetParent(screen.transform, false);
        }

        bannerObject.transform.SetSiblingIndex(1);

        RectTransform rectTransform = bannerObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(0f, 108f);
        rectTransform.sizeDelta = new Vector2(390f, 86f);

        Image image = bannerObject.GetComponent<Image>();
        image.sprite = titleBannerSprite;
        image.type = Image.Type.Simple;
        image.color = Color.white;
        image.raycastTarget = false;
    }

    private void EnsureMainMenuButton(GameObject screen)
    {
        Transform existingButtonTransform = screen.transform.Find(RuntimeMainMenuButtonName);
        Button button = existingButtonTransform != null ? existingButtonTransform.GetComponent<Button>() : null;

        if (button == null)
        {
            GameObject buttonObject = new GameObject(RuntimeMainMenuButtonName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonObject.layer = screen.layer;
            buttonObject.transform.SetParent(screen.transform, false);
            button = buttonObject.GetComponent<Button>();

            GameObject labelObject = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            labelObject.layer = screen.layer;
            labelObject.transform.SetParent(buttonObject.transform, false);

            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI template = screen.GetComponentInChildren<TextMeshProUGUI>(true);
            if (template != null && template.font != null)
            {
                label.font = template.font;
            }
        }

        button.onClick.RemoveListener(ReturnToMainMenu);
        button.onClick.AddListener(ReturnToMainMenu);
    }

    private void StyleMenuTexts(GameObject screen, string titleText, Color accentColor)
    {
        bool titleStyled = false;
        foreach (Text text in screen.GetComponentsInChildren<Text>(true))
        {
            if (IsInsideButton(text.transform))
            {
                continue;
            }

            text.text = titleText;
            text.fontSize = 52;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = accentColor;
            PositionMenuText(text.rectTransform, new Vector2(0f, 104f), new Vector2(380f, 72f));
            titleStyled = true;
            break;
        }

        TextMeshProUGUI subtitleText = null;
        foreach (TextMeshProUGUI text in screen.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (IsInsideButton(text.transform))
            {
                continue;
            }

            if (!titleStyled)
            {
                text.text = titleText;
                text.fontSize = 54;
                text.fontStyle = FontStyles.Bold;
                text.alignment = TextAlignmentOptions.Center;
                text.color = accentColor;
                PositionMenuText(text.rectTransform, new Vector2(0f, 104f), new Vector2(380f, 72f));
                titleStyled = true;
                continue;
            }

            subtitleText = text;
        }

        if (subtitleText != null)
        {
            subtitleText.fontSize = 30;
            subtitleText.fontStyle = FontStyles.Bold;
            subtitleText.alignment = TextAlignmentOptions.Center;
            subtitleText.color = new Color(1f, 0.93f, 0.74f, 1f);
            PositionMenuText(subtitleText.rectTransform, new Vector2(0f, 42f), new Vector2(360f, 44f));
        }
    }

    private void PositionMenuText(RectTransform rectTransform, Vector2 position, Vector2 size)
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;
    }

    private void StyleMenuButtons(GameObject screen, string primaryButtonText, Color buttonColor, Color accentColor)
    {
        foreach (Button button in screen.GetComponentsInChildren<Button>(true))
        {
            bool isMainMenuButton = button.gameObject.name == RuntimeMainMenuButtonName;
            SetButtonLabel(button, isMainMenuButton ? "Main Menu" : primaryButtonText);
            StyleButton(button, isMainMenuButton, new Color(1f, 0.95f, 0.80f, 1f));
        }
    }

    private void StyleButton(Button button, bool secondary, Color textColor)
    {
        Sprite normalSprite = secondary && secondaryButtonSprite != null ? secondaryButtonSprite : primaryButtonSprite;
        Sprite pressedSprite = secondary && secondaryButtonPressedSprite != null ? secondaryButtonPressedSprite : primaryButtonPressedSprite;

        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            if (normalSprite != null)
            {
                image.sprite = normalSprite;
                image.type = Image.Type.Simple;
                image.preserveAspect = false;
                image.raycastTarget = true;
                image.color = Color.white;
            }
            else
            {
                image.color = secondary ? new Color(0.54f, 0.20f, 0.18f, 1f) : new Color(0.26f, 0.36f, 0.48f, 1f);
            }
        }

        SpriteState spriteState = button.spriteState;
        spriteState.highlightedSprite = buttonHoverSprite;
        spriteState.selectedSprite = buttonHoverSprite;
        spriteState.pressedSprite = pressedSprite;
        spriteState.disabledSprite = buttonDisabledSprite;
        button.spriteState = spriteState;

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = Color.white;
        colors.selectedColor = Color.white;
        colors.disabledColor = Color.white;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        Outline outline = button.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
        }

        SetButtonTextColor(button, textColor);
    }

    private void SetButtonLabel(Button button, string labelText)
    {
        TextMeshProUGUI tmpLabel = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmpLabel != null)
        {
            tmpLabel.text = labelText;
            tmpLabel.fontSize = 24;
            tmpLabel.fontStyle = FontStyles.Bold;
            tmpLabel.alignment = TextAlignmentOptions.Center;
            tmpLabel.color = new Color(1f, 0.95f, 0.80f, 1f);
            tmpLabel.raycastTarget = false;
            return;
        }

        Text label = button.GetComponentInChildren<Text>(true);
        if (label != null)
        {
            label.text = labelText;
            label.fontSize = 24;
            label.fontStyle = FontStyle.Bold;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color(1f, 0.95f, 0.80f, 1f);
            label.raycastTarget = false;
        }
    }

    private void SetButtonTextColor(Button button, Color textColor)
    {
        TextMeshProUGUI tmpLabel = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmpLabel != null)
        {
            tmpLabel.color = textColor;
            tmpLabel.fontStyle = FontStyles.Bold;
            tmpLabel.raycastTarget = false;
        }

        Text label = button.GetComponentInChildren<Text>(true);
        if (label != null)
        {
            label.color = textColor;
            label.fontStyle = FontStyle.Bold;
            label.raycastTarget = false;
        }
    }

    private void LayoutMenuButtons(GameObject screen)
    {
        Button[] buttons = screen.GetComponentsInChildren<Button>(true);
        Array.Sort(buttons, (left, right) => left.transform.GetSiblingIndex().CompareTo(right.transform.GetSiblingIndex()));

        float firstY = buttons.Length > 1 ? -36f : -76f;
        const float buttonSpacing = 82f;

        for (int i = 0; i < buttons.Length; i++)
        {
            RectTransform rectTransform = buttons[i].GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(0f, firstY - i * buttonSpacing);
            rectTransform.sizeDelta = MenuButtonSize;
        }
    }

    private bool IsInsideButton(Transform transformToCheck)
    {
        Transform current = transformToCheck.parent;
        while (current != null)
        {
            if (current.GetComponent<Button>() != null)
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    private bool IsInsideScreen(Transform transformToCheck)
    {
        return IsChildOf(transformToCheck, pauseMenu)
            || IsChildOf(transformToCheck, deathScreen)
            || IsChildOf(transformToCheck, victoryScreen);
    }

    private bool IsChildOf(Transform transformToCheck, GameObject parent)
    {
        if (parent == null)
        {
            return false;
        }

        Transform current = transformToCheck;
        while (current != null)
        {
            if (current.gameObject == parent)
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    private T GetOrAddComponent<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();
        if (component != null)
        {
            return component;
        }

        return target.AddComponent<T>();
    }

    private Shadow GetOrAddShadow(GameObject target)
    {
        foreach (Shadow shadow in target.GetComponents<Shadow>())
        {
            if (shadow.GetType() == typeof(Shadow))
            {
                return shadow;
            }
        }

        return target.AddComponent<Shadow>();
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
