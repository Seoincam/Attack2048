using System;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    // public static StageManager Instance { get; private set; }

    public event EventHandler OnSlimeChanged;
    private SlimeBase _currentSlime;

    [Header("UI")]
    [SerializeField] private Transform slimeCanvas;
    [SerializeField] private GameObject NextStagePanel;

    [Space, Header("Setting")]
    [SerializeField, Tooltip("슬라임들의 순서를 결정")]
    private GameObject[] _slimes;

    [SerializeField, Tooltip("(테스트용) 시작 스테이지 / 실제 스테이지 숫자 - 1로 기입")]
    private int _stageIndex = 0;

    // void Awake()
    // {
    //     if (Instance == null)
    //         Instance = this;
    //     else if (Instance != this)
    //         Destroy(gameObject);
    // }

    public void Init()
    {
        SpawnSlime(_stageIndex);
    }

    public void OnGameClear()
    {
        NextStagePanel.SetActive(true);
        GameManager.Instance.IsPaused = true;
    }

    private void SpawnSlime(int index)
    {
        if (_currentSlime != null) _currentSlime.Die();

        _currentSlime = Instantiate(_slimes[index]).GetComponent<SlimeBase>();
        _currentSlime.Init(this);

        GameManager.Instance.SetTurn(_currentSlime.DefaltTurns);
        GameManager.Instance.ClearValue = _currentSlime.ClearValue;
        
        OnSlimeChanged?.Invoke(this,
            new SlimeInfo(clearValue: _currentSlime.ClearValue, stageIndex: index + 1));
    }

    public void NextStageButton()
    {
        _stageIndex++;

        if (_stageIndex < _slimes.Length)
        {
            SpawnSlime(_stageIndex);
            NextStagePanel.SetActive(false);

            // 슬라임 액션 비활성화
            foreach (Transform action in ObjectPoolManager.Instance.SlimeActionGroup)
            {
                if (!action.gameObject.activeSelf) continue;
                SlimeActionBase slimeAction = action.GetComponent<SlimeActionBase>();
                slimeAction.StartCoroutine(slimeAction.DestroySelf());
            }

            // 타일 초기화
            GameManager.Instance.ResetTileArray();
            // 장애물 배열 초기화
            GameManager.Instance.ResetObstacleArray();

            GameManager.Instance.IsPaused = false;
        }

        else
        {
            Debug.Log("클리어!");
        }
    }
    
    public class SlimeInfo : EventArgs
    {
        public int clearValue;
        public int stageIndex;
        
        public SlimeInfo(int clearValue, int stageIndex)
        {
            this.clearValue = clearValue;
            this.stageIndex = stageIndex;
        }
    }
}
