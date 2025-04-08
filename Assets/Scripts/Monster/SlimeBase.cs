using JetBrains.Annotations;
using UnityEngine;

public abstract class SlimeBase : MonoBehaviour
{
    // - - - - - - - - - - - - - - - - - - - - -
    // 필드
    // - - - - - - - - - - - - - - - - - - - - -
    protected int maxHealth;
    [SerializeField] protected int curHealth;


    // - - - - - - - - - - - - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - - - - - - - - - - - - -
    void Start()
    {
        GameManager.Instance.damageInvoker.CombineDamge += GetDamge;
        curHealth = maxHealth;
    }


    // - - - - - - - - - - - - - - - - - - - - -
    // 로직
    // - - - - - - - - - - - - - - - - - - - - -
    private void GetDamge( int damage )
    {   
        curHealth = Mathf.Max(0, curHealth - damage);
        
        if( curHealth == 0 ) {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance.damageInvoker.CombineDamge -= GetDamge;
    }
}
