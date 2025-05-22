using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // - - - - - - - - - - - - - - - - - - - - -
    // 필드
    // - - - - - - - - - - - - - - - - - - - - -
    public static GameManager Instance;
    public DamageInvoker _damageInvoker;
    private PointManager _pointManager;

    public GameObject[,] TileArray = new GameObject[5, 5]; // 타일 배열
    public Obstacle[,] ObstacleArray = new Obstacle[5, 5]; // 장애물 배열 (삭제, 벽, 석화, 감금, 이동)

    private bool _isPaused; // 게임이 멈췄나? (설정, 도감, 상점 등)
    public bool IsPaused { set => _isPaused = value; }

    private int _curTurns;
    private int CurTurns
    {
        get => _curTurns;
        set
        {
            _curTurns = value;
            remainingTurnsText.text = $"Remaining Turns: {_curTurns}";
        }
    }


    private const float xStart = -2.12f; //[0,0]의 x좌표와 y좌표, 그후 증가할때마다의 좌표 차이
    private const float yStart = -3.71f;
    private const float xOffset = 1.06f;
    private const float yOffset = 1.05f; 


    [Header("Setting")]
    [SerializeField, Tooltip("최대 턴")] private int maxTurns;
    [SerializeField, Tooltip("스폰에서 4가 나올 확률")] private int probablity_4 = 15;

    [Space, Header("Object")]
    [SerializeField] private GameObject[]   TilePrefabs;      // 2, 4, 8... 타일 프리팹 배열 (index = log2 - 1)
    [SerializeField] private Transform TileGroup;

    [Space, Header("UI")]
    [SerializeField] private TextMeshProUGUI remainingTurnsText;


    private int x, y, i, j;
    private bool wait;
    private bool move;
    private Vector3 firstPos, gap;

    private HashSet<Tile> _movingTiles;
    public bool _isChecking;


    // - - - - - - - - - - - - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - - - - - - - - - - - - -
    void Awake()
    {
        // 싱글턴
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Obstacle Array 초기화
        for (int x = 0; x < 5; x++) for (int y = 0; y < 5; y++)
        {
            ObstacleArray[x, y] = new Obstacle(x,y);
        }

        _damageInvoker = new DamageInvoker();
        _pointManager = GetComponent<PointManager>();
    }

    void Start()
    {
        CurTurns = maxTurns;
        Debug.Log("게임 시작!");

        Spawn();
        Spawn();
    }

    void Update()
    {
        if (_isChecking) CheckIsMoveEnd();

        if (!_isChecking && !_isPaused) GetMouseOrTouch();
    }



    // - - - - - - - - - - - - - - - - - - - - -
    // 입력 받기
    // - - - - - - - - - - - - - - - - - - - - -
    private void GetMouseOrTouch()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            wait = true;
            firstPos = Input.GetMouseButtonDown(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position;
        }

        if (Input.GetMouseButton(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved))
        {
            gap = (Input.GetMouseButton(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position) - firstPos;

            if (gap.magnitude < 100) return;
            gap.Normalize();
            ExecuteTurn();
        }
    }


