// - - - - - - - - - - - - - - - - - -
// PetrifyPrep.cs
//  - 석화 대기 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;

public class PetrifyPrep : SlimeActionBase, IShowLife, IMakeWarningEffect
{
    // 필드    
    // - - - - - - - - - - 
    [SerializeField] private Sprite life3;
    [SerializeField] private Sprite life2;
    [SerializeField] private Sprite life1;


    private int _x, _y; // Square 배열 상의 현재 위치
    
    private SpriteRenderer _renderer;


    // Unity 콜백
    // - - - - - - - - - - 
    void Awake()
    {
        GetRenderer();
    }

    void Update()
    {
        UpdateWarningEffect();
    }


    // 초기화
    // - - - - - - - - - - 
    public override void Init(int x, int y)
    {
        base.Init(x, y);
        UpdateLifeText();

        _x = x; _y = y;
        GameManager.Instance.ObstacleArray[x, y].PlacePetrifyPrep();
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
        GameManager.Instance.ObstacleArray[_x, _y].RemovePetrifyPrep();

        // Tile이 null이면 실행
        // Tile이 null 아니고 보호 아니면 실행
        // Tile이 null 아니고 보호면 실행x
        if (GameManager.Instance.TileArray[_x, _y] == null
            || (GameManager.Instance.TileArray[_x, _y] != null && !GameManager.Instance.TileArray[_x, _y].GetComponent<Tile>().IsProtected))
        {
            GameManager.Instance.DeleteTile(_x, _y);
            GameObject obj = ObjectPoolManager.Instance.GetObject(22, Group.SlimeAction);
            Petrify petrify = obj.GetComponent<Petrify>();
            petrify.Init(_x, _y);
        }

        base.Execute();
    }


    // Interfaces
    // - - - - - - - - - - 
    public void UpdateLifeText()
    {
        if (_lifeCounter == 0)
            return;

        switch (_lifeCounter)
        {
            case 3: _renderer.sprite = life3; break;
            case 2: _renderer.sprite = life2; break;
            case 1: _renderer.sprite = life1; break;
        }
    }
    
    public void GetRenderer()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }
    
    public void UpdateWarningEffect()
    {
        float alpha = Mathf.PingPong(Time.time * 0.6f, 1f);
        alpha = Mathf.Clamp(alpha, .7f, 1f);
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, alpha);
    }
}