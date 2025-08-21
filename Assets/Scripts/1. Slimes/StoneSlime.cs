using UnityEngine;

public class StoneSlime : SlimeBase
{
    // - - - - - - - - - -
    // 필드
    // - - - - - - - - - -
    [Header("[ Stone Slime Logic ]")]
    [SerializeField, Tooltip("석화 생성 간격")] private int PetrifyInterval;
    [SerializeField, Tooltip("석화 생성 남은 턴 수")] private int _petrifyCounter;

    [Space, SerializeField, Tooltip("벽 생성 간격")] private int WallInterval;
    [SerializeField, Tooltip("벽 생성 남은 턴 수")] private int _wallCounter;


    [Space, SerializeField, Tooltip("제거 생성 간격")] private int DeleteInterval;
    [SerializeField, Tooltip("제거 생성 남은 턴 수")] private int _deleteCounter;



    // - - - - - - - - - - 
    // Unity 콜백
    // - - - - - - - - - - 
    protected override void Start()
    {
        base.Start();

        _petrifyCounter = PetrifyInterval;
        _wallCounter = WallInterval;
        _deleteCounter = DeleteInterval;
    }



    // - - - - - - - - -
    // ITurnListener
    // - - - - - - - - -
    public override void OnEnter_NewTurn()
    {
        CalculatePetrify();
        CalCulateWall();
        CalculateDelete();
    }


    // - - - - - - - - -
    // 로직
    // - - - - - - - - -
    // 석화
    private void CalculatePetrify()
    {
        _petrifyCounter--;

        if (_petrifyCounter == 0)
        {
            _petrifyCounter = PetrifyInterval;
            SlimeActionManager.Instance.Petrify();
        }
    }

    // 이동
    private void CalCulateWall()
    {
        _wallCounter--;

        if (_wallCounter == 0)
        {
            _wallCounter = WallInterval;
            SlimeActionManager.Instance.Wall();
            SoundManager.Instance.PlayRockCreateWallSFX();
        }
    }
    
    // 삭제
    private void CalculateDelete()
    {
        _deleteCounter--;

        if (_deleteCounter == 0)
        {
            _deleteCounter = DeleteInterval;
            // SlimeActionManager.Instance.Delete();
            SoundManager.Instance.PlayRockBreakTileSFX();
        }
    }
}
