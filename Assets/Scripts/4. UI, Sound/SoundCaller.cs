using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SoundCaller : MonoBehaviour
{
    public enum SoundType
    {
        Button,
        Codex,
        Shop,
    }
    [Header("효과음 설정")]
    public SoundType SFXType;

    public void PlaySFX()
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("사운드매니저가 안보임");
            return;
        }
        switch(SFXType)
        {
            case SoundType.Button:
                SoundManager.Instance.PlayButtonClick();
                break;
            case SoundType.Codex:
                SoundManager.Instance.PlayCodexClick();
                break;
            case SoundType.Shop:
                SoundManager.Instance.PlayShopClick();
                break;
            default:
                Debug.LogWarning("사운드타입이 설정 안됨");
                break;
        }
    }
}

