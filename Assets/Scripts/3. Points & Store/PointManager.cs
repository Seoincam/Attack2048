// - - - - - - - - - - - - - - - - - -
// PointManager.cs
//  - 포인트 관리 클래스.
// - - - - - - - - - - - - - - - - - -

using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct GettingPoint {
    [Tooltip("타일 숫자")] public int tileValue;
    [Tooltip("획득 포인트")] public int point;
}

public class PointManager : MonoBehaviour
{
    // - - - - - - - - - -
    // 필드
    // - - - - - - - - - -
    private int _points;
    private int Points {
        get => _points;
        set {
            _points = value;
            pointsText.text = $"{_points}pt";
        }
    }
    
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField, Tooltip( "포인트 규칙" )] private GettingPoint[] PointRules;
    [SerializeField, Tooltip( "(테스트용) 초기 포인트" )] private int TestPoints;


    // - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - -
    void Awake()
    {
        Points = TestPoints;
    }


    // - - - - - - - - - -
    // 로직
    // - - - - - - - - - -

    // 계산 및 포인트 획득
    public void GetPoint(int tileValue) {
        foreach(GettingPoint combineValue in PointRules) {
            if(combineValue.tileValue == tileValue) {
                Points += combineValue.point;
                break;
            }
        }
    }

    public bool UsePoint(int amount) {
        if(Points >= amount) {
            Points -= amount;
            return true;
        }
        else return false;
    }
}
