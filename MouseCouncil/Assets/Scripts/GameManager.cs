using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GamePhase
    {
        MainMenu,
        About,
        StartScreen,
        Intro,
        Playing,
        Ending,
        Result
    }

    [Header("Ссылки")]
    public CameraMove cameraMove;
    public BellSpawner bellSpawner;
    public Transform bellApproachPoint;
    public Transform catTarget;
    public MouseAgent[] mice;
    public Transform[] mouseGameTargets;
    public GameObject startHintText;
    public GameObject sceneBell;

    [Header("Настройки")]
    public int maxThrows = 3;
    public float endDelay = 2f;
    public float throwResolveTimeout = 6f;
    public float missSettleDelay = 1.5f;

    private GamePhase phase = GamePhase.MainMenu;
    private int currentMouseIndex;
    private int throwsCompleted;
    private bool hasWon;
    private bool waitingThrowResult;
    private bool inputLocked;
    private BellController activeBell;

    private GameObject resultPanel;
    private TextMeshProUGUI resultText;
    private GameObject restartHintText;
    private GameObject resultMenuButton;
    private SoundsController soundsController;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        soundsController = GetComponent<SoundsController>();
        DisableLegacyMouseControllers();
        AutoFindReferences();
        SetupMiceAgents();
        SetupResultUI();
    }

    void DisableLegacyMouseControllers()
    {
        CharacterSwitcher[] legacyControllers =
            FindObjectsByType<CharacterSwitcher>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (CharacterSwitcher controller in legacyControllers)
            controller.enabled = false;
    }

    void Start()
    {
        if (sceneBell != null)
            sceneBell.SetActive(false);

        if (bellSpawner != null)
            bellSpawner.Initialize(this);

        HideResultUI();
        EnterMainMenu();
    }

    void Update()
    {
        if (inputLocked || phase == GamePhase.Ending)
            return;

        if (phase == GamePhase.MainMenu || phase == GamePhase.About)
            return;

        if (!WasScreenPressed())
            return;

        if (phase == GamePhase.Result)
        {
            RestartGame();
            return;
        }

        if (phase == GamePhase.StartScreen)
            BeginIntro();
    }

    void AutoFindReferences()
    {
        if (cameraMove == null && Camera.main != null)
            cameraMove = Camera.main.GetComponent<CameraMove>();

        if (bellSpawner == null)
            bellSpawner = FindFirstObjectByType<BellSpawner>();

        if (bellApproachPoint == null)
        {
            GameObject target = GameObject.Find("Target");
            if (target != null)
                bellApproachPoint = target.transform;
        }

        if (catTarget == null)
        {
            GameObject catTargetObj = GameObject.Find("CatTarget");
            if (catTargetObj != null)
                catTarget = catTargetObj.transform;
        }

        if (startHintText == null)
        {
            GameObject hint = GameObject.Find("StartText");
            if (hint != null)
                startHintText = hint;
        }

        if (sceneBell == null)
        {
            GameObject bell = GameObject.Find("Bell_0");
            if (bell != null)
                sceneBell = bell;
        }

        if (mice == null || mice.Length == 0)
            mice = BuildMiceFromScene();

        if (mouseGameTargets == null || mouseGameTargets.Length == 0)
            mouseGameTargets = BuildMouseTargets();
    }

    Transform[] BuildMouseTargets()
    {
        string[] targetNames = { "Target", "Target2", "Target3" };
        var targets = new List<Transform>();

        foreach (string targetName in targetNames)
        {
            GameObject target = GameObject.Find(targetName);
            if (target != null)
                targets.Add(target.transform);
        }

        return targets.ToArray();
    }

    MouseAgent[] BuildMiceFromScene()
    {
        string[] names = { "Mouse", "Mouse2", "Mouse3" };
        var list = new List<MouseAgent>();

        foreach (string mouseName in names)
        {
            GameObject go = GameObject.Find(mouseName);
            if (go == null)
                continue;

            MouseAgent agent = go.GetComponent<MouseAgent>();
            if (agent == null)
                agent = go.AddComponent<MouseAgent>();

            CharacterSwitcher legacy = go.GetComponent<CharacterSwitcher>();
            if (legacy != null)
            {
                agent.idleObject = legacy.idleObject;
                agent.runObject = legacy.runObject;
                agent.speed = legacy.speed;
                legacy.enabled = false;
            }

            agent.CaptureHomePosition();
            list.Add(agent);
        }

        return list.ToArray();
    }

    void SetupMiceAgents()
    {
        if (mice == null)
            return;

        foreach (MouseAgent mouse in mice)
        {
            if (mouse == null)
                continue;

            CharacterSwitcher legacy = mouse.GetComponent<CharacterSwitcher>();
            if (legacy != null)
            {
                if (mouse.idleObject == null)
                    mouse.idleObject = legacy.idleObject;
                if (mouse.runObject == null)
                    mouse.runObject = legacy.runObject;
                if (mouse.speed <= 0f)
                    mouse.speed = legacy.speed;
                legacy.enabled = false;
            }

            mouse.CaptureHomePosition();
        }
    }

    void SetupResultUI()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
            return;

        resultPanel = new GameObject("ResultPanel");
        resultPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = resultPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = resultPanel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.45f);
        panelImage.raycastTarget = false;

        GameObject textObject = new GameObject("ResultText");
        textObject.transform.SetParent(resultPanel.transform, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.1f, 0.55f);
        textRect.anchorMax = new Vector2(0.9f, 0.85f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        resultText = textObject.AddComponent<TextMeshProUGUI>();
        resultText.alignment = TextAlignmentOptions.Center;
        resultText.fontSize = 42;
        resultText.color = Color.white;
        resultText.text = "";

        TMP_FontAsset uiFont = null;
        if (startHintText != null)
            uiFont = startHintText.GetComponent<TextMeshProUGUI>()?.font;

        if (uiFont != null)
            resultText.font = uiFont;

        restartHintText = new GameObject("RestartHint");
        restartHintText.transform.SetParent(resultPanel.transform, false);

        RectTransform hintRect = restartHintText.AddComponent<RectTransform>();
        hintRect.anchorMin = new Vector2(0.31300002f, 0.15738149f);
        hintRect.anchorMax = new Vector2(0.6933368f, 0.2777278f);
        hintRect.offsetMin = Vector2.zero;
        hintRect.offsetMax = Vector2.zero;

        TextMeshProUGUI hintLabel = restartHintText.AddComponent<TextMeshProUGUI>();
        hintLabel.alignment = TextAlignmentOptions.Center;
        hintLabel.fontSize = 50;
        hintLabel.color = Color.white;
        hintLabel.text = "нажми, чтобы начать";
        if (startHintText != null)
        {
            TextMeshProUGUI startLabel = startHintText.GetComponent<TextMeshProUGUI>();
            if (startLabel != null && !string.IsNullOrWhiteSpace(startLabel.text))
                hintLabel.text = startLabel.text;
        }

        if (uiFont != null)
            hintLabel.font = uiFont;

        restartHintText.AddComponent<StartTextAnimation>();

        resultMenuButton = CreateResultButton(
            resultPanel.transform,
            "Вернуться в меню",
            new Vector2(0.22f, 0.34f),
            new Vector2(0.78f, 0.48f),
            uiFont,
            ReturnToMainMenu
        );

        HideResultUI();
    }

    GameObject CreateResultButton(
        Transform parent,
        string label,
        Vector2 anchorMin,
        Vector2 anchorMax,
        TMP_FontAsset font,
        UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject("ResultMenuButton");
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
        text.fontSize = 30;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        if (font != null)
            text.font = font;

        return buttonObject;
    }

    public void EnterMainMenu()
    {
        ResetGameplayState();
        phase = GamePhase.MainMenu;
        ShowStartUI(false);
        HideResultUI();

        if (MenuUI.Instance != null)
            MenuUI.Instance.ShowMainMenu();
    }

    public void EnterStartScreen()
    {
        ResetGameplayState();
        phase = GamePhase.StartScreen;

        if (MenuUI.Instance != null)
            MenuUI.Instance.HideAll();

        ShowStartUI(true);
        HideResultUI();
    }

    public void EnterAboutScreen()
    {
        phase = GamePhase.About;
        ShowStartUI(false);
        HideResultUI();

        if (MenuUI.Instance != null)
            MenuUI.Instance.ShowAbout();
    }

    public void ReturnToMainMenu()
    {
        EnterMainMenu();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void BeginIntro()
    {
        phase = GamePhase.Intro;
        inputLocked = true;
        ShowStartUI(false);

        if (cameraMove != null)
            cameraMove.MoveToGame();

        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        int arrived = 0;
        int total = mice.Length;

        if (bellApproachPoint == null)
        {
            Debug.LogError("GameManager: не задан bellApproachPoint.");
            yield break;
        }

        for (int i = 0; i < mice.Length; i++)
        {
            MouseAgent mouse = mice[i];
            if (mouse == null)
            {
                arrived++;
                continue;
            }

            Vector3 destination = bellApproachPoint.position;
            if (mouseGameTargets != null && i < mouseGameTargets.Length && mouseGameTargets[i] != null)
                destination = mouseGameTargets[i].position;

            mouse.MoveTo(destination, () => arrived++);
        }

        while (arrived < total)
            yield return null;

        phase = GamePhase.Playing;
        inputLocked = false;
        currentMouseIndex = 0;
        throwsCompleted = 0;
        hasWon = false;
        soundsController?.StartMouseChatter();
        StartTurn();
    }

    void StartTurn()
    {
        if (hasWon || throwsCompleted >= maxThrows)
            return;

        StartCoroutine(TurnRoutine());
    }

    IEnumerator TurnRoutine()
    {
        inputLocked = true;
        waitingThrowResult = false;

        MouseAgent activeMouse = mice[currentMouseIndex];
        if (activeMouse == null)
        {
            inputLocked = false;
            yield break;
        }

        Vector3 approachPoint = GetBellApproachPosition(currentMouseIndex);
        bool mouseArrived = false;
        activeMouse.MoveTo(approachPoint, () => mouseArrived = true);

        while (!mouseArrived)
            yield return null;

        activeBell = bellSpawner != null ? bellSpawner.SpawnBellForTurn() : null;
        if (activeBell == null)
        {
            Debug.LogError("GameManager: не удалось создать колокольчик.");
            inputLocked = false;
            yield break;
        }

        activeBell.SetThrowEnabled(true);
        inputLocked = false;

        while (activeBell != null && !activeBell.WasThrown)
            yield return null;
    }

    public void OnBellThrown(BellController bell)
    {
        if (bell != activeBell || bell == null)
            return;

        bell.SetThrowEnabled(false);
        waitingThrowResult = true;
        StartCoroutine(WaitThrowResultRoutine());
    }

    IEnumerator WaitThrowResultRoutine()
    {
        yield return new WaitForSeconds(missSettleDelay);

        float elapsed = missSettleDelay;
        while (waitingThrowResult && elapsed < throwResolveTimeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (waitingThrowResult)
            ResolveMiss();
    }

    public void OnBellHitCat(BellController bell)
    {
        if (!waitingThrowResult || bell != activeBell || hasWon || bell == null || !bell.WasThrown)
            return;

        hasWon = true;
        waitingThrowResult = false;
        inputLocked = true;

        if (bell != null)
            bell.SetThrowEnabled(false);

        StartCoroutine(FinishGameRoutine(true));
    }

    public void OnBellMissed(BellController bell)
    {
        if (!waitingThrowResult || bell != activeBell || hasWon || bell == null || !bell.WasThrown)
            return;

        ResolveMiss();
    }

    void ResolveMiss()
    {
        if (!waitingThrowResult || hasWon)
            return;

        waitingThrowResult = false;
        inputLocked = true;

        if (activeBell != null)
        {
            activeBell.SetThrowEnabled(false);
            Destroy(activeBell.gameObject);
            activeBell = null;
        }

        StartCoroutine(MissRoutine());
    }

    IEnumerator MissRoutine()
    {
        MouseAgent activeMouse = mice[currentMouseIndex];
        if (activeMouse != null)
        {
            bool returned = false;
            activeMouse.ReturnHome(() => returned = true);
            while (!returned)
                yield return null;
        }

        throwsCompleted++;
        currentMouseIndex++;

        if (throwsCompleted >= maxThrows)
        {
            StartCoroutine(FinishGameRoutine(false));
            yield break;
        }

        inputLocked = false;
        StartTurn();
    }

    IEnumerator FinishGameRoutine(bool won)
    {
        phase = GamePhase.Ending;
        soundsController?.StopMouseChatter();
        inputLocked = true;
        waitingThrowResult = false;

        if (activeBell != null)
        {
            Destroy(activeBell.gameObject);
            activeBell = null;
        }

        yield return new WaitForSeconds(endDelay);

        if (cameraMove != null)
            yield return cameraMove.MoveToStartRoutine();

        int returned = 0;
        int total = mice.Length;

        foreach (MouseAgent mouse in mice)
        {
            if (mouse == null)
            {
                returned++;
                continue;
            }

            if (IsMouseAtHome(mouse))
            {
                mouse.SetIdleAtCurrentPosition();
                returned++;
                continue;
            }

            mouse.ReturnHome(() => returned++);
        }

        while (returned < total)
            yield return null;

        phase = GamePhase.Result;
        ShowResultUI(won);
        inputLocked = false;
    }

    public void RestartGame()
    {
        EnterStartScreen();
    }

    void ResetGameplayState()
    {
        StopAllCoroutines();
        soundsController?.StopMouseChatter();
        currentMouseIndex = 0;
        throwsCompleted = 0;
        hasWon = false;
        waitingThrowResult = false;
        inputLocked = false;

        if (activeBell != null)
        {
            Destroy(activeBell.gameObject);
            activeBell = null;
        }

        if (cameraMove != null)
            cameraMove.SnapToStart();

        foreach (MouseAgent mouse in mice)
        {
            if (mouse != null)
                mouse.SnapToHome();
        }

        if (sceneBell != null)
            sceneBell.SetActive(false);
    }

    void ShowStartUI(bool show)
    {
        if (startHintText != null)
            startHintText.SetActive(show);
    }

    void ShowResultUI(bool won)
    {
        if (resultPanel != null)
            resultPanel.SetActive(true);

        if (resultText != null)
            resultText.text = won ? "Вы выиграли!" : "Вы проиграли!";

        if (restartHintText != null)
            restartHintText.SetActive(true);

        if (resultMenuButton != null)
            resultMenuButton.SetActive(true);
    }

    void HideResultUI()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);

        if (restartHintText != null)
            restartHintText.SetActive(false);

        if (resultMenuButton != null)
            resultMenuButton.SetActive(false);
    }

    Vector3 GetBellApproachPosition(int mouseIndex)
    {
        if (mouseGameTargets != null &&
            mouseIndex >= 0 &&
            mouseIndex < mouseGameTargets.Length &&
            mouseGameTargets[mouseIndex] != null)
            return mouseGameTargets[mouseIndex].position;

        if (bellApproachPoint != null)
            return bellApproachPoint.position;

        return Vector3.zero;
    }

    static bool IsMouseAtHome(MouseAgent mouse)
    {
        return Vector2.Distance(mouse.transform.position, mouse.HomePosition) < 0.2f;
    }

    static bool WasScreenPressed()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;

        return false;
    }
}
