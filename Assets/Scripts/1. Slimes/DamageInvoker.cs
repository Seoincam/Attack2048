using UnityEngine;

public class DamageInvoker
{
    public delegate void GetDamage(int damage);
    public event GetDamage CombineDamge;

    public void OnCombine(int squareValue)
    {
        int damage = (int)Mathf.Log(squareValue * 2, 2);
        CombineDamge?.Invoke(damage); // 현재 등록돼 있는 몬스터의 GetDamage 함수 호출
    }
}
