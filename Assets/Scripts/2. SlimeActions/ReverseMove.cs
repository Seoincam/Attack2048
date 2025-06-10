// - - - - - - - - - - - - - - - - - -
// ReverseMove.cs
//  - 상하좌우반전 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;
public class ReverseMove : SlimeActionBase, IShowLife
{
    // 필드
    // - - - - - - - - - - 
    [SerializeField] private Text lifeText;


    // 초기화
    // - - - - - - - - - - 
    public override void Init()
    {
        base.Init();
        UpdateLifeText();

        Vector3 position = new Vector3(2, 2, 0);
        transform.position = position;
        GameManager.Instance.IsReversed = true;
    }


    // 로직
    // - - - - - - - - - - 
    public override void OnEnter_CountDownPhase()
    {
        base.OnEnter_CountDownPhase();
        UpdateLifeText();
    }

    protected override void Execute()
    {
        GameManager.Instance.IsReversed = false;
        base.Execute();
    }


    // Interfaces
    // - - - - - - - - - - 
    public void UpdateLifeText()
    {
        lifeText.text = _lifeCounter.ToString();
    }
}