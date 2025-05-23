using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class SlimeBase : MonoBehaviour, INewTurnListener
{
    // - - - - - - - - - 
    // 필드
    // - - - - - - - - -     

    private Slider _healthSlider;
    private TextMeshProUGUI _healthText;

    [SerializeField] protected float maxHealth;
    protected float curHealth;

    private SlimeManager _slimeManager;



    // - - - - - - - - 
    // Unity 콜백
    // - - - - - - - - 
    protected virtual void Start()
    {
        GameManager.Instance._damageInvoker.OnCombine += GetDamge;
        curHealth = maxHealth;
        Subscribe_NewTurn();

        _healthSlider.value = curHealth / maxHealth;
        _healthText.text = $"{curHealth} / {maxHealth}";
    }

    public void Init(SlimeManager slimeManager, Slider healthSlider, TextMeshProUGUI healthText)
    {
        _slimeManager = slimeManager;
        _healthSlider = healthSlider;
        _healthText = healthText;
    }

    // - - - - - - - - - - 
    // 데미지 로직
    // - - - - - - - - - - 
    protected virtual void GetDamge(float damage)
    {
        _slimeManager.MakeDamageText(damage);

        curHealth = Mathf.Max(0, curHealth - damage);
        _healthSlider.value = curHealth / maxHealth;
        _healthText.text = $"{curHealth} / {maxHealth}";

        if (curHealth == 0) { Die(); }
    }



    // - - - - - - - - - - 
    // 사망 로직
    // - - - - - - - - - - 
    private void Die() {
        GameManager.Instance._damageInvoker.OnCombine -= GetDamge;
        EventManager.Unsubscribe(GameEvent.NewTurn, OnEnter_NewTurn);
        _slimeManager.OnSlimeDie();
        gameObject.SetActive(false);
    }


    // - - - - - - - - - - - 
    // INewTurnListener
    // - - - - - - - - - - -
    public void Subscribe_NewTurn() {
        EventManager.Subscribe(GameEvent.NewTurn, OnEnter_NewTurn);
    }

    public virtual void OnEnter_NewTurn(){} // 각 자식 슬라임 클래스에서 해당 메서드 안에 매턴 마다 발생하는 항목 작성
}
