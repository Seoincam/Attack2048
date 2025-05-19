// - - - - - - - - - - - - - - - - - -
// GameSetting.cs
//  - 게임 설정 클래스.
//  - BGM, SFX 값을 가지고 있음.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour
{
    public static GameSetting instance;

    private Slider _bgmSlider;
    private Slider _sfxSlider;

    public float BgmVolume { get; private set; }
    public float SfxVolume { get; private set; }
    


    void Awake()
    {
        // 싱글턴
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this) Destroy(gameObject);

        // 프레임
        Application.targetFrameRate = 60;

        // 불륨
        BgmVolume = 0.75f;
        SfxVolume = 0.75f;
    }



    public void SetPanel(Slider bgmSlider, Slider sfxSlider)
    {
        _bgmSlider = bgmSlider;
        _sfxSlider = sfxSlider;

        _bgmSlider.value = BgmVolume;
        _sfxSlider.value = SfxVolume;

        _bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        _sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
    }



    private void OnBgmSliderChanged(float volume)
    {
        volume = Mathf.Clamp(volume, 0, 1);
        BgmVolume = volume;
    }
    private void OnSfxSliderChanged(float volume)
    {
        volume = Mathf.Clamp(volume, 0, 1);
        SfxVolume = volume;
    }
}
