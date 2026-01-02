using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public float CameraSize { get; private set; }

    private Camera _camera;
    private float _targetAspect = 1080f / 2400f;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        
        SetFrameRate();
        MakeLetterBox();
        SetCameraSize();

        if (GameSetting.Instance != null)
        {
            Camera.main.orthographicSize = GameSetting.Instance.CameraSize;
        }
    }

    private void SetFrameRate()
    {
        Application.targetFrameRate = 60;
    }

    private void SetCameraSize()
    {
        float currentAspect = (float)Screen.width / Screen.height;

        float baseSize = 5f;
        float scale = _targetAspect / currentAspect;

        CameraSize = baseSize * scale;
    }

    private void MakeLetterBox()
    {
        Rect rect = _camera.rect;
        float scaleheight = ((float)Screen.width / Screen.height) / _targetAspect;
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }
        _camera.rect = rect;
    }
}
