// - - - - - - - - - - - - - - - - - -
// EventManger.cs
//  - GameEvent 열거형으로 이벤트를 구독하고 실행하는 클래스.
//  - Subscribe(), Unsubscribe(), Publish()로 작동.
// - - - - - - - - - - - - - - - - - -

using System.Collections.Generic;
using UnityEngine.Events;

public enum GameEvent 
{
    // [게임 이벤트]
    NewTurn, // 새로운 턴

    // [보스 패턴 이벤트]
    Delete, // 삭제
    Wall, // 벽
    Petrify, // 석화
    Imprison, // 감금
    Translocate, // 이동
} 

public class EventManager
{
    // Dictionary인 Events에 이벤트와 UnityEvent(메서드)를 대응시켜서 저장합니다.
    private static readonly IDictionary<GameEvent, UnityEvent> Events = new Dictionary<GameEvent, UnityEvent>();


    // 이벤트에 리스너(메서드)를 구독합니다.
    // 이벤트가 존재하지 않으면 새로 생성 후 구독합니다.
    public static void Subscribe(GameEvent gameEvent, UnityAction listener) {
        if(Events.TryGetValue(gameEvent, out UnityEvent thisEvent)) {
            thisEvent.AddListener(listener); // 기존 이벤트에 리스너 등록
        }

        else {
            thisEvent = new UnityEvent(); // 새 이벤트 생성
            thisEvent.AddListener(listener); // 리스너 추가
            Events.Add(gameEvent, thisEvent); // 딕셔너리에 등록
        }
    }


    // 이벤트에 리스너(메서드)를 제거합니다.
    // 더 이상 특정 리스너를 사용 안 한다면 꼭 제거해주세요.
    public static void Unsubscribe(GameEvent gameEvent, UnityAction listener) {
        if(Events.TryGetValue(gameEvent, out UnityEvent thisEvent)) {
            thisEvent.RemoveListener(listener);
        }
    }


    // 이벤트에 구독된 모든 리스너(메서드)를 실행시킵니다. 
    public static void Publish(GameEvent gameEvent) {
        if(Events.TryGetValue(gameEvent, out UnityEvent thisEvent)) {
            thisEvent.Invoke();
        }
    }
}
