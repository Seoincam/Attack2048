// - - - - - - - - - - - - - - - - - -
// ReverseMove.cs
//  - 상하좌우반전 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;
public class ReverseMove : SlimeActionBase
{
    [SerializeField] private Text lifeText;
    
    void Start()
    {
        _hasEffect = false;
    }

    public void Init()
    {
        base.Init(-1, 0);
        Vector3 position = new Vector3(2, 2, 0);
        transform.position = position;
        GameManager.Instance.IsReversed = true;
        lifeText.text = _lifeCounter.ToString();
    }
  

    public override void OnEnter_CountDownPhase()
    {
        base.OnEnter_CountDownPhase();
        lifeText.text = _lifeCounter.ToString();
    }

    protected override void Execute()
    {
        GameManager.Instance.IsReversed = false;
        base.Execute();
    }
}