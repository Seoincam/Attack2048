using UnityEngine;

public class ArcherSlime : SlimeBase
{
    // - - - - - - - - - -
    // 필드
    // - - - - - - - - - -
    [Header("[ Archer Slime Logic ]")]
    [SerializeField, Tooltip("삭제 생성 간격")] private int DeleteInterval;
    [SerializeField, Tooltip("삭제 생성 남은 턴 수")] private int _deleteCounter;

    [Space, SerializeField, Tooltip("삭제6 생성 간격")] private int Delete6Interval;
    [SerializeField, Tooltip("삭제6 생성 남은 턴 수")] private int _delete6Counter;

    // - - - - - - - - - - 
    // Unity 콜백
    // - - - - - - - - - - 
    protected override void Start()
    {
        base.Start();

        _deleteCounter = DeleteInterval;
        _delete6Counter = Delete6Interval;
    }

    // - - - - - - - - -
    // ITurnListener
    // - - - - - - - - -
    public override void OnEnter_NewTurn()
    {
        CalculateDelete();
        CalculateDelete6();
    }


    // - - - - - - - - -
    // 로직
    // - - - - - - - - -
    // 삭제
    private void CalculateDelete()
    {
        _deleteCounter--;

        if (_deleteCounter == 0)
        {
            _deleteCounter = DeleteInterval;
            // EventManager.Publish(GameEvent.Delete);
            SlimeActionManager.Instance.DeleteArcher();
        }
    }

    // 삭제6
    private void CalculateDelete6()
    {
        _delete6Counter--;

        if (_delete6Counter == 0)
        {
            _delete6Counter = Delete6Interval;
            // EventManager.Publish(GameEvent.Delete6);
            SlimeActionManager.Instance.Delete6();
            SoundManager.Instance.PlayArcherActiveSFX();
        }
    }
}
