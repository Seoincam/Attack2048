/*
    Main.cs
    - 2048Game 씬의 클래스들 초기화 담당
    - Awake 말고, 임의로 초기화 순서 설정 위해 사용
*/

using System.Collections.Generic;
using UnityEngine;

public enum GameState { Lobby, LoadingToInGame, LoadingToLobby, InGame }
public class Main : SingleTone<Main>
{
    [SerializeField] private LobbyUiManager _lobbyUIManager;
    [SerializeField] private MStageUIManager _stageUIManager;
    [SerializeField] private TransitionManager transition;

    [Space]
    [SerializeField] GameManager _game;
    [SerializeField] PointManager _point;
    [SerializeField] StoreManager _store;
    [SerializeField] StageManager _stage;
    [SerializeField] SlimeActionManager _slimeAction;

    [Space]
    [SerializeField] private SoundManager _sound;
    [SerializeField] ObjectPoolManager _pooler;

    [Header("StageInfo")]
    [SerializeField] List<StageInfoSO> stageInfos;

    private GameState _state;
    private int _currentStageIndex;



    public LobbyUiManager LobbyUI => _lobbyUIManager;
    public MStageUIManager StageUI => _stageUIManager;
    public SlimeActionManager SlimeAction => _slimeAction;
    public ObjectPoolManager Pooler => _pooler;
    public GameManager Game => _game;

    public SoundManager Sound => _sound;

    public GameState State => _state;


    public PointManager Point => _point;
    public StoreManager Store => _store;
    public StageManager Stage => _stage;

    public int CurrentStageIndex => _currentStageIndex;
    public StageInfoSO CurrentStageInfo => stageInfos[_currentStageIndex];


    protected override void Awake()
    {
        base.Awake();

        _sound.Init();
    }

    void Start()
    {
        EventManager.InitEvents();
        _lobbyUIManager.Init(this);
        _stageUIManager.Init(this);

        _state = GameState.Lobby;
        _lobbyUIManager.OnEnterLobby(true);

        _game.Init(_pooler);
        _store.Init(Point);
        _point.Init();
        _stage.Init(this);
        transition.Init(this);
        _slimeAction.Init(this);

        // ui.RefreshAllUi();
    }

    public void LoadToStage(int stageIndex)
    {
        _currentStageIndex = stageIndex;
        transition.TransitToStage(stageInfos[stageIndex]);
    }

    public void LoadToLobby(bool isWin = true)
    {
        transition.TransitToLobby(isWin);
    }    
}
