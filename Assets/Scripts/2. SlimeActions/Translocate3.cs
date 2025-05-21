// - - - - - - - - - - - - - - - - - -
// Translocate3.cs
//  - 이동 클래스 (스테이지 3)
// - - - - - - - - - - - - - - - - - -

using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Translocate3 : SlimeActionBase
{
    [SerializeField]
    private SpriteRenderer t_0_0, t_0_4, t_4_4, t_4_0;
    private List<SpriteRenderer> tList;

    void Start()
    {
        t_0_0.transform.position = GameManager.Instance.LocateTile(0, 0);
        t_0_4.transform.position = GameManager.Instance.LocateTile(0, 4);
        t_4_4.transform.position = GameManager.Instance.LocateTile(4, 4);
        t_4_0.transform.position = GameManager.Instance.LocateTile(4, 0);

        tList = new List<SpriteRenderer>
        {
            t_0_0, t_0_4, t_4_4, t_4_0
        };
    }

    void FixedUpdate()
    {
        float alpha = Mathf.Min(Mathf.Abs(Mathf.Sin(Time.time)), 0.45f);
        foreach (SpriteRenderer renderer in tList)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);    
        }
    }

    protected override void Execute()
    {
        // TODO: GameManager에 이동 실행 알리기 & 타일 이동 시키기
        // GameManager.Instance.
        EventManager.Unsubscribe(GameEvent.NewTurn, OnTurnChanged);
        Destroy(gameObject);
    }
}
