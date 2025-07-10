using System;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public event EventHandler<SlimeInfo> OnSlimeChanged;
    public event Action OnGameClear;
    public event Action OnGameFail;

    private SlimeBase _currentSlime;
    public int maxTurn { get => _currentSlime.DefaltTurns; }

    [Space, Header("Setting")]
    [SerializeField, Tooltip("슬라임들의 순서를 결정")]
    private GameObject[] _slimes;

    public int StageIndex { get; private set; }


    public void Init()
    {
        StageIndex = GameSetting.Instance.testStartIndex;
        SpawnSlime(StageIndex, isRetry: false);
    }

    public void GameClear()
    {
        GameManager.Instance.IsPaused = true;
        OnGameClear?.Invoke();
    }

    public bool StageManagerAlive()
    {
        return _currentSlime != null && _currentSlime.gameObject.activeSelf;
    }

    public void GameFail()
    {
        OnGameFail?.Invoke();
    }

    private void SpawnSlime(int index, bool isRetry)
    {
        if (_currentSlime != null)
            _currentSlime.Die(isRetry);

        _currentSlime = Instantiate(_slimes[index]).GetComponent<SlimeBase>();
        _currentSlime.Init(this);

        GameManager.Instance.SetTurn(_currentSlime.DefaltTurns);
        GameManager.Instance.ClearValue = _currentSlime.ClearValue;
        
        OnSlimeChanged?.Invoke(this,
            new SlimeInfo(clearValue: _currentSlime.ClearValue, stageIndex: index));
    }

    public bool ChangeStage(int stageIndex, bool isRetry)
    {
        if (stageIndex < _slimes.Length)
        {
            StageIndex = stageIndex;
            SpawnSlime(stageIndex, isRetry);
            return true;
        }

        return false;
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
