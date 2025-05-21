using UnityEngine;

public class Board : MonoBehaviour
{
    public int value;
    public int x, y; //현재 좌표
    bool move, _combine;
    int _x2, _y2;


    void Update()
    {
        if (move)
        {
            Move(_x2, _y2, _combine);
        }
    }

    public void Move(int x2, int y2, bool combine)
    {
        move = true;
        _x2 = x2;
        _y2 = y2;
        _combine = combine;
        //현재 좌표를 새로운 위치로 갱신
        x = x2;
        y = y2;

        Vector3 targetPos = GameManager.Instance.LocateTile(x2, y2);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.35f);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            move = false;
            if (combine)
            {
                _combine = false;
                Destroy(gameObject);
            }
        }
    }
}

