// - - - - - - - - - - - - - - - - - -
// SlimeActionManager.cs
//  - 슬라임의 액션을 여기서 실행.
//  - 여기선 단순히 '실제 생성'만.
//  - 로직은 각 action class 내에서 수행
// - - - - - - - - - - - - - - - - - -

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlimeActionManager : MonoBehaviour
{
    // - - - - - - - - - - - - - - - - - - - - -
    // 필드
    // - - - - - - - - - - - - - - - - - - - - -
    private ObjectPoolManager _pooler;


    // - - - - - - - - - - - - - - - - - - - - -
    // Unity 콜백
    // - - - - - - - - - - - - - - - - - - - - -

    // 이벤트 매니저에 각 메서드를 구독.
    void Awake()
    {
        _pooler = ObjectPoolManager.instance;

        EventManager.Subscribe(GameEvent.Delete, Delete);
        EventManager.Subscribe(GameEvent.Delete6, Delete6);
        EventManager.Subscribe(GameEvent.Wall, Wall);
        EventManager.Subscribe(GameEvent.Petrify, Petrify);
        EventManager.Subscribe(GameEvent.Imprison, Imprison);
        EventManager.Subscribe(GameEvent.Change, Change);
        EventManager.Subscribe(GameEvent.Translocate3, Translocate3);
        EventManager.Subscribe(GameEvent.Translocate7, Translocate7);
        EventManager.Subscribe(GameEvent.ForcedMove, ForcedMove);
        EventManager.Subscribe(GameEvent.ReverseMove, ReverseMove);
        EventManager.Subscribe(GameEvent.Blind, Blind);
    }



    // - - - - - - - - - - - - - - - - - - - - -
    // 생성 로직
    // - - - - - - - - - - - - - - - - - - - - -

    // 삭제
    private void Delete()
    {
        GameObject obj = _pooler.GetObject(18, Group.SlimeAction);
        Delete delete = obj.GetComponent<Delete>();

        // 위치 설정
        Vector2Int selected = GetRandomPosition(false);
        delete.Init(selected.x, selected.y, false);
    }

    // 삭제 (6스테이지 한줄 삭제)
    private void Delete6()
    {
        // ToDo : 기존에 delete 없나 체크
        int randomLineX = Random.Range(0, 5);

        int count = 0;
        while (true)
        {
            // 버그 방지
            count++;
            if (count > 500)
            {
                Debug.LogError("Delete6 오류!");
                break;
            }

            bool canPlace = true;
            for (int y = 4; y >=0; y--)
            {
                if (!GameManager.Instance.ObstacleArray[randomLineX, y].CanObstacle)
                {
                    canPlace = false;
                }
            }

            if (canPlace) break;
            else randomLineX = Random.Range(0, 5);
        }


        for (int y = 4; y >= 0; y--)
        {
            GameObject obj = _pooler.GetObject(18, Group.SlimeAction);
            Delete delete = obj.GetComponent<Delete>();
            delete.Init(randomLineX, y, true);
        }
    }

    // 벽
    private void Wall()
    {
        GameObject obj = _pooler.GetObject(25, Group.SlimeAction);
        Wall wall = obj.GetComponent<Wall>();

        // 위치 정하여 Wall.cs에서 위치 설정
        int x1 = 0, y1 = 0, direction;
        Vector2Int dir = Vector2Int.zero;

        // 오류 방지
        int count = 0;
        do
        {
            count++;
            if (count > 500)
            {
                Debug.LogError("Change 오류!");
                break;
            }

            x1 = Random.Range(1, 4);
            y1 = Random.Range(1, 4);
            dir = GetRandomDirection(out direction);
        }
        while (GameManager.Instance.ObstacleArray[x1, y1].CheckWall(direction));
        
        int x2 = x1 + dir.x;
        int y2 = y1 + dir.y;

        wall.Init(x1, y1, x2, y2);
    }

    // 석화 대기
    private void Petrify()
    {
        GameObject obj = _pooler.GetObject(23, Group.SlimeAction);
        PetrifyPrep petrifyPrep = obj.GetComponent<PetrifyPrep>();
        // 위치 설정
        Vector2Int selected = GetRandomPosition(false);
        petrifyPrep.Init(selected.x, selected.y);
    }

    // 감금 대기
    private void Imprison()
    {
        GameObject obj = _pooler.GetObject(21, Group.SlimeAction);
        ImprisonPrep imprison1 = obj.GetComponent<ImprisonPrep>();

        obj = _pooler.GetObject(21, Group.SlimeAction);
        ImprisonPrep imprison2 = obj.GetComponent<ImprisonPrep>();

        // 위치 설정
        Vector2Int selected1 = GetRandomPosition(false);
        Vector2Int selected2 = GetRandomPosition(false);
        //감금이 겹치치 않도록
        do
        {
            selected2 = GetRandomPosition(false);
        }
        while(selected1 == selected2); 

        imprison1.Init(selected1.x, selected1.y);
        imprison2.Init(selected2.x, selected2.y);
    }


    List<Vector2Int> notSelectPos = new List<Vector2Int>
    {
        new Vector2Int(0,0),
        new Vector2Int(0,4),
        new Vector2Int(4,4),
        new Vector2Int(4,0)
    };

    private void Change()
    {
        GameObject obj = _pooler.GetObject(17, Group.SlimeAction);
        Change change = obj.GetComponent<Change>();

        // 변경할 타일 위치 설정
        Vector2Int selected = GetRandomPosition(false);

        // 변경할 타일이 네 모퉁이 아니게
        int count = 0;
        while (true)
        {
            // 버그 방지
            count++;
            if (count > 500)
            {
                Debug.LogError("Change 오류!");
                break;
            }

            bool isValid = true;

            foreach (Vector2Int not in notSelectPos)
            {
                if (selected == not)
                {
                    isValid = false;
                    break;
                }
            }

            if (!isValid)
            {
                selected = GetRandomPosition(false);
                continue;
            }
            break;
        }

        change.Init(selected.x, selected.y);
    }

    // 이동 (3 스테이지)
    private void Translocate3()
    {
        GameObject obj = _pooler.GetObject(24, Group.SlimeAction);
        Translocate3 translocate = obj.GetComponent<Translocate3>();
        translocate.Init();
    }
    
    // 이동 (7 스테이지)
    private void Translocate7()
    {
        // Vertical or Horizontal
        int random = Random.Range(0,2);

        // a, b
        int[] ab = new int[2];
        ab[0] = Random.Range(0, 5);
        while (true)
        {
            int value = Random.Range(0, 5);
            if (value != ab[0])
            {
                ab[1] = value;
                break;
            }
        }

        if (random == 0)
        {
            // Vertical
            GameObject obj = _pooler.GetObject(30, Group.SlimeAction);
            Translocate7Vertical translocate = obj.GetComponent<Translocate7Vertical>();
            translocate.Init(ab.Min(), ab.Max());
        }
        else
        {
            // Horizontal
            GameObject obj = _pooler.GetObject(32, Group.SlimeAction);
            Translocate7Horizontal translocate = obj.GetComponent<Translocate7Horizontal>();
            translocate.Init(ab.Min(), ab.Max());
        }



    }

    private void ReverseMove()
    {
        GameObject obj = _pooler.GetObject(29, Group.SlimeAction);
        ReverseMove reversemove = obj.GetComponent<ReverseMove>();
        reversemove.Init();
    }
    
    // 이동 방향 강제 (4 스테이지)
    private void ForcedMove()
    {
        GameObject obj = _pooler.GetObject(19, Group.SlimeAction);
        ForcedMove forcedmove = obj.GetComponent<ForcedMove>();
        forcedmove.Init();
    }

    private void Blind()
    {
        GameObject obj = _pooler.GetObject(31, Group.SlimeAction);
        Blind blind = obj.GetComponent<Blind>();

        // 위치 설정
            // 이미 타일이 있는 곳에 설치
        Vector2Int selected = GetRandomPosition(true);
        blind.Init(selected.x, selected.y);
    }



    // - - - - - - - - - - - - - - - - - - - - -
    // 랜덤 위치 설정
    // - - - - - - - - - - - - - - - - - - - - -

    // 5*5 보드 중 하나 랜덤 선택
    // [bool onlyTile] true: 이미 타일 있는 칸만, false: 그냥 아무 칸이나
    private Vector2Int GetRandomPosition(bool onlyTile)
    {
        GameManager G = GameManager.Instance;

        Vector2Int selected = new Vector2Int(Random.Range(0, 5), Random.Range(0, 5));

        if (onlyTile)
        {
            int count = 0;
            while (true)
            {
                // 버그 방지
                count++;
                if (count > 500)
                {
                    Debug.LogError("GetRandomPosition 오류!");
                    break;
                }

                if (G.IsTiled(selected.x, selected.y) && G.ObstacleArray[selected.x, selected.y].CanObstacle) break;

                selected = new Vector2Int(Random.Range(0, 5), Random.Range(0, 5));
            }
            return selected;
        }

        else
        {
            int count = 0;
            while (true)
            {
                // 버그 방지
                count++;
                if (count > 500)
                {
                    Debug.LogError("GetRandomPosition 오류!");
                    break;
                }

                if (G.ObstacleArray[selected.x, selected.y].CanObstacle) break;

                selected = new Vector2Int(Random.Range(0, 5), Random.Range(0, 5));
            }
        }

        return selected;
    }

    // 설치할 방향 랜덤 설정 
    private Vector2Int GetRandomDirection(out int direction)
    {
        var directions = new Dictionary<Vector2Int, int>
        {
            {Vector2Int.up, 1},
            {Vector2Int.down, 0},
            {Vector2Int.right, 3},
            {Vector2Int.left, 2}
        };

        int random = Random.Range(0, directions.Count);

        direction = directions.ElementAt(random).Value;
        return directions.ElementAt(random).Key;
    }
}