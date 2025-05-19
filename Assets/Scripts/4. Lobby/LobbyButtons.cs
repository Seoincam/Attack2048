using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyButtons : MonoBehaviour
{
    [SerializeField] private GameObject _creditPanel;

    public void GameStart()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenCredit()
    {
        _creditPanel.SetActive(true);
    }

    public void CloseCredit()
    {
        _creditPanel.SetActive(false);
    }

    public void Exit()
    {

    }
    
}
