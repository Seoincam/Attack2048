using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyUiManager : MonoBehaviour
{
    [SerializeField] private LoadingSO loadingSO;

    [SerializeField] private GameObject creditPanel;
    [SerializeField] private Transform settingPanel;
    [SerializeField] private Transform codexPanel;
    [SerializeField] private GameObject[] Codex;

    private Text _indexText;
    private int _index;


    void Awake()
    {
        SoundManager.Instance.SetPanel
        (
            settingPanel.Find("BGM/BGM Slider").GetComponent<Slider>(),
            settingPanel.Find("SFX/SFX Slider").GetComponent<Slider>()
        );

        _indexText = codexPanel.Find("Index Text").GetComponent<Text>();
    }

    void Start()
    {
        SoundManager.Instance.PlayBGM(SoundManager.Instance.LobbyBGM);
    }


    // Lobby Buttons
    // - - - - - - - - -    
    public void GameStart()
    {
        loadingSO.SceneName = "2048Game";
        SceneManager.LoadScene("Loading");
    }

    public void OpenCredit()
    {
        creditPanel.SetActive(true);
    }

    public void CloseCredit()
    {
        creditPanel.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
    
    // Setting Panel
    // - - - - - - - - -
    public void OpenSetting()
    {
        settingPanel.gameObject.SetActive(true);
    }

    public void CloseSetting()
    {
        settingPanel.gameObject.SetActive(false);
    }

    // Codex Panel
    // - - - - - - - - -
    public void OpenCodex()
    {
        codexPanel.gameObject.SetActive(true);
    }
    
    public void CloseCodex()
    {
        codexPanel.gameObject.SetActive(false);
    }

    public void PreviousButton()
    {
        if (_index > 0)
        {
            Codex[_index].SetActive(false);
            Codex[--_index].SetActive(true);
            _indexText.text = $"{_index + 1} / {Codex.Length}";
        }
    }
    public void NextButton()
    {
        if (_index < Codex.Length - 1)
        {
            Codex[_index].SetActive(false);
            Codex[++_index].SetActive(true);
            _indexText.text = $"{_index + 1} / {Codex.Length}";
        }
    }
}
