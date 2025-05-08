using UnityEngine;

public class DamageInvoker
{
    public delegate void Combine(float damage);
    public event Combine OnCombine;

    private float totalDamage;

    // 데미지 합산
    public void SumDamage( int squareValue ) {
        totalDamage += Mathf.Log(squareValue * 2, 2);
    }

    // 데미지 부과
    public void InvokeDamage() {
        if(totalDamage == 0) { return; } // 데미지 없으면 return

        OnCombine?.Invoke(totalDamage); // 현재 등록돼 있는 슬라임의 GetDamage 함수 호출
        totalDamage = 0;
    }
}
