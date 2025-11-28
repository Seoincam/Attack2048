using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using DG.Tweening;

public class LobbyUiManager : MonoBehaviour
{
    private Main main;

    [Header("Tween Setting")]
    public float creditDuration = 0.4f;
    public float logoDuration = 0.4f;
    public float buttonsDuration = 0.25f;
    public Ease buttonsEase = Ease.OutBack;
    public float panelDuration = 1.2f;

    [Header("Canvas")]
    [SerializeField] private Canvas defaultCanvas;
    [SerializeField] private Canvas aboveCanvas;

    [Header("Images")]
    [SerializeField] private GameObject[] hideObjects;
    [SerializeField] private Image logo;
    [SerializeField] private RectTransform sky;

    [Header("Default Buttons")]
    [SerializeField] private Button startButton;

    [Header("Setting")]
    [SerializeField] private Button settingButton;
    [SerializeField] private Transform settingPanel;
    [SerializeField] private Button closeSettingButton;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Credit")]
    [SerializeField] private Button creditButton;
    [SerializeField] private Transform creditPanel;
    [SerializeField] private Button creditCloseButton;

    [Header("Codex")]
    [SerializeField] private RectTransform codexButton;
    [SerializeField] private Transform codexPanel;
    [SerializeField] private Button codexCloseButton;

    [Header("Stage Selection")] 
    [SerializeField] private Sprite[] descriptionSprites;
    [SerializeField] private Text stageText;
    [SerializeField] private Image stageSelectionPanel;
    [SerializeField] private Image stageSelectionBackground;
    [SerializeField] private RectTransform stageSelectionPrevButton;
    [SerializeField] private RectTransform stageSelectionNextButton;
    [SerializeField] private Button stageSelectionCloseButton;
    [SerializeField] private Button enterStageButton;
    [SerializeField] private Image slimeDescription;
    private float _stageSelectionBackgroundAlpha;

    [Header("Exit")]
    [SerializeField] private Button escapeButton;
    [SerializeField] private Transform exitPanel;
    private bool isEscapePopUp = false;

    [Header("Test")]
    [SerializeField] private InputField testStartIndexInputFied;

    private Action OnEscapeButtonTapped;
    private int _stageIndex;
    
