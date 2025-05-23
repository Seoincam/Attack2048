// - - - - - - - - - - - - - - - - - -
// ObjectPoolManager.cs
//  - 성능 최적화 위해 오브젝트 풀링 관리
// - - - - - - - - - - - - - - - - - -

using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _prefabs;
    private List<List<GameObject>> pools;

    void Awake()
    {
        pools = new List<List<GameObject>>();
        for (int i = 0; i < _prefabs.Count; i++)
        {
            pools.Add(new List<GameObject>());
        }
    }

    public GameObject GetObject(int index, Transform parent)
    {
        // 비활성화된 오브젝트 있으면 return
        foreach (GameObject selected in pools[index])
        {
            if (selected == null) continue;
            if (!selected.activeSelf)
            {
                selected.SetActive(true);
                return selected;
            }
        }

        // 없으면 새로 생성 후 return
        GameObject newObject = Instantiate(_prefabs[index], parent);
        pools[index].Add(newObject);
        return newObject;
    }
}