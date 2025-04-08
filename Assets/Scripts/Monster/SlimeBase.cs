using TMPro;
using UnityEngine;

public abstract class SlimeBase : MonoBehaviour
{
    // - - - - - - - - - - - - - - - - - - - - -
    // 필드
    // - - - - - - - - - - - - - - - - - - - - -
    [Header("Damage Animation")]
    [SerializeField] private Transform slimeCanvas;
    [SerializeField] private TextMeshProUGUI damageText;

    protected int maxHealth;
    [SerializeField] protected int curHealth;


    // - - - - - - - - - - - - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - - - - - - - - - - - - -
    protected virtual void Start()
    {
        GameManager.Instance.damageInvoker.CombineDamge += GetDamge;
        curHealth = maxHealth;
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
}
