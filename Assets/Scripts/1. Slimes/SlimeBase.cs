using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class SlimeBase : MonoBehaviour, ITurnListener
{
    // - - - - - - - - - 
    // 필드
    // - - - - - - - - - 
    [Header("[ Damage Animation ]")]
    [SerializeField] private Transform slimeCanvas;
    [SerializeField] private TextMeshProUGUI damageText;


    [Header("[ Health ]")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [SerializeField] protected float maxHealth;
    [SerializeField] protected float curHealth;



    // - - - - - - - - 
    // Unity 콜백
    // - - - - - - - - 
    protected virtual void Start()
    {
        GameManager.Instance.damageInvoker.OnCombine += GetDamge;
        curHealth = maxHealth;
        Subscribe_NewTurn();

        healthSlider.value = curHealth / maxHealth;
        healthText.text = $"{curHealth} / {maxHealth}";
    }



    // - - - - - - - - - - 
    // 데미지 로직
    // - - - - - - - - - - 
    protected virtual void GetDamge( float damage ) {   
        MakeDamageText(damage);

        curHealth = Mathf.Max(0, curHealth - damage);
        healthSlider.value = curHealth / maxHealth;
        healthText.text = $"{curHealth} / {maxHealth}";

        if( curHealth == 0 ) { Die(); }
    }

    private void MakeDamageText( float damage ) {
        TextMeshProUGUI _damageText = Instantiate(damageText, slimeCanvas);
        _damageText.text = "-" + damage.ToString();
    }



    // - - - - - - - - - - 
    // 사망 로직
    // - - - - - - - - - - 
    private void Die() {
        GameManager.Instance.damageInvoker.OnCombine -= GetDamge;
        gameObject.SetActive(false);
    }


    // - - - - - - - - - - - 
    // ITurnListener
    // - - - - - - - - - - -
    public void Subscribe_NewTurn() {
        EventManager.Subscribe(GameEvent.NewTurn, OnTurnChanged);
    }

    public virtual void OnTurnChanged(){} // 각 자식 슬라임 클래스에서 해당 메서드 안에 매턴 마다 발생하는 항목 작성
}
