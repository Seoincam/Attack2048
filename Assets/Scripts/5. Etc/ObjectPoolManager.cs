// - - - - - - - - - - - - - - - - - -
// ObjectPoolManager.cs
//  - 성능 최적화 위해 오브젝트 풀링 관리
// - - - - - - - - - - - - - - - - - -

using System.Collections.Generic;
using UnityEngine;

public enum Group { Tile, Effect, SlimeAction }

public class ObjectPoolManager : SingleTone<ObjectPoolManager>
{
    [SerializeField] private List<GameObject> _prefabs;
    private List<List<GameObject>> pools;
    private Dictionary<Group, Transform> _groupMap = new();

    [SerializeField] private Transform tileGroup;
    [SerializeField] private Transform effectGroup;
    public Transform slimeActionGroup;

    public bool IsInitialized { get; private set; }

    public void Init()
    {
        if (IsInitialized) return;

        _groupMap[Group.Tile] = tileGroup;
        _groupMap[Group.Effect] = effectGroup;
        _groupMap[Group.SlimeAction] = slimeActionGroup;

        pools = new List<List<GameObject>>();
        for (int i = 0; i < _prefabs.Count; i++)
        {
            pools.Add(new List<GameObject>());
        }

        // 타일 생성
        for (int i = 0; i < 6; i++)
            for (int j = 0; j < 5; j++)
            {
                InitObject(i, Group.Tile);
            }

        // 파티클 생성
        for (int k = 0; k < 10; k++)
        {
            InitObject(27, Group.Effect);
        }

        IsInitialized = true;
    }
    public GameObject GetObject(int index, Group group)
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
        GameObject newObject = Instantiate(_prefabs[index], _groupMap[group]);
        pools[index].Add(newObject);

        if (index == 27)
            newObject.transform.localScale = Vector3.one;
        return newObject;
    }

    // 로딩중 오브젝트 초기화
    public void InitObject(int index, Group group)
    {
        GameObject newObject = Instantiate(_prefabs[index], _groupMap[group]);
        newObject.SetActive(false);
        pools[index].Add(newObject);
    }

    public void ResetObstacles()
    {
        foreach (Transform action in slimeActionGroup)
        {
            if (!action.gameObject.activeSelf)
                continue;
            var slimeAction = action.GetComponent<SlimeActionBase>();
            slimeAction.Destroy();
        }
    }
}