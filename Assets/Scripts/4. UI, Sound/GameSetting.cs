// - - - - - - - - - - - - - - - - - -
// GameSetting.cs
//  - 게임 설정 클래스.
//  - 프레임, 화면비
// - - - - - - - - - - - - - - - - - -

using UnityEngine;

public class GameSetting : SingleTone<GameSetting>
{
    public float CameraSize { get; private set; }
    public int testStartIndex;

    protected override void Awake()
    {
        base.Awake();

        SetUp();
    }

    private void SetUp()
    {
        SetFrameRate();
        SetCameraSize();
    }

    private void SetFrameRate()
    {
        Application.targetFrameRate = 60;
    }
    
    private void SetCameraSize()
    {
        float targetAspect = 9f / 16f;
        float currentAspect = (float)Screen.width / Screen.height;

        float baseSize = 5f;
        float scale = targetAspect / currentAspect;

        CameraSize = baseSize * scale;
    }
}
