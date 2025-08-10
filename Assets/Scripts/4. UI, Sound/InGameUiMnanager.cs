using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class InGameUiMnanager : MonoBehaviour, INewTurnListener
{
    // 필드
    // - - - - - - - - -
    [SerializeField] private LoadingSO loadingSO;

    [Header("Text")]
    [SerializeField] private Image stageText;
    [SerializeField] private Sprite[] stageTextSprites;
    [SerializeField] private Text remainingTurnsText;
    [SerializeField] private Text clearValueText;
    [SerializeField] private Text pointsText;

    [Header("Slime")]
    [SerializeField] private Image slime;
    [SerializeField] private Sprite[] slimeSprites;

    [Header("Setting")]
    [SerializeField] private Button settingButton;
    [SerializeField] private Transform settingPanel;
    
    [Header("Codex")]
    [SerializeField] private Button codexButton;
    [SerializeField] private Transform codexPanel;
    [SerializeField] private CodexSO[] codexSO;
    private int currentCodexIndex;

    [Header("Store")]
    [SerializeField] private Button informationButton;
    [SerializeField] private GameObject informationPanel;
    [SerializeField] private Button preventDestroyButton;
    [SerializeField] private Button addTurnButton;
    [SerializeField] private Button destroyTileButton;

    [Header("Clear & Fail")]
    [SerializeField] private GameObject nextStagePanel;
    [SerializeField] private GameObject failPanel;

    [Header("Dark Background")]
    [SerializeField] private Canvas darkCanvas;
    [SerializeField] private GameObject darkPanel;
    [SerializeField] private GameObject darkPanelText;

    [Header("UI Position")]
    [SerializeField] private RectTransform storeButtonGroup;

    private Main main;
    private Action OnEscapeButtonTapped;
    //클리어 효과음 중복 방지
    public bool isSFXPlayed = false;


    // 뒤로가기 감지
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapeButtonTapped = OnEscapeButtonTapped != null ? OnEscapeButtonTapped : OnOpenSettingButtonTapped;
            OnEscapeButtonTapped?.Invoke();
        }
    }


    // 초기화
    // - - - - - - - - -
    public void Init(Main main)
    {
        this.main = main;

        InitUIPosition();
        InitDelegate();
        InitSetting();
        InitCodex();
        InitStore();

        informationButton.onClick.AddListener(OpenInformation);
        informationPanel.GetComponentInChildren<Button>().onClick.AddListener(CloseInformation);

        this.Subscribe_NewTurn();
        OnPointChanged();
    }

    void InitUIPosition()
    {
        var newY = 0.10417f * Screen.height - 806.67f;
        storeButtonGroup.anchoredPosition = new Vector2(storeButtonGroup.anchoredPosition.x, newY);
    }

    void InitDelegate()
    {
        main.Game.OnRemainingTurnChanged += OnRemainingTurnChanged;
        main.Point.OnPointChanged += OnPointChanged;
        main.Store.OnClickButton += OnClickStoreButton;
        main.Stage.OnSlimeChanged += OnSlimeChanged;
        main.Stage.OnGameClear += OnGameClear;
        main.Stage.OnGameFail += OnGameFail;
    }

    void InitSetting()
    {
        settingButton.onClick.AddListener(OnOpenSettingButtonTapped);
        settingPanel.Find("Close Button").GetComponent<Button>().onClick.AddListener(OnCloseSettingButtonTapped);
        settingPanel.Find("Button Layout Group/Retry Button").GetComponent<Button>().onClick.AddListener(Retry);
        settingPanel.Find("Button Layout Group/Lobby Button").GetComponent<Button>().onClick.AddListener(GoLobbyButton);

        SoundManager.Instance.SetPanel
        (
            settingPanel.Find("BGM Layout Group/BGM Slider").GetComponent<Slider>(),
            settingPanel.Find("SFX Layout Group/SFX Slider").GetComponent<Slider>()
        );
    }

    void InitCodex()
    {
        codexButton.onClick.AddListener(OnOpenCodexButtonTapped);
        codexPanel.Find("Close Button").GetComponent<Button>().onClick.AddListener(OnCloseCodexButtonTapped);
        codexPanel.Find("Prev Button").GetComponent<Button>().onClick.AddListener(OnPreviousCodexButtonTapped);
        codexPanel.Find("Next Button").GetComponent<Button>().onClick.AddListener(OnNextCodexButtonTapped);
        currentCodexIndex = 0;

        UpdateCodexUI();
    }

    void InitStore()
    {
        preventDestroyButton.onClick.AddListener(PreventDestroy);
        addTurnButton.onClick.AddListener(main.Store.AddTurnBtn);
        destroyTileButton.onClick.AddListener(DestroyTile);

        // preventDestroyButton.GetComponentInChildren<Text>().text = $"파괴 방지\n{main.Store.PreventDestroyCost}pt";
        // addTurnButton.GetComponentInChildren<Text>().text = $"턴 추가\n{main.Store.AddTurnCost}pt";
        // destroyTileButton.GetComponentInChildren<Text>().text = $"타일 파괴\n{main.Store.DestroyTileCost}pt";
    }


    // - - - - - - - - - - - 
    // INewTurnListener
    // - - - - - - - - - - -
    public void OnEnter_NewTurn()
    {
        StartCoroutine(DelayUpdateUI());
    }

    public void Subscribe_NewTurn()
    {
        EventManager.Subscribe(GamePhase.NewTurnPhase, OnEnter_NewTurn);
    }


    // 타일이 스폰된 후까지 기다린 후 체크
    private System.Collections.IEnumerator DelayUpdateUI()
    {
        yield return null;
        OnPointChanged();
    }


    // delegate
    // - - - - - - - - -
    private void OnRemainingTurnChanged()
    {
        var currentColor = GameManager.Instance.CurTurns > 5 ? "#6c6ca8" : "#ff4d4d";
        remainingTurnsText.text = $"<color={currentColor}>{GameManager.Instance.CurTurns}</color>/<color=#6c6ca8>{main.Stage.maxTurn}</color>";
    }

    private void OnPointChanged()
    {
        var point = main.Point.Points;

        pointsText.text = $"{point}";

        if (nextStagePanel.activeSelf)
            return;

        preventDestroyButton.interactable = point >= main.Store.PreventDestroyCost;
        addTurnButton.interactable = point >= main.Store.AddTurnCost;
        destroyTileButton.interactable = (point >= main.Store.DestroyTileCost) && (GameManager.Instance.CountTile() > 1);
    }

    private void OnClickStoreButton(object _, StoreManager.ClickInfo clickInfo)
    {
        SetAllButtons(!clickInfo.isSelecting);
        if (!clickInfo.isSelecting)
            SetDarkPanel(false);
    }

    private void OnSlimeChanged(object _, StageManager.SlimeInfo slimeInfo)
    {
        slime.sprite = slimeSprites[slimeInfo.stageIndex];
        slime.SetNativeSize();
        stageText.sprite = stageTextSprites[slimeInfo.stageIndex];
        stageText.SetNativeSize();
        clearValueText.text = $"{slimeInfo.clearValue} 블럭 생성하기";
    }


    // Setting Panel
    // - - - - - - - - -
    private void OnOpenSettingButtonTapped()
    {
        if (!main.Game.CanGetInput)
            return;

        OnEscapeButtonTapped = OnCloseSettingButtonTapped;

        SetAllButtons(false);
        SetDarkPanel(isTurnOn: true);
        main.Game.IsPaused = true;
        settingPanel.gameObject.SetActive(true);
    }

    private void OnCloseSettingButtonTapped()
    {
        OnEscapeButtonTapped = null;

        SetAllButtons(true);
        SetDarkPanel(isTurnOn: false);
        main.Game.IsPaused = false;
        settingPanel.gameObject.SetActive(false);
    }

    private void Retry()
    {
        // 슬라임 액션 비활성화
        foreach (Transform action in ObjectPoolManager.Instance.SlimeActionGroup)
        {
            if (!action.gameObject.activeSelf)
                continue;
            var slimeAction = action.GetComponent<SlimeActionBase>();
            slimeAction.StartCoroutine(slimeAction.DestroySelf());
        }

        GameManager.Instance.ResetTileArray();
        GameManager.Instance.ResetObstacleArray();
        main.Point.ResetPoint();

        main.Stage.ChangeStage(main.Stage.StageIndex, isRetry: true);
        OnCloseSettingButtonTapped();
    }


    // Codex Panel
    // - - - - - - - - -
    private void OnOpenCodexButtonTapped()
    {
        if (!main.Game.CanGetInput)
            return;

        OnEscapeButtonTapped = OnCloseCodexButtonTapped;

        SetAllButtons(false);
        SetDarkPanel(isTurnOn: true);
        main.Game.IsPaused = true;
        codexPanel.gameObject.SetActive(true);
    }

    private void OnCloseCodexButtonTapped()
    {
        OnEscapeButtonTapped = null;

        SetAllButtons(true);
        SetDarkPanel(isTurnOn: false);
        main.Game.IsPaused = false;
        codexPanel.gameObject.SetActive(false);
    }

    private void OnPreviousCodexButtonTapped()
    {
        currentCodexIndex--;
        Mathf.Clamp(currentCodexIndex, min: 0, max: codexSO.Length - 1);
        UpdateCodexUI();
    }

    private void OnNextCodexButtonTapped()
    {
        currentCodexIndex++;
        Mathf.Clamp(currentCodexIndex, min: 0, max: codexSO.Length - 1);
        UpdateCodexUI();
    }

    void UpdateCodexUI()
    {
        codexPanel.Find("Prev Button").GetComponent<Button>().interactable = currentCodexIndex > 0;
        codexPanel.Find("Next Button").GetComponent<Button>().interactable = currentCodexIndex < codexSO.Length - 1;
        codexPanel.Find("Index Text").GetComponent<Text>().text = $"{currentCodexIndex + 1} / {codexSO.Length}";

        codexPanel.Find("Stage Codex/Name Text").GetComponent<Text>().text = codexSO[currentCodexIndex].slimeName ?? "이름이 없음";
        var slimeImage = codexPanel.Find("Stage Codex/Slime Image").GetComponent<Image>();
        slimeImage.sprite = slimeSprites[currentCodexIndex];
        slimeImage.SetNativeSize();
        codexPanel.Find("Stage Codex/Description Text").GetComponent<Text>().text = codexSO[currentCodexIndex].slimeDescription ?? "설명이 없음";
    }


    // Store Button
    // - - - - - - - - -
    private void PreventDestroy()
    {
        SetDarkPanel(true, isStoreButton: true, layerName: "Defalut");
        main.Store.PreventDestroyBtn();
    }

    private void DestroyTile()
    {
        GameManager gamemanager = GameManager.Instance;
        if (gamemanager.CountTile() > 1)
        {
            SetDarkPanel(true, isStoreButton: true, layerName: "Defalut");
            main.Store.DestoryTileBtn();
        }
        else Debug.Log("파괴 불가");
    }


    // Game Claer & Fail
    // - - - - - - - - -
    private void OnGameClear()
    {
        if(!isSFXPlayed)
        {
            SoundManager.Instance.PlayStageClearSFX();
            isSFXPlayed = true;
        }
        SetAllButtons(false);
        SetDarkPanel(isTurnOn: true);
        nextStagePanel.GetComponentInChildren<Button>().onClick.AddListener(NextStageButton);
        nextStagePanel.SetActive(true);
    }

    private void NextStageButton()
    {
        if (main.Stage.ChangeStage(main.Stage.StageIndex + 1, isRetry: false))
        {
            nextStagePanel.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            nextStagePanel.SetActive(false);

            SetAllButtons(true);
            SetDarkPanel(isTurnOn: false);
            main.Game.StartGame();
        }

        else
        {
            Debug.Log("클리어!");
        }
    }

    private void OnGameFail()
    {
        failPanel.GetComponentInChildren<Button>().onClick.AddListener(GoLobbyButton);
        failPanel.SetActive(true);
        SetAllButtons(false);
        SetDarkPanel(isTurnOn: true);
        GameManager.Instance.IsPaused = true;
    }

    private void GoLobbyButton()
    {
        // 슬라임 액션 비활성화
        foreach (Transform action in ObjectPoolManager.Instance.SlimeActionGroup)
        {
            if (!action.gameObject.activeSelf)
                continue;

            var slimeAction = action.GetComponent<SlimeActionBase>();
            slimeAction.StartCoroutine(slimeAction.DestroySelf());
        }

        GameManager.Instance.ClearTileArray();
        GameManager.Instance.ResetObstacleArray();

        loadingSO.SceneName = "Lobby";
        SceneManager.LoadScene("Loading");
    }


    // Information Buttons
    // - - - - - - - - -
    private void OpenInformation()
    {
        OnEscapeButtonTapped += CloseInformation;

        GameManager.Instance.IsPaused = true;
        SetAllButtons(false);
        SetDarkPanel(true);
        informationPanel.SetActive(true);
    }

    private void CloseInformation()
    {
        OnEscapeButtonTapped = null;

        GameManager.Instance.IsPaused = false;
        SetAllButtons(true);
        SetDarkPanel(false);
        informationPanel.SetActive(false);
    }


    // etc
    // - - - - - - - - -
    private void SetAllButtons(bool value)
    {
        var point = main.Point.Points;

        preventDestroyButton.interactable = value ? point >= main.Store.PreventDestroyCost : false;
        addTurnButton.interactable = value ? point >= main.Store.AddTurnCost : false;
        destroyTileButton.interactable = value && (point >= main.Store.DestroyTileCost) && GameManager.Instance.CountTile() > 1;

        settingButton.interactable = value;
        codexButton.interactable = value;
        informationButton.interactable = value;
    }

    private void SetDarkPanel(bool isTurnOn, bool isStoreButton = false, string layerName = "AboveUI")
    {
        darkCanvas.sortingLayerName = layerName;
        darkPanelText.SetActive(isStoreButton);
        darkPanel.SetActive(isTurnOn);
    }
    
    public void RefreshAllUi()
    {
        OnPointChanged();            // 버튼 조건 다시 계산
        OnRemainingTurnChanged();    // 남은 턴 텍스트 갱신
    }
}
