using UnityEngine;

public abstract class SlimeBase : MonoBehaviour, INewTurnListener
{
    // - - - - - - - - - 
    // 필드
    // - - - - - - - - -     
    private StageManager _slimeManager;

    [Header("Setting")]
    public int DefaltTurns;
    public int ClearValue = 128;

    [Header("Sound")]
    [SerializeField] private AudioClip slimeBgm;


    // - - - - - - - - 
    // Unity 콜백
    // - - - - - - - - 
    protected virtual void Start()
    {
        Subscribe_NewTurn();
        if (Main.Instance.Sound != null)
            Main.Instance.Sound.PlayBGM(slimeBgm);
    }

    public void Init(StageManager slimeManager)
    {
        _slimeManager = slimeManager;
    }



    // - - - - - - - - - - 
    // 사망 로직
    // - - - - - - - - - - 
    public void Die(bool isRetry) {
        // if (!isRetry) _slimeManager.GameClear(null);
        EventManager.Unsubscribe(GamePhase.NewTurnPhase, OnEnter_NewTurn);
        gameObject.SetActive(false);
    }

    // - - - - - - - - - - 
    // 패배 로직
    // - - - - - - - - - - 
    public void Fail() {
        EventManager.Unsubscribe(GamePhase.NewTurnPhase, OnEnter_NewTurn);
        _slimeManager.GameClear(null);
        gameObject.SetActive(false);
    }


    // - - - - - - - - - - - 
    // INewTurnListener
    // - - - - - - - - - - -
    public void Subscribe_NewTurn() {
        EventManager.Subscribe(GamePhase.NewTurnPhase, OnEnter_NewTurn);
    }

    public abstract void OnEnter_NewTurn(); // 각 자식 슬라임 클래스에서 해당 메서드 안에 매턴 마다 발생하는 항목 작성
}
