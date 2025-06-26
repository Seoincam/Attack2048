// - - - - - - - - - - - - - - - - - -
// Translocate7Vertical.cs
//  - 이동 (수직) 클래스.
// - - - - - - - - - - - - - - - - - -
using UnityEngine;
using UnityEngine.UI;

public class Translocate7Vertical : SlimeActionBase, IShowLife, IMakeWarningEffect, IMakeDeleteEffect
{
    // 필드
    // - - - - - - - - - - 
    [SerializeField] private Text[] lifeTexts;
    [SerializeField] private SpriteRenderer
        t_a_4, t_a_3, t_a_2, t_a_1, t_a_0,
        t_b_0, t_b_1, t_b_2, t_b_3, t_b_4;

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

        t_a_4.transform.position = G.LocateTile(_a, 4);
        t_a_3.transform.position = G.LocateTile(_a, 3);
        t_a_2.transform.position = G.LocateTile(_a, 2);
        t_a_1.transform.position = G.LocateTile(_a, 1);
        t_a_0.transform.position = G.LocateTile(_a, 0);

        t_b_0.transform.position = G.LocateTile(_b, 0);
        t_b_1.transform.position = G.LocateTile(_b, 1);
        t_b_2.transform.position = G.LocateTile(_b, 2);
        t_b_3.transform.position = G.LocateTile(_b, 3);
        t_b_4.transform.position = G.LocateTile(_b, 4);

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
            if (G.TileArray[_a, i] != null)
                G.TileArray[_a, i].transform.position = G.LocateTile(_b, i);

            if (G.TileArray[_b, i] != null)
                G.TileArray[_b, i].transform.position = G.LocateTile(_a, i);
        }

        // 타일 배열 수정
        for (int i = 0; i < 5; i++)
            (G.TileArray[_a, i], G.TileArray[_b, i]) = (G.TileArray[_b, i], G.TileArray[_a, i]);

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
            t_a_4, t_a_3, t_a_2, t_a_1, t_a_0,
            t_b_0, t_b_1, t_b_2, t_b_3, t_b_4
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
            particle.transform.position = GameManager.Instance.LocateTile(_a, i);
            particle.Play();

            particle = _pooler.GetObject(27, Group.Effect).GetComponent<ParticleSystem>();
            particle.transform.position = GameManager.Instance.LocateTile(_b, i);
            particle.Play();
        }
    }
}
