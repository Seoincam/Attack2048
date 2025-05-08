// - - - - - - - - - - - - - - - - - -
// Wall.cs
//  - 벽 클래스.
// - - - - - - - - - - - - - - - - - -

public class Wall : SlimeActionBase
{
    protected override int Life => 2; // 수명.
    private int x1, y1, x2, y2;
    //Wall의 실제 위치를 지정하고 gamemanager에 알림
    public void Init(int x1, int y1, int x2, int y2)
    {
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
        //위치 계산
        float worldX = (GameManager.xStart + GameManager.xOffset * x1 + GameManager.xStart + GameManager.xOffset * x2) / 2f;
        float worldY = (GameManager.yStart + GameManager.yOffset * y1 + GameManager.yStart + GameManager.yOffset * y2) / 2f;
        transform.position = new UnityEngine.Vector3(worldX, worldY, 0);
        // 벽의 위치에 따라 회전 e.g. 위, 아래에 생성될 경우 90도 회전
        if (x1 == x2)
            transform.rotation = UnityEngine.Quaternion.identity;
        else
            transform.rotation = UnityEngine.Quaternion.Euler(0, 0, 90);
        // Gamemanager의 Wall논리배열에 생성됨을 알림
        GameManager.Instance.PlaceWallBetween(x1, y1, x2, y2);
    }
    protected override void Execute()
    {
        // TODO: GameManager에 벽 삭제 알리기
        GameManager.Instance.RemoveWallBetween(x1,y1,x2,y2);
        EventManager.Unsubscribe(GameEvent.NewTurn, OnTurnChanged);
        Destroy(gameObject);
    }
}