using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/*
    흐름 :

    0. 새 턴
        (GameManager.OnEnter_NewTurn)

    1. 입력 감지                 
        (GameManger.GetMouseOrTouch)

    2. 입력 처리               
        (GameManger.ExecuteInput -> GameManger.MoveOrCombine)

    3. 이동이 발생했다면, 모든 타일 이동 끝날 때까지 대기 
        (GameManger.CheckIsMoveEnd)

    4. 카운트 다운 페이즈
        (CountDownManager)
        e.g. 경고 이펙트 숫자 감소 등  

    5. 실행 페이즈
        (CountDownManager)
        e.g. 0이 되면 실행시킴             
*/

public enum ForcedMovedir { None, Up, Down, Left, Right } // 강제 이동 방향 

public class GameManager : MonoBehaviour, INewTurnListener
{
    // - - - - - - - - - - - - - - - - - - - - -
    // 필드
    // - - - - - - - - - - - - - - - - - - - - -
    public static GameManager Instance;

    private CountDownManager _countManager;
    private ObjectPoolManager _pooler;

    public event Action OnRemainingTurnChanged;
    public event EventHandler OnGetPoint;

    public GameObject[,] TileArray = new GameObject[5, 5]; // 타일 배열
    public Obstacle[,] ObstacleArray = new Obstacle[5, 5]; // 장애물 배열 (삭제, 벽, 석화, 감금, 이동)

    private bool _canGetInput = false; // 입력을 받나? (GameManager 내부에서 설정)
    public bool IsPaused { get; set; } // 멈췄나? (설정, 도감, 상점, 후처리 등 실행중인가?)
    public bool CanMove { get => _canGetInput && !IsPaused; }
    [HideInInspector] public bool IsReversed = false; // 상하좌우 반전중인가?

    private int _curTurns;

    public int ClearValue { get; set; }

    private const float xStart = -2.12f; //[0,0]의 x좌표와 y좌표, 그후 증가할때마다의 좌표 차이
    private const float yStart = -3.71f;
    private const float xOffset = 1.06f;
    private const float yOffset = 1.05f;


    [Header("Setting")]
    [SerializeField, Tooltip("스폰에서 4가 나올 확률")] private int probablity_4 = 15;

    [Space, Header("Object")]
    [SerializeField] private GameObject[] TilePrefabs;      // 2, 4, 8... 타일 프리팹 배열 (index = log2 - 1)


    private int x, y, i, j;
    private bool wait, move;
    private Vector3 firstPos, secondPos, gap;
    [HideInInspector]
    public ForcedMovedir forcedDirection = ForcedMovedir.None; //  강제 이동 방향

    private HashSet<Tile> _movingTiles; // 매턴마다 이동하는 타일 저장
    private bool _isChecking; // 타일 이동 체크 중인가?

    public int CurTurns
    {
        get => _curTurns;
        private set
        {
            _curTurns = value;
            OnRemainingTurnChanged?.Invoke();
        }
    }

    // - - - - - - - - - - - - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - - - - - - - - - - - - -
    public void Init()
    {
        // 싱글턴
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // _pointManager = GetComponent<PointManager>();
        _countManager = GetComponent<CountDownManager>();
        _pooler = ObjectPoolManager.Instance;

        // Obstacle Array 초기화
        for (int x = 0; x < 5; x++) for (int y = 0; y < 5; y++)
            {
                ObstacleArray[x, y] = new Obstacle(x, y);
            }
    }

    public void StartGame()
    {
        Subscribe_NewTurn();

        Debug.Log("게임 시작!");

        Spawn();
        Spawn();
        _canGetInput = true;
    }

    void FixedUpdate()
    {
        if (_isChecking) CheckIsMoveEnd();

        if (!_isChecking && CanMove) GetInput();
    }


    public void Subscribe_NewTurn()
    {
        EventManager.Subscribe(GamePhase.NewTurnPhase, OnEnter_NewTurn);
    }

