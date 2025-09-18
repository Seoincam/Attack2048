using UnityEngine;

public class PierrotSlime : SlimeBase
{
    // - - - - - - - - - -
    // 필드
    // - - - - - - - - - -
    [Header("[ Pierrot Slime Logic ]")]
    [SerializeField, Tooltip("랜덤 변경 생성 간격")] private int ChangeInterval;
    [SerializeField, Tooltip("랜덤 변경 생성 남은 턴 수")] private int _changeCounter;

    [Space, SerializeField, Tooltip("이동 생성 간격")] private int TranslocateInterval;
    [SerializeField, Tooltip("이동 생성 남은 턴 수")] private int _translocateCounter;

    // - - - - - - - - - - 
    // Unity 콜백
    // - - - - - - - - - - 
    protected override void Start()
    {
        base.Start();

        _changeCounter = ChangeInterval;
        _translocateCounter = TranslocateInterval;
    }

    // - - - - - - - - -
    // ITurnListener
    // - - - - - - - - -
    public override void OnEnter_NewTurn()
    {
        CalculateChange();
        CalCulateTranslocate();
    }


    // - - - - - - - - -
    // 로직
    // - - - - - - - - -
    // 랜덤 변경
    private void CalculateChange()
    {
        _changeCounter--;

        if (_changeCounter == 0)
        {
            _changeCounter = ChangeInterval;
            SlimeActionManager.Instance.Change();
            // SoundManager.Instance.PlayPierrotChangeNumberSFX();
        }
    }

    // 이동
    private void CalCulateTranslocate()
    {
        _translocateCounter--;

        if (_translocateCounter == 0)
        {
            _translocateCounter = TranslocateInterval;
            SlimeActionManager.Instance.Translocate3();
        }
    }
}
