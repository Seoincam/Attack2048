// - - - - - - - - - - - - - - - - - -
// SlimeActionBase.cs
//  - 벽, 석화, 감금, 이동 클래스 등의 부모 클래스.
//  - 수명이 다하면 각 로직을 실행 시킴.
// - - - - - - - - - - - - - - - - - -

using System.Collections;
using UnityEngine;

public abstract class SlimeActionBase : MonoBehaviour, ICountDownListener
{
    // 필드
    // - - - - - - - - - - 
    [Tooltip("수명"), SerializeField] protected int Life; // Inspector에서 설정
    [Tooltip("남은 수명"), SerializeField] protected int _lifeCounter;

    private bool isDestroyed;


    // 초기화
    // - - - - - - - - - - 
    public virtual void Init()
    {
        Subscribe_CountDown();
        _lifeCounter = Life;
        isDestroyed = false;
    }

    public virtual void Init(int x, int y)
    {
        Init();
        transform.position = GameManager.Instance.LocateTile(x, y);
    }


    // 로직
    // - - - - - - - - - - 

    // EventManager에 구독.
    public void Subscribe_CountDown()
    {
        EventManager.Subscribe(GamePhase.CountDownPhase, OnEnter_CountDownPhase);
    }

    // 후처리에서 실행.
    public virtual void OnEnter_CountDownPhase()
    {
        _lifeCounter--;

        if (_lifeCounter == 0)
            EventManager.Subscribe(GamePhase.ExecutePhase, Execute);
    }

    // 수명이 다하면 실행할 로직.
    protected virtual void Execute()
    {
        // 자식 클래스가 이곳에 override로 로직 추가
        StartCoroutine(DestroySelf());
    }

    public virtual IEnumerator DestroySelf()
    {
        isDestroyed = true;
        yield return new WaitForSeconds(0.05f);
        EventManager.Unsubscribe(GamePhase.CountDownPhase, OnEnter_CountDownPhase);
        EventManager.Unsubscribe(GamePhase.ExecutePhase, Execute);
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (isDestroyed)
            return;
            
        EventManager.Unsubscribe(GamePhase.CountDownPhase, OnEnter_CountDownPhase);
        EventManager.Unsubscribe(GamePhase.ExecutePhase, Execute);
        StopAllCoroutines();
    }
}
