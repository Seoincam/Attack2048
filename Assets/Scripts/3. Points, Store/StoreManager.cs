// - - - - - - - - - - - - - - - - - -
// StoreManager.cs
//  - 상점 관리 클래스.
// - - - - - - - - - - - - - - - - - -

using System;
using System.Collections;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    // 필드
    // - - - - - - - - - -
    [SerializeField, Tooltip("파괴 방지 비용")] private int preventDestroyCost;
    [SerializeField, Tooltip("타일 파괴 비용")] private int destoryTileCost;
    [SerializeField, Tooltip("턴 추가 비용")] private int addTurnCost;
    [SerializeField, Tooltip("턴 추가량")] private int addTurnAmount;

    public int PreventDestroyCost { get => preventDestroyCost; }
    public int DestroyTileCost { get => destoryTileCost; }
    public int AddTurnCost { get => addTurnCost; }

    public event EventHandler<ClickInfo> OnClickButton;
    private PointManager _pointManager;

    private bool _isPreventing;
    private bool _isDestroying;

    private bool IsPreventing
    {
        get => _isPreventing;
        set
        {
            _isPreventing = value;
            _isDestroying = false; //오류 방지

            // 터치 오류 방지
            if (value)
                GameManager.Instance.IsPaused = true;
            else
                StartCoroutine(FinishPause());
        }
    }

    private bool IsDestroying
    {
        get => _isDestroying;
        set
        {
            _isDestroying = value;
            _isPreventing = false; //오류 방지

            // 터치 오류 방지
            if (value)
                GameManager.Instance.IsPaused = true;
            else
                StartCoroutine(FinishPause());
        }
    }

    private bool IsSelecting
    {
        get => IsDestroying || IsPreventing;
    }


    public void Init(PointManager pointManager)
    {
        _pointManager = pointManager;
    }



    // Unity 콜백
    // - - - - - - - - - -
    void Update()
    {
        if (!IsSelecting)
            return;

        if (Input.GetMouseButtonDown(0) ||
            (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector2 touchPos = Input.GetMouseButtonDown(0) ? Input.mousePosition : Input.GetTouch(0).position;
            touchPos = Camera.main.ScreenToWorldPoint(touchPos);

            var hit = Physics2D.Raycast(touchPos, Vector2.zero);

            if (hit.collider != null)
            {
                if (IsPreventing)
                    PreventDestroy(hit.collider);

                else if (IsDestroying)
                    DestroyTile(hit.collider);
            }

            // 빈곳 누르면 취소
            else
            {
                if (IsPreventing)
                    IsPreventing = false;

                else if (IsDestroying)
                    IsDestroying = false;
            }

            StartCoroutine(FinishSelect());
        }
    }



    // 버튼 로직
    // - - - - - - - - - -
    public void PreventDestroyBtn()
    {
        if (!GameManager.Instance.CanGetInput)
            return;

        if (!_pointManager.CheckPoint(PreventDestroyCost))
            return;

        OnClickButton?.Invoke(this, new ClickInfo(isSelecting: true));
        IsPreventing = true;
    }

    public void AddTurnBtn()
    {
        if (!GameManager.Instance.CanGetInput)
            return;

        if (!_pointManager.CheckPoint(AddTurnCost))
            return;

        _pointManager.UsePoint(AddTurnCost);
        GameManager.Instance.AddTurns(addTurnAmount);
        // SoundManager.Instance.PlayAddTurnSFX();
    }

    public void DestoryTileBtn()
    {
        if (!GameManager.Instance.CanGetInput)
            return;

        if (!_pointManager.CheckPoint(DestroyTileCost))
            return;

        OnClickButton?.Invoke(this, new ClickInfo(isSelecting: true));
        IsDestroying = true;
    }



    // 실행 로직
    // - - - - - - - - - -
    private void PreventDestroy(Collider2D selectedTile)
    {
        int x = selectedTile.GetComponent<Tile>().x;
        int y = selectedTile.GetComponent<Tile>().y;

        if (GameManager.Instance.TileArray[x, y].GetComponent<Tile>().IsProtected)
            Debug.Log("이미 보호됨");

        else
        {
            _pointManager.UsePoint(DestroyTileCost);
            IsPreventing = false;
            GameManager.Instance.TileArray[x, y].GetComponent<Tile>().StartProtect();
            // SoundManager.Instance.PlayPreventDestroySFX();
        }
    }

    private void DestroyTile(Collider2D selectedTile)
    {
        int x = selectedTile.GetComponent<Tile>().x;
        int y = selectedTile.GetComponent<Tile>().y;

        if (GameManager.Instance.ObstacleArray[x, y].HasImprison())
            Debug.Log("감금 중엔 삭제 불가");

        // 오류 방지
        else if (GameManager.Instance.DeleteTile(x, y))
        {
            _pointManager.UsePoint(DestroyTileCost);
            IsDestroying = false;
            // SoundManager.Instance.PlayBreakBlockSFX();
        }
    }

    private IEnumerator FinishPause()
    {
        yield return new WaitForSeconds(0.15f);
        GameManager.Instance.IsPaused = false;
    }

    private IEnumerator FinishSelect()
    {
        yield return new WaitForSeconds(0.1f);
        OnClickButton?.Invoke(this, new ClickInfo(isSelecting: false));
    }


    public class ClickInfo : EventArgs
    {
        public bool isSelecting;

        public ClickInfo(bool isSelecting)
        {
            this.isSelecting = isSelecting;
        }
    }
}