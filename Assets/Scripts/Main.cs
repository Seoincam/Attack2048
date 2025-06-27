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
    public StoreManager Store { get; private set; }
    public StageManager Stage { get; private set; }

    void Awake()
    {
        // 로딩 됐나 체크
        if (ObjectPoolManager.Instance == null)
            SceneManager.LoadScene("Lobby");

        Game = Managers.GetComponent<GameManager>();
        Point = Managers.GetComponent<PointManager>();
        Store = Managers.GetComponent<StoreManager>();
        Stage = Managers.GetComponent<StageManager>();
    }

    void Start()
    {
        Game.Init();
        Store.Init(Point);
        Point.Init();
        Stage.Init();

        EventManager.InitEvents();
        var ui = UiManager.GetComponent<InGameUiMnanager>();
        ui.Init(this);
        GameManager.Instance.StartGame();
        ui.RefreshAllUi();
    }
}