    // 뒤로가기 감지
    void Update()
    {
        if (main.State != GameState.Lobby)
            return;

        MoveImage();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapeButtonTapped ??= OpenExitPanel;
            OnEscapeButtonTapped?.Invoke();
        }
    }

    // 초기화
    // - - - - - - - - -
    public void Init(Main main)
    {
        this.main = main;

        InitStart();
        InitSetting();
        // InitCodex();
        InitCredit();
        InitEscape();
        main.Sound.PlayBGM(main.Sound.LobbyBGM);
    }

    public void OnEnterLobby(bool isFirst = false)
    {
        defaultCanvas.gameObject.SetActive(true);
        aboveCanvas.gameObject.SetActive(true);
        codexButton.gameObject.SetActive(true);
        logo.gameObject.SetActive(true);
        main.Sound.InitPanel(bgmSlider, sfxSlider);
        

        if (isFirst)
        {
            SetAllButton(false);

            var sequence = DOTween.Sequence();
            sequence.Append(creditButton.GetComponent<RectTransform>().DOAnchorPosX(0, creditDuration).SetEase(Ease.OutCubic));
            sequence.Append(logo.DOColor(Color.white, logoDuration).SetEase(Ease.OutCubic));

            FadeInButton(sequence, startButton, 550);
            FadeInButton(sequence, settingButton, 350);
            FadeInButton(sequence, escapeButton, 150);

            sequence.OnComplete(() => SetAllButton(true));
        }
    }

    void InitSetting()
    {
        settingButton.onClick.AddListener(OnOpenSettingButtonTapped);
        closeSettingButton.onClick.AddListener(OnCloseSettingButtonTapped);
        main.Sound.InitPanel(bgmSlider, sfxSlider);
    }

    void InitCodex()
    {
        // codexButton.onClick.AddListener(OnOpenCodexButtonTapped);
        codexCloseButton.onClick.AddListener(OnCloseCodexButtonTapped);
    }

    void InitCredit()
    {
        creditButton.onClick.AddListener(OnOpenCreditButtonTapped);
        creditPanel.Find("Close Button").GetComponent<Button>().onClick.AddListener(OnCloseCreditButtonTapped);
    }

    void InitEscape()
    {
        escapeButton.onClick.AddListener(OpenExitPanel);
        exitPanel.Find("Button Layout Group/Exit Button").GetComponent<Button>().onClick.AddListener(OnExitButtonTapped);
        exitPanel.Find("Button Layout Group/Cancle Button").GetComponent<Button>().onClick.AddListener(OnCancleExitButtonTapped);
    }

    private void InitStart()
    {
        startButton.onClick.AddListener(OnStartButtonTapped);
        stageSelectionCloseButton.onClick.AddListener(OnCloseStageSelectionButtonTapped);
        enterStageButton.onClick.AddListener(OnEnterStageButtonTapped);
        stageSelectionPrevButton.GetComponent<Button>().onClick.AddListener(OnPrevStageButtonTapped);
        stageSelectionNextButton.GetComponent<Button>().onClick.AddListener(OnNextStageButtonTapped);
        _stageIndex = 0;
        ChangeStageInfo(_stageIndex);
    }


    // Lobby Buttons
    // - - - - - - - - -    
    private void OnStartButtonTapped()
    {
        OnEscapeButtonTapped = OnCloseStageSelectionButtonTapped;
        
        // 준비
        _stageSelectionBackgroundAlpha = stageSelectionBackground.color.a;
        stageSelectionBackground.color -= new Color(0, 0, 0, _stageSelectionBackgroundAlpha);
        stageSelectionBackground.gameObject.SetActive(true);
        
        stageSelectionPanel.rectTransform.localScale = Vector3.zero;
        stageSelectionPanel.gameObject.SetActive(true);
        
        ChangeStageInfo(_stageIndex, true);
        
        // 트윈
        var seq = DOTween.Sequence()
                .Append(stageSelectionBackground.DOColor(
            stageSelectionBackground.color + new Color(0, 0, 0, _stageSelectionBackgroundAlpha), .4f))
                .Join(stageSelectionPanel.rectTransform.DOScale(Vector3.one, .4f)
                    .SetEase(Ease.OutBack));
    }

    private void OnCloseStageSelectionButtonTapped()
    {
        OnEscapeButtonTapped = null;
        
        var seq = DOTween.Sequence()
            .Append(stageSelectionPanel.rectTransform.DOScale(Vector3.zero, .4f)
                .SetEase(Ease.InBack))
            .Join(stageSelectionBackground.DOColor(
                stageSelectionBackground.color - new Color(0, 0, 0, _stageSelectionBackgroundAlpha), .8f))
            .OnComplete(() =>
            {
                stageSelectionPanel.gameObject.SetActive(false);
                stageSelectionBackground.gameObject.SetActive(false);
                stageSelectionBackground.color += new Color(0, 0, 0, _stageSelectionBackgroundAlpha);
            });
    }
    
    private void OnPrevStageButtonTapped() => ChangeStageInfo(--_stageIndex);

    private void OnNextStageButtonTapped()
    {
        if (main.stagesOpened[_stageIndex + 2])
        {
            main.Sound.PlayCodexClick();
            ChangeStageInfo(++_stageIndex);    
        }
        else
        {
            main.Sound.PlayLockSFX();
        }
        
    }

    private void ChangeStageInfo(int stageIndex, bool isOpen = false)
    {
        if (!stageSelectionPanel.gameObject.activeSelf)
            return;

        if (stageIndex is < 0 or > 6)
            stageIndex = Mathf.Clamp(0, 6, stageIndex);

        stageText.text = $"{stageIndex + 1} STAGE";

        if (isOpen)
        {
            Change(stageIndex);

            stageSelectionPrevButton.GetComponent<Button>().interactable = main.stagesOpened[stageIndex];
            stageSelectionNextButton.GetComponent<Image>().color = main.stagesOpened[stageIndex + 2]
                ? Color.white
                : new Color(.8f, .8f, .8f);
                
            
            switch (stageIndex)
            {
                case 0:
                    stageSelectionPrevButton.gameObject.SetActive(false);
                    return;
                case 6:
                    stageSelectionNextButton.gameObject.SetActive(false);
                    return;
            }

            stageSelectionNextButton.gameObject.SetActive(true);
            stageSelectionPrevButton.gameObject.SetActive(true);
            return;
        }

        var dur = .4f;
        stageSelectionNextButton.GetComponent<Button>().interactable = false;
        stageSelectionPrevButton.GetComponent<Button>().interactable = false;
        enterStageButton.interactable = false;
        var seq = DOTween.Sequence()
            .Append(slimeDescription.DOFade(0f, dur)
                .SetEase(Ease.InCubic))
            .AppendCallback(() => Change(stageIndex))
            .Append(slimeDescription.DOFade(1f, dur)
                .SetEase(Ease.OutCubic))
            .OnComplete(() =>
            {
                enterStageButton.interactable = true;

                stageSelectionPrevButton.GetComponent<Button>().interactable = true;
                stageSelectionNextButton.GetComponent<Button>().interactable = true;
                stageSelectionNextButton.GetComponent<Image>().color = main.stagesOpened[stageIndex + 2]
                    ? Color.white
                    : new Color(.8f, .8f, .8f);
            });

        switch (stageIndex)
        {
            case 0:
                stageSelectionPrevButton.DOScale(Vector3.zero, dur)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => stageSelectionPrevButton.gameObject.SetActive(false));
                return;
            case 6:
                stageSelectionNextButton.DOScale(Vector3.zero, dur)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => stageSelectionNextButton.gameObject.SetActive(false));
                return;
        }

        stageSelectionNextButton.gameObject.SetActive(true);
        stageSelectionPrevButton.gameObject.SetActive(true);
        
        stageSelectionPrevButton.DOScale(Vector3.one, dur)
            .SetEase(Ease.OutBack);
        stageSelectionNextButton.DOScale(Vector3.one, dur)
            .SetEase(Ease.OutBack);
        
        
        return;
        void Change(int stageIndex)
        {
            slimeDescription.sprite = descriptionSprites[stageIndex];
            slimeDescription.SetNativeSize();
        }
    }
    
    private void OnEnterStageButtonTapped()
    {
        main.LoadToStage(_stageIndex);
        OnCloseStageSelectionButtonTapped();
    }

    public void TurnOff()
    {
        defaultCanvas.gameObject.SetActive(false);
        aboveCanvas.gameObject.SetActive(false);
        codexButton.gameObject.SetActive(false);
        logo.gameObject.SetActive(false);
    }
    // Exit Panel
    // - - - - - - - - 
    private void OpenExitPanel()
    {
        OnEscapeButtonTapped = OnCancleExitButtonTapped;
        SetAllButton(canInteractive: false);
        isEscapePopUp = true;
        exitPanel.gameObject.SetActive(true);

        var sequence = DOTween.Sequence();
        OpenPanel(sequence, exitPanel);
    }

    private void OnCancleExitButtonTapped()
    {
        OnEscapeButtonTapped = null;

        var sequence = DOTween.Sequence();
        ClosePanel(sequence, exitPanel);

        sequence.OnComplete(() =>
        {
            exitPanel.gameObject.SetActive(false);
            SetAllButton(canInteractive: true);
            isEscapePopUp = false;
        });
    }

    private void OnExitButtonTapped()
    {
        main.Sound.SaveSetting();

#if UNITY_ANDROID && !UNITY_EDITOR
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call<bool>("moveTaskToBack", true);
        }
