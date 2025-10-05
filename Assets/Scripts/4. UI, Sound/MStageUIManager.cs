using System;
using DG.Tweening;
using NUnit.Framework.Interfaces;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Fail")]
    [SerializeField] Transform failPanel;

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
        slimeRect = slimeImage.rectTransform;

        InitDelegate();
        InitSetting();
        InitStore();
        Subscribe_NewTurn();
        pointHintToggleButton.onClick.AddListener(TogglePointHint);

        OnPointChanged();
        UpdateStoreButtonState();
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

        failPanel.gameObject.SetActive(false);

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
            var scale = slimeImage.transform.localScale.x * 1.3f;
            sequence.Append(slimeImage.transform.DOScale(scale, .1f)
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
            // TurnOn();
            main.LoadToLobby();
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
}

