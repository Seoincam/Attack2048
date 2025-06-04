using UnityEngine;

public class SettingButton : MonoBehaviour
{
    [SerializeField] private GameObject _settingPanel;

    public void OpenSetting()
    {
        if (GameManager.Instance != null) GameManager.Instance.IsPaused = true;
        _settingPanel.SetActive(true);
    }

    public void CloseSetting()
    {
        if (GameManager.Instance != null) GameManager.Instance.IsPaused = false;
        _settingPanel.SetActive(false);
    }
}
