using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int value;
    public int x, y; //현재 좌표
    public bool move, _combine;
    int _x2, _y2;
    Vector3 targetPos;

    // 이동이 끝났나 체크
    public bool IsMoving { get => move; }

    // 보호되나?
    public bool IsProtected { get; private set; }
    [SerializeField] GameObject protectText;

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

    // 버그 방지
    void OnDisable()
    {
        if (IsProtected)
        {
            protectText.SetActive(false);
            IsProtected = false;
            EventManager.Unsubscribe(GamePhase.NewTurnPhase, FinishProtect);
        }
    }



    void Update()
    {
        if (move)
            Move(_x2, _y2, _combine);
    }

    public void Init(int x, int y)
    {
        move = false;
        this.x = x;
        this.y = y;
    }

    public void StartMove(int x2, int y2, bool combine)
    {
        move = true;
        _x2 = x2;
        _y2 = y2;
        _combine = combine;

        //현재 좌표를 새로운 위치로 갱신
        x = x2;
        y = y2;

        targetPos = GameManager.Instance.LocateTile(x2, y2);
    }

    private void Move(int x2, int y2, bool combine)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.35f);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            move = false;
            if (combine)
            {
                _combine = false;
                gameObject.SetActive(false);
            }
        }
    }
}

