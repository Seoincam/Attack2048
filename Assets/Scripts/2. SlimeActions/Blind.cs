// - - - - - - - - - - - - - - - - - -
// Blind.cs
//  - 블라인드 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;

public class Blind : SlimeActionBase, IShowLife, IMakeDeleteEffect
{
    // - - - - - - - - - - 
    // 필드
    [SerializeField] private Text lifeText;


    // - - - - - - - - - - 
    // 초기화
    public override void Init(int x, int y)
    {
        base.Init(x, y);
        UpdateLifeText();
    }


    // - - - - - - - - - - 
    // 로직
    public override void OnEnter_CountDownPhase()
    {
        base.OnEnter_CountDownPhase();
        UpdateLifeText();
    }

    protected override void Execute()
    {
        MakeDeleteEffect();
        base.Execute();
    }


    // - - - - - - - - - - 
    // Interfaces
    public void UpdateLifeText()
    {
        lifeText.text = _lifeCounter.ToString();
    }

    public void MakeDeleteEffect()
    {
        ParticleSystem particle = ObjectPoolManager.Instance.GetObject(27, Group.Effect).GetComponent<ParticleSystem>();
        particle.transform.position = transform.position;
        particle.Play();
    }
}
