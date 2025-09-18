/*
    Main.cs
    - 2048Game 씬의 클래스들 초기화 담당
    - Awake 말고, 임의로 초기화 순서 설정 위해 사용
*/

using DG.Tweening;
using UnityEngine;

public enum GameState { Lobby, LoadingToInGame, LoadingToLobby, InGame }
public class Main : SingleTone<Main>
{
    [SerializeField] private LobbyUiManager lobbyUIManager;
    [SerializeField] private StageUIManager stageUIManager;

    [SerializeField] GameManager _game;
    [SerializeField] PointManager _point;
    [SerializeField] StoreManager _store;
    [SerializeField] StageManager _stage;

    [Space]
    [SerializeField] private SoundManager _sound;
    [SerializeField] ObjectPoolManager _pooler;

    [Header("Loading Transitions")]
    [SerializeField] private Transform loadingCircleA;
    [SerializeField] private Transform loadingCircleB;




    private GameState _state;

    public SoundManager Sound => _sound;
    public ObjectPoolManager Pooler => _pooler;

    public GameState State => _state;
    public int StageIndex { get; set; }


    public GameManager Game => _game;
    public PointManager Point => _point;
    public StoreManager Store => _store;
    public StageManager Stage => _stage;



    protected override void Awake()
    {
        base.Awake();

        _sound.Init();

        lobbyUIManager.Init(this);
        stageUIManager.Init(this);
        EventManager.InitEvents();

        _state = GameState.Lobby;
        lobbyUIManager.OnEnterLobby();
    }

    void Start()
    {
        _game.Init(_pooler);
        _store.Init(Point);
        _point.Init();
        _stage.Init();

        // ui.RefreshAllUi();
    }

    public void LoadToInGame()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(loadingCircleA.DOScale(new Vector3(1.2f, 1.2f, 1.2f), .5f)
            .SetEase(Ease.OutBack));
        sequence.Join(loadingCircleB.DOScale(new Vector3(1.7f, 1.7f, 1.7f), .7f)
            .SetEase(Ease.OutBack));

        sequence.AppendCallback(() =>
        {
            lobbyUIManager.TurnOffLobby();
            _pooler.Init();
            stageUIManager.OnEnterStage();
            _game.OnEnterStage();
        });

        sequence.AppendInterval(5.0f);

        sequence.Append(loadingCircleB.DOScale(Vector3.zero, .5f)
            .SetEase(Ease.InBack));
        sequence.Join(loadingCircleA.DOScale(Vector3.zero, .7f)
            .SetEase(Ease.InBack));

        sequence.OnComplete(() =>
        {
            _game.StartGame();
        });
    }

    public void LoadToLobby()
    {
        
    }

    public void StartLoading()
    {

        var sequence = DOTween.Sequence();

        sequence.AppendInterval(1.5f);

        _state = _state == GameState.Lobby ? GameState.LoadingToInGame : GameState.LoadingToLobby;

        if (_state == GameState.LoadingToInGame)
        {
            LoadInGame();
        }
        else if (_state == GameState.LoadingToLobby)
        {

        }

    }

    private void LoadInGame()
    {
        Pooler.Init(); 
    }
    
}
