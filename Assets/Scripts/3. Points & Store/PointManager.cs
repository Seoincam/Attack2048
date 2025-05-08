// - - - - - - - - - - - - - - - - - -
// PointManager.cs
//  - 포인트 관리 클래스.
// - - - - - - - - - - - - - - - - - -

using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct GettingPoint {
    [Tooltip("타일 숫자")] public int squareValue;
    [Tooltip("획득 포인트")] public int point;
}

public class PointManager : MonoBehaviour
{
    private int _points;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField, Tooltip("포인트 규칙")] private GettingPoint[] PointRules;



    void Awake()
    {
        pointsText.text = $"{_points}pt";
    }



    // 계산 및 포인트 획득
    public void GetPoint(int squareValue) {
        foreach(GettingPoint combineValue in PointRules) {
            if(combineValue.squareValue == squareValue) {
                _points += combineValue.point;
                break;
            }
        }
        pointsText.text = $"{_points}pt";
    }

    public bool UsePoint(int amount) {
        if(_points >= amount) {
            _points -= amount;
            pointsText.text = $"{_points}pt";

            return true;
        }
        else return false;
    }
}
