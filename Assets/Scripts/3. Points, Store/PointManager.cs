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
    // 필드
    // - - - - - - - - - -
    [SerializeField, Tooltip("포인트 규칙")] private PointRule[] PointRules;
    [SerializeField, Tooltip("(테스트용) 초기 포인트")] private int TestPoints;

    public event Action OnPointChanged;

    private int _points;
    public int Points
    {
        get => _points;
        private set
        {
            _points = value;
            OnPointChanged?.Invoke();
        }
    }
    public void ResetToZeroPoints()
    {
        _points=0;
        OnPointChanged?.Invoke();
    }


    public void Init()
    {
        GameManager.Instance.OnGetPoint += GetPoint;
        Points = TestPoints;
    }

    // 로직
    // - - - - - - - - - -
    // 계산 및 포인트 획득
    public void GetPoint(object _, PointGetInfo pointInfo)
    {
        foreach (PointRule combineValue in PointRules)
        {
            if (combineValue.tileValue == pointInfo.tileValue)
            {
                Points += combineValue.point;
                break;
            }
        }
    }

    // 돈 충분한가 체크
    public bool CheckPoint(int amount)
    {
        if (Points >= amount)
        {
            return true;
        }
        else return false;
    }

    public void UsePoint(int amount)
    {
        Points -= amount;
    }

    public void ResetPoint()
    {
        Points = TestPoints;
    }



    public class PointGetInfo : EventArgs
    {
        public int tileValue;

        public PointGetInfo(int tileValue)
        {
            this.tileValue = tileValue;
        }
    }
}
