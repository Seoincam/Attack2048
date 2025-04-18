// - - - - - - - - - - - - - - - - - -
// SlimeActionBase.cs
//  - 벽, 석화, 감금, 이동 클래스의 부모 클래스.
//  - 수명이 다하면 각 로직을 실행 시킴.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;

public abstract class SlimeActionBase : MonoBehaviour, ITurnListener
{
    // - - - - - - - - - - - - - - - - - - - - -
    // 필드
    // - - - - - - - - - - - - - - - - - - - - -
    protected virtual int Life => 0; // 수명. 자식 클래스에 각각 설정.
    private int _lifeCounter;


    // - - - - - - - - - - - - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - - - - - - - - - - - - -
    void Awake() {
        Subscribe_NewTurn();
        _lifeCounter = Life;
    }
    

    // - - - - - - - - - - - - - - - - - - - - -
    // 로직
    // - - - - - - - - - - - - - - - - - - - - -

    // EventManager에 구독.
    public void Subscribe_NewTurn() {
        EventManager.Subscribe(GameEvent.NewTurn, OnTurnChanged);
    }

    // 턴이 바뀔 때마다 실행.
    public void OnTurnChanged() {
        _lifeCounter --;

        if(_lifeCounter == 0) {
            Execute();
        }
    }

    // 수명이 다하면 실행할 로직.
    protected virtual void Execute() {}
}
