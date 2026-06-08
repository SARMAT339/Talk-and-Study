using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuUI : MonoBehaviour
{
    public static MenuUI Instance { get; private set; }

    private const string AboutBody =
        "Привет от создателей!\n\n" +
        "Добро пожаловать за кулисы нашего проекта! Игра «Аргъау» родилась из простой идеи: взять за основу сюжет литературной осетинской сказки и с помощью современных технологий и геймификации привлечь внимание детей к родному языку, сохраняя и популяризируя его в увлекательной игровой форме.\n\n" +
        "Мы хотели создать проект, который подарит детям радость от взаимодействия с осетинским языком через запоминающихся сказочных персонажей — кота и мышей, превратит изучение простых фраз в веселую игровую механику, а также заложит основу для будущей экосистемы по поддержке осетинского языка. Надеемся, нам это удалось!\n\n" +
        "Наша команда:\n\n" +
        "Бдайциев Сармат — Программист / Разработчик игровой логики\n\n" +
        "Пагаева Диана — Художник / 2D-моделлер\n\n" +
        "Гусалова Милена — Художник / 2D-моделлер\n\n" +
        "Таршхоева Марета — Художник фонов / Саунд-дизайнер";

    private GameObject mainMenuPanel;
    private GameObject aboutPanel;
    private TMP_FontAsset uiFont;
    private Canvas canvas;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        canvas = FindFirstObjectByType<Canvas>();
        ResolveFont();
        BuildMainMenu();
        BuildAboutScreen();
    }

    void ResolveFont()
    {
        GameObject startText = GameObject.Find("StartText");
        if (startText != null)
            uiFont = startText.GetComponent<TextMeshProUGUI>()?.font;
    }

    public void ShowMainMenu()
    {
        if (aboutPanel != null)
            aboutPanel.SetActive(false);
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    public void ShowAbout()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (aboutPanel != null)
            aboutPanel.SetActive(true);
    }

    public void HideAll()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (aboutPanel != null)
            aboutPanel.SetActive(false);
    }

    void BuildMainMenu()
    {
        mainMenuPanel = CreatePanel("MainMenuPanel", new Color(0f, 0f, 0f, 0.55f));

        CreateTitle(mainMenuPanel.transform, "Аргъау", new Vector2(0.1f, 0.72f), new Vector2(0.9f, 0.92f), 56);

        CreateButton(mainMenuPanel.transform, "StartGameButton", "Начать игру",
            new Vector2(0.25f, 0.52f), new Vector2(0.75f, 0.64f),
            () => GameManager.Instance.EnterStartScreen());

        CreateButton(mainMenuPanel.transform, "AboutButton", "О разработчиках",
            new Vector2(0.25f, 0.38f), new Vector2(0.75f, 0.5f),
            () => GameManager.Instance.EnterAboutScreen());

        CreateButton(mainMenuPanel.transform, "ExitButton", "Выход",
            new Vector2(0.25f, 0.24f), new Vector2(0.75f, 0.36f),
            () => GameManager.Instance.ExitGame());
    }

    void BuildAboutScreen()
    {
        aboutPanel = CreatePanel("AboutPanel", new Color(0f, 0f, 0f, 0.65f));

        CreateTitle(aboutPanel.transform, "О разработчиках", new Vector2(0.08f, 0.86f), new Vector2(0.92f, 0.96f), 40);

        GameObject scrollRoot = new GameObject("AboutScroll");
        scrollRoot.transform.SetParent(aboutPanel.transform, false);
        RectTransform scrollRect = scrollRoot.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0.08f, 0.22f);
        scrollRect.anchorMax = new Vector2(0.92f, 0.84f);
        scrollRect.offsetMin = Vector2.zero;
        scrollRect.offsetMax = Vector2.zero;

        Image scrollBg = scrollRoot.AddComponent<Image>();
        scrollBg.color = new Color(0f, 0f, 0f, 0.35f);

        ScrollRect scroll = scrollRoot.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.movementType = ScrollRect.MovementType.Clamped;

        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollRoot.transform, false);
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = new Vector2(8f, 8f);
        viewportRect.offsetMax = new Vector2(-8f, -8f);
        viewport.AddComponent<RectMask2D>();
        scroll.viewport = viewportRect;

        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0f, 1200f);

        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        TextMeshProUGUI body = content.AddComponent<TextMeshProUGUI>();
        body.text = AboutBody;
        body.fontSize = 24;
        body.color = Color.white;
        body.alignment = TextAlignmentOptions.TopLeft;
        body.enableWordWrapping = true;
        if (uiFont != null)
            body.font = uiFont;

        scroll.content = contentRect;

        CreateButton(aboutPanel.transform, "BackButton", "Вернуться в меню",
            new Vector2(0.08f, 0.08f), new Vector2(0.48f, 0.18f),
            () => GameManager.Instance.EnterMainMenu());

        CreateButton(aboutPanel.transform, "AboutExitButton", "Выход",
            new Vector2(0.52f, 0.08f), new Vector2(0.92f, 0.18f),
            () => GameManager.Instance.ExitGame());

        aboutPanel.SetActive(false);
    }

    GameObject CreatePanel(string name, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(canvas.transform, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = panel.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = true;

        return panel;
    }

    void CreateTitle(Transform parent, string text, Vector2 anchorMin, Vector2 anchorMax, float fontSize)
    {
        GameObject titleObject = new GameObject("Title");
        titleObject.transform.SetParent(parent, false);

        RectTransform rect = titleObject.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        TextMeshProUGUI label = titleObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        if (uiFont != null)
            label.font = uiFont;
    }

    void CreateButton(Transform parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.2f, 0.55f, 0.25f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 28;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        if (uiFont != null)
            text.font = uiFont;
    }
}
