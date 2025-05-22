// - - - - - - - - - - - - - - - - - -
// SlimeActionManager.cs
//  - 슬라임의 액션을 여기서 실행.
//  - 여기선 단순히 '실제 생성'만.
//  - 로직은 각 action class 내에서 수행
// - - - - - - - - - - - - - - - - - -

using Unity.Collections;
using UnityEngine;

public class SlimeActionManager : MonoBehaviour
{
    // - - - - - - - - - - - - - - - - - - - - -
    // 필드
    // - - - - - - - - - - - - - - - - - - - - -

    // 각각 프리팹으로 저장해놓고 호출될 때 생성
    [SerializeField] private GameObject _deletePrefab; // 삭제
    [SerializeField] private GameObject _wallPrefab; // 벽
    [SerializeField] private GameObject _petrifyPrefab; // 석화
    [SerializeField] private GameObject _imprisonPrefab; // 감금
    [SerializeField] private GameObject _changePrefab; // 숫자 랜덤 변경
    [SerializeField] private GameObject _translocate3Prefab; // 이동 (3 스테이지)
    [SerializeField] private GameObject _translocate7Prefab; // 이동 (7 스테이지)



    // - - - - - - - - - - - - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - - - - - - - - - - - - -

    // 이벤트 매니저에 각 메서드를 구독.
    void Awake()
    {
        EventManager.Subscribe(GameEvent.Delete, Delete);
        EventManager.Subscribe(GameEvent.Delete6, Delete6);
        EventManager.Subscribe(GameEvent.Wall, Wall);
        EventManager.Subscribe(GameEvent.Petrify, Petrify);
        EventManager.Subscribe(GameEvent.Imprison, Imprison);
        EventManager.Subscribe(GameEvent.Change, Change);
        EventManager.Subscribe(GameEvent.Translocate3, Translocate3);
        EventManager.Subscribe(GameEvent.Translocate7, Translocate7);
    }



    // - - - - - - - - - - - - - - - - - - - - -
    // 생성 로직
    // - - - - - - - - - - - - - - - - - - - - -

    // 삭제
    private void Delete()
    {
        Delete delete = Instantiate(_deletePrefab).GetComponent<Delete>();

        // 위치 설정
        Vector2Int selected = GetRandomPosition(false);
        delete.Init(selected.x, selected.y, false);
    }

    // 삭제 (6스테이지 한줄 삭제)
    private void Delete6()
    {
        // ToDo : 기존에 delete 없나 체크
        int randomLineX = Random.Range(0, 5);

        int count = 0;
        while (true)
        {
            // 버그 방지
            count++;
            if (count > 500)
            {
                Debug.LogError("Delete6 오류!");
                break;
            }

            bool canPlace = true;
            for (int y = 0; y < 5; y++)
            {
                if (!GameManager.Instance.ObstacleArray[randomLineX, y].CanObstacle)
                {
                    canPlace = false;
                }
            }

            if (canPlace) break;
            else randomLineX = Random.Range(0, 5);
        }


        for (int y = 0; y < 5; y++)
        {
            Delete delete = Instantiate(_deletePrefab).GetComponent<Delete>();
            delete.Init(randomLineX, y, true);
        }
    }

    // 벽
    private void Wall()
    {
        Wall wall = Instantiate(_wallPrefab).GetComponent<Wall>();

        // 위치 정하여 Wall.cs에서 위치 설정
        int x1 = Random.Range(1, 4);
        int y1 = Random.Range(1, 4);
        Vector2Int dir = GetRandomDirection();
        int x2 = x1 + dir.x;
        int y2 = y1 + dir.y;

        wall.Init(x1, y1, x2, y2);
    }

    // 석화
    private void Petrify()
    {
        Petrify petrify = Instantiate(_petrifyPrefab).GetComponent<Petrify>();

        // 위치 설정
        Vector2Int selected = GetRandomPosition(false);
        petrify.Init(selected.x, selected.y);
    }

    // 감금
    private void Imprison()
    {
        Debug.Log("[Slime Action Manager] 감금");
        Imprison imprision = Instantiate(_imprisonPrefab).GetComponent<Imprison>();
        // TODO: 위치 설정
        // TODO: GameManager에 알려야함.
    }

    private void Change()
    {
        Change change = Instantiate(_changePrefab).GetComponent<Change>();

        // 변경할 타일 위치 설정
        Vector2Int selected = GetRandomPosition(false);
        change.Init(selected.x, selected.y);
    }

    // 이동 (3 스테이지)
    private void Translocate3()
    {
        Translocate3 translocate = Instantiate(_translocate3Prefab).GetComponent<Translocate3>();
    }
    
    // 이동 (7 스테이지)
    private void Translocate7()
    {
        Translocate7 translocate = Instantiate(_translocate7Prefab).GetComponent<Translocate7>();
    }



    // - - - - - - - - - - - - - - - - - - - - -
    // 랜덤 위치 설정
    // - - - - - - - - - - - - - - - - - - - - -

    //ToDo: 위치 고정형 패턴에서, 그 자리에 이미 다른 장애물이 있을경우 처리

    // 5*5 보드 중 하나 랜덤 선택
    // [bool onlyTile] true: 이미 타일 있는 칸만, false: 그냥 아무 칸이나
    private Vector2Int GetRandomPosition(bool onlyTile)
    {
        GameManager G = GameManager.Instance;

        Vector2Int selected = new Vector2Int(Random.Range(0, 5), Random.Range(0, 5));

        if (onlyTile)
        {
            int count = 0;
            while (true)
            {
                // 버그 방지
                count++;
                if (count > 500)
                {
                    Debug.LogError("GetRandomPosition 오류!");
                    break;
                }

                if (G.IsTiled(selected.x, selected.y) && G.ObstacleArray[selected.x, selected.y].CanObstacle) break;

                selected = new Vector2Int(Random.Range(0, 5), Random.Range(0, 5));
            }
            return selected;
        }

        else
        {
            int count = 0;
            while (true)
            {
                // 버그 방지
                count++;
                if (count > 500)
                {
                    Debug.LogError("GetRandomPosition 오류!");
                    break;
                }

                if (G.ObstacleArray[selected.x, selected.y].CanObstacle) break;

                selected = new Vector2Int(Random.Range(0, 5), Random.Range(0, 5));
            }
        }

        return selected;
    }

    // 설치할 방향 랜덤 설정 
    private Vector2Int GetRandomDirection()
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0,1), // 위
            new Vector2Int(0,-1), // 아래
            new Vector2Int(1,0), // 오른쪽
            new Vector2Int(-1, 0) // 왼쪽
        };

        return directions[Random.Range(0, directions.Length)];
    }
}
