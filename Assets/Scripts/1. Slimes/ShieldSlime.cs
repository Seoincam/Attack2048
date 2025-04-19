// - - - - - - - - - - - - - - - - - -
// ShieldSlime.cs
//  - 방패 슬라임 클래스.
// - - - - - - - - - - - - - - - - - -

public class ShieldSlime : SlimeBase
{
    // - - - - - - - - - - - - - - - - - - - - -
    // 필드
    // - - - - - - - - - - - - - - - - - - - - -
    private const int ShieldInterval = 8; // 받는 데미지를 0으로 하는 간격.
    private int _shieldCounter;

    private bool _isWeakened = false; // 1.5배 데미지 받을지 여부. (ShieldInterval 다음 턴에)
    // TODO: 1.5배 데미지 받기 구현.

    private const int WallInterval = 3; // 벽 생성할 간격.
    private int _wallCounter;



    // - - - - - - - - - - - - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - - - - - - - - - - - - -
    protected override void Start() {
        maxHealth = 20;
        base.Start();
        
        _shieldCounter = ShieldInterval;
        _wallCounter = 1;
    } 



    // - - - - - - - - - - - - - - - - - - - - -
    // 로직
    // - - - - - - - - - - - - - - - - - - - - -
    
    // 턴이 바뀔 때마다 실행.
    public override void OnTurnChanged()
    {
        CalShield();
        CalWall();
    }

    // 방패 생성 간격을 계산 및 실행.
    private void CalShield() {
        _shieldCounter --;

        if(_shieldCounter == 0) {
            // TODO: 방패 생성 호출
            _shieldCounter = ShieldInterval;
            _isWeakened = true;
        }
    }

    // 벽 생성 간격을 계산 및 실행.
    private void CalWall() {
        _wallCounter --;

        if(_wallCounter == 0) {
            EventManager.Publish(GameEvent.Wall);
            _wallCounter = WallInterval;
        }
    }
}
