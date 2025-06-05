// - - - - - - - - - - - - - - - - - -
// SlimeActionBase.cs
//  - 벽, 석화, 감금, 이동 클래스의 부모 클래스.
//  - 수명이 다하면 각 로직을 실행 시킴.
// - - - - - - - - - - - - - - - - - -

using System.Collections;
using UnityEngine;

public abstract class SlimeActionBase : MonoBehaviour, ICountDownListener
{
    // - - - - - - - - - - - - - - - - - - - - -
    // 필드
    // - - - - - - - - - - - - - - - - - - - - -

    [Tooltip("수명")][SerializeField] protected int Life; // Inspector에서 설정
    [Tooltip("남은 수명")][SerializeField] protected int _lifeCounter;

    // 파티클을 재생하는가?
    protected bool _hasEffect = true;

    protected SpriteRenderer _renderer;


    // - - - - - - - - - - - - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - - - - - - - - - - - - -
    void Awake()
    {
        TryGetComponent(out _renderer);
    }

    // x = -1: 좌표 설정 안 함
    public virtual void Init(int x, int y)
    {
        Subscribe_CountDown();
        _lifeCounter = Life;
        
        if (x != -1)
            transform.position = GameManager.Instance.LocateTile(x, y);
    }


    // - - - - - - - - - - - - - - - - - - - - -
    // 로직
    // - - - - - - - - - - - - - - - - - - - - -

    // EventManager에 구독.
    public void Subscribe_CountDown()
    {
        EventManager.Subscribe(GameEvent.CountDownPhase, OnEnter_CountDownPhase);
    }

    // 후처리에서 실행.
    public virtual void OnEnter_CountDownPhase()
    {
        _lifeCounter--;

        if (_lifeCounter == 0)
        {
            EventManager.Subscribe(GameEvent.TriggerPhase, Execute);
        }
    }

    // 수명이 다하면 실행할 로직.
    protected virtual void Execute()
    {
        if (_hasEffect)
        {
            ParticleSystem particle = ObjectPoolManager.instance.GetObject(27, Group.Effect).GetComponent<ParticleSystem>();
            particle.transform.position = transform.position;
            particle.Play();
        }

        StartCoroutine(DestroySelf());
    }

    public IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(0.05f);
        EventManager.Unsubscribe(GameEvent.CountDownPhase, OnEnter_CountDownPhase);
        EventManager.Unsubscribe(GameEvent.TriggerPhase, Execute);
        gameObject.SetActive(false);
    }
}
