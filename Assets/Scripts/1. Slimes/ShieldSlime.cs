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
    public override void OnEnter_NewTurn()
    {
        if (HasShield)
        {
            HasShield = false;
            IsWeakened = true;
        }
        else if (IsWeakened)
        {
            IsWeakened = false;
        }
        CalculateShield();
        CalculateWall();
    }


    // - - - - - - - - -
    // 방패
    // - - - - - - - - -
    private void CalculateShield()
    {
        _shieldCounter--;

        if (_shieldCounter == 0)
        {
            _shieldCounter = ShieldInterval;
            HasShield = true;
        }
    }


    // - - - - - - - - -
    // 벽
    // - - - - - - - - -
    private void CalculateWall() {
        _wallCounter --;

        if (_wallCounter == 0)
        {
            EventManager.Publish(GameEvent.Wall);
            _wallCounter = WallInterval;
        }
    }



    // - - - - - - - - -
    // 데미지 로직 override (방패 & 약화 적용 위해서)
    // - - - - - - - - -
    protected override void GetDamge(float damage)
    {
        if (HasShield)
        {
            Debug.Log("방패! 데미지 0");
            damage = 0;
            return;
        }
        else if (IsWeakened)
        {
            Debug.Log($"[약화] >> 데미지 : {damage} * 1.5배 = {damage * 1.5f}");
            damage = damage * 1.5f;
        }

        base.GetDamge(damage);
    }
}
