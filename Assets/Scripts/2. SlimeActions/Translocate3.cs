// - - - - - - - - - - - - - - - - - -
// Translocate3.cs
//  - 이동 클래스 (스테이지 3)
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;

public class Translocate3 : SlimeActionBase
{
    [SerializeField] private SpriteRenderer t_0_0, t_0_4, t_4_4, t_4_0;
    [SerializeField] private Text[] lifeTexts;
    private SpriteRenderer[] tList;

    void Start()
    {
        GameManager G = GameManager.Instance;

        t_0_0.transform.position = G.LocateTile(0, 0);
        t_0_4.transform.position = G.LocateTile(0, 4);
        t_4_4.transform.position = G.LocateTile(4, 4);
        t_4_0.transform.position = G.LocateTile(4, 0);

        G.ObstacleArray[0, 0].PlaceTranslocate();
        G.ObstacleArray[0, 4].PlaceTranslocate();
        G.ObstacleArray[4, 4].PlaceTranslocate();
        G.ObstacleArray[4, 0].PlaceTranslocate();

        tList = new SpriteRenderer[]
        {
            t_0_0, t_0_4, t_4_4, t_4_0
        };

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
        if (G.TileArray[0, 0] != null) G.TileArray[0, 0].transform.position = G.LocateTile(0, 4);
        if (G.TileArray[0, 4] != null) G.TileArray[0, 4].transform.position = G.LocateTile(4, 4);
        if (G.TileArray[4, 4] != null) G.TileArray[4, 4].transform.position = G.LocateTile(4, 0);
        if (G.TileArray[4, 0] != null) G.TileArray[4, 0].transform.position = G.LocateTile(0, 0);

        // 타일 배열 수정
        (G.TileArray[0, 0], G.TileArray[0, 4], G.TileArray[4, 4], G.TileArray[4, 0])
            = (G.TileArray[4, 0], G.TileArray[0, 0], G.TileArray[0, 4], G.TileArray[4, 4]);

        // 장애물 배열 수정
        G.ObstacleArray[0, 0].RemoveTranslocate();
        G.ObstacleArray[0, 4].RemoveTranslocate();
        G.ObstacleArray[4, 4].RemoveTranslocate();
        G.ObstacleArray[4, 0].RemoveTranslocate();

        base.Execute();
    }
}
