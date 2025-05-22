using UnityEngine;

public class PoliceSlime : SlimeBase
{
    // - - - - - - - - - -
    // 필드
    // - - - - - - - - - -
    [Header("[ Police Slime Logic ]")]
    [SerializeField, Tooltip("감금 생성 간격")] private int ImprisionInterval;
    [SerializeField, Tooltip("감금 생성 남은 턴 수")] private int _imprisionCounter;



    // - - - - - - - - - - 
    // Unity 콜백
    // - - - - - - - - - - 
    protected override void Start()
    {
        base.Start();

        _imprisionCounter = ImprisionInterval;
    }



    // - - - - - - - - -
    // ITurnListener
    // - - - - - - - - -
    public override void OnEnter_NewTurn()
    {
        CalculateImprision();
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
            EventManager.Publish(GameEvent.Imprison);
        }
    }
}
