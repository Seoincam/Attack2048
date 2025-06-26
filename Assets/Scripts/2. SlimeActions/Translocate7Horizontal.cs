// - - - - - - - - - - - - - - - - - -
// Translocate7Horizontal.cs
//  - 이동 (수평) 클래스.
// - - - - - - - - - - - - - - - - - -
using UnityEngine;
using UnityEngine.UI;

public class Translocate7Horizontal : SlimeActionBase, IShowLife, IMakeWarningEffect, IMakeDeleteEffect
{
    // 필드
    // - - - - - - - - - - 
    [SerializeField] private Text[] lifeTexts;
    [SerializeField] private SpriteRenderer
        t_4_a, t_3_a, t_2_a, t_1_a, t_0_a,
        t_0_b, t_1_b, t_2_b, t_3_b, t_4_b;
    
    int _a, _b;

    private SpriteRenderer[] tList;
    private ObjectPoolManager _pooler;


    // Unity 콜백
    // - - - - - - - - - - 
    void Awake()
    {
        _pooler = ObjectPoolManager.Instance;

        GetRenderer();
    }

    void Update()
    {
        UpdateWarningEffect();
    }


    // 초기화
    // - - - - - - - - - - 
    public override void Init(int a, int b)
    {
        var G = GameManager.Instance;

        base.Init();
        UpdateLifeText();

        _a = a;
        _b = b;

        t_4_a.transform.position = G.LocateTile(4, _a);
        t_3_a.transform.position = G.LocateTile(3, _a);
        t_2_a.transform.position = G.LocateTile(2, _a);
        t_1_a.transform.position = G.LocateTile(1, _a);
        t_0_a.transform.position = G.LocateTile(0, _a);

        t_0_b.transform.position = G.LocateTile(0, _b);
        t_1_b.transform.position = G.LocateTile(1, _b);
        t_2_b.transform.position = G.LocateTile(2, _b);
        t_3_b.transform.position = G.LocateTile(3, _b);
        t_4_b.transform.position = G.LocateTile(4, _b);

        // 장애물 배열 수정
        // *마지막 스테이지여서 생략
        // for (int i = 0; i < 5; i++)
        // {
        //     G.ObstacleArray[1, i].PlaceTranslocate();
        //     G.ObstacleArray[4, i].PlaceTranslocate();
        // }  
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
        for (int i = 0; i < 5; i++)
        {
            if (G.TileArray[i, _a] != null)
                G.TileArray[i, _a].transform.position = G.LocateTile(i, _b);

            if (G.TileArray[i, _b] != null)
                G.TileArray[i, _b].transform.position = G.LocateTile(i, _a);
        }

        // 타일 배열 수정
        for (int i = 0; i < 5; i++)
            (G.TileArray[i, _a], G.TileArray[i, _b]) = (G.TileArray[i, _b], G.TileArray[i, _a]);

        // 장애물 배열 수정
        // *마지막 스테이지여서 생략
        // G.ObstacleArray[0, 0].RemoveTranslocate();
        // G.ObstacleArray[0, 4].RemoveTranslocate();
        // G.ObstacleArray[4, 4].RemoveTranslocate();
        // G.ObstacleArray[4, 0].RemoveTranslocate();

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
            t_4_a, t_3_a, t_2_a, t_1_a, t_0_a,
            t_0_b, t_1_b, t_2_b, t_3_b, t_4_b
        };
    }

    public void UpdateWarningEffect()
    {
        float alpha = Mathf.PingPong(Time.time * 0.45f, 0.5f);
        foreach (SpriteRenderer renderer in tList)
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
    }
    
    public void MakeDeleteEffect()
    {
        ParticleSystem particle;

        for (int i = 0; i < 5; i++)
        {
            particle = _pooler.GetObject(27, Group.Effect).GetComponent<ParticleSystem>();
            particle.transform.position = GameManager.Instance.LocateTile(i, _a);
            particle.Play();

            particle = _pooler.GetObject(27, Group.Effect).GetComponent<ParticleSystem>();
            particle.transform.position = GameManager.Instance.LocateTile(i, _b);
            particle.Play();
        }
    }
}
