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
    private GameObject talePanel;
    private RectTransform taleProgressFillRect;
    private TextMeshProUGUI talePlayPauseLabel;
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
        BuildTaleScreen();
    }

    void ResolveFont()
    {
        GameObject startText = GameObject.Find("StartText");
        if (startText != null)
            uiFont = startText.GetComponent<TextMeshProUGUI>()?.font;
    }

    public void ShowMainMenu()
    {
        SetPanel(aboutPanel, false);
        SetPanel(talePanel, false);
        SetPanel(mainMenuPanel, true);
    }

    public void ShowAbout()
    {
        SetPanel(mainMenuPanel, false);
        SetPanel(talePanel, false);
        SetPanel(aboutPanel, true);
    }

    public void ShowTale()
    {
        SetPanel(mainMenuPanel, false);
        SetPanel(aboutPanel, false);
        SetPanel(talePanel, true);
        RefreshTaleControls();
    }

    public void HideAll()
    {
        SetPanel(mainMenuPanel, false);
        SetPanel(aboutPanel, false);
        SetPanel(talePanel, false);
    }

    static void SetPanel(GameObject panel, bool visible)
    {
        if (panel != null)
            panel.SetActive(visible);
    }

    void Update()
    {
        if (talePanel != null && talePanel.activeSelf)
            RefreshTaleControls();
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

        CreateButton(mainMenuPanel.transform, "ListenTaleButton", "Прослушать сказку",
            new Vector2(0.25f, 0.24f), new Vector2(0.75f, 0.36f),
            () => GameManager.Instance.EnterTaleScreen());
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
            new Vector2(0.25f, 0.08f), new Vector2(0.75f, 0.18f),
            () => GameManager.Instance.EnterMainMenu());

        aboutPanel.SetActive(false);
    }

    void BuildTaleScreen()
    {
        Transform taleCanvas = CreateTaleWorldCanvas();
        talePanel = CreatePanel(taleCanvas, "TalePanel", Color.clear);
        Image panelImage = talePanel.GetComponent<Image>();
        if (panelImage != null)
            panelImage.raycastTarget = false;

        CreateTitle(talePanel.transform, "Прослушивание сказки", new Vector2(0.08f, 0.82f), new Vector2(0.92f, 0.96f), 40);

        Button playPauseButton = CreateButton(talePanel.transform, "TalePlayPauseButton", "Пауза",
            new Vector2(0.08f, 0.5f), new Vector2(0.48f, 0.64f),
            () => GameManager.Instance.ToggleTalePlayback(),
            28);
        talePlayPauseLabel = playPauseButton.GetComponentInChildren<TextMeshProUGUI>();

        CreateButton(talePanel.transform, "TaleRestartButton", "Заново",
            new Vector2(0.52f, 0.5f), new Vector2(0.92f, 0.64f),
            () => GameManager.Instance.RestartTale(),
            28);

        CreateProgressBar(talePanel.transform);

        CreateButton(talePanel.transform, "TaleBackButton", "Вернуться в меню",
            new Vector2(0.25f, 0.08f), new Vector2(0.75f, 0.2f),
            () => GameManager.Instance.EnterMainMenu());

        talePanel.SetActive(false);
    }

    Transform CreateTaleWorldCanvas()
    {
        GameObject taleCanvasObject = new GameObject("TaleCanvas");
        Canvas taleCanvas = taleCanvasObject.AddComponent<Canvas>();
        taleCanvas.renderMode = RenderMode.WorldSpace;
        taleCanvas.worldCamera = Camera.main;
        taleCanvas.sortingOrder = 1;
        taleCanvas.additionalShaderChannels =
            AdditionalCanvasShaderChannels.TexCoord1 |
            AdditionalCanvasShaderChannels.Normal |
            AdditionalCanvasShaderChannels.Tangent;

        RectTransform sourceRect = canvas.GetComponent<RectTransform>();
        RectTransform taleRect = taleCanvasObject.GetComponent<RectTransform>();
        taleRect.sizeDelta = sourceRect.sizeDelta;
        taleRect.localScale = sourceRect.localScale;
        taleRect.position = new Vector3(-19.2f, -11f, sourceRect.position.z);

        taleCanvasObject.AddComponent<GraphicRaycaster>();
        return taleCanvasObject.transform;
    }

    void CreateProgressBar(Transform parent)
    {
        GameObject bar = new GameObject("TaleProgressBar");
        bar.transform.SetParent(parent, false);

        RectTransform barRect = bar.AddComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0.1f, 0.32f);
        barRect.anchorMax = new Vector2(0.9f, 0.42f);
        barRect.offsetMin = Vector2.zero;
        barRect.offsetMax = Vector2.zero;

        Image background = bar.AddComponent<Image>();
        background.color = new Color(0.12f, 0.12f, 0.12f, 0.9f);

        GameObject fillObject = new GameObject("Fill");
        fillObject.transform.SetParent(bar.transform, false);

        taleProgressFillRect = fillObject.AddComponent<RectTransform>();
        taleProgressFillRect.anchorMin = Vector2.zero;
        taleProgressFillRect.anchorMax = new Vector2(0f, 1f);
        taleProgressFillRect.offsetMin = new Vector2(4f, 4f);
        taleProgressFillRect.offsetMax = new Vector2(-4f, -4f);
        taleProgressFillRect.pivot = new Vector2(0f, 0.5f);

        Image fillImage = fillObject.AddComponent<Image>();
        fillImage.color = new Color(0.35f, 0.75f, 0.4f, 1f);
        fillImage.raycastTarget = false;
    }

    void RefreshTaleControls()
    {
        SoundsController sounds = GameManager.Instance != null
            ? GameManager.Instance.Sounds
            : null;

        if (taleProgressFillRect != null)
        {
            float progress = sounds != null ? sounds.TaleProgress : 0f;
            taleProgressFillRect.anchorMax = new Vector2(progress, 1f);
        }

        if (talePlayPauseLabel != null)
            talePlayPauseLabel.text = sounds != null && sounds.IsTalePlaying ? "Пауза" : "Продолжить";
    }

    GameObject CreatePanel(string name, Color color)
    {
        return CreatePanel(canvas.transform, name, color);
    }

    GameObject CreatePanel(Transform parent, string name, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

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

    Button CreateButton(Transform parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax, UnityEngine.Events.UnityAction onClick, float fontSize = 28f)
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
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        if (uiFont != null)
            text.font = uiFont;

        return button;
    }
}
