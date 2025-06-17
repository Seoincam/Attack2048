using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    void Start()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetPanel(_bgmSlider, _sfxSlider);
    }
}