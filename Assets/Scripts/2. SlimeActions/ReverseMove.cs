// - - - - - - - - - - - - - - - - - -
// ReverseMove.cs
//  - 상하좌우반전 클래스.
// - - - - - - - - - - - - - - - - - -

using System.Collections;
using UnityEngine;
public class ReverseMove : SlimeActionBase, IShowLife, IMakeWarningEffect
{
    // 필드
    // - - - - - - - - - - 
    [SerializeField] private Sprite life3;
    [SerializeField] private Sprite life2;
    [SerializeField] private Sprite life1;

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
    public override void Init()
    {
        base.Init();
        UpdateLifeText();

        GameManager.Instance.IsReversed = true;
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
        GameManager.Instance.IsReversed = false;
        base.Execute();
    }

    public override IEnumerator DestroySelf()
    {
        GameManager.Instance.IsReversed = false;
        return base.DestroySelf();
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
        float alpha = Mathf.PingPong(Time.time * 0.45f, .95f);
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, alpha);
    }
}