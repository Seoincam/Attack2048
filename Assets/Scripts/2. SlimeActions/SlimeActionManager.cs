// - - - - - - - - - - - - - - - - - -
// SlimeActionManager.cs
//  - 슬라임의 액션을 여기서 실행.
//  - 명령은 슬라임 클래스에서 하고, 여기선 단순히 '실제 생성'만.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;

public class SlimeActionManager : MonoBehaviour
{
    // - - - - - - - - - - - - - - - - - - - - -
    // 필드
    // - - - - - - - - - - - - - - - - - - - - -

    // 각각 프리팹으로 저장해놓고 호출될 때 생성
    [SerializeField] private GameObject _destroyPrefab; // 삭제
    [SerializeField] private GameObject _wallPrefab; // 벽
    [SerializeField] private GameObject _petrifyPrefab; // 석화
    [SerializeField] private GameObject _imprisonPrefab; // 감금
    [SerializeField] private GameObject _translocatePrefab; // 이동



    // - - - - - - - - - - - - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - - - - - - - - - - - - -

    // 이벤트 매니저에 각 메서드를 구독.
    void Awake()
    {
        EventManager.Subscribe(GameEvent.Delete, Delete);
        EventManager.Subscribe(GameEvent.Wall, Wall);
        EventManager.Subscribe(GameEvent.Petrify, Petrify);
        EventManager.Subscribe(GameEvent.Imprison, Imprison);
        EventManager.Subscribe(GameEvent.Translocate, Translocate);
    }



    // - - - - - - - - - - - - - - - - - - - - -
    // 생성 로직
    // - - - - - - - - - - - - - - - - - - - - -

    // 삭제
    private void Delete()
    {
        Delete destroy = Instantiate(_destroyPrefab).GetComponent<Delete>();

        // 삭제할 타일 위치 설정
        Vector2Int selected = GetRandomPosition(false);
        destroy.Init(selected.x, selected.y);
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

        // GameManager에 알려야함. -> Wall.cs에서 알림
        wall.Init(x1, y1, x2, y2);
    }

    // 석화
    private void Petrify()
    {
        Debug.Log("[Slime Action Manager] 석화");
        Petrify petrify = Instantiate(_petrifyPrefab).GetComponent<Petrify>();
        // TODO: 위치 설정
        // TODO: GameManager에 알려야함.
    }

    // 감금
    private void Imprison()
    {
        Debug.Log("[Slime Action Manager] 감금");
        Imprision imprision = Instantiate(_imprisonPrefab).GetComponent<Imprision>();
        // TODO: 위치 설정
        // TODO: GameManager에 알려야함.
    }

    // 이동
    private void Translocate()
    {
        Debug.Log("[Slime Action Manager] 이동");
        Translocate translocate = Instantiate(_translocatePrefab).GetComponent<Translocate>();
        // TODO: 위치 설정
        // TODO: GameManager에 알려야함.
    }
    


    // - - - - - - - - - - - - - - - - - - - - -
    // 랜덤 위치 설정
    // - - - - - - - - - - - - - - - - - - - - -

    // 5*5 보드 중 하나 랜덤 선택
    // true: 이미 타일 있는 칸만, false: 그냥 아무 칸이나
    private Vector2Int GetRandomPosition(bool onlyTile) {
        Vector2Int selected = new Vector2Int(Random.Range(0,5), Random.Range(0,5));

        if(onlyTile) {
            while(GameManager.Instance.CheckTile(selected.x, selected.y)) {
                selected = new Vector2Int(Random.Range(0,5), Random.Range(0,5));
            }
            return selected;
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
