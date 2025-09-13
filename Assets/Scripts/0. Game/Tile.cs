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

    public void Init(int x, int y)
    {
        IsMoving = false;
        this.x = x; this.y = y;
    }
    /* 타일을 주어진 좌표로 이동시킴
     * 이동중 : IsMoving = true -> 이동 완료 후 좌표 최신위치로 갱신 -> IsMoving = false
     * 이동 끝나면 onArrived -> 게임매니저에 알림
     * moveTween?.Kill() -> 이전 이동 강제 중단 후 새 트윈 시작 */
    public void TweenMoveTo(int x2, int y2, Action onArrived = null)
    {
        IsMoving = true;
        var targetPos = GameManager.Instance.LocateTile(x2, y2);

        moveTween?.Kill();
        moveTween = transform.DOMove(targetPos, MoveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            x = x2; y = y2;
            IsMoving = false;
            onArrived?.Invoke();
        });
    }
    // 반환될때 트윈 강제 종료
    private void OnDisable()
    {
        moveTween?.Kill();
        IsMoving = false;
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
