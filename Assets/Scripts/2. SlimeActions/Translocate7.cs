// - - - - - - - - - - - - - - - - - -
// Translocate.cs
//  - 이동 클래스.
// - - - - - - - - - - - - - - - - - -
using UnityEngine;
using UnityEngine.UI;

public class Translocate7 : SlimeActionBase
{
    [SerializeField] private SpriteRenderer t_1_4, t_1_3, t_1_2, t_1_1, t_1_0,
        t_3_0, t_3_1, t_3_2, t_3_3, t_3_4;

    [SerializeField] private Text[] lifeTexts;
    private SpriteRenderer[] tList;

    void Start()
    {
        GameManager G = GameManager.Instance;

        t_1_4.transform.position = G.LocateTile(1, 4);
        t_1_3.transform.position = G.LocateTile(1, 3);
        t_1_2.transform.position = G.LocateTile(1, 2);
        t_1_1.transform.position = G.LocateTile(1, 1);
        t_1_0.transform.position = G.LocateTile(1, 0);

        t_3_0.transform.position = G.LocateTile(3, 0);
        t_3_1.transform.position = G.LocateTile(3, 1);
        t_3_2.transform.position = G.LocateTile(3, 2);
        t_3_3.transform.position = G.LocateTile(3, 3);
        t_3_4.transform.position = G.LocateTile(3, 4);

        tList = new SpriteRenderer[]
        {
            t_1_4, t_1_3, t_1_2, t_1_1, t_1_0,
            t_3_0, t_3_1, t_3_2, t_3_3, t_3_4
        };
    }

    public void Init(ObjectPoolManager pooler)
    {
        base.Init(-1, 0, pooler);
        GameManager G = GameManager.Instance;

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
        float alpha = Mathf.Min(Mathf.Abs(Mathf.Sin(Time.time)), 0.45f);
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
        if (G.TileArray[1, 0] != null) G.TileArray[1, 0].transform.position = G.LocateTile(3, 0);
        if (G.TileArray[1, 1] != null) G.TileArray[1, 1].transform.position = G.LocateTile(3, 1);
        if (G.TileArray[1, 2] != null) G.TileArray[1, 2].transform.position = G.LocateTile(3, 2);
        if (G.TileArray[1, 3] != null) G.TileArray[1, 3].transform.position = G.LocateTile(3, 3);
        if (G.TileArray[1, 4] != null) G.TileArray[1, 4].transform.position = G.LocateTile(3, 4);

        if (G.TileArray[3, 0] != null) G.TileArray[3, 0].transform.position = G.LocateTile(1, 0);
        if (G.TileArray[3, 1] != null) G.TileArray[3, 1].transform.position = G.LocateTile(1, 1);
        if (G.TileArray[3, 2] != null) G.TileArray[3, 2].transform.position = G.LocateTile(1, 2);
        if (G.TileArray[3, 3] != null) G.TileArray[3, 3].transform.position = G.LocateTile(1, 3);
        if (G.TileArray[3, 4] != null) G.TileArray[3, 4].transform.position = G.LocateTile(1, 4);

        // 타일 배열 수정
        (G.TileArray[1, 0], G.TileArray[1, 1], G.TileArray[1, 2], G.TileArray[1, 3], G.TileArray[1, 4],
        G.TileArray[3, 4], G.TileArray[3, 3], G.TileArray[3, 2], G.TileArray[3, 1], G.TileArray[3, 0]) 
            = 
        (G.TileArray[3, 0], G.TileArray[3, 1], G.TileArray[3, 2], G.TileArray[3, 3], G.TileArray[3, 4],
        G.TileArray[1, 4], G.TileArray[1, 3], G.TileArray[1, 2], G.TileArray[1, 1], G.TileArray[1, 0]);

        // 장애물 배열 수정
        // 마지막 스테이지여서 생략
        // G.ObstacleArray[0, 0].RemoveTranslocate();
        // G.ObstacleArray[0, 4].RemoveTranslocate();
        // G.ObstacleArray[4, 4].RemoveTranslocate();
        // G.ObstacleArray[4, 0].RemoveTranslocate();

        if (_pooler != null)
        {
            for (int i = 0; i < 5; i++)
            {
                ParticleSystem particle = _pooler.GetObject(27, _particleGroup).GetComponent<ParticleSystem>();
                particle.transform.position = G.LocateTile(1, i);
                particle.Play();

                particle = _pooler.GetObject(27, _particleGroup).GetComponent<ParticleSystem>();
                particle.transform.position = G.LocateTile(3, i);
                particle.Play();
            }
        }

        StartCoroutine(DestroySelf());
    }
}
