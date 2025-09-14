using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

// 연출 전담 클래스
public class Tile : MonoBehaviour
{
    public int value;
    public int x, y; // 현재 좌표
    [SerializeField]
    private float MoveDuration = 0.2f; // 타일 움직임시 걸리는 시간
    // 이동이 끝났나 체크
    public bool IsMoving { get; private set; }

    // 보호되나?
    public bool IsProtected { get; private set; }
    [SerializeField] GameObject protectText;

    private Tween moveTween;

    //렌더 순서 관리
    private SpriteRenderer _sr;
    private int _baseOrder;
    private const int OrderOffset = 1000; // 이동 중 위로 올라가게 할 오프셋

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }
    // 좌표 기반으로 기본 order 계산
    // 겹칠경우 프레임마다 랜덤으로 렌더 순서 겹치는경우 방지용
    public static int ComputeBaseOrder(int x, int y) => (y * 10) + x;

    public void RecomputeBaseOrder()
    {
        _baseOrder = ComputeBaseOrder(x, y);
        if (_sr != null) _sr.sortingOrder = _baseOrder;
    }
    // 이동 시작/ 끝 order 제어 위함
    private void BeginMoveVisual()
    {
        if (_sr != null) _sr.sortingOrder = _baseOrder + OrderOffset;
    }

    private void EndMoveVisual()
    {
        RecomputeBaseOrder();
    }
    public void Init(int x, int y)
    {
        IsMoving = false;
        this.x = x; this.y = y;
        RecomputeBaseOrder();
    }
    /* 타일을 주어진 좌표로 이동시킴
     * 이동중 : IsMoving = true -> 이동 완료 후 좌표 최신위치로 갱신 -> IsMoving = false
     * 이동 끝나면 onArrived -> 게임매니저에 알림
     * moveTween?.Kill() -> 이전 이동 강제 중단 후 새 트윈 시작 */
    public void TweenMoveTo(int x2, int y2, Action onArrived = null)
    {
        IsMoving = true;
        var targetPos = GameManager.Instance.LocateTile(x2, y2);

        // 트윈 시작 시 Order 최상단으로
        BeginMoveVisual();

        moveTween?.Kill();
        moveTween = transform.DOMove(targetPos, MoveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            x = x2; y = y2;
            IsMoving = false;

            // 트윈 끝나면 Order 원래대로
            EndMoveVisual();
            onArrived?.Invoke();
        });
    }
    // 반환될때 트윈 강제 종료
    private void OnDisable()
    {
        moveTween?.Kill();
        IsMoving = false;

        // 비활성화 되기 직전에도 원래대로
        if(_sr != null) _sr.sortingOrder = _baseOrder;

        if (IsProtected)
        {
            if (protectText != null) protectText.SetActive(false);
            IsProtected = false;
            EventManager.Unsubscribe(GamePhase.NewTurnPhase, FinishProtect);
        }
    }
    public void StartProtect()
    {
        EventManager.Subscribe(GamePhase.NewTurnPhase, FinishProtect);
        protectText.SetActive(true);
        IsProtected = true;
    }

    private void FinishProtect()
    {
        protectText.SetActive(false);
        IsProtected = false;

        if (gameObject.activeSelf)
            StartCoroutine(Unsubscribe());
    }

    private IEnumerator Unsubscribe()
    {
        yield return new WaitForSeconds(0.05f);
        EventManager.Unsubscribe(GamePhase.NewTurnPhase, FinishProtect);
    }
}
