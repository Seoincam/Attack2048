using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class InGameUiMnanager : MonoBehaviour, INewTurnListener
{
    // 필드
    // - - - - - - - - -
    [SerializeField] private LoadingSO loadingSO;

    [Space, SerializeField] private TextMeshProUGUI remainingTurnsText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI clearValueText;

    [Header("Setting")]
    [SerializeField] private Button settingButton;
    [SerializeField] private Transform settingPanel;
    
    [Header("Codex")]
    [SerializeField] private Button codexButton;
    [SerializeField] private Transform codexPanel;
    [SerializeField] private CodexSO[] codexSO;
    private int currentCodexIndex;

    [Space, SerializeField] private Button informationButton;
    [SerializeField] private GameObject informationPanel;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private Button preventDestroyButton;
    [SerializeField] private Button addTurnButton;
    [SerializeField] private Button destroyTileButton;

    [Space, SerializeField] private GameObject nextStagePanel;
    [SerializeField] private GameObject failPanel;

    [Space, SerializeField] private Canvas darkCanvas;
    [SerializeField] private GameObject darkPanel;
    [SerializeField] private GameObject darkPanelText;

    private Main main;
    private event Action OnEscapeButtonTapped;


    // 뒤로가기 감지
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var current = OnEscapeButtonTapped;
            OnEscapeButtonTapped?.Invoke();

            // 기존에 설정이 열려있었다면 설정을 닫기만 함.
            if (current != OnCloseSettingButtonTapped)
                OnOpenSettingButtonTapped();
        }
    }


    // 초기화
    // - - - - - - - - -
    public void Init(Main main)
    {
        this.main = main;

        InitDelegate();
        InitSetting();
        InitCodex();
        InitStore();

        informationButton.onClick.AddListener(OpenInformation);
        informationPanel.GetComponentInChildren<Button>().onClick.AddListener(CloseInformation);

        this.Subscribe_NewTurn();
        OnPointChanged();
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

        preventDestroyButton.GetComponentInChildren<Text>().text = $"파괴 방지\n{main.Store.PreventDestroyCost}pt";
        addTurnButton.GetComponentInChildren<Text>().text = $"턴 추가\n{main.Store.AddTurnCost}pt";
        destroyTileButton.GetComponentInChildren<Text>().text = $"타일 파괴\n{main.Store.DestroyTileCost}pt";
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
        remainingTurnsText.text = $"Remaining Turns: {GameManager.Instance.CurTurns}";
    }

    private void OnPointChanged()
    {
        var point = main.Point.Points;

        pointsText.text = $"{point}pt";

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
        stageText.text = $"Stage {slimeInfo.stageIndex}";
        clearValueText.text = $"Clear: {slimeInfo.clearValue}";
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

        main.Stage.ChangeStage(0, isRetry: true);
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
        codexPanel.Find("Stage Codex/Description Text").GetComponent<Text>().text = codexSO[currentCodexIndex].slimeDescription ?? "설명이 없음";
    }


    // Store Button
    // - - - - - - - - -
    private void PreventDestroy()
    {
        SetDarkPanel(true, isStoreButton: true, layerName: "Defalut");
        main.Store.PreventDestroyBtn();
    }

    //todo
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
            main.Point.ResetToZeroPoints();
            SetAllButtons(true);
            SetDarkPanel(isTurnOn: false);
            GameManager.Instance.IsPaused = false;
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


    // Bottom Buttons
    private void OpenInformation()
    {
        GameManager.Instance.IsPaused = true;
        SetAllButtons(false);
        SetDarkPanel(true);
        informationPanel.SetActive(true);
    }

    private void CloseInformation()
    {
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

    private void SetDarkPanel(bool isTurnOn, bool isStoreButton = false, string layerName = "DefaultUI")
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
