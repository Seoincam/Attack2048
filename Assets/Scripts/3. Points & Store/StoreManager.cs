// - - - - - - - - - - - - - - - - - -
// StoreManager.cs
//  - 상점 관리 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;

public class StoreManager : MonoBehaviour
{    
    // - - - - - - - - - -
    // 필드
    // - - - - - - - - - -
    [SerializeField] private GameObject darkBackground;
    private PointManager _pointManager;

    private bool _isPreventing;
    private bool IsPreventing {
        set {
            _isPreventing = value;
            _isDestroying = false; //오류 방지
            GameManager.Instance.IsPaused = value;
            darkBackground.SetActive(value);
        }
    }

    private bool _isDestroying;
    private bool IsDestroying {
        set {
            _isDestroying = value;
            _isPreventing = false; //오류 방지
            GameManager.Instance.IsPaused = value;
            darkBackground.SetActive(value);
        }
    }

    private bool CanSelect {
        get => _isDestroying || _isPreventing; 
    }

    [SerializeField, Tooltip( "파괴 방지 값" )] private int PreventDestroyCost;
    [SerializeField, Tooltip( "타일 파괴 값" )] private int DestoryTileCost;
    [SerializeField, Tooltip( "턴 추가 값" )] private int AddTurnCost;



    // - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - -
    void Awake()
    {
        _pointManager = GetComponent<PointManager>();
    }
    void Update()
    {
        if(!CanSelect) return;

        // 클릭 로직
        if( Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began) ) {
            Vector2 touchPos = Input.GetMouseButtonDown(0)? Input.mousePosition : Input.GetTouch(0).position;
            touchPos = Camera.main.ScreenToWorldPoint(touchPos);

            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

            if(hit.collider != null) { 
                if(_isPreventing) PreventDestroy(hit.collider);
                else if(_isDestroying) DestroyTile(hit.collider);
            }
        }
    }

    public void PreventDestroyBtn() {
        // 포인트 부족하면 return 
        if( !_pointManager.UsePoint(PreventDestroyCost) ) return;
        IsPreventing = true;
    }

    public void DestoryTileBtn() {
        if( !_pointManager.UsePoint(DestoryTileCost) ) return;
        IsDestroying = true;
    }

    public void AddTurnBtn() {
        if( !_pointManager.UsePoint(AddTurnCost) ) return;
    }



    // - - - - - - - - - -
    // 로직
    // - - - - - - - - - -
    private void PreventDestroy(Collider2D selectedTile) {}
    private void DestroyTile(Collider2D selectedTile) {
        selectedTile.GetComponent<Board>();
        // TODO. 삭제 구현
    }
}