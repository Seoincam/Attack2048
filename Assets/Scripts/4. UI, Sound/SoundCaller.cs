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
    [Header("ȿ���� ����")]
    public SoundType SFXType;

    public void PlaySFX()
    {
        /*
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("����Ŵ����� �Ⱥ���");
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
                Debug.LogWarning("����Ÿ���� ���� �ȵ�");
                break;
        }
        */
    }
}

