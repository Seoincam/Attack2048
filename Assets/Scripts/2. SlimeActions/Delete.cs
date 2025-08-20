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
    private Main _main;


    // Unity 콜백
    // - - - - - - - - - - 
    void Awake()
    {
        GetRenderer();
        if(_main == null)
            _main = Object.FindFirstObjectByType<Main>();
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
        var currentSlime = _main?.Stage?.CurrentSlime;
        if(currentSlime == null)
        {
            Debug.Log("현재 슬라임을 찾을 수 없음");
        }
        if(currentSlime.CompareTag("Shield Slime"))
        {
            SoundManager.Instance.PlayShieldDeleteSFX();
            Debug.Log("방패 슬라임 삭제 사운드 재생");
        }
        else if(currentSlime.CompareTag("Archer Slime"))
        {
            SoundManager.Instance.PlayArcherBreakTileSFX();
            Debug.Log("아처 슬라임 삭제 사운드 재생");
        }

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
