using UnityEngine;

public abstract class SlimeBase : MonoBehaviour, INewTurnListener
{
    // - - - - - - - - - 
    // 필드
    // - - - - - - - - -     
    private StageManager _slimeManager;

    [Header("Setting")]
    public int DefaltTurns;
    public int ClearValue = 128;


    // - - - - - - - - 
    // Unity 콜백
    // - - - - - - - - 
    protected virtual void Start()
    {
        Subscribe_NewTurn();

    }

    public void Init(StageManager slimeManager)
    {
        _slimeManager = slimeManager;
    }



    // - - - - - - - - - - 
    // 사망 로직
    // - - - - - - - - - - 
    public void Die() {
        EventManager.Unsubscribe(GameEvent.NewTurn, OnEnter_NewTurn);
        _slimeManager.OnGameClear();
        gameObject.SetActive(false);
    }


    // - - - - - - - - - - - 
    // INewTurnListener
    // - - - - - - - - - - -
    public void Subscribe_NewTurn() {
        EventManager.Subscribe(GameEvent.NewTurn, OnEnter_NewTurn);
    }

    public virtual void OnEnter_NewTurn(){} // 각 자식 슬라임 클래스에서 해당 메서드 안에 매턴 마다 발생하는 항목 작성
}
