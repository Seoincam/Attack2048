using UnityEngine;

public class Board : MonoBehaviour
{
    public int value;
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

        Vector3 targetPos = new Vector3(GameManager.xStart + GameManager.xOffset * x2,
                                        GameManager.yStart + GameManager.yOffset * y2, 0);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.1f);

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

