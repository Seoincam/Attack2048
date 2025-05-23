// - - - - - - - - - - - - - - - - - -
// ImprisonPrep.cs
//  - 감금 대기 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;

public class ImprisonPrep : SlimeActionBase
{
    private int _x, _y; // Square 배열 상의 현재 위치
    [SerializeField] private Text lifeText;
    [SerializeField] private GameObject _imprisonPrefab; // 감금

    public void Init(int x, int y)
    {
        GameManager G = GameManager.Instance;

        _x = x; _y = y;
        transform.position = G.LocateTile(x, y);
        lifeText.text = _lifeCounter.ToString();

        G.ObstacleArray[x, y].PlaceImprisonPrep();
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
        Imprison imprison = Instantiate(_imprisonPrefab).GetComponent<Imprison>();

        // 위치 설정
        imprison.Init(_x, _y);
        base.Execute();
    }
}