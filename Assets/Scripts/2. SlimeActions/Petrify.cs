// - - - - - - - - - - - - - - - - - -
// Petrify.cs
//  - 석화 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;

public class Petrify : SlimeActionBase, IShowLife, IMakeDeleteEffect
{
    // 필드    
    // - - - - - - - - - - 
    [SerializeField] private Text lifeText;
    
    private int _x, _y; // Square 배열 상의 현재 위치


    // 초기화
    // - - - - - - - - - - 
    public override void Init(int x, int y)
    {
        base.Init(x, y);
        UpdateLifeText();

        _x = x; _y = y;
        GameManager.Instance.ObstacleArray[x, y].PlacePetrify();
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
        GameManager.Instance.ObstacleArray[_x, _y].RemovePetrify();

        MakeDeleteEffect();
        base.Execute();
    }


    // Interfaces
    // - - - - - - - - - - 
    public void UpdateLifeText()
    {
        lifeText.text = _lifeCounter.ToString();
    }

    public void MakeDeleteEffect()
    {
        ParticleSystem particle = ObjectPoolManager.instance.GetObject(27, Group.Effect).GetComponent<ParticleSystem>();
        particle.transform.position = transform.position;
        particle.Play();
    }
}