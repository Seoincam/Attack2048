// - - - - - - - - - - - - - - - - - -
// INewTurnListener.cs
//  - 새 턴에 진입할 때마다 영향을 받는 클래스들이 구현해야함
// - - - - - - - - - - - - - - - - - -

public interface INewTurnListener
{
    void Subscribe_NewTurn(); // EventManager의 NewTurn 이벤트에 OnTurnChanged를 구독합니다.
    void OnEnter_NewTurn(); // 턴이 바뀔 때마다 호출됩니다.
}
