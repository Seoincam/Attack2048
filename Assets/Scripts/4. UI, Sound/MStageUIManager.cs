using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MStageUIManager : MonoBehaviour, INewTurnListener
{
    private Main main;
    private RectTransform slimeRect;

    private bool _isPanelOn;
    //클리어 효과음 중복 방지
    public bool isSFXPlayed = false;
    private bool isPointHint = false;

    [Header("Slime Movement")]
    [SerializeField] float moveAmount = 16;

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

    [Header("End")] 
    [SerializeField] private Sprite clearSprite;
    [SerializeField] private Sprite failSprite;
    [SerializeField] private Sprite nextStageSprite;
    [SerializeField] private Sprite endRetrySprite;
    [SerializeField] private Image endBackground;
    private const float endBGAlpha = .85f;
    [SerializeField] private Transform endPanel;
    [SerializeField] private Image endMessage;
    [SerializeField] private Button endNextOrRetryButton;
    [SerializeField] private Button endLobbyButton;
    [Space] 
    [SerializeField] private Sprite winCatSprite;
    [SerializeField] private Sprite failCatSprite;

    [Header("Setting")]
    [SerializeField] Transform settingPanel;
    [SerializeField] Button settingButton;
    [SerializeField] Button closeSettingButton;
    [SerializeField] Button openRetryButton;
    [SerializeField] Button openLobbyButton;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;
    [Space]
    [SerializeField] Transform retryPanel;
    [SerializeField] Button closeRetryButton;
    [SerializeField] Button retryButton;
    [Space]
    [SerializeField] Transform lobbyPanel;
    [SerializeField] Button closeLobbyButton;
    [SerializeField] Button lobbyButton;

    [Header("Slime HPBar")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image targetIconImage;
    private Tween _hpTween;
    
    private bool _isEnd;
    private bool _isWin;
    private Sprite _catCache;

    [Header("Escape")]
    private Action OnEscapeButtonTapped;

    //HP
    private HashSet<int> _hit = new();// 달성 기록용
    private int _a1, _a2, _a3, _a4; // 왼쪽부터 타겟/8, 타겟/4 ...
    private float HP
    {
        get => hpSlider != null ? 1 - hpSlider.value : 1f;
        set
        {
            if (hpSlider == null) return;
            
            _hpTween?.Kill();
            _hpTween = DOTween.To(
                    () => hpSlider.value,
                    x => hpSlider.value = x,
                    1 - Mathf.Clamp01(value),
                    .3f)
                .SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    _hpTween = null;
                    if (!_isEnd) return;

                    ChangeEndPanel(_isWin);
                    endPanel.gameObject.SetActive(true);
                });
        }
    }

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
        slimeRect = slimeImage.rectTransform;

        InitDelegate();
        InitSetting();
        InitStore();
        Subscribe_NewTurn();
        pointHintToggleButton.onClick.AddListener(TogglePointHint);
        RefreshAndResetHP();

        OnPointChanged();
        UpdateStoreButtonState();

        endLobbyButton.onClick.AddListener(() =>
        {
            main.LoadToLobby(true);
        });
    }

    // 뒤로가기 감지
    void Update()
    {
        if (main.State != GameState.Stage)
            return;

        MoveImage();

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
        //HPBar구독
        main.Stage.OnSlimeChanged += OnSlimeChanged_RefreshHP;
        main.Game.OnGetPoint += OnGetPoint_ProgressHP;
    }

    private void InitSetting()
    {
        settingButton.onClick.AddListener(OnOpenSettingButtonTapped);
        closeSettingButton.onClick.AddListener(OnCloseSettingButtonTapped);
        openRetryButton.onClick.AddListener(OnOpenRetryTapped);
        openLobbyButton.onClick.AddListener(OnOpenLobbyTapped);
        main.Sound.InitPanel(bgmSlider, sfxSlider);

        closeRetryButton.onClick.AddListener(OnCloseRetryTapped);
        retryButton.onClick.AddListener(Retry);
        closeLobbyButton.onClick.AddListener(OnCloseLobbyTapped);
        lobbyButton.onClick.AddListener(GoLobby);
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
        if (_catCache)
            pointHintToggleButton.image.sprite = _catCache;
        pointHintToggleButton.interactable = true;
            
        _isEnd = false;
        endPanel.gameObject.SetActive(false);
        endBackground.gameObject.SetActive(false);
        
        defaultCanvas.gameObject.SetActive(true);
        aboveCanvas.gameObject.SetActive(true);
        wallImage.sprite = wallSprite;
        wallImage.SetNativeSize();

        main.Stage.SpawnSlime(main.CurrentStageIndex);

        var info = main.CurrentStageInfo;
        slimeImage.sprite = info.SlimeSprite;
        slimeImage.SetNativeSize();
        slimeImage.gameObject.SetActive(true);
        slimeImage.transform.localScale = info.StageIndex == 3
            ? new Vector3(.5f, .5f)
            : new Vector3(.75f, .75f);

        stageText.sprite = info.StageTextSprite;
        stageText.SetNativeSize();

        turnText.text = $"{info.MaxTurn}/{info.MaxTurn}";

        targetTileText.text = info.TargetTile.ToString();

        //스테이지 켜질때 HP 리셋
        RefreshAndResetHP();
        SetTargetIcon();
        main.Sound.InitPanel(bgmSlider, sfxSlider);
        SetAllButtons(true);
    }

    public void OnEnterStage()
    {
        main.Pooler.transform.localScale = Vector3.zero;
        board.localScale = Vector3.zero;
        board.gameObject.SetActive(true);

        var dur = main.LobbyUI.panelDuration + .2f;
        var sequence = DOTween.Sequence();
        sequence.Append(main.Pooler.transform.DOScale(Vector3.one, dur)
            .SetEase(Ease.OutBack));
        sequence.Join(board.DOScale(Vector3.one, dur)
            .SetEase(Ease.OutBack));
        sequence.OnComplete(() => main.Game.StartGame());
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
            main.Sound.PlayStageClearSFX();
            isSFXPlayed = true;
        }
        SetAllButtons(false);

        var nextStage = main.CurrentStageIndex + 2;
        if (nextStage <= 7)
        {
            main.stagesOpened[nextStage] = true;
            PlayerPrefs.SetInt($"stage{nextStage}", 1);
        }

        _isEnd = true;
        _isWin = true;
    }

    // Fail
    // - - - - - - - - - - - -
    private void OnGameFail()
    {
        main.Game.IsPaused = true;
        SetAllButtons(false);

        if (_hpTween != null)
        {
            _isEnd = true;
            _isWin = false;
        }
        else
        {
            ChangeEndPanel(false);
            endPanel.gameObject.SetActive(true);
        }
    }

    private void ChangeEndPanel(bool isClear)
    {
        endNextOrRetryButton.onClick.RemoveAllListeners();
        if (isClear)
        {
            _catCache = pointHintToggleButton.image.sprite;
            pointHintToggleButton.image.sprite = winCatSprite;
            
            endMessage.sprite = clearSprite;
            endNextOrRetryButton.image.sprite = nextStageSprite;
            endNextOrRetryButton.onClick.AddListener(() =>
            {
                CloseEndPanel().OnComplete(() =>
                {
                    endBackground.gameObject.SetActive(false);
                    endPanel.gameObject.SetActive(false);
                    main.LoadToStage(main.CurrentStageIndex + 1);
                    board.DOScale(Vector3.zero, .5f)
                        .SetEase(Ease.InBack);
                });
            });
        }
        else
        {
            _catCache = pointHintToggleButton.image.sprite;
            pointHintToggleButton.image.sprite = failCatSprite;
            
            endMessage.sprite = failSprite;
            endNextOrRetryButton.image.sprite = endRetrySprite;
            endNextOrRetryButton.onClick.AddListener(() =>
            {
                CloseEndPanel().OnComplete(() =>
                {
                    endBackground.gameObject.SetActive(false);
                    endPanel.gameObject.SetActive(false);
                    main.Game.OnEnterStage();
                    TurnOn();
                    main.Game.StartGame();
                });
            });
        }

        endBackground.color -= new Color(0, 0, 0, endBackground.color.a);
        endBackground.gameObject.SetActive(true);
        endPanel.transform.localScale = Vector3.zero;

        var seq = DOTween.Sequence()
            .Append(endPanel.DOScale(Vector3.one, .5f)
                .SetEase(Ease.OutBack))
            .Join(endBackground.DOColor(endBackground.color + new Color(0, 0, 0, endBGAlpha), .4f));

        pointHintToggleButton.interactable = false;
    }

    private Sequence CloseEndPanel()
    {
        return DOTween.Sequence()
            .Append(endPanel.DOScale(Vector3.zero, .5f)
                .SetEase(Ease.InBack))
            .Join(endBackground.DOColor(endBackground.color - new Color(0, 0, 0, endBackground.color.a), .4f));
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
        
        main.Sound.SaveSetting();

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

    // Retry
    // - - - - - - - - -
    private void OnOpenRetryTapped()
    {
        OnEscapeButtonTapped = OnCloseRetryTapped;

        var sequence = DOTween.Sequence();
        ClosePanel(sequence, settingPanel, isPopOver: true);
        sequence.AppendCallback(() => settingPanel.gameObject.SetActive(false));
        OpenPanel(sequence, retryPanel, isPopOver: true);
    }

    private void OnCloseRetryTapped()
    {
        OnEscapeButtonTapped = OnCloseSettingButtonTapped;
        var sequence = DOTween.Sequence();
        ClosePanel(sequence, retryPanel, isPopOver: true);
        sequence.AppendCallback(() => retryPanel.gameObject.SetActive(false));
        OpenPanel(sequence, settingPanel, isPopOver: true);
    }

    private void Retry()
    {
        main.Game.OnEnterStage();

        var sequence = DOTween.Sequence();
        ClosePanel(sequence, retryPanel);
        sequence.OnComplete(() =>
        {
            retryPanel.gameObject.SetActive(false);
            TurnOn();
            main.Game.StartGame();
        });
    }

    // Lobby
    // - - - - - - - - -
    private void OnOpenLobbyTapped()
    {
        OnEscapeButtonTapped = OnCloseLobbyTapped;

        var sequence = DOTween.Sequence();
        ClosePanel(sequence, settingPanel, isPopOver: true);
        sequence.AppendCallback(() => settingPanel.gameObject.SetActive(false));
        OpenPanel(sequence, lobbyPanel, isPopOver: true);
    }

    private void OnCloseLobbyTapped()
    {
        OnEscapeButtonTapped = OnCloseSettingButtonTapped;
        var sequence = DOTween.Sequence();
        ClosePanel(sequence, lobbyPanel, isPopOver: true);
        sequence.AppendCallback(() => lobbyPanel.gameObject.SetActive(false));
        OpenPanel(sequence, settingPanel, isPopOver: true);
    }

    private void GoLobby()
    {
        var sequence = DOTween.Sequence();
        ClosePanel(sequence, lobbyPanel);
        sequence.OnComplete(() =>
        {
            retryPanel.gameObject.SetActive(false);
            endBackground.gameObject.SetActive(false);
            // TurnOn();
            main.LoadToLobby(true);
        });
    }


    // point Hint
    // - - - - - - - - - - - -
    private void TogglePointHint()
    {
        pointHintToggleButton.interactable = false;
        isPointHint = !isPointHint;
        var sequence = DOTween.Sequence();
        sequence.Append(pointHintToggleButton.transform.DOScale(1.1f, .1f));
        sequence.AppendCallback(() => pointHintToggleButton.image.sprite = isPointHint ? closeEye : openEye);
        sequence.Append(pointHintToggleButton.transform.DOScale(1f, .1f));
        if (isPointHint)
        {
            pointHintPanel.SetActive(true);
            sequence.Join(preventDestroyButton.transform.DOScale(0f, .2f)
                .SetEase(Ease.InBack));
            sequence.Join(addTurnButton.transform.DOScale(0f, .2f)
                .SetEase(Ease.InBack));
            sequence.Join(destroyTileButton.transform.DOScale(0f, .2f)
                .SetEase(Ease.InBack));
            sequence.Append(pointHintPanel.transform.DOScale(1f, .2f)
                .SetEase(Ease.OutBack));
            sequence.OnComplete(() => pointHintToggleButton.interactable = true);
        }

        else
        {
            sequence.Join(pointHintPanel.transform.DOScale(0f, .2f)
                .SetEase(Ease.InBack));
            sequence.Append(preventDestroyButton.transform.DOScale(1f, .2f)
                .SetEase(Ease.OutBack));
            sequence.Join(addTurnButton.transform.DOScale(1f, .2f)
                .SetEase(Ease.OutBack));
            sequence.Join(destroyTileButton.transform.DOScale(1f, .2f)
                .SetEase(Ease.OutBack));

            sequence.OnComplete(() =>
            {
                pointHintPanel.SetActive(false);
                pointHintToggleButton.interactable = true;
            });
        }
    }

    // Store 
    // - - - - - - - - - - - -
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
    private void OpenPanel(Sequence sequence, Transform panel, bool isPopOver = false)
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

        if (!isPopOver)
        {
            dur -= .1f;
            sequence.Join(main.Pooler.transform.DOScale(Vector3.zero, dur)
                .SetEase(Ease.InBack));
            sequence.Join(board.DOScale(Vector3.zero, dur)
                .SetEase(Ease.InBack));

            if (!isPointHint)
            {
                sequence.Join(preventDestroyButton.transform.DOLocalMoveX(-storeButtonX, dur)
                    .SetEase(Ease.InBack)
                    .SetRelative());
                sequence.Join(addTurnButton.transform.DOLocalMoveX(-storeButtonX, dur)
                    .SetEase(Ease.InBack)
                    .SetRelative());
                sequence.Join(destroyTileButton.transform.DOLocalMoveX(-storeButtonX, dur)
                    .SetEase(Ease.InBack)
                    .SetRelative());
            }
            else
            {
                sequence.Join(pointHintPanel.transform.DOLocalMoveX(-storeButtonX, dur)
                    .SetEase(Ease.InBack)
                    .SetRelative());
            }

            sequence.Join(pointHintToggleButton.transform.DOLocalMoveX(storeButtonX, dur)
                .SetRelative());
        }

    }

    private void ClosePanel(Sequence sequence, Transform panel, bool isPopOver = false)
    {
        if (panel == null) return;
        var rect = panel as RectTransform;
        if (rect == null) return;

        var dur = main.LobbyUI.panelDuration;
        sequence.Append(rect.DOAnchorPosY(-1600, dur)
            .SetEase(Ease.InBack));
        sequence.Join(rect.DOScale(Vector3.zero, dur)
            .SetEase(Ease.InBack));

        if (!isPopOver)
        {
            dur += .4f;
            sequence.Join(main.Pooler.transform.DOScale(Vector3.one, dur)
                .SetEase(Ease.OutBack));
            sequence.Join(board.DOScale(Vector3.one, dur)
                .SetEase(Ease.OutBack));

            if (!isPointHint)
            {
                sequence.Join(preventDestroyButton.transform.DOLocalMoveX(storeButtonX, dur)
                    .SetEase(Ease.OutBack)
                    .SetRelative());
                sequence.Join(addTurnButton.transform.DOLocalMoveX(storeButtonX, dur)
                    .SetEase(Ease.OutBack)
                    .SetRelative());
                sequence.Join(destroyTileButton.transform.DOLocalMoveX(storeButtonX, dur)
                    .SetEase(Ease.OutBack)
                    .SetRelative());
            }
            else
            {
                sequence.Join(pointHintPanel.transform.DOLocalMoveX(storeButtonX, dur)
                    .SetEase(Ease.OutBack)
                    .SetRelative());
            }
            sequence.Join(pointHintToggleButton.transform.DOLocalMoveX(-storeButtonX, dur)
                .SetRelative());
        }
    }
    

    private void MoveImage()
    {
        if (slimeRect == null)
            return;
            
        var tiltSpeed = 7;
        float lerpX = Mathf.LerpAngle(slimeRect.eulerAngles.x, Mathf.Sin(Time.time) * moveAmount, tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(slimeRect.eulerAngles.y, Mathf.Cos(Time.time) * moveAmount, tiltSpeed * Time.deltaTime);
        float y = Mathf.Lerp(slimeRect.anchoredPosition.y, 550 + Mathf.Sin(Time.time) * 10, Time.deltaTime);
        slimeRect.eulerAngles = new Vector3(lerpX, lerpY, 0);
        slimeRect.anchoredPosition = new Vector2(0, y);
    }

    //HP Bar 로직
    private void OnSlimeChanged_RefreshHP(object _, StageManager.SlimeInfo __)
    {
        RefreshAndResetHP();
        SetTargetIcon();
    }

    private void RefreshAndResetHP()
    {
        _hit.Clear();

        int target = (main.Stage.CurrentSlime != null) ? main.Stage.CurrentSlime.ClearValue :
            (main.CurrentStageInfo != null) ? main.CurrentStageInfo.TargetTile : 128;
        _a1 = target / 8;
        _a2 = target / 4;
        _a3 = target / 2;
        _a4 = target;
        HP = 1f;
    }

    private void OnGetPoint_ProgressHP(object _, PointManager.PointGetInfo info)
    {
        // 타일 값이 처음 달성할시 표시하고 HP 감소
        int v = info.tileValue;
        if(v == _a4)
        {
            TryMark(_a4);
            HP = 0f;
            return;
        }
        if(v == _a3)
        {
            if (TryMark(_a3)) HP -= 0.25f;
            return;
        }
        if(v == _a2)
        {
            if (TryMark(_a2)) HP -= 0.25f;
            return;
        }
        if (v == _a1)
        { 
            if (TryMark(_a1)) HP -= 0.25f;
            return;
        }
    }

    private bool TryMark(int a)
    {
        if (_hit.Contains(a)) return false;
        _hit.Add(a);
        return true;
    }

    private void SetTargetIcon()
    {
        if (targetIconImage == null) return;

        Sprite spr = (main.CurrentStageInfo != null) ? main.CurrentStageInfo.TargetTileSprite : null;
        if (spr != null) targetIconImage.sprite = spr;
        targetIconImage.raycastTarget = false;
    }
}


