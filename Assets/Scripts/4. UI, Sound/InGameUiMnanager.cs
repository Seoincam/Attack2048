using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InGameUiMnanager : MonoBehaviour, INewTurnListener
{
    // 필드
    // - - - - - - - - -
    [SerializeField] private LoadingSO loadingSO;

    [Space, SerializeField] private TextMeshProUGUI remainingTurnsText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI clearValueText;

    [Space, SerializeField] private Button settingButton;
    [SerializeField] private Button codexButton;

    [Space, SerializeField] private Transform settingPanel;
    [SerializeField] private Transform codexPanel;
    [SerializeField] private GameObject[] Codex;

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

    private Text _indexText;
    private int _index;

    private Main main;



    // 초기화
    // - - - - - - - - -
    public void Init(Main main)
    {
        this.main = main;

        main.Game.OnRemainingTurnChanged += OnRemainingTurnChanged;
        main.Point.OnPointChanged += OnPointChanged;
        main.Store.OnClickButton += OnClickStoreButton;

        main.Stage.OnSlimeChanged += OnSlimeChanged;

        main.Stage.OnGameClear += OnGameClear;
        main.Stage.OnGameFail += OnGameFail;

        SoundManager.Instance.SetPanel
        (
            settingPanel.Find("BGM/BGM Slider").GetComponent<Slider>(),
            settingPanel.Find("SFX/SFX Slider").GetComponent<Slider>()
        );

        settingButton.onClick.AddListener(OpenSetting);
        codexButton.onClick.AddListener(OpenCodex);

        settingPanel.Find("Retry Button").GetComponent<Button>().onClick.AddListener(Retry);
        settingPanel.Find("Lobby Button").GetComponent<Button>().onClick.AddListener(GoLobbyButton);

        _indexText = codexPanel.Find("Index Text").GetComponent<Text>();

        informationButton.onClick.AddListener(OpenInformation);
        informationPanel.GetComponentInChildren<Button>().onClick.AddListener(CloseInformation);

        preventDestroyButton.onClick.AddListener(PreventDestroy);
        addTurnButton.onClick.AddListener(main.Store.AddTurnBtn);
        destroyTileButton.onClick.AddListener(DestroyTile);

        preventDestroyButton.GetComponentInChildren<Text>().text = $"파괴 방지\n{main.Store.PreventDestroyCost}pt";
        addTurnButton.GetComponentInChildren<Text>().text = $"턴 추가\n{main.Store.AddTurnCost}pt";
        destroyTileButton.GetComponentInChildren<Text>().text = $"타일 파괴\n{main.Store.DestroyTileCost}pt";
        this.Subscribe_NewTurn();
        OnPointChanged();
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
    private void OpenSetting()
    {
        if (!main.Game.CanGetInput)
            return;

        SetAllButtons(false);
        SetDarkPanel(value: true);
        main.Game.IsPaused = true;
        settingPanel.gameObject.SetActive(true);
    }

    private void CloseSetting()
    {
        SetAllButtons(true);
        SetDarkPanel(value: false);
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
        CloseSetting();
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

    // Codex Panel
    // - - - - - - - - -
    public void OpenCodex()
    {
        if (!main.Game.CanGetInput)
            return;

        SetAllButtons(false);
        SetDarkPanel(value: true);
        main.Game.IsPaused = true;
        codexPanel.gameObject.SetActive(true);
    }

    public void CloseCodex()
    {
        SetAllButtons(true);
        SetDarkPanel(value: false);
        main.Game.IsPaused = false;
        codexPanel.gameObject.SetActive(false);
    }

    public void PreviousButton()
    {
        if (_index > 0)
        {
            Codex[_index].SetActive(false);
            Codex[--_index].SetActive(true);
            _indexText.text = $"{_index + 1} / {Codex.Length}";
        }
    }
    public void NextButton()
    {
        if (_index < Codex.Length - 1)
        {
            Codex[_index].SetActive(false);
            Codex[++_index].SetActive(true);
            _indexText.text = $"{_index + 1} / {Codex.Length}";
        }
    }


    // Game Claer & Fail
    // - - - - - - - - -
    private void OnGameClear()
    {
        SetAllButtons(false);
        SetDarkPanel(value: true);
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
            SetDarkPanel(value: false);
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
        SetDarkPanel(value: true);
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

    private void SetDarkPanel(bool value, bool isStoreButton = false, string layerName = "AboveTile")
    {
        darkCanvas.sortingLayerName = layerName;
        darkPanelText.SetActive(isStoreButton);
        darkPanel.SetActive(value);
    }
    
    public void RefreshAllUi()
    {
        OnPointChanged();            // 버튼 조건 다시 계산
        OnRemainingTurnChanged();    // 남은 턴 텍스트 갱신
    }
}
