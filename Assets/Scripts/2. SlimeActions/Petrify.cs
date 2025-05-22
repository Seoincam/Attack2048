// - - - - - - - - - - - - - - - - - -
// Petrify.cs
//  - 석화 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;

public class Petrify : SlimeActionBase
{
    private int _x, _y; // Square 배열 상의 현재 위치
    [SerializeField] private Text lifeText;

    public void Init(int x, int y)
    {
        GameManager G = GameManager.Instance;

        _x = x; _y = y;
        transform.position = G.LocateTile(x, y);
        lifeText.text = _lifeCounter.ToString();

        Destroy(G.TileArray[x, y]);
        G.TileArray[x, y] = null;

        G.ObstacleArray[x, y].PlacePetrify();
    }

    public override void OnTurnChanged()
    {
        base.OnTurnChanged();
        lifeText.text = _lifeCounter.ToString();        
    }

    protected override void Execute()
    {
        GameManager.Instance.ObstacleArray[_x, _y].RemovePetrify();
        EventManager.Unsubscribe(GameEvent.NewTurn, OnTurnChanged);
        Destroy(gameObject);
    }
}