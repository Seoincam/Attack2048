using UnityEngine;
using UnityEngine.UI;

public class SoundSetting
{
    public float BgmVolume = .25f;
    public float SfxVolume = .75f;

    public SoundSetting(float bgmVolume, float sfxVolume)
    {
        BgmVolume = bgmVolume;
        SfxVolume = sfxVolume;
    }
}

public class SoundManager : MonoBehaviour
{
    // 필드    
    // - - - - - - - - - - 
    [Header("Audio Player")]
    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioSource SFX;

    [Header("Audio Clip")]
    public AudioClip LobbyBGM;
    public AudioClip stageBGM;
    
    [Header("Button Clip")]
    public AudioClip ButtonSFX;
    [Header("Codex Clip")]
    public AudioClip CodexSFX;
    [Header("Shop Clip")]
    public AudioClip ShopSFX;
    [Header("Combine Clip")]
    public AudioClip CombineSFX;
    [Header("블록 파괴")]
    public AudioClip BreakBlockSFX;
    [Header("추가 턴")]
    public AudioClip AddTurnSFX;
    [Header("블록파괴 방어")]
    public AudioClip PreventDestroySFX;
    [Header("스테이지 클리어")]
    public AudioClip StageClearSFX;
    [Header("아처 슬라임")]
    public AudioClip ArcherActiveSFX;
    public AudioClip ArcherBreakTileSFX;
    [Header("바위 슬라임")]
    public AudioClip RockBreakTileSFX;
    public AudioClip RockCreateWallSFX;
    [Header("삐에로 슬라임")]
    public AudioClip PierrotChangeNumberSFX;
    [Header("병정 슬라임")]
    public AudioClip GuardForcedSlideSFX;
    [Header("마법사 슬라임")]
    public AudioClip WizardMoveNumberSFX;
    public AudioClip WizardFlipSFX;
    public AudioClip WizardSwapTwoRowsSFX;
    public AudioClip WizardBlindSFX;
    [Header("방패슬라임")]
    public AudioClip ShieldCreateWallSFX; // 방패 슬라임 벽생성 겸 병정 감옥 패턴
    public AudioClip ShieldDeleteSFX;

    [Header("Lock")] 
    [SerializeField] private AudioClip lockSfx;

    private string _saveFileName = "soundSettings.json";
    private SoundSetting _soundSetting;

    public void Init()
    {
        _soundSetting = LoadSetting();
        if(SFX == null || !SFX.gameObject.scene.IsValid())
        {
            Debug.Log("SFX AudioSource가 없거나 유효하지 않습니다. 새로 생성합니다.");
            SFX = gameObject.AddComponent<AudioSource>();
            SFX.loop = false;
        }

        if ((BGM == null) || !BGM.gameObject.scene.IsValid())
        {
            Debug.Log("BGM AudioSource가 없거나 유요하지 않습니다. 새로 생성합니다.");
            BGM = gameObject.AddComponent<AudioSource>();
            BGM.loop = true;
        }
        SFX.volume = _soundSetting.SfxVolume;
        BGM.volume = _soundSetting.BgmVolume;
    }


    // 설정
    // - - - - - - - - - -
    public void InitPanel(Slider bgmSlider, Slider sfxSlider)
    {
        bgmSlider.value = 1 - _soundSetting.BgmVolume;
        sfxSlider.value = 1 - _soundSetting.SfxVolume;

        bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
    }

    private void OnBgmSliderChanged(float volume)
    {
        volume = Mathf.Clamp(volume, 0, 1);
        _soundSetting.BgmVolume = 1 - volume;
        BGM.volume = 1 - volume;
    }

    private void OnSfxSliderChanged(float volume)
    {
        volume = Mathf.Clamp(volume, 0, 1);
        _soundSetting.SfxVolume = 1 - volume;
        SFX.volume = 1 - volume;
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

    public void PlaySFX(AudioClip sfxClip)
    {
        if (sfxClip == null) return;

        SFX.PlayOneShot(sfxClip);
    }

    public void PlayButtonClick()
    {
        PlaySFX(ButtonSFX);
    }

    public void PlayCodexClick()
    {
        PlaySFX(CodexSFX);
    }
    public void PlayBreakBlockSFX()
    {
        PlaySFX(BreakBlockSFX);
    }

    public void PlayAddTurnSFX()
    {
        PlaySFX(AddTurnSFX);
    }
    public void PlayPreventDestroySFX()
    {
        PlaySFX(PreventDestroySFX);
    }
    public void PlayStageClearSFX()
    {
        PlaySFX(StageClearSFX);
    }
    public void PlayShopClick()
    {
        PlaySFX(ShopSFX);
    }
    public void PlayCombineSFX()
    {
        PlaySFX(CombineSFX);
    }
    //마법사 슬라임 효과음
    public void PlayWizardMoveNumberSFX()
    {
        PlaySFX(WizardMoveNumberSFX);
    }
    public void PlayWizardFlipSFX()
    {
        PlaySFX(WizardFlipSFX);
    }
    public void PlayWizardSwapTwoRowsSFX()
    {
        PlaySFX(WizardSwapTwoRowsSFX);
    }
    public void PlayWizardBlindSFX()
    {
        PlaySFX(WizardBlindSFX);
    }
    //아처 슬라임 효과음
    public void PlayArcherActiveSFX()
    {
        PlaySFX(ArcherActiveSFX);
    }
    public void PlayArcherBreakTileSFX()
    {
        PlaySFX(ArcherBreakTileSFX);
    }
    //바위 슬라임 효과음
    public void PlayRockBreakTileSFX()
    {
        PlaySFX(RockBreakTileSFX);
    }
    public void PlayRockCreateWallSFX()
    {
        PlaySFX(RockCreateWallSFX);
    }
    //삐에로 슬라임 효과음
    public void PlayPierrotChangeNumberSFX()
    {
        PlaySFX(PierrotChangeNumberSFX);
    }
    //병정 슬라임 효과음
    public void PlayGuardForcedSlideSFX()
    {
        PlaySFX(GuardForcedSlideSFX);
    }
    //방패 슬라임 효과음
    public void PlayShieldCreateWallSFX()
    {
        PlaySFX(ShieldCreateWallSFX);
    }
    public void PlayShieldDeleteSFX()
    {
        PlaySFX(ShieldDeleteSFX);
    }

    public void PlayLockSFX()
    {
        PlaySFX(lockSfx);
    }
    
    // 저장 
    public void SaveSetting()
    {
        PlayerPrefs.SetFloat("bgm", _soundSetting.BgmVolume);
        PlayerPrefs.SetFloat("sfx", _soundSetting.SfxVolume);
    }

    private SoundSetting LoadSetting()
    {
        return new SoundSetting(PlayerPrefs.GetFloat("bgm", .5f),  PlayerPrefs.GetFloat("sfx", .5f));
    }
}
