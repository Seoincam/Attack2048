// - - - - - - - - - - - - - - - - - -
// Delete.cs
//  - 삭제 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;

public class Delete : SlimeActionBase, IShowLife, IMakeWarningEffect, IMakeDeleteEffect
{
    // 필드    
    // - - - - - - - - - - 
    [SerializeField] private Text lifeText;

    [Space, SerializeField, Header("기본 한 칸 삭제 수명")]
    private int DefaultLife;
    [Space, SerializeField, Header("(아처 슬라임) 한 줄 삭제 수명")]
    private int LineDeleteLife;

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
        UpdateWarningEffect();
    }


    // 초기화
    // - - - - - - - - - - 
    public void Init(int x, int y, bool isLineDelete)
    {
        Life = isLineDelete ? LineDeleteLife : DefaultLife;

        base.Init(x, y);
        UpdateLifeText();

        _x = x; _y = y;
        G.ObstacleArray[_x, _y].PlaceDelete();
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
        if (G.TileArray[_x, _y] != null && !G.TileArray[_x, _y].GetComponent<Tile>().IsProtected)
            G.DeleteTile(_x, _y);
        G.ObstacleArray[_x, _y].RemoveDelete();

        MakeDeleteEffect();
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

    public void MakeDeleteEffect()
    {
        ParticleSystem particle = ObjectPoolManager.Instance.GetObject(27, Group.Effect).GetComponent<ParticleSystem>();
        particle.transform.position = transform.position;
        particle.Play();
    }
}
