// - - - - - - - - - - - - - - - - - -
// PetrifyPrep.cs
//  - 석화 대기 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;

public class PetrifyPrep : SlimeActionBase, IShowLife
{
    // 필드    
    // - - - - - - - - - - 
    [SerializeField] private Text lifeText;

    private int _x, _y; // Square 배열 상의 현재 위치
    
    private SpriteRenderer _renderer;
    private GameManager G;


    // Unity 콜백
    // - - - - - - - - - - 
    void Awake()
    {
        G = GameManager.Instance;
        GetRenderer();
    }

    void Update()
    {
        UpdateLifeText();
    }


    // 초기화
    // - - - - - - - - - - 
    public override void Init(int x, int y)
    {
        base.Init(x, y);
        UpdateLifeText();

        _x = x; _y = y;
        G.ObstacleArray[x, y].PlacePetrifyPrep();
    }


    // 로직  
    // - - - - - - - - - - 
    public override void OnEnter_CountDownPhase()
    {
        base.OnEnter_CountDownPhase();
        UpdateLifeText();
    }

    protected override void Execute()
    {
        G.ObstacleArray[_x, _y].RemovePetrifyPrep();

        // Tile이 null이면 실행
        // Tile이 null 아니고 보호 아니면 실행
        // Tile이 null 아니고 보호면 실행x
        if (G.TileArray[_x, _y] == null || (G.TileArray[_x, _y] != null && !G.TileArray[_x, _y].GetComponent<Tile>().IsProtected))
        {
            G.DeleteTile(_x, _y);
            GameObject obj = ObjectPoolManager.instance.GetObject(22, Group.SlimeAction);
            Petrify petrify = obj.GetComponent<Petrify>();
            petrify.Init(_x, _y);
        }

        base.Execute();
    }


    // Interfaces
    // - - - - - - - - - - 
    public void UpdateLifeText()
    {
        lifeText.text = _lifeCounter.ToString();
    }
    
    public void GetRenderer()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }
    
    public void UpdateWarningEffect()
    {
        float alpha = Mathf.PingPong(Time.time * 0.45f, 0.5f);
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, alpha);
    }
}