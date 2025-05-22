// - - - - - - - - - - - - - - - - - -
// EventManger.cs
//  - GameEvent 열거형으로 이벤트를 구독하고 실행
// - - - - - - - - - - - - - - - - - -

using System.Collections.Generic;
using UnityEngine.Events;

public enum GameEvent
{
    // [게임 이벤트]
    NewTurn, // 새로운 턴
    CountDownPhase, // 숫자 줄어드는 단계
    TriggerPhase, // 0이 됐다면 실행하는 단계

    // [보스 패턴 이벤트]
    Delete, // 삭제
    Delete6, // 삭제 (6스테이지 한 줄 삭제)
    Wall, // 벽
    Petrify, // 석화
    Imprison, // 감금
    Change, // 숫자 랜덤 변경
    Translocate3, // 이동 (3스테이지)
    Translocate7, // 이동 (7스테이지)
    ForcedMove, // 강제 이동(4스테이지)
}

public class EventManager
{
    // Dictionary인 Events에 이벤트와 UnityAction형 리스트를 대응시켜서 저장
    private static readonly IDictionary<GameEvent, List<UnityAction>> Events = new Dictionary<GameEvent, List<UnityAction>>();


    // 이벤트에 리스너(메서드)를 구독
    // 이벤트가 존재하지 않으면 새로 생성 후 구독
    public static void Subscribe(GameEvent gameEvent, UnityAction listener)
    {
        if (Events.TryGetValue(gameEvent, out List<UnityAction> thisEvents))
        {
            thisEvents.Add(listener); // 기존 이벤트에 리스너 등록
        }

        else
        {
            // 새 이벤트 생성 
            thisEvents = new List<UnityAction>
            {
                listener // 리스너 추가
            };
            Events.Add(gameEvent, thisEvents); // 딕셔너리에 등록
        }
    }


    // 이벤트에 리스너(메서드)를 제거
    // 더 이상 특정 리스너를 사용 안 한다면 꼭 제거 필요
    public static void Unsubscribe(GameEvent gameEvent, UnityAction listener)
    {
        if (Events.TryGetValue(gameEvent, out List<UnityAction> thisEvents))
        {
            thisEvents.Remove(listener);
        }
    }


    // 이벤트에 구독된 모든 리스너(메서드)를 실행
    public static void Publish(GameEvent gameEvent)
    {
        if (Events.TryGetValue(gameEvent, out List<UnityAction> thisEvents))
        {
            foreach (UnityAction thisEvent in thisEvents) thisEvent?.Invoke();
        }
    }

    // 이벤트에 구독된 모든 리스터(메서드)를 반환
    public static List<UnityAction> GetEvent(GameEvent gameEvent)
    {
        Events.TryGetValue(gameEvent, out List<UnityAction> thisEvents);
        return thisEvents;
    }
}
