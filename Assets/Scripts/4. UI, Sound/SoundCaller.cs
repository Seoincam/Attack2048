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
        if (Main.Instance.Sound == null)
        {
            Debug.LogWarning("sound가 null입니다.");
            return;
        }
        switch(SFXType)
        {
            case SoundType.Button:
                Main.Instance.Sound.PlayButtonClick();
                break;
            case SoundType.Codex:
                Main.Instance.Sound.PlayCodexClick();
                break;
            case SoundType.Shop:
                Main.Instance.Sound.PlayShopClick();
                break;
            default:
                Debug.LogWarning("����Ÿ���� ���� �ȵ�");
                break;
        }
    }
}

