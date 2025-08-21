// - - - - - - - - - - - - - - - - - -
// Blind.cs
//  - 블라인드 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;

public class Blind : SlimeActionBase, IShowLife, IMakeDeleteEffect
{
    // 필드
    // - - - - - - - - - - 
    [SerializeField] private Sprite life2;
    [SerializeField] private Sprite life1;

    private int _x, _y;

    private SpriteRenderer _renderer;

    // 초기화
    // - - - - - - - - - - 
    void Awake()
    {
        GetRenderer();
    }
    
    public override void Init(int x, int y)
    {
        base.Init(x, y);
        UpdateLifeText();

        _x = x;
        _y = y;
    }


    // - - - - - - - - - - 
    // 로직
    public override void OnEnter_CountDownPhase()
    {
        base.OnEnter_CountDownPhase();
        UpdateLifeText();
    }

    protected override void Execute()
    {
        GameManager.Instance.ObstacleArray[_x, _y].RemoveBlind();
        MakeDeleteEffect();
        base.Execute();
    }


    // - - - - - - - - - - 
    // Interfaces
    public void UpdateLifeText()
    {
        if (_lifeCounter == 0)
            return;

        switch (_lifeCounter)
        {
            case 2: _renderer.sprite = life2; break;
            case 1: _renderer.sprite = life1; break;
        }
    }

    public void MakeDeleteEffect()
    {
        ParticleSystem particle = ObjectPoolManager.Instance.GetObject(27, Group.Effect).GetComponent<ParticleSystem>();
        particle.transform.position = transform.position;
        particle.Play();
    }

    public void GetRenderer()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }
}
