using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    void Start()
    {
        if (SoundManager.instance != null)
        {
            // GameSetting 싱글턴에 할당
            SoundManager.instance.SetPanel(_bgmSlider, _sfxSlider);
        }
    }
}
