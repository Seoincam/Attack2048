using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
class ChangeRule
{
    public int tileValue;
    public int probabilty;
}

public class Change : SlimeActionBase
{
    protected int _x, _y; // Square 배열 상의 현재 위치
    [SerializeField] private Text lifeText;

    [SerializeField] private ChangeRule[] ChangeRules;

    void Update()
    {
        float alpha = Mathf.Min(Mathf.Abs(Mathf.Sin(Time.time)), 0.7f);
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, alpha);
    }


    public override void Init(int x, int y, ObjectPoolManager pooler)
    {
        base.Init(x, y, pooler);

        _x = x; _y = y;
        GameManager.Instance.ObstacleArray[x, y].PlaceChange();

        lifeText.text = _lifeCounter.ToString();
    }


    protected override void Execute()
    {        
        GameManager G = GameManager.Instance;
        if (G.DeleteTile(_x, _y))
        {
            int random = Random.Range(1, 101);
            int tileValue = 2;
            int probabilty = 0;

            foreach (ChangeRule rule in ChangeRules)
            {
                probabilty += rule.probabilty;
                if (random <= probabilty)
                {
                    tileValue = rule.tileValue;
                    break;
                }
            }

            G.Spawn(tileValue, _x, _y);
        }
 
        G.ObstacleArray[_x, _y].RemoveChange();

        base.Execute();
    }

    public override void OnEnter_CountDownPhase()
    {
        base.OnEnter_CountDownPhase();
        lifeText.text = _lifeCounter.ToString();
    }
}
