using UnityEngine; 
using UnityEngine.UI; 

public class SoundManager : MonoBehaviour
{
    // 필드    
    // - - - - - - - - - - 
    public static SoundManager instance;

    [Header("Audio Player")]
    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioSource SFX;

    [Header("Audio Clip")]
    [SerializeField] private AudioClip LobbyBGM;

    private float BgmVolume;
    private float SfxVolume;


    // Unity 콜백    
    // - - - - - - - - - - 
    void Awake()
    {
        // 싱글톤
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);

        // 기본 볼륨
        BgmVolume = 0.75f;
        SfxVolume = 0.75f;
    }

    void Start()
    {
        PlayBGM(LobbyBGM);
    }


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
