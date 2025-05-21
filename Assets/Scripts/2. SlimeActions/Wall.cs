// - - - - - - - - - - - - - - - - - -
// Wall.cs
//  - 벽 클래스.
// - - - - - - - - - - - - - - - - - -
using UnityEngine;

public class Wall : SlimeActionBase
{
    private int x1, y1, x2, y2;

    // Wall의 실제 위치를 지정하고 gamemanager에 알림
    public void Init(int x1, int y1, int x2, int y2)
    {
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;

        // 위치 계산
        transform.position = (GameManager.Instance.LocateTile(x1, y1) + GameManager.Instance.LocateTile(x2, y2)) / 2;

        // 벽의 위치에 따라 회전 e.g. 위, 아래에 생성될 경우 90도 회전
        if (x1 == x2)
            transform.rotation = Quaternion.Euler(0, 0, 90);
        else
            transform.rotation = Quaternion.identity;

        // Gamemanager의 Wall논리배열에 생성됨을 알림
        PlaceWallBetween(x1, y1, x2, y2);

    }

    protected override void Execute()
    {
        // GameManager에 벽 삭제 알리기
        RemoveWallBetween(x1, y1, x2, y2);
        EventManager.Unsubscribe(GameEvent.NewTurn, OnTurnChanged);
        Destroy(gameObject);
    }
    

    // - - - - - - - - - - - - - - - - - - - - -
    // 장애물 배열
    // - - - - - - - - - - - - - - - - - - - - -
    public void PlaceWallBetween(int x1, int y1, int x2, int y2)
    {
        if (Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2) != 1) return; // Wall이 한칸 사이에 존재하는지 확인
        GameManager g = GameManager.Instance;

        if (x1 == x2)
        {
            if (y1 < y2)
            {
                g.ObstacleArray[x1, y1].PlaceWall(1);
                g.ObstacleArray[x2, y2].PlaceWall(0);
            }
            else
            {
                g.ObstacleArray[x1, y1].PlaceWall(0);
                g.ObstacleArray[x2, y2].PlaceWall(1);
            }
        }
        
        else if (y1 == y2)
        {
            if (x1 < x2)
            {
                g.ObstacleArray[x1, y1].PlaceWall(3);
                g.ObstacleArray[x2, y2].PlaceWall(2);
            }
            else
            {
                g.ObstacleArray[x1, y1].PlaceWall(2);
                g.ObstacleArray[x2, y2].PlaceWall(3);
            }
        }
    }

    //Wall이 사라질때 논리배열에서 false로 바꿈
    public void RemoveWallBetween(int x1, int y1, int x2, int y2)
    {
        if (Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2) != 1) return; // Wall이 한칸 사이에 존재하는지 확인
        GameManager g = GameManager.Instance;

        if (x1 == x2)
        {
            if (y1 < y2)
            {
                g.ObstacleArray[x1, y1].RemoveWall(1);
                g.ObstacleArray[x2, y2].RemoveWall(0);
            }
            else
            {
                g.ObstacleArray[x1, y1].RemoveWall(0);
                g.ObstacleArray[x2, y2].RemoveWall(1);
            }
        }

        else if (y1 == y2)
        {
            if (x1 < x2)
            {
                g.ObstacleArray[x1, y1].RemoveWall(3);
                g.ObstacleArray[x2, y2].RemoveWall(2);
            }
            else
            {
                g.ObstacleArray[x1, y1].RemoveWall(2);
                g.ObstacleArray[x2, y2].RemoveWall(3);
            }
        }
    }
}