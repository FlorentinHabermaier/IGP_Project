using UnityEngine;
using UnityEngine.UIElements;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private UIDocument shopUI;
    private VisualElement root;

    private Button buyTowerHpButton;
    private Button buyHitSpeedButton;
    private Button buyHitDmgButton;
    private Button buySpikesButton;
    private Button buyLifestealButton;
    private Button closeShopButton;
    private Button[] shopButtons;
    private int selectedIndex = 0;
    private ScrollView shopScrollView;

    private Label towerHpPriceLabel;
    private Label hitSpeedPriceLabel;
    private Label hitDmgPriceLabel;
    private Label spikesNameLabel;
    private Label spikesDescriptionLabel;
    private Label spikesPriceLabel;
    private Label lifestealPriceLabel;
    private Label goldLabel;

    private void Start()
    {
        root = shopUI.rootVisualElement;
        shopScrollView = root.Q<ScrollView>();
        root.style.display = DisplayStyle.None;

        buyTowerHpButton = root.Q<Button>("buy-towerhp-button");
        buyHitSpeedButton = root.Q<Button>("buy-hitspeed-button");
        buyHitDmgButton = root.Q<Button>("buy-hitdmg-button");
        buySpikesButton = root.Q<Button>("buy-spikes-button");
        buyLifestealButton = root.Q<Button>("buy-lifesteal-button");
        closeShopButton = root.Q<Button>("close-shop-button");

        towerHpPriceLabel = root.Q<Label>("towerhp-price-label");
        hitSpeedPriceLabel = root.Q<Label>("hitspeed-price-label");
        hitDmgPriceLabel = root.Q<Label>("hitdmg-price-label");
        spikesNameLabel = root.Q<Label>("spikes-name-label");
        spikesDescriptionLabel = root.Q<Label>("spikes-description-label");
        spikesPriceLabel = root.Q<Label>("spikes-price-label");
        lifestealPriceLabel = root.Q<Label>("lifesteal-price-label");
        goldLabel = root.Q<Label>("gold-label");

        buyTowerHpButton.clicked += BuyTowerHP;
        buyHitSpeedButton.clicked += BuyHitSpeed;
        buyHitDmgButton.clicked += BuyHitDmg;
        buySpikesButton.clicked += BuySpikes;
        buyLifestealButton.clicked += BuyLifesteal;
        closeShopButton.clicked += CloseShop;

        shopButtons = new Button[]
        {
            buyTowerHpButton,
            buyHitSpeedButton,
            buyHitDmgButton,
            buySpikesButton,
            buyLifestealButton,
            closeShopButton
        };

        RefreshShop();
    }

    public void OpenShop()
    {
        root.style.display = DisplayStyle.Flex;
        if (Mastermind.instance != null)
        {
            Mastermind.instance.NotifyShopOpened();
        }
        else
        {
            Time.timeScale = 0f;
        }
        RefreshShop();

        selectedIndex = 0;
        UpdateSelectionVisual();
    }

    public void CloseShop()
    {
        root.style.display = DisplayStyle.None;
        if (Mastermind.instance != null)
        {
            Mastermind.instance.NotifyShopClosed();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void ToggleShop()
    {
        if (root.style.display == DisplayStyle.None)
        {
            OpenShop();
        }
        else
        {
            CloseShop();
        }
    }

    private void RefreshShop()
    {
        if (Mastermind.instance == null)
            return;

        float gold = Mastermind.instance.getGold();

        float towerCost = Mastermind.instance.getTowerUgpradeCost();
        float hitSpeedCost = Mastermind.instance.getHitSpeedUpgradeCost();
        float hitDmgCost = Mastermind.instance.getHitDmgUpgradeCost();
        float spikeCost = Mastermind.instance.getSpikeCost();
        float lifestealCost = Mastermind.instance.getLifestealCost();

        goldLabel.text = "Gold: " + gold.ToString("0");

        towerHpPriceLabel.text = towerCost.ToString("0");
        hitSpeedPriceLabel.text = hitSpeedCost.ToString("0");
        hitDmgPriceLabel.text = hitDmgCost.ToString("0");
        spikesPriceLabel.text = spikeCost.ToString("0");
        lifestealPriceLabel.text = lifestealCost.ToString("0");

        if (Mastermind.instance.AreAllRankenUnlocked())
        {
            spikesNameLabel.text = "Ranken Damage";
            spikesDescriptionLabel.text = "Erhoeht den Schaden aller Ranken";
        }
        else
        {
            spikesNameLabel.text = "Ranken";
            spikesDescriptionLabel.text = "Schaltet die naechste Ranke frei";
        }

        SetButtonState(buyTowerHpButton, gold >= towerCost);
        SetButtonState(buyHitSpeedButton, gold >= hitSpeedCost);
        SetButtonState(buyHitDmgButton, gold >= hitDmgCost);
        SetButtonState(buySpikesButton, gold >= spikeCost);

        bool lifestealBought = Mastermind.instance.getLifesteal();
        bool canBuyLifesteal = gold >= lifestealCost && !lifestealBought;

        SetButtonState(buyLifestealButton, canBuyLifesteal);
        buyLifestealButton.text = lifestealBought ? "Gekauft" : "Kaufen";
    }

    private void SetButtonState(Button button, bool canBuy)
    {
        button.SetEnabled(canBuy);
        button.EnableInClassList("buy-button-disabled", !canBuy);
    }

    private void BuyTowerHP()
    {
        float cost = Mastermind.instance.getTowerUgpradeCost();

        if (Mastermind.instance.getGold() < cost)
            return;

        Mastermind.instance.setGold(Mastermind.instance.getGold() - cost);
        Mastermind.instance.TowerUpgrade();
        RefreshShop();
    }

    private void BuyHitSpeed()
    {
        float cost = Mastermind.instance.getHitSpeedUpgradeCost();

        if (Mastermind.instance.getGold() < cost)
            return;

        Mastermind.instance.setGold(Mastermind.instance.getGold() - cost);
        Mastermind.instance.HitSpeedUpgrade();
        RefreshShop();
    }

    private void BuyHitDmg()
    {
        float cost = Mastermind.instance.getHitDmgUpgradeCost();

        if (Mastermind.instance.getGold() < cost)
            return;

        Mastermind.instance.setGold(Mastermind.instance.getGold() - cost);
        Mastermind.instance.HitDmgUpgrade();
        RefreshShop();
    }

    private void BuySpikes()
    {
        float cost = Mastermind.instance.getSpikeCost();

        if (Mastermind.instance.getGold() < cost)
            return;

        if (Mastermind.instance.SpikeUpgrade())
        {
            Mastermind.instance.setGold(Mastermind.instance.getGold() - cost);
        }
        RefreshShop();
    }

    private void BuyLifesteal()
    {
        float cost = Mastermind.instance.getLifestealCost();

        if (Mastermind.instance.getLifesteal())
            return;

        if (Mastermind.instance.getGold() < cost)
            return;

        Mastermind.instance.setGold(Mastermind.instance.getGold() - cost);
        Mastermind.instance.UnlockLifesteal();
        RefreshShop();
    }

    public bool IsOpen()
    {
        return root.style.display != DisplayStyle.None;
    }

        public void SelectNext()
    {
        selectedIndex++;
        if (selectedIndex >= shopButtons.Length)
            selectedIndex = 0;

        UpdateSelectionVisual();
    }

    public void SelectPrevious()
    {
        selectedIndex--;
        if (selectedIndex < 0)
            selectedIndex = shopButtons.Length - 1;

        UpdateSelectionVisual();
    }

    public void BuySelected()
    {
        if (selectedIndex == 0) BuyTowerHP();
        else if (selectedIndex == 1) BuyHitSpeed();
        else if (selectedIndex == 2) BuyHitDmg();
        else if (selectedIndex == 3) BuySpikes();
        else if (selectedIndex == 4) BuyLifesteal();
        else if (selectedIndex == 5)
        {
            CloseShop();
            return;
        }

        UpdateSelectionVisual();
    }

    private void UpdateSelectionVisual()
    {
        if (shopButtons == null) return;

        for (int i = 0; i < shopButtons.Length; i++)
        {
            shopButtons[i].EnableInClassList("shop-button-selected", i == selectedIndex);
        }

        shopButtons[selectedIndex].Focus();

        if (shopScrollView != null)
        {
            shopScrollView.ScrollTo(shopButtons[selectedIndex]);
        }
    }
}