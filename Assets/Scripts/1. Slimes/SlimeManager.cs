 using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlimeManager : MonoBehaviour
{
    [SerializeField] private Transform slimeCanvas;
    [SerializeField] private TextMeshProUGUI stageText;

    [SerializeField] private GameObject NextStagePanel;
    [SerializeField] private Transform SlimeActionGroup;

    [SerializeField] private GameObject[] _slimes;
    [SerializeField] private int _stageIndex = 0;

    private ObjectPoolManager _pooler;
    private SlimeBase _currentSlime;


    void Awake()
    {
        _pooler = GetComponent<ObjectPoolManager>();
    }
    void Start()
    {
        _currentSlime = Instantiate(_slimes[_stageIndex]).GetComponent<SlimeBase>();
        _currentSlime.Init(this);
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
            SlimeBase slime = Instantiate(_slimes[_stageIndex]).GetComponent<SlimeBase>();
            slime.Init(this);
            stageText.text = $"Stage {_stageIndex + 1}";
            NextStagePanel.SetActive(false);
            GameManager.Instance.IsPaused = false;

            // 슬라임 액션 비활성화
            foreach (Transform action in SlimeActionGroup)
            {
                if (!action.gameObject.activeSelf) continue;
                SlimeActionBase slimeAction = action.GetComponent<SlimeActionBase>();
                slimeAction.StartCoroutine(slimeAction.DestroySelf());
            }

            // 타일 초기화
            GameManager.Instance.ResetTileArray();
            // 장애물 배열에서 제거
            GameManager.Instance.ResetObstacleArray();
        }

        else
        {
            Debug.Log("클리어!");
        }
    }
}
