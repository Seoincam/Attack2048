public class Obstacle
{
    private int _x, _y; // 본인의 위치
    private int direction; // 이동할 방향

    // 타일이 이동할 때, 장애물 있나?
    private bool IsObstacled { get => isWall[direction] || isPetrify || isImprison; }

    // 장애물을 설치할 때, 설치 가능한가?
    public bool CanObstacle { get => !isDelete && !isPetrify && !isImprison && !isChange && !isTranslocate; }

    // 타일이 스폰 가능한가?
    public bool CanSpawn { get => !isPetrify && !isImprison; }

    private bool isDelete;
    private bool[] isWall = new bool[4]; // 방향 : 0 = 하, 1 = 상, 2 = 좌, 3 = 우
    private bool isPetrify;
    private bool isImprison;
    private bool isChange;
    private bool isTranslocate;


    public Obstacle(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public void PlaceDelete() => isDelete = true;
    public void RemoveDelete() => isDelete = false;

    public void PlaceWall(int direction)
    {
        isWall[direction] = true;
    }
    public void RemoveWall(int direction)
    {
        isWall[direction] = false;
    }

    public void PlacePetrify() => isPetrify = true;
    public void RemovePetrify() => isPetrify = false;

    public void PlaceImprison() => isImprison = true;
    public bool HasImprison() => isImprison; // 해당 칸의 감금 여부
    public void RemoveImprison() => isImprison = false;

    public void PlaceChange() => isChange = true;
    public void RemoveChange() => isChange = false;

    public void PlaceTranslocate() => isTranslocate = true;
    public void RemoveTranslocate() => isTranslocate = false;


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