    // - - - - - - - - - - - - - - - - - - - - -
    // 로직
    // - - - - - - - - - - - - - - - - - - - - -
    public void OnEnter_NewTurn()
    {
        CurTurns--;
        var stage = GetComponent<StageManager>();

        if (!CheckCanMove() || (CurTurns <= 0 && stage.StageManagerAlive()))
        {
            stage.OnGameFail();
            return;
        }

        Spawn();

        for (x = 0; x < 5; x++)
            for (y = 0; y < 5; y++)
                if (TileArray[x, y] != null)
                    TileArray[x, y].tag = "Untagged";

        _canGetInput = true;
    }
    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    // 해당 방향으로 이동이 가능한지 체크 
    public bool CheckCanMove(Vector3 dir)
    {
        if (dir.y > 0 && Mathf.Abs(dir.x) < 0.5f) // 위
        {
            for (x = 0; x < 5; x++)
                for (y = 0; y < 4; y++)
                    for (i = 4; i > y; i--)
                        if (CheckMoveOrCombine(x, i - 1, x, i)) return true;
        }
        else if (dir.y < 0 && Mathf.Abs(dir.x) < 0.5f) // 아래
        {
            for (x = 0; x < 5; x++)
                for (y = 4; y > 0; y--)
                    for (i = 0; i < y; i++)
                        if (CheckMoveOrCombine(x, i + 1, x, i)) return true;
        }
        else if (dir.x > 0 && Mathf.Abs(dir.y) < 0.5f) // 오른쪽
        {
            for (y = 0; y < 5; y++)
                for (x = 0; x < 4; x++)
                    for (i = 4; i > x; i--)
                        if (CheckMoveOrCombine(i - 1, y, i, y)) return true;
        }
        else if (dir.x < 0 && Mathf.Abs(dir.y) < 0.5f) // 왼쪽
        {
            for (y = 0; y < 5; y++)
                for (x = 4; x > 0; x--)
                    for (i = 0; i < x; i++)
                        if (CheckMoveOrCombine(i + 1, y, i, y)) return true;
        }
        return false;
    }
    //상하좌우 하나라도 이동 가능한지 체크
    //전체 다 이동 불가면 False, 한 방향이라도 이동 가능하면 true
    public bool CheckCanMove()
    {
        for (x = 0; x < 5; x++)
            for (y = 0; y < 4; y++)
                for (i = 4; i > y; i--)
                    if (CheckMoveOrCombine(x, i - 1, x, i)) return true;
        for (x = 0; x < 5; x++)
            for (y = 4; y > 0; y--)
                for (i = 0; i < y; i++)
                    if (CheckMoveOrCombine(x, i + 1, x, i)) return true;
        for (y = 0; y < 5; y++)
            for (x = 0; x < 4; x++)
                for (i = 4; i > x; i--)
                    if (CheckMoveOrCombine(i - 1, y, i, y)) return true;
        for (y = 0; y < 5; y++)
            for (x = 4; x > 0; x--)
                for (i = 0; i < x; i++)
                    if (CheckMoveOrCombine(i + 1, y, i, y)) return true;
        return false;
    }
    bool CheckMoveOrCombine(int x1, int y1, int x2, int y2)
    {
        bool canMove = false;
        // 이동 가능한지 확인
        if (!ObstacleArray[x2, y2].CanMove(x1, y1)) return canMove; // 추후 막히는 애니메이션 추가
        // 해당 칸이 감금되어있는지 확인
        if (ObstacleArray[x1, y1].HasImprison()) return canMove;

        // 이동
        if (TileArray[x2, y2] == null && TileArray[x1, y1] != null)
        {
            canMove = true;
        }
        // 병합
        if (
            TileArray[x1, y1] != null &&
            TileArray[x2, y2] != null &&
            TileArray[x1, y1].name == TileArray[x2, y2].name &&
            TileArray[x1, y1].tag != "Combine" &&
            TileArray[x2, y2].tag != "Combine"
        )
        {
            canMove = true;
        }
        return canMove;
    }
    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

    private void GetInput()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            wait = true;
            firstPos = Input.GetMouseButtonDown(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position;
        }

        if (Input.GetMouseButton(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved))
        {
            secondPos = Input.GetMouseButton(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position;
            gap = secondPos - firstPos;

            if (gap.magnitude < 100) return;
            gap.Normalize();
            ExecuteInput();
        }
    }

