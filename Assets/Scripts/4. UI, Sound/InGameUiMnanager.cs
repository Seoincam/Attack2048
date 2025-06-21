using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InGameUiMnanager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI remainingTurnsText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI clearValueText;

    [SerializeField] private Transform settingPanel;
    [SerializeField] private Transform codexPanel;
    [SerializeField] private GameObject[] Codex;

    private Text _indexText;
    private int _index;

    private Main main;


    public void Init(Main main)
    {
        this.main = main;

        main.Game.OnRemainingTurnChanged += OnRemainingTurnChanged;
        main.Point.OnPointChanged += OnPointChanged;
        main.Stage.OnSlimeChanged += OnSlimeChanged;

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
}
