using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlimeManager : MonoBehaviour
{
    [SerializeField] private Transform slimeCanvas;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI stageText;

    [SerializeField] private GameObject NextStagePanel;
    [SerializeField] private Transform SlimeActionGroup;

    [SerializeField] private GameObject[] _slimes;
    [SerializeField] private int _stageIndex = 0;

    private ObjectPoolManager _pooler;


    void Awake()
    {
        _pooler = GetComponent<ObjectPoolManager>();
    }
    void Start()
    {
        SlimeBase slime = Instantiate(_slimes[_stageIndex]).GetComponent<SlimeBase>();
        slime.Init(this, hpSlider, hpText);
        stageText.text = $"Stage {_stageIndex + 1}";
    }



    public void MakeDamageText(float damage)
    {
        TextMeshProUGUI _damageText = _pooler.GetObject(28, slimeCanvas).GetComponent<TextMeshProUGUI>();
        _damageText.text = "-" + damage.ToString();
    }

    public void OnSlimeDie()
    {
        NextStagePanel.SetActive(true);
        GameManager.Instance.IsPaused = true;
    }

    public void NextStageButton()
    {
        _stageIndex++;

        if (_stageIndex < _slimes.Length)
        {
            SlimeBase slime = Instantiate(_slimes[_stageIndex]).GetComponent<SlimeBase>();
            slime.Init(this, hpSlider, hpText);
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

            // 장애물 배열에서 제거
            GameManager.Instance.ResetObstacleArray();
        }

        else
        {
            Debug.Log("클리어!");
        }
    }
}
