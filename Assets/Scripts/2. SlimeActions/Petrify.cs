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

        GameManager G = GameManager.Instance;

        _x = x; _y = y;
        G.ObstacleArray[x, y].PlacePetrify();

        lifeText.text = _lifeCounter.ToString();

        G.DestroyTile(x, y);
        G.TileArray[x, y] = null;
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