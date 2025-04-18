using TMPro;
using UnityEngine;

public abstract class SlimeBase : MonoBehaviour, ITurnListener
{
    // - - - - - - - - - - - - - - - - - - - - -
    // 필드
    // - - - - - - - - - - - - - - - - - - - - -
    [Header("Damage Animation")]
    [SerializeField] private Transform slimeCanvas;
    [SerializeField] private TextMeshProUGUI damageText;

    [Header("Health")]
    protected int maxHealth;
    [SerializeField] protected int curHealth;


    // - - - - - - - - - - - - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - - - - - - - - - - - - -
    protected virtual void Start()
    {
        GameManager.Instance.damageInvoker.CombineDamge += GetDamge;
        curHealth = maxHealth;
        Subscribe_NewTurn();
    }


    // - - - - - - - - - - - - - - - - - - - - -
    // 로직
    // - - - - - - - - - - - - - - - - - - - - -
    private void GetDamge( int damage )
    {   
        MakeDamageText(damage);

        curHealth = Mathf.Max(0, curHealth - damage);
        if( curHealth == 0 ) {
            Die();
        }
    }

    private void MakeDamageText( int damage )
    {
        TextMeshProUGUI _damageText = Instantiate(damageText, slimeCanvas);
        _damageText.text = "-" + damage.ToString();

        // 추후 DOTween으로 애니메이션 구현 예정
    }

    private void Die()
    {
        GameManager.Instance.damageInvoker.CombineDamge -= GetDamge;
        gameObject.SetActive(false);
    }

    public void Subscribe_NewTurn()
    {
        EventManager.Subscribe(GameEvent.NewTurn, OnTurnChanged);
    }

    public virtual void OnTurnChanged(){} // 각 자식 슬라임 클래스에서 해당 메서드 안에 매턴 마다 발생하는 항목 작성
}
