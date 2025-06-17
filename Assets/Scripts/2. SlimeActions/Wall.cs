// - - - - - - - - - - - - - - - - - -
// Wall.cs
//  - 벽 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using UnityEngine.UI;

public class Wall : SlimeActionBase, IShowLife, IMakeDeleteEffect
{
    private int x1, y1, x2, y2;
    [SerializeField] private Text lifeText;

    private GameManager G;

    void Awake()
    {
        G = GameManager.Instance;
    }

    // Wall의 실제 위치를 지정하고 gamemanager에 알림
    public void Init(int x1, int y1, int x2, int y2)
    {
        base.Init();
        UpdateLifeText();

        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;

        // 위치 계산
        transform.position = (G.LocateTile(x1, y1) + G.LocateTile(x2, y2)) / 2;

        // 벽의 위치에 따라 회전 e.g. 위, 아래에 생성될 경우 90도 회전
        if (x1 == x2)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            lifeText.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
            transform.rotation = Quaternion.identity;

        PlaceWallBetween(x1, y1, x2, y2);
    }

    public override void OnEnter_CountDownPhase()
    {
        base.OnEnter_CountDownPhase();
        UpdateLifeText();
    }

    protected override void Execute()
    {
        RemoveWallBetween(x1, y1, x2, y2);

        MakeDeleteEffect();
        base.Execute();
    }
    

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
    

    // - - - - - - - - - - - - - - - - - - - - -
    // 장애물 배열
    // - - - - - - - - - - - - - - - - - - - - -
    public void PlaceWallBetween(int x1, int y1, int x2, int y2)
    {
        if (Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2) != 1) return; // Wall이 한칸 사이에 존재하는지 확인

        if (x1 == x2)
        {
            if (y1 < y2)
            {
                G.ObstacleArray[x1, y1].PlaceWall(1);
                G.ObstacleArray[x2, y2].PlaceWall(0);
            }
            else
            {
                G.ObstacleArray[x1, y1].PlaceWall(0);
                G.ObstacleArray[x2, y2].PlaceWall(1);
            }
        }

        else if (y1 == y2)
        {
            if (x1 < x2)
            {
                G.ObstacleArray[x1, y1].PlaceWall(3);
                G.ObstacleArray[x2, y2].PlaceWall(2);
            }
            else
            {
                G.ObstacleArray[x1, y1].PlaceWall(2);
                G.ObstacleArray[x2, y2].PlaceWall(3);
            }
        }
    }

    public void RemoveWallBetween(int x1, int y1, int x2, int y2)
    {
        if (Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2) != 1) return; // Wall이 한칸 사이에 존재하는지 확인

        if (x1 == x2)
        {
            if (y1 < y2)
            {
                G.ObstacleArray[x1, y1].RemoveWall(1);
                G.ObstacleArray[x2, y2].RemoveWall(0);
            }
            else
            {
                G.ObstacleArray[x1, y1].RemoveWall(0);
                G.ObstacleArray[x2, y2].RemoveWall(1);
            }
        }

        else if (y1 == y2)
        {
            if (x1 < x2)
            {
                G.ObstacleArray[x1, y1].RemoveWall(3);
                G.ObstacleArray[x2, y2].RemoveWall(2);
            }
            else
            {
                G.ObstacleArray[x1, y1].RemoveWall(2);
                G.ObstacleArray[x2, y2].RemoveWall(3);
            }
        }
    }
}