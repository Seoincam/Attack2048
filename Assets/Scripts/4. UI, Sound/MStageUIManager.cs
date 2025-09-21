using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MStageUIManager : MonoBehaviour, INewTurnListener
{
    private Main main;

    private bool _isPanelOn;
    //클리어 효과음 중복 방지
    public bool isSFXPlayed = false;

    [Header("Tween Setting")]
    [SerializeField] float storeButtonX = 800;

    [Header("Default")]
    [SerializeField] Canvas defaultCanvas;
    [SerializeField] Canvas aboveCanvas;
    [SerializeField] Image wallImage;
    [SerializeField] Image slimeImage;
    [SerializeField] Sprite wallSprite;

    [Space]
    [SerializeField] Image stageText;
    [SerializeField] Text turnText;
    [SerializeField] Text targetTileText;
    [SerializeField] Text pointsText;

    [Header("On Enter")]
    [SerializeField] Transform board;

    [Header("Store")]
    [SerializeField] Button preventDestroyButton;
    [SerializeField] Button addTurnButton;
    [SerializeField] Button destroyTileButton;

    [Header("Point")]
    [SerializeField] Transform pointSymbolImage;
    [SerializeField] private Button pointHintToggleButton;
    [SerializeField] private GameObject pointHintPanel;
    [SerializeField] private Sprite openEye;
    [SerializeField] private Sprite closeEye;

    [Header("Fail")]
    [SerializeField] Transform failPanel;

    [Header("Setting")]
    [SerializeField] Transform settingPanel;
    [SerializeField] Button settingButton;
    [SerializeField] Button closeSettingButton;
    [SerializeField] Button retryButton;
    [SerializeField] Button lobbyButton;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    [Header("Escape")]
    private Action OnEscapeButtonTapped;


    public bool IsPanelOn
    {
        get => _isPanelOn;
        set
        {
            _isPanelOn = value;
            UpdateStoreButtonState();
        }
    }

    public void Init(Main main)
    {
        this.main = main;
        InitDelegate();
        InitSetting();
        InitStore();
        Subscribe_NewTurn();

        OnPointChanged();
        UpdateStoreButtonState();
    }

    // 뒤로가기 감지
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapeButtonTapped ??= OnOpenSettingButtonTapped;
            OnEscapeButtonTapped?.Invoke();
        }
    }

    private void InitDelegate()
    {
        main.Point.OnPointChanged += OnPointChanged;
        main.Stage.OnGameClear += OnGameClear;
        main.Game.OnRemainingTurnChanged += OnRemainingTurnChanged;
        main.Stage.OnGameFail += OnGameFail;
        main.Store.OnClickButton += OnClickStoreButton;
    }

    private void InitSetting()
    {
        settingButton.onClick.AddListener(OnOpenSettingButtonTapped);
        closeSettingButton.onClick.AddListener(OnCloseSettingButtonTapped);
        // retryButton.onClick.AddListener(Retry);
        // lobbyButton.onClick.AddListener(GoLobbyButton);
        main.Sound.InitPanel(bgmSlider, sfxSlider);
    }

    void InitStore()
    {
        preventDestroyButton.onClick.AddListener(main.Store.PreventDestroyBtn);
        addTurnButton.onClick.AddListener(main.Store.AddTurnBtn);
        destroyTileButton.onClick.AddListener(main.Store.DestoryTileBtn);
    }



    // INewTurnListener
    // - - - - - - - - - - - -
    public void Subscribe_NewTurn()
    {
        EventManager.Subscribe(GamePhase.NewTurnPhase, OnEnter_NewTurn);
    }

    public void OnEnter_NewTurn()
    {
        StartCoroutine(DelayUpdateUI());
    }


    // - - - - - - - - - - - -
    public void TurnOn()
    {
        defaultCanvas.gameObject.SetActive(true);
        aboveCanvas.gameObject.SetActive(true);
        wallImage.sprite = wallSprite;
        wallImage.SetNativeSize();

        main.Stage.SpawnSlime(main.CurrentStageIndex);

        var info = main.CurrentStageInfo;
        slimeImage.sprite = info.SlimeSprite;
        slimeImage.SetNativeSize();
        slimeImage.gameObject.SetActive(true);

        stageText.sprite = info.StageTextSprite;
        stageText.SetNativeSize();

        turnText.text = $"{info.MaxTurn}/{info.MaxTurn}";

        targetTileText.text = info.TargetTile.ToString();

        failPanel.gameObject.SetActive(false);

        main.Sound.InitPanel(bgmSlider, sfxSlider);
        SetAllButtons(true);
    }

    public void OnEnterStage()
    {
        board.gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        defaultCanvas.gameObject.SetActive(false);
        aboveCanvas.gameObject.SetActive(false);
        slimeImage.gameObject.SetActive(false);
        // main.Pooler.gameObject.SetActive(false);
        board.gameObject.SetActive(false);
    }


    // 타일이 스폰된 후까지 기다린 후 체크
    private System.Collections.IEnumerator DelayUpdateUI()
    {
        yield return null;
        // OnPointChanged();
    }

    private void OnPointChanged()
    {
        var point = main.Point.Points;
        pointsText.text = $"{point}";

        pointSymbolImage.DOScale(2f, .2f)
            .SetEase(Ease.InOutBounce)
            .SetLoops(2, LoopType.Yoyo);

        // if (nextStagePanel.activeSelf)
        //     return;

        UpdateStoreButtonState();
    }

    private void OnRemainingTurnChanged()
    {
        var currentColor = GameManager.Instance.CurTurns > 5 ? "#6c6ca8" : "#ff4d4d";
        turnText.text = $"<color={currentColor}>{GameManager.Instance.CurTurns}</color>/<color=#6c6ca8>{main.Stage.MaxTurn}</color>";
    }

    // Game Clear
    // - - - - - - - - - - - -
    private void OnGameClear(Transform clearTile)
    {
        if (!isSFXPlayed)
        {
            // SoundManager.Instance.PlayStageClearSFX();
            isSFXPlayed = true;
        }
        SetAllButtons(false);

        if (clearTile != null)
        {
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(1f);
            sequence.Append(clearTile.DOLocalMove(new Vector3(0, 2.6f), .6f)
                .SetEase(Ease.InBack))
                .SetSpeedBased();
            sequence.AppendCallback(() =>
            {
                var particle = main.Pooler.GetObject(27, Group.Effect).GetComponent<ParticleSystem>();
                particle.transform.localScale = new Vector3(2f, 2f);
                particle.transform.position = clearTile.position;
                particle.Play();
                clearTile.DOScale(Vector3.zero, 0.3f);
            });
            sequence.Append(slimeImage.transform.DOScale(1.2f, .2f)
                .SetLoops(2, LoopType.Yoyo));
            sequence.OnComplete(() =>
            {
                main.LoadToLobby();
            });
        }
    }

    // Fail
    // - - - - - - - - - - - -
    private void OnGameFail()
    {
        main.Game.IsPaused = true;
        SetAllButtons(false);
        failPanel.GetComponentInChildren<Button>().onClick.AddListener(LoadToLobbyOnFail);
        failPanel.gameObject.SetActive(true);
    }

    private void LoadToLobbyOnFail()
    {
        main.LoadToLobby(isWin: false);
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
        // SetActiveHideObjects(false);

        settingPanel.gameObject.SetActive(true);
        var sequence = DOTween.Sequence();

        OpenPanel(sequence, settingPanel);
    }

    private void OnCloseSettingButtonTapped()
    {
        OnEscapeButtonTapped = null;

        // SetActiveHideObjects(true);
        var sequence = DOTween.Sequence();
        ClosePanel(sequence, settingPanel);
        sequence.OnComplete(() =>
        {
            SetAllButtons(true);
            main.Game.IsPaused = false;
            settingPanel.gameObject.SetActive(false);
        });
    }

    private void OnClickStoreButton(object _, StoreManager.ClickInfo clickInfo)
    {
        SetAllButtons(!clickInfo.isSelecting);
    }

    // Button Interactable Helper
    // - - - - - - - - - - - -
    private void UpdateStoreButtonState()
    {
        var point = main.Point.Points;

        preventDestroyButton.interactable = !_isPanelOn && point >= main.Store.PreventDestroyCost;
        addTurnButton.interactable = !_isPanelOn && point >= main.Store.AddTurnCost;
        destroyTileButton.interactable = !_isPanelOn && (point >= main.Store.DestroyTileCost) && (GameManager.Instance.CountTile() > 1);
    }

    private void SetAllButtons(bool value)
    {
        var point = main.Point.Points;

        preventDestroyButton.interactable = value ? point >= main.Store.PreventDestroyCost : false;
        addTurnButton.interactable = value ? point >= main.Store.AddTurnCost : false;
        destroyTileButton.interactable = value && (point >= main.Store.DestroyTileCost) && GameManager.Instance.CountTile() > 1;

        settingButton.interactable = value;
        // codexButton.interactable = value;
        // pointHintToggleButton.interactable = value;
    }


    // DOTween Helper
    private void OpenPanel(Sequence sequence, Transform panel)
    {
        if (panel == null) return;
        var rect = panel as RectTransform;
        if (rect == null) return;

        panel.gameObject.SetActive(true);

        var dur = main.LobbyUI.panelDuration;
        sequence.Append(rect.DOAnchorPosY(-155, dur)
            .SetEase(Ease.OutBack));
        sequence.Join(rect.DOScale(Vector3.one, dur)
            .SetEase(Ease.OutBack));

        dur -= .1f;
        sequence.Join(main.Pooler.transform.DOScale(Vector3.zero, dur)
            .SetEase(Ease.InBack));
        sequence.Join(board.DOScale(Vector3.zero, dur)
            .SetEase(Ease.InBack));

        sequence.Join(preventDestroyButton.transform.DOLocalMoveX(-storeButtonX, dur)
            .SetEase(Ease.InBack)
            .SetRelative());
        sequence.Join(addTurnButton.transform.DOLocalMoveX(-storeButtonX, dur)
            .SetEase(Ease.InBack)
            .SetRelative());
        sequence.Join(destroyTileButton.transform.DOLocalMoveX(-storeButtonX, dur)
            .SetEase(Ease.InBack)
            .SetRelative());
        sequence.Join(pointHintToggleButton.transform.DOLocalMoveX(storeButtonX, dur)
            .SetRelative());
    }

    private void ClosePanel(Sequence sequence, Transform panel)
    {
        if (panel == null) return;
        var rect = panel as RectTransform;
        if (rect == null) return;

        var dur = main.LobbyUI.panelDuration;
        sequence.Append(rect.DOAnchorPosY(-1600, dur)
            .SetEase(Ease.InBack));
        sequence.Join(rect.DOScale(Vector3.zero, dur)
            .SetEase(Ease.InBack));

        dur += .4f;
        sequence.Join(main.Pooler.transform.DOScale(Vector3.one, dur)
            .SetEase(Ease.OutBack));
        sequence.Join(board.DOScale(Vector3.one, dur)
            .SetEase(Ease.OutBack));

        sequence.Join(preventDestroyButton.transform.DOLocalMoveX(storeButtonX, dur)
            .SetEase(Ease.OutBack)
            .SetRelative());
        sequence.Join(addTurnButton.transform.DOLocalMoveX(storeButtonX, dur)
            .SetEase(Ease.OutBack)
            .SetRelative());
        sequence.Join(destroyTileButton.transform.DOLocalMoveX(storeButtonX, dur)
            .SetEase(Ease.OutBack)
            .SetRelative());
        sequence.Join(pointHintToggleButton.transform.DOLocalMoveX(-storeButtonX, dur)
            .SetRelative());

    }
}

