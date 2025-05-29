// - - - - - - - - - - - - - - - - - -
// PetrifyPrep.cs
//  - 석화 대기 클래스.
// - - - - - - - - - - - - - - - - - -

using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PetrifyPrep : SlimeActionBase
{
    protected int _x, _y; // Square 배열 상의 현재 위치
    [SerializeField] private Text lifeText;
    
    protected Transform _slimeActionGroup;

    public void Init(int x, int y, ObjectPoolManager pooler, Transform slimeActionGroup)
    {
        base.Init(x, y, pooler);

        _x = x; _y = y;

        _pooler = pooler;
        _slimeActionGroup = slimeActionGroup;

        lifeText.text = _lifeCounter.ToString();
        GameManager.Instance.ObstacleArray[x, y].PlacePetrifyPrep();
    }
    
    void Update()
    {
        float alpha = Mathf.Min(Mathf.Abs(Mathf.Sin(Time.time)), 0.7f);
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, alpha);
    }

    public override void OnEnter_CountDownPhase()
    {
        base.OnEnter_CountDownPhase();
        lifeText.text = _lifeCounter.ToString();
    }

    protected override void Execute()
    {
        GameManager G = GameManager.Instance;

        G.ObstacleArray[_x, _y].RemovePetrifyPrep();
        
        // null이면 실행
        // null 아니고 보호 아니면 실행
        // null 아니고 보호면 실행x
        if (G.TileArray[_x, _y] == null || (G.TileArray[_x, _y] != null && !G.TileArray[_x, _y].GetComponent<Tile>().IsProtected))
        {
            G.DeleteTile(_x, _y);
            GameObject obj = _pooler.GetObject(22, _slimeActionGroup);
            Petrify petrify = obj.GetComponent<Petrify>();
            petrify._particleGroup = _particleGroup;
            petrify.Init(_x, _y, _pooler);
        }

        _pooler = null;
        base.Execute();
    }
}