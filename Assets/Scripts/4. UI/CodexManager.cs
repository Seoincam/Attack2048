using UnityEngine;
using UnityEngine.UI;

public class CodexManager : MonoBehaviour
{
    [SerializeField] private GameObject[] Codex;
    [SerializeField] private Text IndexText;
    private int _index;
    private int Index
    {
        get => _index;
        set
        {
            _index = value;
            IndexText.text = $"{_index + 1} / {Codex.Length}";
        }
    }

    public void OpenBtn()
    {
        if (GameManager.Instance != null) GameManager.Instance.IsPaused = true;
        gameObject.SetActive(true);
    }
    public void ExitBtn()
    {
        if (GameManager.Instance != null) GameManager.Instance.IsPaused = false;
        gameObject.SetActive(false);
    }

    void Start()
    {
        Index = 0;
    }

    public void PrevBtn()
    {
        if (Index > 0)
        {
            Codex[Index].SetActive(false);
            Codex[--Index].SetActive(true);
        }
    }
    public void NextBtn()
    {
        if (Index < Codex.Length - 1)
        {
            Codex[Index].SetActive(false);
            Codex[++Index].SetActive(true);
        }
    }
}
