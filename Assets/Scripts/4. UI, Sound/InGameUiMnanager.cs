using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class InGameUiMnanager : MonoBehaviour
{
    [SerializeField] private LoadingSO loadingSO;

    [SerializeField] private TextMeshProUGUI remainingTurnsText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI clearValueText;

    [SerializeField] private Transform settingPanel;
    [SerializeField] private Transform codexPanel;
    [SerializeField] private GameObject[] Codex;

    [SerializeField] private GameObject NextStagePanel;
    [SerializeField] private GameObject FailPanel;

    private Text _indexText;
    private int _index;

    private Main main;


    public void Init(Main main)
    {
        this.main = main;

        main.Game.OnRemainingTurnChanged += OnRemainingTurnChanged;
        main.Point.OnPointChanged += OnPointChanged;
        main.Stage.OnSlimeChanged += OnSlimeChanged;

        main.Stage.OnGameClear += OnGameClear;
        main.Stage.OnGameFail += OnGameFail;

        SoundManager.Instance.SetPanel
        (
            settingPanel.Find("BGM/BGM Slider").GetComponent<Slider>(),
            settingPanel.Find("SFX/SFX Slider").GetComponent<Slider>()
        );

        _indexText = codexPanel.Find("Index Text").GetComponent<Text>();
    }

    private void OnRemainingTurnChanged()
    {
        remainingTurnsText.text = $"Remaining Turns: {GameManager.Instance.CurTurns}";
    }

    private void OnPointChanged()
    {
        pointsText.text = $"{main.Point.Points}pt";
    }

    private void OnSlimeChanged(object _, EventArgs slimeInfo)
    {
        if (slimeInfo is StageManager.SlimeInfo info)
        {
            stageText.text = $"Stage {info.stageIndex}";
            clearValueText.text = $"Clear: {info.clearValue}";
        }
    }

    // Setting Panel
    // - - - - - - - - -
    public void OpenSetting()
    {
        main.Game.IsPaused = true;
        settingPanel.gameObject.SetActive(true);
    }

    public void CloseSetting()
    {
        main.Game.IsPaused = false;
        settingPanel.gameObject.SetActive(false);
    }

    // Codex Panel
    // - - - - - - - - -
    public void OpenCodex()
    {
        main.Game.IsPaused = true;
        codexPanel.gameObject.SetActive(true);
    }

    public void CloseCodex()
    {
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
        NextStagePanel.GetComponentInChildren<Button>().onClick.AddListener(NextStageButton);
        NextStagePanel.SetActive(true);
        GameManager.Instance.IsPaused = true;
    }

    private void NextStageButton()
    {
        if (main.Stage.GoNextStage())
        {
            NextStagePanel.SetActive(false);

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
            GameManager.Instance.IsPaused = false;
        }

        else
        {
            Debug.Log("클리어!");
        }
    }

    private void OnGameFail()
    {
        FailPanel.GetComponentInChildren<Button>().onClick.AddListener(GoLobbyButton);
        FailPanel.SetActive(true);
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
}
