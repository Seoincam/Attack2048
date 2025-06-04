// - - - - - - - - - - - - - - - - - -
// StoreManager.cs
//  - 상점 관리 클래스.
// - - - - - - - - - - - - - - - - - -

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
            darkBackground.SetActive(value);

            // 터치 오류 방지
            if(value) { GameManager.Instance.IsPaused = true; }
            else { StartCoroutine(FinishPause()); }
        }
    }

    private bool _isDestroying;
    private bool IsDestroying {
        set {
            _isDestroying = value;
            _isPreventing = false; //오류 방지
            darkBackground.SetActive(value);

            // 터치 오류 방지
            if(value) { GameManager.Instance.IsPaused = true; }
            else { StartCoroutine(FinishPause()); }
        }
    }

    private bool CanSelect {
        get => _isDestroying || _isPreventing; 
    }

    [SerializeField, Tooltip( "파괴 방지 비용" )] private int PreventDestroyCost;
    [SerializeField, Tooltip( "타일 파괴 비용" )] private int DestoryTileCost;
    [SerializeField, Tooltip( "턴 추가 비용" )] private int AddTurnCost;
    [SerializeField, Tooltip( "턴 추가량" )] private int AddTurnAmount;



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

            // 다른데 누르면 취소
            else {
                if(_isPreventing) IsPreventing = false;
                else if(_isDestroying) IsDestroying = false;
            }
        }
    }

    public void PreventDestroyBtn()
    {
        if (GameManager.Instance.IsPaused) return;

        // 포인트 부족하면 return 
        if (!_pointManager.CheckPoint(PreventDestroyCost)) return;
        IsPreventing = true;
    }

    public void DestoryTileBtn()
    {
        if (GameManager.Instance.IsPaused) return;

        // 포인트 부족하면 return 
        if (!_pointManager.CheckPoint(DestoryTileCost)) return;
        IsDestroying = true;
    }

    public void AddTurnBtn()
    {
        if (GameManager.Instance.IsPaused) return;

        // 포인트 부족하면 return 
        if (!_pointManager.CheckPoint(AddTurnCost)) return;
        _pointManager.UsePoint(AddTurnCost);
        GameManager.Instance.CurTurns += AddTurnAmount;
    }



    // - - - - - - - - - -
    // 로직
    // - - - - - - - - - -
    private IEnumerator FinishPause() {
        yield return new WaitForSeconds(0.3f);
        GameManager.Instance.IsPaused = false;
    }

    private void PreventDestroy(Collider2D selectedTile)
    {
        int x = selectedTile.GetComponent<Tile>().x;
        int y = selectedTile.GetComponent<Tile>().y;

        if (GameManager.Instance.TileArray[x, y].GetComponent<Tile>().IsProtected)
            Debug.Log("이미 보호됨");

        else
        {
            _pointManager.UsePoint(DestoryTileCost);
            IsPreventing = false;
            GameManager.Instance.TileArray[x, y].GetComponent<Tile>().StartProtect();
        }
    }

    private void DestroyTile(Collider2D selectedTile) {
        int x= selectedTile.GetComponent<Tile>().x;
        int y= selectedTile.GetComponent<Tile>().y;

        if (GameManager.Instance.ObstacleArray[x, y].HasImprison())
            Debug.Log("감금 중엔 삭제 불가");
        
        // 오류 방지
        else if (GameManager.Instance.DeleteTile(x, y))
        {
            _pointManager.UsePoint(DestoryTileCost);
            IsDestroying = false;
        }
    }
}