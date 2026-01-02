using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Stage Info")]
public class StageInfoSO : ScriptableObject
{
    [SerializeField, Tooltip("1부터 시작")] int _stageIndex;
    [SerializeField] Sprite _slimeSprite;
    [SerializeField] Sprite _stageTextSprite;
    [SerializeField] Sprite _targetTileSprite;
    [SerializeField] int _targetTile;
    [SerializeField] int _maxTurn;

    public int StageIndex => _stageIndex;
    public Sprite SlimeSprite => _slimeSprite;
    public Sprite StageTextSprite => _stageTextSprite;
    public Sprite TargetTileSprite => _targetTileSprite;
    public int TargetTile => _targetTile;
    public int MaxTurn => _maxTurn;
}
