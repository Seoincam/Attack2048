// - - - - - - - - - - - - - - - - - -
// Petrify.cs
//  - 석화 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;

public class Petrify : SlimeActionBase
{
    protected int _x, _y; // Square 배열 상의 현재 위치
    [SerializeField] private Text lifeText;

    public override void Init(int x, int y, ObjectPoolManager pooler)
    {
        base.Init(x, y, pooler);

        _x = x; _y = y;
        GameManager.Instance.ObstacleArray[x, y].PlacePetrify();

        lifeText.text = _lifeCounter.ToString();
    }

    public override void OnEnter_CountDownPhase()
    {
        base.OnEnter_CountDownPhase();
        lifeText.text = _lifeCounter.ToString();        
    }

    protected override void Execute()
    {
        GameManager.Instance.ObstacleArray[_x, _y].RemovePetrify();
        base.Execute();
    }
}