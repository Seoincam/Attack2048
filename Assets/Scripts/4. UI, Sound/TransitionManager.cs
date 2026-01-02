using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] Transform circleA;
    [SerializeField] Transform circleB;
    [SerializeField] Transform circleC;
    [SerializeField] Transform circleD;
    [SerializeField] Transform circleE;

    [SerializeField] Image stageText;
    [SerializeField] Image targetTile;

    private Main main;

    public void Init(Main main)
    {
        this.main = main;
    }


    public void TransitToStage(StageInfoSO stageInfo)
    {
        SetStageInfoSprite(stageInfo);

        var sequence = DOTween.Sequence();

        sequence.Append(circleA.DOScale(new Vector3(1.2f, 1.2f, 1.2f), .5f)
            .SetEase(Ease.OutBack));
        sequence.Join(circleB.DOScale(new Vector3(1.7f, 1.7f, 1.7f), .7f)
            .SetEase(Ease.OutBack));
        sequence.Join(circleC.DOScale(new Vector3(.8f, .8f, .8f), .6f)
            .SetEase(Ease.OutBack));
        sequence.Join(circleD.DOScale(new Vector3(.8f, .8f, .8f), .6f)
            .SetEase(Ease.OutBack));

        sequence.AppendCallback(() =>
        {
            main.Sound.PlayBGM(main.Sound.stageBGM);
            main.Pooler.Init();
            main.LobbyUI.TurnOff();
            main.StageUI.TurnOn();
            main.Game.OnEnterStage();
        });

        sequence.AppendInterval(2.5f);
        sequence.Append(circleB.DOScale(Vector3.zero, .5f)
            .SetEase(Ease.InBack));
        sequence.Join(circleA.DOScale(Vector3.zero, .7f)
            .SetEase(Ease.InBack));
        sequence.Join(circleC.DOScale(Vector3.zero, .6f)
            .SetEase(Ease.InBack));
        sequence.Join(circleD.DOScale(Vector3.zero, .6f)
            .SetEase(Ease.InBack));

        sequence.OnComplete(() =>
        {
            main.StageUI.OnEnterStage();
        });
    }


    public void TransitToLobby(bool isWin)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(circleE.DOScale(new Vector3(1.35f, 1.35f), .5f));
        sequence.AppendCallback(() =>
        {
            main.Sound.PlayBGM(main.Sound.LobbyBGM);
            main.StageUI.TurnOff();
            main.Game.ResetTileArray();
            main.Game.ResetObstacleArray();
            main.Pooler.ResetObstacles();
        });
        sequence.AppendInterval(2.5f);
        sequence.Append(circleE.DOScale(Vector3.zero, .6f).SetEase(Ease.InBack));
        sequence.OnComplete(() => main.LobbyUI.OnEnterLobby());
    }


    private void SetStageInfoSprite(StageInfoSO stageInfo)
    {
        stageText.sprite = stageInfo.StageTextSprite;
        stageText.SetNativeSize();

        targetTile.sprite = stageInfo.TargetTileSprite;
        targetTile.SetNativeSize();
    }
}
