using UnityEngine;

public class SettingButton : MonoBehaviour
{
    [SerializeField] private GameObject _settingPanel;

    public void OpenSetting()
    {
        _settingPanel.SetActive(true);
    }

    public void CloseSetting()
    {
        _settingPanel.SetActive(false);
    }
}