/*
    흐름 :
    1. 입력 감지                 
        (GetMouseOrTouch)
    2. 턴 실행                  
        (ExecuteTurn -> MoveOrCombine)
    3. 이동이 발생했다면, 모든 타일 이동 끝날 때까지 대기 
        (CheckIsMoveEnd)
    4. 후처리                   
        (NextPahse)
*/


    private void ExecuteTurn()
    {
        if (wait)
        {
            wait = false;
            _movingTiles = new HashSet<Tile>();

            // 방향 판별 후 MoveOrCombine 호출
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

            // 이동이 발생했으면 처리
            if (move)
            {
                move = false;
                _isChecking = true;
            }
        }
    }

    private void CheckIsMoveEnd()
    {
        // Debug.Log(_movingTiles.Count);
        if (_movingTiles.Count != 0)
        {
            foreach (Tile tile in _movingTiles)
            {
                if (tile == null) continue;
                if (tile.IsMoving) return;
            }
        }

        // 모든 타일의 움직임이 끝났다면
        _isChecking = false;
        _movingTiles.Clear();
        NextPhase();
    }

    private void NextPhase()
    {
        _damageInvoker.InvokeDamage(); // 데미지 합산 전부 끝내고 데미지 부과
        EventManager.Publish(GameEvent.NewTurn); // 새로운 턴임을 Event Manager에 알리기.

        CurTurns--;

        Spawn();

        for (x = 0; x < 5; x++)
            for (y = 0; y < 5; y++)
                if (TileArray[x, y] != null)
                    TileArray[x, y].tag = "Untagged";

        if (CurTurns <= 0)
        {
            Debug.Log("이동 횟수 소진! 게임 종료!");
            ResetGame();
        }
    }



    // - - - - - - - - - - - - - - - - - - - - -
    // 2048 로직
    // - - - - - - - - - - - - - - - - - - - - -
    void MoveOrCombine(int x1, int y1, int x2, int y2)
    {
        // 이동 가능한지 확인
        if (!ObstacleArray[x2, y2].CanMove(x1, y1)) return; // 추후 막히는 애니메이션 추가

        // 이동
        if (TileArray[x2, y2] == null && TileArray[x1, y1] != null)
        {
            move = true;

            _movingTiles.Add(TileArray[x1, y1].GetComponent<Tile>());

            TileArray[x1, y1].GetComponent<Tile>().Move(x2, y2, false);
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
            _damageInvoker.SumDamage(value);

            for (j = 0; j < TilePrefabs.Length; j++)
            {
                if (TileArray[x2, y2].name == TilePrefabs[j].name + "(Clone)") break;
            }

            TileArray[x1, y1].GetComponent<Tile>().Move(x2, y2, true);
            Destroy(TileArray[x2, y2]);
            TileArray[x1, y1] = null;
            TileArray[x2, y2] = Instantiate(TilePrefabs[j + 1], LocateTile(x2, y2), Quaternion.identity, TileGroup);
            TileArray[x2, y2].GetComponent<Tile>().value = value * 2;
            //결합시에도 위치 전달
            TileArray[x2, y2].GetComponent<Tile>().x = x2;
            TileArray[x2, y2].GetComponent<Tile>().y = y2;
            TileArray[x2, y2].tag = "Combine";

            // 32 이상의 타일을 만들면 포인트 획득
            if (TileArray[x2, y2].GetComponent<Tile>().value >= 32) { _pointManager.GetPoint(TileArray[x2, y2].GetComponent<Tile>().value); }
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
            TileArray[x, y] = Instantiate(TilePrefabs[0], LocateTile(x, y), Quaternion.identity, TileGroup);
            TileArray[x, y].GetComponent<Tile>().value = 2;
        }
        else
        {
            TileArray[x, y] = Instantiate(TilePrefabs[1], LocateTile(x, y), Quaternion.identity, TileGroup);
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
        TileArray[x, y] = Instantiate(TilePrefabs[index], LocateTile(x, y), Quaternion.identity, TileGroup);

        Tile tile = TileArray[x, y].GetComponent<Tile>();
        tile.value = value; 
        tile.Init(x, y);
    }



    // - - - - - - - - - - - - - - - - - - - - -
    // 재시작
    // - - - - - - - - - - - - - - - - - - - - -
    void ResetGame()
    {
        Debug.Log("게임 재시작!");

        // 기존 블록 삭제
        for (int x = 0; x < 5; x++)
            for (int y = 0; y < 5; y++)
            {
                if (TileArray[x, y] != null)
                {
                    Destroy(TileArray[x, y]);
                    TileArray[x, y] = null;
                }
            }

        CurTurns = maxTurns;

        Spawn();
        Spawn();

        Debug.Log($"게임 재시작! 이동 횟수: {CurTurns}");
    }



    // - - - - - - - - - - - - - - - - - - - - -
    // 타일 위치 설정
    // - - - - - - - - - - - - - - - - - - - - -
    // 파라메터 x, y: Square 배열 상의 좌표
    public Vector3 LocateTile(int x, int y)
    {
        return new Vector3(xStart + xOffset * x, yStart + yOffset * y, 0);
    }



    // - - - - - - - - - - - - - - - - - - - - -
    // 타일 있나 체크
    // - - - - - - - - - - - - - - - - - - - - -
    public bool IsTiled(int x, int y) {
        if(TileArray[x,y] != null) return true;
        return false;
    }


    // - - - - - - - - - - - - - - - - - - - - -
    // 타일 삭제
    // - - - - - - - - - - - - - - - - - - - - -
    public bool DestroyTile(int x, int y) {
        if(TileArray[x,y] != null) {
            Destroy(TileArray[x,y]);
            TileArray[x,y] = null;
            return true;
        }

        return false; 
    }
}
