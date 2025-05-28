using UnityEngine;

public class WizardSlime : SlimeBase
{
    // - - - - - - - - - -
    // 필드
    // - - - - - - - - - -
    [Header("[ Wizard Slime Logic ]")]
    [SerializeField, Tooltip("상하좌우반전 생성 간격")] private int ReverseMoveInterval;
    [SerializeField, Tooltip("상하좌우반전 생성 남은 턴 수")] private int _reverseMoveCounter;




    // - - - - - - - - - - 
    // Unity 콜백
    // - - - - - - - - - - 
    protected override void Start()
    {
        base.Start();

        _reverseMoveCounter = ReverseMoveInterval;
    }



    // - - - - - - - - -
    // ITurnListener
    // - - - - - - - - -
    public override void OnEnter_NewTurn()
    {
        CalculateReverse();
    }


    // - - - - - - - - -
    // 로직
    // - - - - - - - - -
    // 상하좌우반전
    private void CalculateReverse()
    {
        _reverseMoveCounter--;

        if (_reverseMoveCounter == 0)
        {
            _reverseMoveCounter = ReverseMoveInterval;
            EventManager.Publish(GameEvent.ReverseMove);
        }
    }

}
