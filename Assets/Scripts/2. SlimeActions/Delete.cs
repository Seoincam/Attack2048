// - - - - - - - - - - - - - - - - - -
// Delete.cs
//  - 삭제 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;

public class Delete : SlimeActionBase, IShowLife, IMakeWarningEffect, IMakeDeleteEffect
{
    // 필드    
    // - - - - - - - - - - 
    public enum Type { Shield, Archer, LineDelete, LineDeleteVisual }
    private Type type = Type.Archer;

    [SerializeField] private Sprite shieldLife3;
    [SerializeField] private Sprite shieldLife2;
    [SerializeField] private Sprite shieldLife1;

    [Space, SerializeField] private Sprite archerLife2;
    [SerializeField] private Sprite archerLife1;

    [Space, SerializeField] private Sprite lineLife3;
    [SerializeField] private Sprite lineLife2;
    [SerializeField] private Sprite lineLife1;

    [Space, SerializeField, Header("기본 한 칸 삭제 수명")]
    private int DefaultLife;
    [Space, SerializeField, Header("(아처 슬라임) 한 줄 삭제 수명")]
    private int LineDeleteLife;

    private int _x, _y; // Square 배열 상의 현재 위치

    private SpriteRenderer _renderer;
    private Main _main;


    // Unity 콜백
    // - - - - - - - - - - 
    void Awake()
    {
        GetRenderer();
        if(_main == null)
            _main = FindFirstObjectByType<Main>();
    }
    void Update()
    {
        UpdateWarningEffect();
    }


    // 초기화
    // - - - - - - - - - - 
    public void Init(int x, int y, Type type)
    {
        this.type = type;
        if (type == Type.LineDelete)
            _renderer.sprite = null;

        transform.localScale = type == Type.LineDeleteVisual ? new Vector3(.5f, .5f, 1f) : new Vector3(.1f, .1f, 1f);
        Life = (type == Type.LineDelete || type == Type.LineDeleteVisual) ? LineDeleteLife : DefaultLife;


        base.Init(x, y);
        UpdateLifeText();

        _x = x; _y = y;
        GameManager.Instance.ObstacleArray[_x, _y].PlaceDelete();
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
        if (GameManager.Instance.TileArray[_x, _y] != null
            && !GameManager.Instance.TileArray[_x, _y].GetComponent<Tile>().IsProtected)
        {
            GameManager.Instance.DeleteTile(_x, _y);
        }
            
        GameManager.Instance.ObstacleArray[_x, _y].RemoveDelete();

        MakeDeleteEffect();
        PlayDeleteSFX();
        base.Execute();
    }
    
    private void PlayDeleteSFX()
    {
        if (type == Type.Shield)
        {
            SoundManager.Instance.PlayShieldDeleteSFX();
            Debug.Log("방패 슬라임 삭제 사운드 재생");
        }
        else if (type == Type.Archer || type == Type.LineDelete || type == Type.LineDeleteVisual)
        {
            SoundManager.Instance.PlayArcherBreakTileSFX();
            Debug.Log("아처 슬라임 삭제 사운드 재생");
        }
        else
        {
            Debug.Log("현재 슬라임을 찾을 수 없음");
        }
    }

    // Interfaces
    // - - - - - - - - - - 
    public void UpdateLifeText()
    {
        if (_lifeCounter == 0)
            return;

        if (type == Type.Shield)
            switch (_lifeCounter)
            {
                case 3: _renderer.sprite = shieldLife3; break;
                case 2: _renderer.sprite = shieldLife2; break;
                case 1: _renderer.sprite = shieldLife1; break;
            }

        else if (type == Type.Archer)
            switch (_lifeCounter)
            {
                case 2: _renderer.sprite = archerLife2; break;
                case 1: _renderer.sprite = archerLife1; break;
            }

        else if (type == Type.LineDeleteVisual)
            switch (_lifeCounter)
            {
                case 3: _renderer.sprite = lineLife3; break;
                case 2: _renderer.sprite = lineLife2; break;
                case 1: _renderer.sprite = lineLife1; break;
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

    public void MakeDeleteEffect()
    {
        ParticleSystem particle = ObjectPoolManager.Instance.GetObject(27, Group.Effect).GetComponent<ParticleSystem>();
        particle.transform.position = transform.position;
        particle.Play();
    }
}