    private void ExecuteInput()
    {
        if (wait)
        {
            wait = false;

            _movingTiles = new HashSet<Tile>();
            _canGetInput = false;
            // 강제 이동이 설정된 경우 그 방향 외에는 입력을 받지 않음
            if (forcedDirection != ForcedMovedir.None)
            {
                bool isValid = false;
                switch (forcedDirection)
                {
                    case ForcedMovedir.Up:
                        isValid = gap.y > 0 && Mathf.Abs(gap.x) < 0.5f;
                        break;
                    case ForcedMovedir.Down:
                        isValid = gap.y < 0 && Mathf.Abs(gap.x) < 0.5f;
                        break;
                    case ForcedMovedir.Left:
                        isValid = gap.x < 0 && Mathf.Abs(gap.y) < 0.5f;
                        break;
                    case ForcedMovedir.Right:
                        isValid = gap.x > 0 && Mathf.Abs(gap.y) < 0.5f;
                        break;
                }
                if (!isValid)
                {
                    Debug.Log("이동 불가! 화살표 방향으로 이동하세요");
                    _canGetInput = true;
                    return;
                }
            }

            // 방향 판별 후 MoveOrCombine 호출 , 반전 아닐 때
            if (!IsReversed)
            {
                if (gap.y > 0 && Mathf.Abs(gap.x) < 0.5f) // 위
                {
                    for (x = 0; x < 5; x++)
                        for (y = 0; y < 4; y++)
                            for (i = 4; i > y; i--)
                                MoveOrCombine(x, i - 1, x, i);
                }
                else if (gap.y < 0 && Mathf.Abs(gap.x) < 0.5f) // 아래
                {
                    for (x = 0; x < 5; x++)
                        for (y = 4; y > 0; y--)
                            for (i = 0; i < y; i++)
                                MoveOrCombine(x, i + 1, x, i);
                }
                else if (gap.x > 0 && Mathf.Abs(gap.y) < 0.5f) // 오른쪽
                {
                    for (y = 0; y < 5; y++)
                        for (x = 0; x < 4; x++)
                            for (i = 4; i > x; i--)
                                MoveOrCombine(i - 1, y, i, y);
                }
                else if (gap.x < 0 && Mathf.Abs(gap.y) < 0.5f) // 왼쪽
                {
                    for (y = 0; y < 5; y++)
                        for (x = 4; x > 0; x--)
                            for (i = 0; i < x; i++)
                                MoveOrCombine(i + 1, y, i, y);
                }
            }
            //반전일 때 
            else if (IsReversed)
            {
                if (gap.y > 0 && Mathf.Abs(gap.x) < 0.5f) // 위 -> 아래로 반전됨
                {
                    for (x = 0; x < 5; x++)
                        for (y = 4; y > 0; y--)
                            for (i = 0; i < y; i++)
                                MoveOrCombine(x, i + 1, x, i);
                }
                else if (gap.y < 0 && Mathf.Abs(gap.x) < 0.5f) // 아래 -> 위로 반전됨
                {
                    for (x = 0; x < 5; x++)
                        for (y = 0; y < 4; y++)
                            for (i = 4; i > y; i--)
                                MoveOrCombine(x, i - 1, x, i);
                }
                else if (gap.x > 0 && Mathf.Abs(gap.y) < 0.5f) // 오른쪽 -> 왼쪽으로 반전됨
                {
                    for (y = 0; y < 5; y++)
                        for (x = 4; x > 0; x--)
                            for (i = 0; i < x; i++)
                                MoveOrCombine(i + 1, y, i, y);
                }
                else if (gap.x < 0 && Mathf.Abs(gap.y) < 0.5f) // 왼쪽 -> 오른쪽으로 반전됨
                {
                    for (y = 0; y < 5; y++)
                        for (x = 0; x < 4; x++)
                            for (i = 4; i > x; i--)
                                MoveOrCombine(i - 1, y, i, y);
                }
            }
        }

        // 이동이 발생했으면 처리
        if (move)
        {
            move = false;
            _isChecking = true;
        }
        else
        {
            _canGetInput = true;
            gap = firstPos;
        }
    }

    private void CheckIsMoveEnd()
    {
        if (_movingTiles.Count != 0)
        {
            foreach (Tile tile in _movingTiles)
            {
                if (tile == null) continue;
                if (!tile.gameObject.activeSelf) continue;
                if (tile.IsMoving) return;
            }
        }

        // 모든 타일의 움직임이 끝났다면
        _isChecking = false;
        _movingTiles.Clear();
        CountDownPhase();
    }

    private void CountDownPhase()
    {
        _countManager.CountDown();
    }







    // - - - - - - - - - - - - - - - - - - - - -
    // 2048 로직
    // - - - - - - - - - - - - - - - - - - - - -
    void MoveOrCombine(int x1, int y1, int x2, int y2)
    {
        // 이동 가능한지 확인
        if (!ObstacleArray[x2, y2].CanMove(x1, y1)) return; // 추후 막히는 애니메이션 추가
        // 해당 칸이 감금되어있는지 확인
        if (ObstacleArray[x1, y1].HasImprison()) return;

        // 이동
        if (TileArray[x2, y2] == null && TileArray[x1, y1] != null)
        {
            move = true;

            _movingTiles.Add(TileArray[x1, y1].GetComponent<Tile>());

            TileArray[x1, y1].GetComponent<Tile>().StartMove(x2, y2, false);
            TileArray[x2, y2] = TileArray[x1, y1];
            TileArray[x1, y1] = null;
        }
        // 병합
        if (
            TileArray[x1, y1] != null &&
            TileArray[x2, y2] != null &&
            TileArray[x1, y1].name == TileArray[x2, y2].name &&
            TileArray[x1, y1].tag != "Combine" &&
            TileArray[x2, y2].tag != "Combine"
        )
        {
            move = true;

            // 데미지 계산
            int value = TileArray[x2, y2].GetComponent<Tile>().value;

            for (j = 0; j < TilePrefabs.Length; j++)
            {
                if (TileArray[x2, y2].name == TilePrefabs[j].name + "(Clone)") break;
            }

            TileArray[x1, y1].GetComponent<Tile>().StartMove(x2, y2, true);
            DeleteTile(x2, y2);
            TileArray[x1, y1] = null;

            TileArray[x2, y2] = _pooler.GetObject(j + 1, Group.Tile);
            TileArray[x2, y2].transform.position = LocateTile(x2, y2);

            TileArray[x2, y2].GetComponent<Tile>().value = value * 2;
            //결합시에도 위치 전달
            TileArray[x2, y2].GetComponent<Tile>().x = x2;
            TileArray[x2, y2].GetComponent<Tile>().y = y2;
            TileArray[x2, y2].tag = "Combine";

            // 포인트 획득
            if (TileArray[x2, y2].GetComponent<Tile>().value >= 4)
                OnGetPoint?.Invoke(this, new PointManager.PointInfo(TileArray[x2, y2].GetComponent<Tile>().value));

            // 클리어
            if (TileArray[x2, y2].GetComponent<Tile>().value >= ClearValue)
                GetComponent<StageManager>().OnGameClear();
        }
    }


