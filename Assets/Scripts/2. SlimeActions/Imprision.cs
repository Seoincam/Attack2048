// - - - - - - - - - - - - - - - - - -
// Imprison.cs
//  - 감금 클래스.
// - - - - - - - - - - - - - - - - - -

public class Imprision : SlimeActionBase
{
    protected override int Life => 3; // 수명.

    protected override void Execute()
    {
        // TODO: GameManager에 감금 삭제 알리기
        // TODO: 다시 숫자 타일 복구
        EventManager.Unsubscribe(GameEvent.NewTurn, OnTurnChanged);
        Destroy(gameObject);
    }
}
