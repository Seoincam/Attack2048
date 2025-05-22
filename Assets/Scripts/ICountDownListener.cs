// - - - - - - - - - - - - - - - - - -
// ICountDownListener.cs
//  - 플레이어 액션 후 숫자가 줄어드는 클래스가 구현해야함
// - - - - - - - - - - - - - - - - - -

public interface ICountDownListener
{
    void Subscribe_CountDown(); // EventManager의 NewTurn 이벤트에 OnTurnChanged를 구독합니다.
    void OnEnter_CountDownPhase(); // 턴이 바뀔 때마다 호출됩니다.
}
