using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SoundSetting
{
    public float BgmVolume = .25f;
    public float SfxVolume = .75f;
}

public class SoundManager : SingleTone<SoundManager>
{
    // 필드    
    // - - - - - - - - - - 
    [Header("Audio Player")]
    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioSource SFX;

    [Header("Audio Clip")]
    public AudioClip LobbyBGM;

    private string _saveFileName = "soundSettings.json";
    private SoundSetting _soundSetting;

    protected override void Awake()
    {
        base.Awake();
        _soundSetting = LoadSetting();
        Debug.Log(Application.persistentDataPath);
    }


    // 설정
    // - - - - - - - - - -
    public void SetPanel(Slider bgmSlider, Slider sfxSlider)
    {
        bgmSlider.value = _soundSetting.BgmVolume;
        sfxSlider.value = _soundSetting.SfxVolume;

        bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
    }

    private void OnBgmSliderChanged(float volume)
    {
        volume = Mathf.Clamp(volume, 0, 1);
        _soundSetting.BgmVolume = volume;
        BGM.volume = volume;
    }

    private void OnSfxSliderChanged(float volume)
    {
        volume = Mathf.Clamp(volume, 0, 1);
        _soundSetting.SfxVolume = volume;
        // SFX.volume = volume;
    }


    // 로직
    // - - - - - - - - - - 
    public void PlayBGM(AudioClip bgmClip)
    {
        BGM.Stop();
        BGM.clip = bgmClip;
        BGM.volume = _soundSetting.BgmVolume;
        BGM.Play();
    }


    // 저장 
    public void SaveSetting()
    {
        string json = JsonUtility.ToJson(_soundSetting, prettyPrint: true);
        string path = Path.Combine(Application.persistentDataPath, _saveFileName);

        try
        {
            File.WriteAllText(path, json);
        }
        catch (IOException e)
        {
            Debug.LogError($"사운드 설정 저장을 실패했습니다: {e.Message}");
        }
    }

    private SoundSetting LoadSetting()
    {
        string path = Path.Combine(Application.persistentDataPath, _saveFileName);

        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<SoundSetting>(json);
            }

            catch (IOException e)
            {
                Debug.LogError($"사운드 설정 불러오기를 실패했습니다: {e.Message}");
            }
        }

        return new SoundSetting();
    }
}
