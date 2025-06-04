// - - - - - - - - - - - - - - - - - -
// Imprison.cs
//  - 감금 클래스.
// - - - - - - - - - - - - - - - - - -
using UnityEngine;
using UnityEngine.UI;
public class Imprison : SlimeActionBase
{
    protected int _x, _y;

    [SerializeField] private Text lifeText;

    public override void Init(int x, int y)
    {
        base.Init(x, y);

        _x = x; _y = y;
        GameManager.Instance.ObstacleArray[x, y].PlaceImprison();

        lifeText.text = _lifeCounter.ToString();
    }

    public override void OnEnter_CountDownPhase()
    {
        base.OnEnter_CountDownPhase();
        lifeText.text = _lifeCounter.ToString();
    }


    protected override void Execute()
    {
        // TODO: GameManager에 감금 삭제 알리기
        GameManager.Instance.ObstacleArray[_x, _y].RemoveImprison();
        // TODO: 다시 숫자 타일 복구
        base.Execute();
    }
}
