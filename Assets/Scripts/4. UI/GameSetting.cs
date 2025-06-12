// - - - - - - - - - - - - - - - - - -
// GameSetting.cs
//  - 게임 설정 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;

public class GameSetting : MonoBehaviour
{
    void Awake()
    {
        // 프레임
        Application.targetFrameRate = 60;
    }
}
