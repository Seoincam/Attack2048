using UnityEngine;

public class PoliceSlime : SlimeBase
{
    // - - - - - - - - - -
    // 필드
    // - - - - - - - - - -
    [Header("[ Police Slime Logic ]")]
    [SerializeField, Tooltip("감금 생성 간격")] private int ImprisionInterval;
    [SerializeField, Tooltip("감금 생성 남은 턴 수")] private int _imprisionCounter;
    [Space, SerializeField, Tooltip("강제 이동 생성 간격")] private int ForcedMoveInterval;
    [SerializeField, Tooltip("강제 이동 남은 턴 수")] private int _forcedMoveCounter;



    // - - - - - - - - - - 
    // Unity 콜백
    // - - - - - - - - - - 
    protected override void Start()
    {
        base.Start();

        _imprisionCounter = ImprisionInterval;
        _forcedMoveCounter = ForcedMoveInterval;
    }



    // - - - - - - - - -
    // ITurnListener
    // - - - - - - - - -
    public override void OnEnter_NewTurn()
    {
        CalculateImprision();
        CalculateForcedMove();
    }


    // - - - - - - - - -
    // 로직
    // - - - - - - - - -
    // 감금
    private void CalculateImprision()
    {
        _imprisionCounter--;

        if (_imprisionCounter == 0)
        {
            _imprisionCounter = ImprisionInterval;
            SlimeActionManager.Instance.Imprison();
        }
    }
     // 강제 이동
    private void CalculateForcedMove()
    {
        _forcedMoveCounter--;

        if (_forcedMoveCounter == 0)
        {
            _forcedMoveCounter = ForcedMoveInterval;
            SlimeActionManager.Instance.ForcedMove();
            SoundManager.Instance.PlayGuardForcedSlideSFX();
        }
    }
}
