// - - - - - - - - - - - - - - - - - -
// ForcedMove.cs
//  - 석화 클래스.
// - - - - - - - - - - - - - - - - - -

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ForcedMove : SlimeActionBase
{
    private int _x = 2, _y = 2; // Square 배열 상의 현재 위치
    private ForcedMovedir dir;
    private static readonly Dictionary<ForcedMovedir, Vector3> directionVectors = new()
    {
        {ForcedMovedir.Up, Vector3.up},
        {ForcedMovedir.Down, Vector3.down},
        {ForcedMovedir.Left, Vector3.left},
        {ForcedMovedir.Right, Vector3.right}
    };

    public void Init()
    {
        GameManager G = GameManager.Instance;
        transform.position = G.LocateTile(_x, _y);
        // 최대 10회까지 무작위 방향 시도
        for (int attempt = 0; attempt < 10; attempt++)
        {
            ForcedMovedir randomDir = (ForcedMovedir)Random.Range(1, 5); // 1~4
            Vector3 checkDir = directionVectors[randomDir];

            if (G.CheckCanMove(checkDir))
            {
                dir = randomDir;
                G.forcedDirection = dir;
                switch (dir)
                {
                    case ForcedMovedir.Up:
                        transform.rotation = Quaternion.Euler(0, 0, 180);
                        break;
                    case ForcedMovedir.Down:
                        transform.rotation = Quaternion.Euler(0, 0, 0);
                        break;
                    case ForcedMovedir.Left:
                        transform.rotation = Quaternion.Euler(0, 0, -90);
                        break;
                    case ForcedMovedir.Right:
                        transform.rotation = Quaternion.Euler(0, 0, 90);
                        break;
                }
                return;
            }
        }
    }

    public override void OnEnter_CountDownPhase()
    {
        base.OnEnter_CountDownPhase();
    }

    protected override void Execute()
    {
        GameManager.Instance.forcedDirection = ForcedMovedir.None;
        base.Execute();
    }
}