    // 랜덤 스폰 (2, 4)
    void Spawn()
    {
        int count = 0;
        while (true)
        {
            // 버그 방지
            count++;
            if (count > 500)
            {
                Debug.LogError("Spawn 오류!");
                break;
            }

            // 타일 랜덤 선택
            x = Random.Range(0, 5);
            y = Random.Range(0, 5);

            if (TileArray[x, y] == null && ObstacleArray[x, y].CanSpawn) break;
        }

        if (Random.Range(1, 100) > probablity_4)
        {
            TileArray[x, y] = _pooler.GetObject(0, Group.Tile);
            TileArray[x, y].transform.position = LocateTile(x, y);
            TileArray[x, y].GetComponent<Tile>().value = 2;
        }
        else
        {
            TileArray[x, y] = _pooler.GetObject(1, Group.Tile);
            TileArray[x, y].transform.position = LocateTile(x, y);
            TileArray[x, y].GetComponent<Tile>().value = 4;
        }

        //보드 생성 시 Board에 위치 전달
        TileArray[x, y].GetComponent<Tile>().Init(x, y);
    }

    // 숫자 및 위치 지정 스폰
    // value는 실제 숫자(2,4,8 ...)
    public void Spawn(int value, int x, int y)
    {
        if (TileArray[x, y] != null)
        {
            Debug.LogError("이미 타일이 있는 곳에 설치를 시도했습니다!");
            return;
        }

        int index = (int)Mathf.Log(value, 2) - 1;

        TileArray[x, y] = _pooler.GetObject(index, Group.Tile);
        TileArray[x, y].transform.position = LocateTile(x, y);

        Tile tile = TileArray[x, y].GetComponent<Tile>();
        tile.value = value;
        tile.Init(x, y);
    }



    // - - - - - - - - - - - - - - - - - - - - -
    // 재시작
    // - - - - - - - - - - - - - - - - - - - - -
    // 기존 타일 삭제, 두 개 스폰
    public void ResetTileArray()
    {
        for (int x = 0; x < 5; x++)
            for (int y = 0; y < 5; y++)
            {
                DeleteTile(x, y);
            }

        Spawn();
        Spawn();
    }

    public void ClearTileArray()
    {
        for (int x = 0; x < 5; x++)
            for (int y = 0; y < 5; y++)
            {
                DeleteTile(x, y);
            }
    }

    // 기존 장애물 삭제
    public void ResetObstacleArray()
    {
        for (int x = 0; x < 5; x++)
            for (int y = 0; y < 5; y++)
            {
                ObstacleArray[x, y].ResetObstacle();
            }
    }


    // - - - - - - - - - - - - - - - - - - - - -
    // 외부에서 접근용 메서드
    // - - - - - - - - - - - - - - - - - - - - -

    // 타일 위치 설정
    // 파라메터 x, y: Square 배열 상의 좌표
    public Vector3 LocateTile(int x, int y)
    {
        return new Vector3(xStart + xOffset * x, yStart + yOffset * y, 0);
    }

    // 타일 있나 체크
    public bool IsTiled(int x, int y)
    {
        if (TileArray[x, y] != null) return true;
        return false;
    }

    // 타일 삭제
    public bool DeleteTile(int x, int y)
    {
        if (TileArray[x, y] != null)
        {
            TileArray[x, y].SetActive(false);
            TileArray[x, y].GetComponent<Tile>().move = false;
            TileArray[x, y] = null;
            return true;
        }

        return false;
    }

    public void SetTurn(int amount)
    {
        CurTurns = amount;
    }
    public void AddTurns(int amount)
    {
        CurTurns += amount;
    }
}
