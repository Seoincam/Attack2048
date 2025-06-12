using UnityEngine;
using UnityEngine.UI;

public class Change : SlimeActionBase, IShowLife, IMakeWarningEffect, IMakeDeleteEffect
{
    // - - - - - - - - - - 
    // 필드
    [SerializeField] private Text lifeText;

    [System.Serializable] struct ChangeRule
    {
        public int tileValue;
        public int probabilty;
    }
    [SerializeField] private ChangeRule[] ChangeRules;

    private int _x, _y; // Square 배열 상의 현재 위치
    private SpriteRenderer _renderer;


    // - - - - - - - - - - 
    // Unity 콜백
    public void Awake()
    {
        GetRenderer();
    }
    void Update()
    {
        UpdateWarningEffect();
    }


    // - - - - - - - - - - 
    // 초기화
    public override void Init(int x, int y)
    {
        base.Init(x, y);
        UpdateLifeText();

        _x = x; _y = y;
        GameManager.Instance.ObstacleArray[x, y].PlaceChange();
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
        // Change Logic
        GameManager G = GameManager.Instance;
        if (G.DeleteTile(_x, _y))
        {
            int random = Random.Range(1, 101);
            int tileValue = 2;
            int probabilty = 0;

            foreach (ChangeRule rule in ChangeRules)
            {
                probabilty += rule.probabilty;
                if (random <= probabilty)
                {
                    tileValue = rule.tileValue;
                    break;
                }
            }

            G.Spawn(tileValue, _x, _y);
        }

        G.ObstacleArray[_x, _y].RemoveChange();

        MakeDeleteEffect();
        base.Execute();
    }


    // - - - - - - - - - - 
    // Interfaces
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
        ParticleSystem particle = ObjectPoolManager.instance.GetObject(27, Group.Effect).GetComponent<ParticleSystem>();
        particle.transform.position = transform.position;
        particle.Play();
    }
}
