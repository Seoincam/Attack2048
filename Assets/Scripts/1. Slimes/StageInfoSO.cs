using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Stage Info")]
public class StageInfoSO : ScriptableObject
{
    [SerializeField] Sprite _slimeSprite;
    [SerializeField] Sprite _stageTextSprite;
    [SerializeField] Sprite _targetTileSprite;
    [SerializeField] int _targetTile;
    [SerializeField] int _maxTurn;

    public Sprite SlimeSprite => _slimeSprite;
    public Sprite StageTextSprite => _stageTextSprite;
    public Sprite TargetTileSprite => _targetTileSprite;
    public int TargetTile => _targetTile;
    public int MaxTurn => _maxTurn;
}
