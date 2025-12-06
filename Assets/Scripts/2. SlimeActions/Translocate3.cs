// - - - - - - - - - - - - - - - - - -
// Translocate3.cs
//  - 이동 클래스 (스테이지 3)
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;

public class Translocate3 : SlimeActionBase, IShowLife, IMakeWarningEffect, IMakeDeleteEffect
{
    // 필드
    // - - - - - - - - - - 
    [SerializeField] private SpriteRenderer t_0_0, t_0_4, t_4_4, t_4_0;
    [SerializeField] private Text[] lifeTexts;
    
    private SpriteRenderer[] tList;

    private ObjectPoolManager _pooler;


    // Unity 콜백
    // - - - - - - - - - - 
    void Awake()
    {
        t_0_0.transform.position = GameManager.Instance.LocateTile(0, 0);
        t_0_4.transform.position = GameManager.Instance.LocateTile(0, 4);
        t_4_4.transform.position = GameManager.Instance.LocateTile(4, 4);
        t_4_0.transform.position = GameManager.Instance.LocateTile(4, 0);

        GetRenderer();

        _pooler = ObjectPoolManager.Instance;
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

        for (int x = 0; x < 5; x += 4)
            for (int y = 0; y < 5; y += 4)
                GameManager.Instance.ObstacleArray[x, y].PlaceTranslocate();        
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
        var G = GameManager.Instance;

        // 실제 이동
        if (G.TileArray[0, 0] != null) G.TileArray[0, 0].transform.position = G.LocateTile(0, 4);
        if (G.TileArray[0, 4] != null) G.TileArray[0, 4].transform.position = G.LocateTile(4, 4);
        if (G.TileArray[4, 4] != null) G.TileArray[4, 4].transform.position = G.LocateTile(4, 0);
        if (G.TileArray[4, 0] != null) G.TileArray[4, 0].transform.position = G.LocateTile(0, 0);

        // 기존 타일 백업
        var t00 = G.TileArray[0, 0];
        var t04 = G.TileArray[0, 4];
        var t44 = G.TileArray[4, 4];
        var t40 = G.TileArray[4, 0];

        // 배열 교환
        G.TileArray[0, 0] = t40;
        G.TileArray[0, 4] = t00;
        G.TileArray[4, 4] = t04;
        G.TileArray[4, 0] = t44;

        // 타일 내부 정보 업데이트
        if (t40 != null) t40.GetComponent<Tile>().Init(0, 0);
        if (t00 != null) t00.GetComponent<Tile>().Init(0, 4);
        if (t04 != null) t04.GetComponent<Tile>().Init(4, 4);
        if (t44 != null) t44.GetComponent<Tile>().Init(4, 0);

        // 장애물 배열 수정
        for (int x = 0; x < 5; x += 4)
            for (int y = 0; y < 5; y += 4)
                G.ObstacleArray[x, y].RemoveTranslocate();

        MakeDeleteEffect();
        base.Execute();
    }


    // Interfaces
    // - - - - - - - - - - 
    public void UpdateLifeText()
    {
        foreach (Text lifeText in lifeTexts)
            lifeText.text = _lifeCounter.ToString();
    }

    public void GetRenderer()
    {
        tList = new SpriteRenderer[]
        {
            t_0_0, t_0_4, t_4_4, t_4_0
        };
    }

    public void UpdateWarningEffect()
    {
        float alpha = Mathf.PingPong(Time.time * 0.45f, 0.9f);
        alpha = Mathf.Clamp(alpha, .3f, .9f);
        foreach (SpriteRenderer renderer in tList)
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
    }

    public void MakeDeleteEffect()
    {
        ParticleSystem particle;

        for (int x = 0; x < 5; x += 4)
            for (int y = 0; y < 5; y += 4)
            {
                particle = _pooler.GetObject(27, Group.Effect).GetComponent<ParticleSystem>();
                particle.transform.position = GameManager.Instance.LocateTile(x, y);
                particle.Play();
            }
    }
}
