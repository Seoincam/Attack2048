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
    [SerializeField] private GameObject _wallPrefab; // 벽
    [SerializeField] private GameObject _petrifyPrefab; // 석화
    [SerializeField] private GameObject _imprisonPrefab; // 감금
    [SerializeField] private GameObject _translocatePrefab; // 이동



    // - - - - - - - - - - - - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - - - - - - - - - - - - -

    // 이벤트 매니저에 각 메서드를 구독.
    void Awake() { 
        EventManager.Subscribe(GameEvent.Delete, Delete);
        EventManager.Subscribe(GameEvent.Wall, Wall);
        EventManager.Subscribe(GameEvent.Petrify, Petrify);
        EventManager.Subscribe(GameEvent.Imprison, Imprison);
        EventManager.Subscribe(GameEvent.Translocate, Translocate);
    }



    // - - - - - - - - - - - - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - - - - - - - - - - - - -

    // 삭제
    private void Delete() {
        Debug.Log("[Slime Action Manager] 삭제");
        // TODO: 삭제할 타일 위치 설정
        // TODO: 삭제 및 GameManager에 알리기.
    }

    // 벽
    private void Wall() {
        Debug.Log("[Slime Action Manager] 벽");
        Wall wall = Instantiate(_wallPrefab).GetComponent<Wall>();
        // TODO: 위치 설정
        // TODO: GameManager에 알려야함.
    }

    // 석화
    private void Petrify() {
        Debug.Log("[Slime Action Manager] 석화");
        Petrify petrify = Instantiate(_petrifyPrefab).GetComponent<Petrify>();
        // TODO: 위치 설정
        // TODO: GameManager에 알려야함.
    }

    // 감금
    private void Imprison() {
        Debug.Log("[Slime Action Manager] 감금");
        Imprision imprision = Instantiate(_imprisonPrefab).GetComponent<Imprision>();
        // TODO: 위치 설정
        // TODO: GameManager에 알려야함.
    }

    // 이동
    private void Translocate() {
        Debug.Log("[Slime Action Manager] 이동");
        Translocate translocate = Instantiate(_translocatePrefab).GetComponent<Translocate>();
        // TODO: 위치 설정
        // TODO: GameManager에 알려야함.
    }
}
