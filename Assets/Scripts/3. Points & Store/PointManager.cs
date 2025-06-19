// - - - - - - - - - - - - - - - - - -
// PointManager.cs
//  - 포인트 관리 클래스.
// - - - - - - - - - - - - - - - - - -

using System;
using UnityEngine;

[Serializable]
public struct PointRule {
    [Tooltip("타일 숫자")] public int tileValue;
    [Tooltip("획득 포인트")] public int point;
}

public class PointManager : MonoBehaviour
{
    // - - - - - - - - - -
    // 필드
    // - - - - - - - - - -
    public event Action OnPointChanged;

    private int _point;
    public int Point {
        get => _point;
        private set
        {
            _point = value;
            OnPointChanged?.Invoke();
        }
    }
    
    
    [SerializeField, Tooltip( "포인트 규칙" )] private PointRule[] PointRules;
    [SerializeField, Tooltip( "(테스트용) 초기 포인트" )] private int TestPoints;


    // - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - -
    void Awake()
    {
        Point = TestPoints;
    }


    // - - - - - - - - - -
    // 로직
    // - - - - - - - - - -

    // 계산 및 포인트 획득
    public void GetPoint(int tileValue) {
        foreach(PointRule combineValue in PointRules) {
            if(combineValue.tileValue == tileValue) {
                Point += combineValue.point;
                break;
            }
        }
    }

    // 돈 충분한가 체크
    public bool CheckPoint(int amount) {
        if(Point >= amount) {
            return true;
        }
        else return false;
    }

    public void UsePoint(int amount) {
        Point -= amount;
    }
}
