using UnityEngine; 
using UnityEngine.UI; 

public class SoundManager : SingleTone<SoundManager>
{
    // 필드    
    // - - - - - - - - - - 
    [Header("Audio Player")]
    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioSource SFX;

    [Header("Audio Clip")]
    public AudioClip LobbyBGM;

    private float BgmVolume = .25f;
    private float SfxVolume = .75f;


    // 설정
    // - - - - - - - - - -
    public void SetPanel(Slider bgmSlider, Slider sfxSlider)
    {
        bgmSlider.value = BgmVolume;
        sfxSlider.value = SfxVolume;

        bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
    }

    private void OnBgmSliderChanged(float volume)
    {
        volume = Mathf.Clamp(volume, 0, 1);
        BgmVolume = volume;
        BGM.volume = volume;
    }
    
    private void OnSfxSliderChanged(float volume)
    {
        volume = Mathf.Clamp(volume, 0, 1);
        SfxVolume = volume;
        // SFX.volume = volume;
    }


    // 로직
    // - - - - - - - - - - 
    public void PlayBGM(AudioClip bgmClip)
    {
        BGM.Stop();
        BGM.clip = bgmClip;
        BGM.volume = BgmVolume;
        BGM.Play();
    }
}
