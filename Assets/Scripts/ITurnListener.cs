// - - - - - - - - - - - - - - - - - -
// ITurnListener.cs
//  - 턴이 바뀔 때마다 영향을 받는 클래스들에 추가합니다.
// - - - - - - - - - - - - - - - - - -

public interface ITurnListener
{
    void Subscribe_NewTurn(); // EventManager의 NewTurn 이벤트에 OnTurnChanged를 구독합니다.
    void OnTurnChanged(); // 턴이 바뀔 때마다 호출됩니다.
}
