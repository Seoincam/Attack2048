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
    [SerializeField, Tooltip( "방패 생성 간격" )]  private int ShieldInterval;
    [SerializeField, Tooltip( "방패 남은 턴 수" )] private int _shieldCounter;
    private bool _hasShield;
    private bool HasShield { 
        get => _hasShield;
        set { _hasShield = value; hasShieldText.SetActive(value); }
    }

    private bool _isWeakened = false;
    private bool IsWeakened { 
        get => _isWeakened;
        set { _isWeakened = value; isWeakendText.SetActive(value); }
    }

    [Space, SerializeField, Tooltip( "벽 생성 간격" )] private int WallInterval;
    [SerializeField, Tooltip( "벽 생성 남은 턴 수" )]   private int _wallCounter;

    [Space, SerializeField] private GameObject hasShieldText;
    [SerializeField] private GameObject isWeakendText;



    // - - - - - - - - - - 
    // Unity 콜백
    // - - - - - - - - - - 
    protected override void Start() {
        base.Start();
        
        _shieldCounter = ShieldInterval;
        _wallCounter = WallInterval;
        IsWeakened = false;
        HasShield = false;
    } 



    // - - - - - - - - -
    // ITurnListener
    // - - - - - - - - -
    public override void OnTurnChanged()
    {
        CalShield();
        CalWall();
    }


    // - - - - - - - - -
    // 방패
    // - - - - - - - - -
    private void CalShield() {
        if(HasShield) { return; } // 방패가 있다면 _shieldCounter 유지
    
        _shieldCounter --;

        if(_shieldCounter == 0) {
            Debug.Log(" [방패 슬라임] 방패 생성");
            _shieldCounter = ShieldInterval;
            HasShield = true;
        }
    }


    // - - - - - - - - -
    // 벽
    // - - - - - - - - -
    private void CalWall() {
        _wallCounter --;

        if(_wallCounter == 0) {
            EventManager.Publish(GameEvent.Wall);
            _wallCounter = WallInterval;
        }
    }



    // - - - - - - - - -
    // 데미지 로직 override (방패 & 약화 적용 위해서)
    // - - - - - - - - -
    protected override void GetDamge(float damage)
    {
        // 방패 있을 때 데미지 발생하면
        //  - 데미지 안 받기
        //  - 다음 턴 약화
        if(HasShield) { 
            Debug.Log(" [방패 슬라임] 방패 사용");

            HasShield = false; 
            IsWeakened = true; 

            return; 
        }

        // 약화일 때 데미지 발생하면
        else if(IsWeakened) { 
            Debug.Log(" [방패 슬라임] 약화");
            Debug.Log($"데미지 : {damage} * 1.5배 = {damage * 1.5f}");
            
            damage = (int)(damage * 1.5f); 
            IsWeakened = false;
        }

        base.GetDamge(damage);
    }
}
