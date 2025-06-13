using UnityEngine;

public class CameraController : MonoBehaviour
{
    void Awake()
    {
        if (GameSetting.Instance != null)
        {
            Camera.main.orthographicSize = GameSetting.Instance.CameraSize;
        }
    }
}
