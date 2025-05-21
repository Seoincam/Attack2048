public class Obstacle
{
    private int _x, _y; // 본인의 위치
    private int direction; // 이동할 방향

    // 타일이 이동할 때, 막혔는가?
    private bool IsObstacled { get => isWall[direction] || isPerify || isImprison; }

    // 장애물을 설치할 때, 설치 가능한가?
    public bool CanObstacle { get => isDelete || isPerify || isImprison || isTranslocate; }

    private bool isDelete;
    private bool[] isWall = new bool[4]; // 방향 : 0 = 하, 1 = 상, 2 = 좌, 3 = 우
    private bool isPerify;
    private bool isImprison;
    private bool isTranslocate;



    public void Init(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public void PlaceWall(int direction)
    {
        isWall[direction] = true;
    }
    public void RemoveWall(int direction)
    {
        isWall[direction] = false;
    }



    // (x1, y1): 타일의 이동 전 위치
    public bool CanMove(int x1, int y1)
    {
        direction = SetDirection(x1, y1);
        return !IsObstacled;
    }


    private int SetDirection(int x1, int y1)
    {
        if (x1 == _x)
        {
            if (_y == y1 + 1) return 0;
            if (_y == y1 - 1) return 1;
        }
        else if (y1 == _y)
        {
            if (_x == x1 + 1) return 2;
            if (_x == x1 - 1) return 3;
        }

        return -1;
    }
}
