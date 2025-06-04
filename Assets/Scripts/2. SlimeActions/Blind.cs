// - - - - - - - - - - - - - - - - - -
// Blind.cs
//  - 블라인드 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;

public class Blind : SlimeActionBase
{
    [SerializeField] private Text lifeText;

    public override void Init(int x, int y)
    {
        base.Init(x, y);

        lifeText.text = _lifeCounter.ToString();
    }

    public override void OnEnter_CountDownPhase()
    {
        base.OnEnter_CountDownPhase();
        lifeText.text = _lifeCounter.ToString();
    }
}
