/*
    Main.cs
    - 2048Game 씬의 클래스들 초기화 담당
    - Awake 말고, 임의로 초기화 순서 설정 위해 사용
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    [SerializeField] private Transform Managers;
    [SerializeField] private Transform UiManager;

    public GameManager Game { get; private set; }
    public PointManager Point { get; private set; }
    public StageManager Stage { get; private set; }

    void Awake()
    {
        // 로딩 됐나 체크
        if (ObjectPoolManager.Instance == null)
            SceneManager.LoadScene("Lobby");

        Game = Managers.GetComponent<GameManager>();
        Point = Managers.GetComponent<PointManager>();
        Stage = Managers.GetComponent<StageManager>();
    }

    void Start()
    {
        UiManager.GetComponent<InGameUiMnanager>().Init(this);

        Game.Init();
        Point.Init();
        Stage.Init();

        EventManager.InitEvents();
        GameManager.Instance.StartGame();
    }
}
