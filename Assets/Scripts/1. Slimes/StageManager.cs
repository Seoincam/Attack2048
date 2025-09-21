using System;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private Main main;

    public event EventHandler<SlimeInfo> OnSlimeChanged;
    public event Action<Transform> OnGameClear;
    public event Action OnGameFail;

    private SlimeBase _currentSlime;
    public int MaxTurn { get => _currentSlime.DefaltTurns; }

    [Space, Header("Setting")]
    [SerializeField, Tooltip("슬라임들의 순서를 결정")]
    private GameObject[] _slimes;
    public SlimeBase CurrentSlime => _currentSlime; // 외부 참조용 현재 슬라임
    public int StageIndex { get; private set; }

    public void Init(Main main)
    {
        this.main = main;
    }

    public void GameClear(Transform clearTile)
    {
        main.Game.IsPaused = true;
        OnGameClear?.Invoke(clearTile);
    }

    public bool StageManagerAlive()
    {
        return _currentSlime != null && _currentSlime.gameObject.activeSelf;
    }

    public void GameFail()
    {
        OnGameFail?.Invoke();
    }

    public void SpawnSlime(int index, bool isRetry = false)
    {
        if (_currentSlime != null)
            _currentSlime.Die(isRetry);

        _currentSlime = Instantiate(_slimes[index]).GetComponent<SlimeBase>();
        _currentSlime.Init(this);

        main.Game.SetTurn(_currentSlime.DefaltTurns);
        main.Game.ClearValue = _currentSlime.ClearValue;
        
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
