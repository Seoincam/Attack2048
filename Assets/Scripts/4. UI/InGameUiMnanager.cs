using UnityEngine;
using TMPro;
using System;

public class InGameUiMnanager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI remainingTurnsText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI clearValueText;

    void Awake()
    {
        // 로딩 됐나 체크
        if (ObjectPoolManager.Instance == null)
            return;

        GameManager.Instance.OnRemainingTurnChanged += OnRemainingTurnChanged;
        GameManager.Instance._pointManager.OnPointChanged += OnPointChanged;
        StageManager.Instance.OnSlimeChanged += OnSlimeChanged;
    }

    private void OnRemainingTurnChanged()
    {
        remainingTurnsText.text = $"Remaining Turns: {GameManager.Instance.CurTurns}";
    }

    private void OnPointChanged()
    {
        pointsText.text = $"{GameManager.Instance._pointManager.Point}pt";
    }

    private void OnSlimeChanged(object _, EventArgs slimeInfo)
    {
        if (slimeInfo is StageManager.SlimeInfo info)
        {
            stageText.text = $"Stage {info.stageIndex}";
            clearValueText.text = $"Clear: {info.clearValue}";
        }
    }
}
