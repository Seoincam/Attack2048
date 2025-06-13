// - - - - - - - - - - - - - - - - - -
// GameSetting.cs
//  - 게임 설정 클래스.
//  - 프레임, 화면비
// - - - - - - - - - - - - - - - - - -

using UnityEngine;

public class GameSetting : SingleTone<GameSetting>
{
    public float CameraSize { get; private set; }
    
    public override void Awake()
    {
        base.Awake();

        // 기준 해상도 비율
        float targetAspect = 9f / 16f;
        float currentAspect = (float)Screen.width / Screen.height;

        float baseSize = 5f;
        float scale = targetAspect / currentAspect;

        // 화면이 더 길면 카메라 크기를 키움, 짧으면 그대로
        CameraSize = baseSize * scale;

        // 프레임 고정
        Application.targetFrameRate = 60;
    }
}
