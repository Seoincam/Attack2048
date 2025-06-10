// - - - - - - - - - - - - - - - - - -
// Imprison.cs
//  - 감금 클래스.
// - - - - - - - - - - - - - - - - - -
using UnityEngine;
using UnityEngine.UI;
public class Imprison : SlimeActionBase, IShowLife, IMakeDeleteEffect
{
    // 필드    
    // - - - - - - - - - - 
    [SerializeField] private Text lifeText;

    private int _x, _y;


    // 초기화 
    // - - - - - - - - - - 
    public override void Init(int x, int y)
    {
        base.Init(x, y);
        UpdateLifeText();

        _x = x; _y = y;
        GameManager.Instance.ObstacleArray[x, y].PlaceImprison(); 
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
        GameManager.Instance.ObstacleArray[_x, _y].RemoveImprison();

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
