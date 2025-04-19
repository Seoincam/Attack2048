// - - - - - - - - - - - - - - - - - -
// Petrify.cs
//  - 석화 클래스.
// - - - - - - - - - - - - - - - - - -

public class Petrify : SlimeActionBase
{
    protected override int Life => 3; // 수명.

    protected override void Execute() {
        // TODO: GameManager에 석화 삭제 알리기
        EventManager.Unsubscribe(GameEvent.NewTurn, OnTurnChanged);
        Destroy(gameObject);
    }
}