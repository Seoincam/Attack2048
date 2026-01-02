using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class StageUIManager : MonoBehaviour, INewTurnListener
{
    // 필드
    // - - - - - - - - -
    public static StageUIManager Instance { get; private set; }

    [SerializeField] private Canvas defaultCanvas;
    [SerializeField] private Canvas aboveCanvas;

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
    [SerializeField] private Button closeSettingButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button lobbyButton;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [Header("설정창 열릴 때 가려질 것")] public List<GameObject> hideObjects = new();
    
    [Header("Codex")]
    [SerializeField] private Button codexButton;
    [SerializeField] private Transform codexPanel;
    [SerializeField] private Button codexCloseButton;

    [Header("Store")]
    [SerializeField] private Button preventDestroyButton;
    [SerializeField] private Button addTurnButton;
    [SerializeField] private Button destroyTileButton;

    [Header("Point")]
    [SerializeField] private Button pointHintToggleButton;
    [SerializeField] private GameObject pointHintPanel;
    [SerializeField] private Sprite openEye;
    [SerializeField] private Sprite closeEye;

    [Header("Clear & Fail")]
    [SerializeField] private GameObject nextStagePanel;
    [SerializeField] private GameObject finalStagePanel;
    [SerializeField] private GameObject failPanel;

    [Header("UI Position")]
    [SerializeField] private RectTransform storeButtonGroup;

    private Main main;
    private Action OnEscapeButtonTapped;
    //클리어 효과음 중복 방지
    public bool isSFXPlayed = false;

    private bool isPointHint = false;


    // 뒤로가기 감지
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapeButtonTapped ??= OnOpenSettingButtonTapped;
            OnEscapeButtonTapped?.Invoke();
        }
    }


    // 초기화
    // - - - - - - - - -
    void Awake()
    {
        Instance = this;    
    }

    public void OnEnterStage()
    {
        defaultCanvas.gameObject.SetActive(true);
        aboveCanvas.gameObject.SetActive(true);
    }

    public void Init(Main main)
    {
        this.main = main;

        // InitDelegate();
        // InitSetting();
        // InitCodex();
        // InitStore();

        // pointHintToggleButton.onClick.AddListener(TogglePointHint);

        // this.Subscribe_NewTurn();
        // OnPointChanged();
        // SetActiveHideObjects(true);
    }

    void InitDelegate()
    {
        main.Game.OnRemainingTurnChanged += OnRemainingTurnChanged;
        main.Point.OnPointChanged += OnPointChanged;
        main.Store.OnClickButton += OnClickStoreButton;
        main.Stage.OnSlimeChanged += OnSlimeChanged;
        // main.Stage.OnGameClear += OnGameClear;
        main.Stage.OnGameFail += OnGameFail;
    }

    void InitSetting()
    {
        settingButton.onClick.AddListener(OnOpenSettingButtonTapped);
        closeSettingButton.GetComponent<Button>().onClick.AddListener(OnCloseSettingButtonTapped);
        retryButton.onClick.AddListener(Retry);
        lobbyButton.onClick.AddListener(GoLobbyButton);
        // SoundManager.Instance.InitPanel(bgmSlider, sfxSlider);
    }

    void InitCodex()
    {
        codexButton.onClick.AddListener(OnOpenCodexButtonTapped);
        codexCloseButton.GetComponent<Button>().onClick.AddListener(OnCloseCodexButtonTapped);
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
        remainingTurnsText.text = $"<color={currentColor}>{GameManager.Instance.CurTurns}</color>/<color=#6c6ca8>{main.Stage.MaxTurn}</color>";
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
        main.Game.IsPaused = true;
        SetActiveHideObjects(false);

        settingPanel.gameObject.SetActive(true);
    }

    private void OnCloseSettingButtonTapped()
    {
        OnEscapeButtonTapped = null;

        SetAllButtons(true);
        main.Game.IsPaused = false;
        SetActiveHideObjects(true);

        settingPanel.gameObject.SetActive(false);
    }

    private void Retry()
    {
        // 슬라임 액션 비활성화
        // foreach (Transform action in SlimeActionGroup.Instance.transform)
        // {
        //     if (!action.gameObject.activeSelf)
        //         continue;
        //     var slimeAction = action.GetComponent<SlimeActionBase>();
        //     slimeAction.Destroy();
        // }

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
        main.Game.IsPaused = true;
        SetActiveHideObjects(false);
        codexPanel.gameObject.SetActive(true);
    }

    private void OnCloseCodexButtonTapped()
    {
        OnEscapeButtonTapped = null;

        SetAllButtons(true);
        main.Game.IsPaused = false;
        SetActiveHideObjects(true);
        codexPanel.gameObject.SetActive(false);
    }

    // Store Button
    // - - - - - - - - -
    private void PreventDestroy()
    {
        main.Store.PreventDestroyBtn();
    }

    private void DestroyTile()
    {
        GameManager gamemanager = GameManager.Instance;
        if (gamemanager.CountTile() > 1)
        {
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
            // SoundManager.Instance.PlayStageClearSFX();
            isSFXPlayed = true;
        }

        SetAllButtons(false);
        if (main.Stage.StageIndex == 6)
        {
            finalStagePanel.GetComponentInChildren<Button>().onClick.AddListener(GoLobbyButton);
            finalStagePanel.SetActive(true);
        }
        else
        {
            nextStagePanel.GetComponentInChildren<Button>().onClick.AddListener(NextStageButton);
            nextStagePanel.SetActive(true);
        }

    }

    private void NextStageButton()
    {
        if (main.Stage.ChangeStage(main.Stage.StageIndex + 1, isRetry: false))
        {
            nextStagePanel.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            nextStagePanel.SetActive(false);

            SetAllButtons(true);
            main.Game.StartGame();
        }
    }

    private void OnGameFail()
    {
        failPanel.GetComponentInChildren<Button>().onClick.AddListener(GoLobbyButton);
        failPanel.SetActive(true);
        SetAllButtons(false);
        GameManager.Instance.IsPaused = true;
    }

    private void GoLobbyButton()
    {
        hideObjects.Clear();

        // 슬라임 액션 비활성화
        // foreach (Transform action in SlimeActionGroup.Instance.transform)
        // {
        //     if (!action.gameObject.activeSelf)
        //         continue;

        //     var slimeAction = action.GetComponent<SlimeActionBase>();
        //     slimeAction.Destroy();
        // }

        GameManager.Instance.ResetTileArray();
        GameManager.Instance.ResetObstacleArray();

        // SceneManager.LoadScene("Loading");
    }


    // Point Hint
    // - - - - - - - - -
    private void TogglePointHint()
    {
        isPointHint = !isPointHint;

        pointHintPanel.SetActive(isPointHint);
        pointHintToggleButton.image.sprite = isPointHint ? closeEye : openEye;

        preventDestroyButton.gameObject.SetActive(!isPointHint);
        addTurnButton.gameObject.SetActive(!isPointHint);
        destroyTileButton.gameObject.SetActive(!isPointHint);     
    }


    // etc
    // - - - - - - - - -
    private void SetActiveHideObjects(bool value)
    {
        foreach (var obj in hideObjects)
        {
            obj.SetActive(value);
            if (value && obj.name == "Point Hint Panel" && !isPointHint)
                obj.SetActive(false);
        }

        // SlimeActionGroup.Instance.gameObject.SetActive(value);
        // TileGroup.Instance.gameObject.SetActive(value);
    }
    private void SetAllButtons(bool value)
    {
        var point = main.Point.Points;

        preventDestroyButton.interactable = value ? point >= main.Store.PreventDestroyCost : false;
        addTurnButton.interactable = value ? point >= main.Store.AddTurnCost : false;
        destroyTileButton.interactable = value && (point >= main.Store.DestroyTileCost) && GameManager.Instance.CountTile() > 1;

        settingButton.interactable = value;
        codexButton.interactable = value;
        pointHintToggleButton.interactable = value;
    }
    
    public void RefreshAllUi()
    {
        OnPointChanged();            // 버튼 조건 다시 계산
        OnRemainingTurnChanged();    // 남은 턴 텍스트 갱신
    }
}
