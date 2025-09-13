using System.Collections.Generic;
using UnityEngine;

// 한번의 입력으로 실행할 이동, 병합 계획의 단위
public struct MoveStep
{
    public int x1, y1;      // 시작
    public int x2, y2;      // 도착
    public bool willMerge;  // 도착지와 합쳐지는가
    public int victimX, victimY; // 병합 시 사라질 기존 타일 좌표
}

public sealed class MovePlan
{
    public readonly List<MoveStep> steps = new();
    public bool Any => steps.Count > 0;
}

/* 계산 전용 로직
 * shadow : 현재 보드의 값을 복사해서 시뮬레이션용
 * consumed : 병합되는 타일의 칸 표시용
 * */
public static class BoardPlanner
{
    // dir: (0,1)=Up, (0,-1)=Down, (1,0)=Right, (-1,0)=Left
    public static MovePlan ComputePlan(
        Vector2Int dir,
        GameObject[,] tileArray,
        Obstacle[,] obstacleArray)
    {
        var plan = new MovePlan();
        const int N = 5;

        // 스캔 순서 : 이동방향쪽에서 반대편으로 ex) Up이면 y=4부터0으로
        IEnumerable<(int x, int y)> Order()
        {
            const int N = 5;
            if (dir == Vector2Int.up) { for (int y = N-1; y >=0; y--) for (int x = 0; x < N; x++) yield return (x, y); }
            else if (dir == Vector2Int.down) { for (int y = 0; y <N; y++) for (int x = 0; x < N; x++) yield return (x, y); }
            else if (dir == Vector2Int.right) { for (int x = N - 1; x >= 0; x--) for (int y = 0; y < N; y++) yield return (x, y); }
            else /*left*/                 { for (int x = 0; x < N; x++) for (int y = 0; y < N; y++) yield return (x, y); }
        }

        // value = 현재 칸 숫자, merged = 이번턴에 병합 했는지
        var shadow = new (int value, bool merged)[N, N];
        for (int y = 0; y < N; y++)
            for (int x = 0; x < N; x++)
                if (tileArray[x, y] != null)
                {
                    shadow[x, y] = (tileArray[x,y].GetComponent<Tile>().value, false);
                }

        var consumed = new bool[N, N]; // 이번턴에 병합된 타일

        //모든 칸 스캔
        foreach (var (sx, sy) in Order())
        {
            // 이미 병합으로 소모됐으면 넘김
            if (consumed[sx, sy]) continue;
            if (shadow[sx, sy].value == 0) continue;
            int cx = sx, cy = sy;
            int curVal = shadow[sx, sy].value;
            bool mergedOnce = false;
            int victimX = -1, victimY = -1;

            shadow[sx, sy] = (0, false); // 원래 자리 비우고 목적지로 밀어 넣음
            while (true)
            {
                int nx = cx + dir.x, ny = cy + dir.y; // 다음 칸 좌표
                if (nx < 0 || nx >= N || ny < 0 || ny >= N) break;

                if (!obstacleArray[nx, ny].CanMove(cx, cy)) break;
                if (obstacleArray[cx, cy].HasImprison()) break;

                var dest = shadow[nx, ny];

                if (dest.value == 0) { cx = nx; cy = ny; continue; }  // 빈칸이면 계속 이동

                // 같은 값, 아직 병합 안함, 목적지도 병합 안함이면 병합
                if (!mergedOnce && !dest.merged && dest.value == curVal)
                {
                    mergedOnce = true;
                    victimX = nx; victimY = ny;
                    consumed[victimX, victimY] = true;
                    curVal *= 2;
                    // 병합한 칸으로 현재값을 옮기고 병합 표시
                    cx = nx; cy = ny;
                    shadow[nx, ny] = (curVal, true);
                    continue;
                }
                //다른 값이면 그 직전 칸에서 정지
                break;
            }
            //도착지점에 현재 값 기록(빈칸일때)
            //병합했으면 그대로 유지
            if (shadow[cx, cy].value == 0)
                shadow[cx, cy] = (curVal, shadow[cx, cy].merged || mergedOnce);

            plan.steps.Add(new MoveStep
            {
                x1 = sx,
                y1 = sy,
                x2 = cx,
                y2 = cy,
                willMerge = mergedOnce,
                victimX = victimX,
                victimY = victimY
            });
        }

        return plan;
    }
}