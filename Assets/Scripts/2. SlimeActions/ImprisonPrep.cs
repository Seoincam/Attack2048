// - - - - - - - - - - - - - - - - - -
// ImprisonPrep.cs
//  - 감금 대기 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;

public class ImprisonPrep : SlimeActionBase, IShowLife, IMakeWarningEffect
{
    // 필드    
    // - - - - - - - - - - 
    [SerializeField] private Text lifeText;

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
        GameManager.Instance.ObstacleArray[x, y].PlaceImprisonPrep();  
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
        GameManager.Instance.ObstacleArray[_x, _y].RemoveImprisonPrep();
        GameObject obj = ObjectPoolManager.instance.GetObject(20, Group.SlimeAction);
        Imprison imprison = obj.GetComponent<Imprison>();

        // 위치 설정
        imprison.Init(_x, _y);
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