#elif UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }


    // Credit Panel
    // - - - - - - - - -
    private void OnOpenCreditButtonTapped()
    {
        OnEscapeButtonTapped = OnCloseCreditButtonTapped;

        SetAllButton(canInteractive: false);
        creditPanel.gameObject.SetActive(true);

        var sequence = DOTween.Sequence();
        OpenPanel(sequence, creditPanel);
        sequence.OnComplete(() =>
        {
            creditPanel.GetComponentInChildren<ScrollRect>().enabled = true;
        });
    }

    private void OnCloseCreditButtonTapped()
    {
        OnEscapeButtonTapped = OnCloseSettingButtonTapped;

        creditPanel.GetComponentInChildren<ScrollRect>().enabled = false;
        var sequence = DOTween.Sequence();
        ClosePanel(sequence, creditPanel);
        sequence.OnComplete(() =>
        {
            creditPanel.gameObject.SetActive(false);
            SetAllButton(canInteractive: true);
        });
    }

    // Setting Panel
    // - - - - - - - - -
    public void OnOpenSettingButtonTapped()
    {
        OnEscapeButtonTapped = OnCloseSettingButtonTapped;
        SetAllButton(canInteractive: false);
        settingPanel.gameObject.SetActive(true);

        var sequence = DOTween.Sequence();
        OpenPanel(sequence, settingPanel);
    }

    public void OnCloseSettingButtonTapped()
    {
        OnEscapeButtonTapped = null;

        main.Sound.SaveSetting();
        var sequence = DOTween.Sequence();
        ClosePanel(sequence, settingPanel);
        sequence.OnComplete(() =>
        {
            settingPanel.gameObject.SetActive(false);
            SetAllButton(canInteractive: true);
        });
    }


    // Codex Panel
    // - - - - - - - - -
    private void OnOpenCodexButtonTapped()
    {
        OnEscapeButtonTapped = OnCloseCodexButtonTapped;

        SetAllButton(canInteractive: false);
        codexPanel.gameObject.SetActive(true);
    }

    private void OnCloseCodexButtonTapped()
    {
        OnEscapeButtonTapped = null;

        SetAllButton(canInteractive: true);
        codexPanel.gameObject.SetActive(false);
    }


    // All Buttons
    private void SetAllButton(bool canInteractive)
    {
        startButton.interactable = canInteractive;
        creditButton.interactable = canInteractive;
        escapeButton.interactable = canInteractive;

        settingButton.interactable = canInteractive;
        // codexButton.interactable = canInteractive;
    }

    private void MoveImage()
    {
        var tiltSpeed = 7;
        float lerpX = Mathf.LerpAngle(codexButton.eulerAngles.x, Mathf.Sin(Time.time) * 6, tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(codexButton.eulerAngles.y, Mathf.Cos(Time.time) * 6, tiltSpeed * Time.deltaTime);
        float y = Mathf.Lerp(codexButton.anchoredPosition.y, 600 + Mathf.Sin(Time.time) * 15, Time.deltaTime);
        codexButton.eulerAngles = new Vector3(lerpX, lerpY, 0);
        codexButton.anchoredPosition = new Vector2(0, y);

        var skyLerpX = Mathf.LerpAngle(codexButton.eulerAngles.x, Mathf.Sin(Time.time) * 10, tiltSpeed * Time.deltaTime);
        var skyLerpY = Mathf.LerpAngle(codexButton.eulerAngles.y, Mathf.Cos(Time.time) * 10, tiltSpeed * Time.deltaTime);
        sky.eulerAngles = new Vector3(skyLerpX, skyLerpY, 0);
    }

    // DOTween Helper
    private void FadeInButton(Sequence sequence, Button button, float targetY)
    {
        if (button == null) return;
        var rect = button.transform as RectTransform;
        var image = button.image;
        if (rect == null || image == null) return;

        sequence.Append(rect.DOAnchorPosY(targetY, buttonsDuration)
            .SetEase(buttonsEase));
        sequence.Join(image.DOColor(Color.white, buttonsDuration)
            .SetEase(Ease.OutCubic));
    }

    /// <summary>투명색</summary>
    private Color Transparent => new(1, 1, 1, 0);

    private void OpenPanel(Sequence sequence, Transform panel)
    {
        if (panel == null) return;
        var rect = panel as RectTransform;
        if (rect == null) return;
        
        panel.gameObject.SetActive(true);

        sequence.Append(rect.DOAnchorPosY(-155, panelDuration)
            .SetEase(Ease.OutBack));
        sequence.Join(rect.DOScale(Vector3.one, panelDuration)
            .SetEase(Ease.OutBack));

        sequence.Join(startButton.GetComponent<Image>().DOColor(Transparent, panelDuration)
            .SetEase(Ease.OutQuart));
        sequence.Join(settingButton.GetComponent<Image>().DOColor(Transparent, panelDuration)
            .SetEase(Ease.OutQuart));
        sequence.Join(escapeButton.GetComponent<Image>().DOColor(Transparent, panelDuration)
            .SetEase(Ease.OutQuart));
        sequence.Join(logo.DOColor(Transparent, panelDuration)
            .SetEase(Ease.OutQuart));
    }

    private void ClosePanel(Sequence sequence, Transform panel)
    {
        if (panel == null) return;
        var rect = panel as RectTransform;
        if (rect == null) return;

        sequence.Append(rect.DOAnchorPosY(-1500, panelDuration)
            .SetEase(Ease.InBack));
        sequence.Join(rect.DOScale(Vector3.zero, panelDuration)
            .SetEase(Ease.InBack));


        sequence.Join(startButton.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(settingButton.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(escapeButton.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(logo.DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
    }

}
