// - - - - - - - - - - - - - - - - - -
// EventManger.cs
//  - GameEvent 열거형으로 이벤트를 구독하고 실행
// - - - - - - - - - - - - - - - - - -

using System.Collections.Generic;
using UnityEngine.Events;

public enum GamePhase
{
    // [게임 이벤트]
    NewTurnPhase, // 새로운 턴
    CountDownPhase, // 숫자 줄어드는 단계
    ExecutePhase, // 0이 됐다면 실행하는 단계
}

public class EventManager
{
    // Dictionary인 Events에 이벤트와 UnityAction형 리스트를 대응시켜서 저장
    private static IDictionary<GamePhase, List<UnityAction>> Events;
    public static void InitEvents() => Events = new Dictionary<GamePhase, List<UnityAction>>();

    // 이벤트에 리스너(메서드)를 구독
    public static void Subscribe(GamePhase gameEvent, UnityAction listener)
    {
        if (Events.TryGetValue(gameEvent, out List<UnityAction> thisEvents))
        {
            thisEvents.Add(listener); // 기존 이벤트에 리스너 등록
        }

        else
        {
            // 새 이벤트 생성 
            thisEvents = new List<UnityAction> { listener };
            Events.Add(gameEvent, thisEvents);
        }
    }

    // 이벤트에 리스너(메서드)를 제거
    // 더 이상 특정 리스너를 사용 안 한다면 꼭 제거 필요
    public static void Unsubscribe(GamePhase gameEvent, UnityAction listener)
    {
        if (Events.TryGetValue(gameEvent, out List<UnityAction> thisEvents))
        {
            thisEvents.Remove(listener);
        }
    }

    // 이벤트에 구독된 모든 리스너(메서드)를 실행
    public static void Publish(GamePhase gameEvent)
    {
        if (Events.TryGetValue(gameEvent, out List<UnityAction> thisEvents))
        {
            foreach (UnityAction thisEvent in thisEvents) thisEvent?.Invoke();
        }
    }

    // 이벤트에 구독된 모든 리스터(메서드)를 반환
    public static List<UnityAction> GetEvent(GamePhase gameEvent)
    {
        Events.TryGetValue(gameEvent, out List<UnityAction> thisEvents);
        return thisEvents;
    }
}
