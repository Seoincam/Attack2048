// - - - - - - - - - - - - - - - - - -
// Wall.cs
//  - 벽 클래스.
// - - - - - - - - - - - - - - - - - -

public class Wall : SlimeActionBase
{
    protected override int Life => 2; // 수명.

    protected override void Execute() {
        // TODO: GameManager에 벽 삭제 알리기
        EventManager.Unsubscribe(GameEvent.NewTurn, OnTurnChanged);
        Destroy(gameObject);
    }
}