// - - - - - - - - - - - - - - - - - -
// Translocate.cs
//  - 이동 클래스.
// - - - - - - - - - - - - - - - - - -

public class Translocate : SlimeActionBase
{
    protected override void Execute() {
        // TODO: GameManager에 이동 실행 알리기 & 타일 이동 시키기
        EventManager.Unsubscribe(GameEvent.NewTurn, OnTurnChanged);
        Destroy(gameObject);
    }
}
