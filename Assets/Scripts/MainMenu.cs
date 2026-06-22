using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private static readonly Vector2 MenuButtonSize = new Vector2(240f, 76f);

    private Button[] menuButtons;
    private int selectedIndex = 0;

    [Header("UI Art")]
    [SerializeField] private Sprite primaryButtonSprite;
    [SerializeField] private Sprite primaryButtonPressedSprite;
    [SerializeField] private Sprite secondaryButtonSprite;
    [SerializeField] private Sprite secondaryButtonPressedSprite;
    [SerializeField] private Sprite buttonHoverSprite;
    [SerializeField] private Sprite buttonDisabledSprite;

    void Start()
    {
        StyleButtons();
        menuButtons = new Button[]
        {
            GameObject.Find("Start").GetComponent<Button>(),
            GameObject.Find("Quit").GetComponent<Button>()
        };

        UpdateSelection();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Paul");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void StyleButtons()
    {
        foreach (Button button in FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            bool secondary = button.gameObject.name == "Quit";
            Sprite normalSprite = secondary && secondaryButtonSprite != null ? secondaryButtonSprite : primaryButtonSprite;
            Sprite pressedSprite = secondary && secondaryButtonPressedSprite != null ? secondaryButtonPressedSprite : primaryButtonPressedSprite;

            Image image = button.GetComponent<Image>();
            if (image != null && normalSprite != null)
            {
                image.sprite = normalSprite;
                image.type = Image.Type.Simple;
                image.preserveAspect = false;
                image.raycastTarget = true;
                image.color = Color.white;
            }

            RectTransform rectTransform = button.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = MenuButtonSize;
                if (button.gameObject.name == "Start")
                {
                    rectTransform.anchoredPosition = new Vector2(0f, 12f);
                }
                else if (button.gameObject.name == "Quit")
                {
                    rectTransform.anchoredPosition = new Vector2(0f, -82f);
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

            StyleButtonLabel(button);
        }
    }

    private void StyleButtonLabel(Button button)
    {
        TextMeshProUGUI tmpLabel = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmpLabel != null)
        {
            tmpLabel.fontSize = 30;
            tmpLabel.fontStyle = FontStyles.Bold;
            tmpLabel.color = new Color(1f, 0.95f, 0.80f, 1f);
            tmpLabel.raycastTarget = false;
        }

        Text label = button.GetComponentInChildren<Text>(true);
        if (label != null)
        {
            label.fontSize = 30;
            label.fontStyle = FontStyle.Bold;
            label.color = new Color(1f, 0.95f, 0.80f, 1f);
            label.raycastTarget = false;
        }
    }

    public void SelectNext()
    {
        selectedIndex++;
        if (selectedIndex >= menuButtons.Length)
            selectedIndex = 0;

        UpdateSelection();
    }

    public void SelectPrevious()
    {
        selectedIndex--;
        if (selectedIndex < 0)
            selectedIndex = menuButtons.Length - 1;

        UpdateSelection();
    }

    public void PressSelected()
    {
        menuButtons[selectedIndex].onClick.Invoke();
    }

   private void UpdateSelection()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            RectTransform rect = menuButtons[i].GetComponent<RectTransform>();

            if (i == selectedIndex)
                rect.localScale = new Vector3(1.15f, 1.15f, 1f);
            else
                rect.localScale = Vector3.one;
        }

        menuButtons[selectedIndex].Select();
    }
    }
