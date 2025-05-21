// - - - - - - - - - - - - - - - - - -
// Translocate.cs
//  - 이동 클래스.
// - - - - - - - - - - - - - - - - - -

public class Translocate7 : SlimeActionBase
{
    private int _x1, _y1;   // 본인 위치
    private int _x2, _y2;   // 이동 시킬 위치

    public void Init(int x1, int y1, int x2, int y2)
    {
        _x1 = x1;
        _y1 = y1;
        _x2 = x2;
        _y2 = y2;

        transform.position = GameManager.Instance.LocateTile(_x1, _y1);
    }
    
    protected override void Execute()
    {
        // TODO: GameManager에 이동 실행 알리기 & 타일 이동 시키기
        // GameManager.Instance.
        EventManager.Unsubscribe(GameEvent.NewTurn, OnTurnChanged);
        Destroy(gameObject);
    }
}
