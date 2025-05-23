// - - - - - - - - - - - - - - - - - -
// ImprisonPrep.cs
//  - 감금 대기 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;

public class ImprisonPrep : SlimeActionBase
{
    protected int _x, _y; // Square 배열 상의 현재 위치
    [SerializeField] private Text lifeText;

    protected Transform _slimeActionGroup;


    public void Init(int x, int y, ObjectPoolManager pooler, Transform slimeActionGroup)
    {
        base.Init(x, y, pooler);

        _x = x; _y = y;
        GameManager.Instance.ObstacleArray[x, y].PlaceImprisonPrep();

        _slimeActionGroup = slimeActionGroup;
        lifeText.text = _lifeCounter.ToString(); 
    }
    
    void Update()
    {
        float alpha = Mathf.Min(Mathf.Abs(Mathf.Sin(Time.time)), 0.7f);
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, alpha);
    }

    public override void OnEnter_CountDownPhase()
    {
        base.OnEnter_CountDownPhase();
        lifeText.text = _lifeCounter.ToString();
    }

    protected override void Execute()
    {
        GameManager.Instance.ObstacleArray[_x, _y].RemoveImprisonPrep();
        GameObject obj = _pooler.GetObject(20, _slimeActionGroup);
        Imprison imprison = obj.GetComponent<Imprison>();

        // 위치 설정
        imprison.Init(_x, _y, _pooler);
        base.Execute();
    }
}