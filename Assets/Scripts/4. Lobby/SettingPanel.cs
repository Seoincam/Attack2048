using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    void Start()
    {
        if (GameSetting.instance != null)
        {
            Debug.Log("할당!");
            // GameSetting 싱글턴에 할당
            GameSetting.instance.SetPanel(_bgmSlider, _sfxSlider);
        }
    }
}
