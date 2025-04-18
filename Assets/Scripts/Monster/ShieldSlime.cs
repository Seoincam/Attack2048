using UnityEngine;

public class ShieldSlime : SlimeBase
{
    private const int ShieldInterval = 8; // 1-a. 이 간격마다 받는 데미지 0
    private int _shieldCounter;

    private bool _isWeakened = false; // 1-b. 데미지 안 받고, 다음 턴 1.5배 데미지 받음.

    private const int WallInterval = 5; // 2. 이 간격마다 벽 생성
    private int _wallCounter;

    protected override void Start() {
        maxHealth = 20;
        base.Start();
        
        _shieldCounter = ShieldInterval;
        _wallCounter = WallInterval;
    } 



    // 턴이 바뀔 때마다 실행됩니다.
    public override void OnTurnChanged()
    {
        CalShield();
        CalWall();
    }



    // 방패 생성 간격을 계산 및 실행합니다.
    private void CalShield() {
        _shieldCounter --;

        if(_shieldCounter == 0) {
            // 방패 생성 호출
            _shieldCounter = ShieldInterval;
        }
    }

    // 벽 생성 간격을 계산 및 실행합니다.
    private void CalWall() {
        _wallCounter --;

        if(_wallCounter == 0) {
            // 벽 생성 호출
            _wallCounter = WallInterval;
        }
    }
}
