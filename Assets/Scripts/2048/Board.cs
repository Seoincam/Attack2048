using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Board : MonoBehaviour
{
    public int value;
    bool move,_combine;
    int _x2,_y2;
    void Update()
    {
        if(move)
        {
            Move(_x2,_y2,_combine);
        }
    }
    public void Move(int x2,int y2, bool combine)
    {
        move = true;
        _x2 = x2;
        _y2 = y2;
        _combine = combine;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(-1.63f + 1.08f * x2,-3.41f + 1.05f * y2, 0),0.3f);
        if(transform.position == new Vector3(-1.63f + 1.08f * x2,-3.41f + 1.05f * y2, 0))
        {
            move = false;
            if(combine == true)
            {
                _combine = false;
                Destroy(gameObject);
            }
        } 
    }
}
