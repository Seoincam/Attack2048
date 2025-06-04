// - - - - - - - - - - - - - - - - - -
// Translocate7Horizontal.cs
//  - 이동 (수평) 클래스.
// - - - - - - - - - - - - - - - - - -
using UnityEngine;
using UnityEngine.UI;

public class Translocate7Horizontal : SlimeActionBase
{
    int _a, _b;

    [SerializeField] private SpriteRenderer t_4_a, t_3_a, t_2_a, t_1_a, t_0_a,
        t_0_b, t_1_b, t_2_b, t_3_b, t_4_b;

    [SerializeField] private Text[] lifeTexts;
    private SpriteRenderer[] tList;

    private ObjectPoolManager _pooler;

    void Start()
    {
        _pooler = ObjectPoolManager.instance;

        tList = new SpriteRenderer[]
        {
            t_4_a, t_3_a, t_2_a, t_1_a, t_0_a,
            t_0_b, t_1_b, t_2_b, t_3_b, t_4_b
        };
    }

    public override void Init(int a, int b)
    {
        base.Init(-1, 0);
        _a = a;
        _b = b;

        GameManager G = GameManager.Instance;

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
        // 마지막 스테이지여서 생략
        // for (int i = 0; i < 5; i++)
        // {
        //     G.ObstacleArray[1, i].PlaceTranslocate();
        //     G.ObstacleArray[4, i].PlaceTranslocate();
        // }

        foreach (Text lifeText in lifeTexts)
        {
            lifeText.text = _lifeCounter.ToString();
        }
    }

    void FixedUpdate()
    {
        float alpha = Mathf.Min(Mathf.Abs(Mathf.Sin(Time.time * 1.2f)), 0.45f);
        foreach (SpriteRenderer renderer in tList)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);    
        }
    }

    public override void OnEnter_CountDownPhase()
    {
        base.OnEnter_CountDownPhase();
        foreach (Text lifeText in lifeTexts)
        {
            lifeText.text = _lifeCounter.ToString();
        }
    }

    protected override void Execute()
    {
        GameManager G = GameManager.Instance;

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
        {
            (G.TileArray[i, _a], G.TileArray[i, _b]) = (G.TileArray[i, _b], G.TileArray[i, _a]);
        }

        // 장애물 배열 수정
        // 마지막 스테이지여서 생략
        // G.ObstacleArray[0, 0].RemoveTranslocate();
        // G.ObstacleArray[0, 4].RemoveTranslocate();
        // G.ObstacleArray[4, 4].RemoveTranslocate();
        // G.ObstacleArray[4, 0].RemoveTranslocate();

        for (int i = 0; i < 5; i++)
        {
            ParticleSystem particle = _pooler.GetObject(27, Group.Effect).GetComponent<ParticleSystem>();
            particle.transform.position = G.LocateTile(i, _a);
            particle.Play();

            particle = _pooler.GetObject(27, Group.Effect).GetComponent<ParticleSystem>();
            particle.transform.position = G.LocateTile(i, _b);
            particle.Play();
        }

        StartCoroutine(DestroySelf());
    }
}
