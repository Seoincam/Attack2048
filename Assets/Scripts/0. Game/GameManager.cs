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

    2. 입력 처리(방향 판정) -> 계산만 먼저               
        (GameManger.ExecuteInput -> BoardPlanner.ComputePlan
         : 타일 이동/병합 계획(MovePlan) 계산만 수행 (GameObject 건들X))

    3. 트윈으로 연출 후 타일 배열 갱신
        (GameManager.ApplyPlan -> GamaManager.OnAllMovesCompleted)

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
    private PointManager _pointManager;
    private ObjectPoolManager _pooler;

    public event Action OnRemainingTurnChanged;
    public event EventHandler<PointManager.PointGetInfo> OnGetPoint;

    public GameObject[,] TileArray = new GameObject[5, 5]; // 타일 배열
    public Obstacle[,] ObstacleArray = new Obstacle[5, 5]; // 장애물 배열 (삭제, 벽, 석화, 감금, 이동)

    [HideInInspector] public bool IsReversed = false; // 상하좌우 반전중인가?
    private bool _canGetInput = false; // 입력을 받나? (GameManager 내부에서 설정)
    public bool IsPaused { private get; set; } // 설정, 도감, 상점 등 실행중인가?
    public bool IsExecuting { private get; set; } // 슬라임 패턴 실행 중인가? 
    public bool CanGetInput { get => _canGetInput && !IsPaused && !IsExecuting; }


    private int _curTurns;

    private int pendingMoves = 0; // 현재 진행중인 트윈 수
    private readonly List<GameObject> mergeVictims = new(); // 병합으로 사라질 도착지점 타일들
    private readonly List<(int x, int y, int newVal)> mergesToApply = new(); // 완료 후 타일 교체 정보
    private bool anyMergeThisTurn = false; // 이번 턴에 병합 여부
    private int ValueToIndex(int v) => Mathf.Max(0, (int)Mathf.Log(v, 2) - 1);

    public int ClearValue { get; set; }

    private const float xStart = -1.5f; //[0,0]의 x좌표와 y좌표, 그후 증가할때마다의 좌표 차이
    private const float yStart = -1.5f;
    private const float xOffset = .75f;
    private const float yOffset = .75f;


    [Header("Setting")]
    [SerializeField, Tooltip("스폰에서 4가 나올 확률")] private int probablity_4 = 15;

    [Space, Header("Object")]
    [SerializeField] private GameObject[] TilePrefabs;      // 2, 4, 8... 타일 프리팹 배열 (index = log2 - 1)


    private int x, y, i, j;
    private bool wait;
    private Vector3 firstPos, secondPos, gap;
    [HideInInspector]
    public ForcedMovedir forcedDirection = ForcedMovedir.None; //  강제 이동 방향

    public int CurTurns
    {
        get => _curTurns;
        private set
        {
            _curTurns = value;
            OnRemainingTurnChanged?.Invoke();
        }
    }
    // 타일 개수 확인 함수
    public int CountTile()
    {
        int count = 0;

        if (TileArray == null)
        {
            return 0;
        }

        for (int x = 0; x < 5; x++)
            for (int y = 0; y < 5; y++)
                if (TileArray[x, y] != null) count++;

        return count;
    }




    public void OnEnterStage()
    {
        // 슬라임 액션 비활성화
        // foreach (Transform action in SlimeActionGroup.Instance.transform)
        // {
        //     if (!action.gameObject.activeSelf)
        //         continue;
        //     var slimeAction = action.GetComponent<SlimeActionBase>();
        //     slimeAction.Destroy();
        // }
        _pooler.ResetObstacles();

        ResetTileArray();
        ResetObstacleArray();
        _pointManager.ResetPoint();
    } 



    // - - - - - - - - - - - - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - - - - - - - - - - - - -
    public void Init(ObjectPoolManager pooler)
    {
        // 싱글턴
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _pointManager = GetComponent<PointManager>();
        _countManager = GetComponent<CountDownManager>();
        _pooler = pooler;

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

        _canGetInput = true;
        IsPaused = false;

        Spawn();
        Spawn();
    }

    void FixedUpdate()
    {
        if (CanGetInput) GetInput();
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
            stage.GameFail();
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
        if (!wait) return;
        wait = false;

        _canGetInput = false;

        //강제 방향 체크
        if (forcedDirection != ForcedMovedir.None)
        {
            bool isValid = false;
            switch (forcedDirection)
            {
                case ForcedMovedir.Up:
                    isValid = gap.y > 0 && Mathf.Abs(gap.x) < 0.5f; break;
                case ForcedMovedir.Down:
                    isValid = gap.y < 0 && Mathf.Abs(gap.x) < 0.5f; break;
                case ForcedMovedir.Left:
                    isValid = gap.x < 0 && Mathf.Abs(gap.y) < 0.5f; break;
                case ForcedMovedir.Right:
                    isValid = gap.x > 0 && Mathf.Abs(gap.y) < 0.5f; break;
            }
            if (!isValid) { _canGetInput = true; return; }
        }

        //방향 벡터 산출(반전 포함)
        Vector2Int dir = Vector2Int.zero;
        if (!IsReversed)
        {
            if (gap.y > 0 && Mathf.Abs(gap.x) < 0.5f) dir = Vector2Int.up;          // 위
            else if (gap.y < 0 && Mathf.Abs(gap.x) < 0.5f) dir = Vector2Int.down;   // 아래
            else if (gap.x > 0 && Mathf.Abs(gap.y) < 0.5f) dir = Vector2Int.right;  // 오른쪽
            else if (gap.x < 0 && Mathf.Abs(gap.y) < 0.5f) dir = Vector2Int.left;   // 왼쪽
        }
        else
        {
            if (gap.y > 0 && Mathf.Abs(gap.x) < 0.5f) dir = Vector2Int.down;        // 위 -> 아래로 반전
            else if (gap.y < 0 && Mathf.Abs(gap.x) < 0.5f) dir = Vector2Int.up;     // 위
            else if (gap.x > 0 && Mathf.Abs(gap.y) < 0.5f) dir = Vector2Int.left;   // 왼쪽
            else if (gap.x < 0 && Mathf.Abs(gap.y) < 0.5f) dir = Vector2Int.right;  // 오른쪽

        }

        if (dir == Vector2Int.zero) { _canGetInput = true; return; } // 유효하지 않은 입력

        //계산만
        var plan = BoardPlanner.ComputePlan(dir, TileArray, ObstacleArray);
        if (!plan.HasEffect) { _canGetInput = true; return; } // 이동 불가

        //연출
        ApplyPlan(plan);
    }
    /* MovePlan에 따라 실제 보드 옮기고 비주얼은 트윈으로
     * 병합 예정 타일은 victim에 기록
     * 트윈이 하나도 없으면 즉시 완료 처리
     */
    private void ApplyPlan(MovePlan plan)
    {
        if (!plan.HasEffect) return;
        pendingMoves = 0;
        anyMergeThisTurn = false;
        mergeVictims.Clear();
        mergesToApply.Clear();

        foreach (var step in plan.steps)
        {
            var mover = TileArray[step.x1, step.y1];
            if (mover == null) continue;

            //병합 대상 타일 기록
            if (step.willMerge && step.victimX >= 0 && step.victimY >= 0)
            {
                var victim = TileArray[step.victimX, step.victimY];
                if (victim != null) mergeVictims.Add(victim);
                anyMergeThisTurn = true;

                //최종 도착지에서 적용할 새 값(= mover의 값 * 2)
                int curVal = mover.GetComponent<Tile>().value;
                mergesToApply.Add((step.x2, step.y2, curVal * 2));
            }
            //실제로 좌표가 바뀌는지 체크
            bool moved = (step.x1 != step.x2) || (step.y1 != step.y2);

            if(moved)
            {
                //논리 보드 갱신, 비주얼은 트윈으로 이동
                TileArray[step.x2, step.y2] = mover;
                TileArray[step.x1, step.y1] = null;

                var tile = mover.GetComponent<Tile>();
                pendingMoves++;
                tile.TweenMoveTo(step.x2, step.y2, () =>
                {
                    pendingMoves--;
                    if (pendingMoves == 0) OnAllMovesCompleted(plan);
                });
            }
        }

        if (anyMergeThisTurn)
            // SoundManager.Instance.PlayCombineSFX();
        //트윈이 하나도 없으면 즉시 완료 처리s
        if (pendingMoves == 0)
            OnAllMovesCompleted(plan);
    }
    /* 모든 트윈이 끝난 후 실행
     * 병합, 점수 계산, 클리어 체크
     * */
    private void OnAllMovesCompleted(MovePlan plan)
    {
        //병합 처리, 도착지 타일 비활성화
        foreach (var victim in mergeVictims)
        {
            if (victim == null) continue;
            var vt = victim.GetComponent<Tile>();
            if(vt != null)
            {
                if (TileArray[vt.x, vt.y] == victim)
                    TileArray[vt.x, vt.y] = null;
            }

            victim.SetActive(false);
        }
        mergeVictims.Clear();

        //이동 시킨 타일을 상위 타일로 교체
        foreach (var (x, y, newVal) in mergesToApply)
        {
            var moverGo = TileArray[x, y];
            if (moverGo == null) continue;

            //기존 이동 타일 풀에 반납
            moverGo.SetActive(false);

            //상위 타일로 교체
            int newIndex = ValueToIndex(newVal);
            var newGo = _pooler.GetObject(newIndex, Group.Tile);
            newGo.transform.position = LocateTile(x, y);

            var newTile = newGo.GetComponent<Tile>();
            newTile.value = newVal;
            newTile.Init(x, y);

            TileArray[x, y] = newGo;

            //점수/ 클리어 계산
            if (newVal >= 4)
                OnGetPoint?.Invoke(this, new PointManager.PointGetInfo(newVal));
            if (newVal >= ClearValue)
            {
                GetComponent<StageManager>().GameClear(newTile.transform);
                EventManager.Unsubscribe(GamePhase.NewTurnPhase, OnEnter_NewTurn);
            }
        }
        mergesToApply.Clear();
        CountDownPhase();
        _canGetInput = true;
        
    }

    private void CountDownPhase()
    {
        _countManager.CountDown();
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
        var go = TileArray[x, y];
        if (go == null) return false;
        go.SetActive(false);
        TileArray[x, y] = null;
        return true;
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
