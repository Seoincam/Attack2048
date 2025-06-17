// - - - - - - - - - - - - - - - - - -
// ShieldSlime.cs
//  - 방패 슬라임 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;

public class ShieldSlime : SlimeBase
{
    // - - - - - - - - - -
    // 필드
    // - - - - - - - - - -
    [Header("[ Shield Slime Logic ]")]
    [SerializeField, Tooltip("벽 생성 간격")] private int WallInterval;
    [SerializeField, Tooltip("벽 생성 개수")] private int WallCount;
    [SerializeField, Tooltip("벽 생성 남은 턴 수")] private int _wallCounter;

    [Space, SerializeField, Tooltip("삭제 생성 간격")] private int DeleteInterval;
    [SerializeField, Tooltip("삭제 생성 남은 턴 수")] private int _deleteCounter;




    // - - - - - - - - - - 
    // Unity 콜백
    // - - - - - - - - - - 
    protected override void Start()
    {
        base.Start();

        _wallCounter = WallInterval;
        _deleteCounter = DeleteInterval;
    }



    // - - - - - - - - -
    // ITurnListener
    // - - - - - - - - -
    public override void OnEnter_NewTurn()
    {
        CalculateWall();
        CalculateDelete();
    }




    // - - - - - - - - -
    // 로직
    // - - - - - - - - -
    // 벽
    private void CalculateWall()
    {
        _wallCounter--;

        if (_wallCounter == 0)
        {
            for (int i = 0; i < WallCount; i++)
                SlimeActionManager.Instance.Wall();

            _wallCounter = WallInterval;
        }
    }
    
    // 삭제
    private void CalculateDelete()
    {
        _deleteCounter--;

        if (_deleteCounter == 0)
        {
            _deleteCounter = DeleteInterval;
            SlimeActionManager.Instance.Delete();
        }
    }
}
