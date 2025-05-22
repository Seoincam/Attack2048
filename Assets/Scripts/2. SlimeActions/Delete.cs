// - - - - - - - - - - - - - - - - - -
// Delete.cs
//  - 삭제 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;

public class Delete : SlimeActionBase
{
    
    private int _x, _y; // Square 배열 상의 현재 위치
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Text lifeText;

    [Space, SerializeField, Header("스테이지6 한 줄 삭제 수명")]
    private int LifeForStage6;



    void Update()
    {
        float alpha = Mathf.Min(Mathf.Abs(Mathf.Sin(Time.time)), 0.7f);
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, alpha);
    }


    public void Init(int x, int y, bool isStage6)
    {
        _x = x; _y = y;
        transform.position = GameManager.Instance.LocateTile(_x, _y);

        if (isStage6)
        {
            Life = LifeForStage6;
            _lifeCounter = Life;
        }
        
        lifeText.text = _lifeCounter.ToString();

        GameManager.Instance.ObstacleArray[_x, _y].PlaceDelete();
    }


    protected override void Execute()
    {
        GameManager.Instance.DestroyTile(_x,_y);
        GameManager.Instance.ObstacleArray[_x, _y].RemoveDelete();
        base.Execute();
    }

    public override void OnEnter_CountDownPhase()
    {
        base.OnEnter_CountDownPhase();
        lifeText.text = _lifeCounter.ToString();
    }
}
