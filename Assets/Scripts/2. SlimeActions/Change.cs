using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Change : SlimeActionBase
{
    private int _x, _y; // Square 배열 상의 현재 위치
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Text lifeText;

    private int[] randomNum = new int[]
    {
        2,4,8,16,32
    };

    void Update()
    {
        float alpha = Mathf.Min(Mathf.Abs(Mathf.Sin(Time.time)), 0.7f);
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, alpha);
    }


    public void Init(int x, int y)
    {
        _x = x; _y = y;
        transform.position = GameManager.Instance.LocateTile(_x, _y);
        GameManager.Instance.ObstacleArray[x, y].PlaceChange();
        lifeText.text = _lifeCounter.ToString();
    }


    protected override void Execute()
    {
        int random = randomNum[Random.Range(0, randomNum.Length)];

        GameManager G = GameManager.Instance;
        if (G.DestroyTile(_x, _y)) G.Spawn(random, _x, _y);
        G.ObstacleArray[_x, _y].RemoveChange();

        base.Execute();
    }

    public override void OnEnter_CountDownPhase()
    {
        base.OnEnter_CountDownPhase();
        lifeText.text = _lifeCounter.ToString();
    }
}
