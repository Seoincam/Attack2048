// - - - - - - - - - - - - - - - - - -
// Blind.cs
//  - 블라인드 클래스.
// - - - - - - - - - - - - - - - - - -

using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Blind : SlimeActionBase, IShowLife, IMakeDeleteEffect
{
    // - - - - - - - - - - 
    // 필드
    [SerializeField] private Text lifeText;
    private int _x, _y;


    // - - - - - - - - - - 
    // 초기화
    public override void Init(int x, int y)
    {
        base.Init(x, y);
        UpdateLifeText();

        _x = x;
        _y = y;
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
        GameManager.Instance.ObstacleArray[_x, _y].RemoveBlind();
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
