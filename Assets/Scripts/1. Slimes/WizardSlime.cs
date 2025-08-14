using UnityEngine;

public class WizardSlime : SlimeBase
{
    // - - - - - - - - - -
    // 필드
    // - - - - - - - - - -
    [Header("[ Wizard Slime Logic ]")]
    [SerializeField, Tooltip("상하좌우반전 생성 간격")] private int ReverseMoveInterval;
    [SerializeField, Tooltip("상하좌우반전 생성 남은 턴 수")] private int _reverseMoveCounter;

    [Space, SerializeField, Tooltip("이동 생성 간격")] private int TranslocateInterval;
    [SerializeField, Tooltip("이동 생성 남은 턴 수")] private int _translocateCounter;

    [Space, SerializeField, Tooltip("블라인드 생성 간격")] private int BlindInterval;
    [SerializeField, Tooltip("블라인드 생성 남은 턴 수")] private int _blindCounter;
    [SerializeField, Tooltip("블라인드 생성 수")] private int blindCount;


    // FixMe: 다시하기 눌렀을 때 상하좌우반전 패턴이었다면 상하좌우반전 유지됨.
    // - - - - - - - - - - 
    // Unity 콜백
    // - - - - - - - - - - 
    protected override void Start()
    {
        base.Start();

        _reverseMoveCounter = ReverseMoveInterval;
        _translocateCounter = TranslocateInterval;
        _blindCounter = BlindInterval;
    }



    // - - - - - - - - -
    // ITurnListener
    // - - - - - - - - -
    public override void OnEnter_NewTurn()
    {
        CalculateReverse();
        CalCulateTranslocate();
        CalculateBlind();
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
            SlimeActionManager.Instance.ReverseMove();
            SoundManager.Instance.PlayWizardFlipSFX();
        }
    }

    // 이동
    private void CalCulateTranslocate()
    {
        _translocateCounter--;

        if (_translocateCounter == 0)
        {
            _translocateCounter = TranslocateInterval;
            SlimeActionManager.Instance.Translocate7();
            SoundManager.Instance.PlayWizardMoveNumberSFX();
        }
    }

    // 블라인드
    private void CalculateBlind()
    {
        _blindCounter--;

        if (_blindCounter == 0)
        {
            _blindCounter = BlindInterval;
            for (int i = 0; i < blindCount; i++)
                SlimeActionManager.Instance.Blind();
            SoundManager.Instance.PlayWizardBlindSFX();
        }
    }
}
