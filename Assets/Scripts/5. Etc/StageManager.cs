 using TMPro;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform slimeCanvas;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI clearValueText;

    [SerializeField] private GameObject NextStagePanel;

    
    private SlimeBase _currentSlime;

    [Space, Header("Setting")]
    [SerializeField, Tooltip("슬라임들의 순서를 결정")]
    private GameObject[] _slimes;

    [SerializeField, Tooltip("(테스트용) 시작 스테이지 / 실제 스테이지 숫자 - 1로 기입")]
    private int _stageIndex = 0;

    void Start()
    {
        _currentSlime = Instantiate(_slimes[_stageIndex]).GetComponent<SlimeBase>();
        _currentSlime.Init(this);
        GameManager.Instance.CurTurns = _currentSlime.DefaltTurns;
        stageText.text = $"Stage {_stageIndex + 1}";
    }

    public void OnGameClear()
    {
        NextStagePanel.SetActive(true);
        GameManager.Instance.IsPaused = true;
    }

    public void NextStageButton()
    {
        _currentSlime.Die();
        _stageIndex++;

        if (_stageIndex < _slimes.Length)
        {
            _currentSlime = Instantiate(_slimes[_stageIndex]).GetComponent<SlimeBase>();
            _currentSlime.Init(this);
            GameManager.Instance.CurTurns = _currentSlime.DefaltTurns;

            stageText.text = $"Stage {_stageIndex + 1}";
            NextStagePanel.SetActive(false);

            // 슬라임 액션 비활성화
            foreach (Transform action in ObjectPoolManager.instance.SlimeActionGroup)
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
}
