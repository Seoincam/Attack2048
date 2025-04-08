using UnityEngine;

public class GameManager : MonoBehaviour
{
    // - - - - - - - - - - - - - - - - - - - - -
    // 필드
    // - - - - - - - - - - - - - - - - - - - - -
    public static GameManager Instance;

    public GameObject[] Board; // 2, 4, 8... 프리팹 배열 (index = log2 - 1)
    public int maxTurns = 50;

    private int curTurns;
    private int gold;

    private int x, y, i, j;
    private bool wait;
    private bool move;
    private Vector3 firstPos, gap;

    private GameObject[,] Square = new GameObject[4, 4];

    public DamageInvoker damageInvoker;

    // - - - - - - - - - - - - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - - - - - - - - - - - - -
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        damageInvoker = new DamageInvoker();
    }

    void Start()
    {
        
        curTurns = maxTurns;
        Debug.Log($"게임 시작! 남은 이동 횟수: {curTurns}");

        Spawn();
        Spawn();
    }

    void Update()
    {
        GetMouseOrTouch();
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
            What();
        }
    }

        // 메서드 이름
    private void What() 
    {
        if (wait)
        {
            wait = false;

            // 방향 판별 후 MoveOrCombine 호출
            if (gap.y > 0 && Mathf.Abs(gap.x) < 0.5f) // 위
            {
                for (x = 0; x < 4; x++)
                    for (y = 0; y < 3; y++)
                        for (i = 3; i > y; i--)
                            MoveOrCombine(x, i - 1, x, i);
            }
            else if (gap.y < 0 && Mathf.Abs(gap.x) < 0.5f) // 아래
            {
                for (x = 0; x < 4; x++)
                    for (y = 3; y > 0; y--)
                        for (i = 0; i < y; i++)
                            MoveOrCombine(x, i + 1, x, i);
            }
            else if (gap.x > 0 && Mathf.Abs(gap.y) < 0.5f) // 오른쪽
            {
                for (y = 0; y < 4; y++)
                    for (x = 0; x < 3; x++)
                        for (i = 3; i > x; i--)
                            MoveOrCombine(i - 1, y, i, y);
            }
            else if (gap.x < 0 && Mathf.Abs(gap.y) < 0.5f) // 왼쪽
            {
                for (y = 0; y < 4; y++)
                    for (x = 3; x > 0; x--)
                        for (i = 0; i < x; i++)
                            MoveOrCombine(i + 1, y, i, y);
            }

            // 이동이 발생했으면 처리
            if (move)
            {
                move = false;
                curTurns--;
                Debug.Log($"이동! 남은 이동 횟수: {curTurns}");

                Spawn();

                for (x = 0; x < 4; x++)
                    for (y = 0; y < 4; y++)
                        if (Square[x, y] != null)
                            Square[x, y].tag = "Untagged";

                if (curTurns <= 0)
                    {
                        Debug.Log("이동 횟수 소진! 게임 종료!");
                        ResetGame();
                    }
            }
        }
    }



    // - - - - - - - - - - - - - - - - - - - - -
    // 2048 로직
    // - - - - - - - - - - - - - - - - - - - - -
    void MoveOrCombine(int x1, int y1, int x2, int y2)
    {
        // 이동
        if (Square[x2, y2] == null && Square[x1, y1] != null)
        {
            move = true;
            Square[x1, y1].GetComponent<Board>().Move(x2, y2, false);
            Square[x2, y2] = Square[x1, y1];
            Square[x1, y1] = null;
        }
        // 병합
        if (
            Square[x1, y1] != null &&
            Square[x2, y2] != null &&
            Square[x1, y1].name == Square[x2, y2].name &&
            Square[x1, y1].tag != "Combine" &&
            Square[x2, y2].tag != "Combine"
        )
        {
            move = true;

            // 데미지 계산
            int value = Square[x2, y2].GetComponent<Board>().value;
            // gold += damage;
            damageInvoker.OnCombine(value);

            for (j = 0; j < Board.Length; j++)
            {
                if (Square[x2, y2].name == Board[j].name + "(Clone)") break;
            }

            Square[x1, y1].GetComponent<Board>().Move(x2, y2, true);
            Destroy(Square[x2, y2]);
            Square[x1, y1] = null;
            Square[x2, y2] = Instantiate(Board[j + 1], new Vector3(-1.63f + 1.08f * x2, -3.41f + 1.05f * y2, 0), Quaternion.identity);
            Square[x2, y2].GetComponent<Board>().value = value * 2;
            Square[x2, y2].tag = "Combine";
        }
    }

    void Spawn()
    {
        while (true)
        {
            x = Random.Range(0, 4);
            y = Random.Range(0, 4);
            if (Square[x, y] == null)
            {
                Square[x, y] = Instantiate(Board[0], new Vector3(-1.63f + 1.08f * x, -3.41f + 1.05f * y, 0), Quaternion.identity);
                Square[x, y].GetComponent<Board>().value = 2;
                break;
            }
        }
    }



    // - - - - - - - - - - - - - - - - - - - - -
    // 재시작
    // - - - - - - - - - - - - - - - - - - - - -
    void ResetGame()
    {
        Debug.Log("게임 재시작!");

        // 기존 블록 삭제
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
            {
                if (Square[x, y] != null)
                {
                    Destroy(Square[x, y]);
                    Square[x, y] = null;
                }
            }

        curTurns = maxTurns;

        Spawn();
        Spawn();

        Debug.Log($"게임 재시작! 이동 횟수: {curTurns}");
    }
}
