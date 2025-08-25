// - - - - - - - - - - - - - - - - - -
// ObjectPoolManager.cs
//  - 성능 최적화 위해 오브젝트 풀링 관리
// - - - - - - - - - - - - - - - - - -

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Group { Tile, Effect, SlimeAction }

public class ObjectPoolManager : SingleTone<ObjectPoolManager>
{
    [SerializeField] private List<GameObject> _prefabs;
    private List<List<GameObject>> pools;
    private Dictionary<Group, Transform> _groupMap = new();

    [SerializeField] private Transform tileGroup;
    [SerializeField] private Transform EffectGroup;
    public Transform slimeActionGroup;

    public bool IsInitialized { get; private set; }
    [HideInInspector] public float _initProgress = 0; // 로딩바 반영

    public void Init()
    {
        DontDestroyOnLoad(EffectGroup);

        _groupMap[Group.Tile] = TileGroup.Instance == null ? tileGroup : TileGroup.Instance.transform;
        _groupMap[Group.Effect] = EffectGroup;
        _groupMap[Group.SlimeAction] = SlimeActionGroup.Instance == null ? slimeActionGroup : SlimeActionGroup.Instance.transform;

        StartCoroutine(InitAsync());
    }

    private IEnumerator InitAsync()
    {
        float numerator = 0;
        float denominator = _prefabs.Count + 6 * 5 + 10; // prefabs.Count + i * j + k

        pools = new List<List<GameObject>>();
        for (int i = 0; i < _prefabs.Count; i++)
        {
            pools.Add(new List<GameObject>());
            _initProgress = ++numerator / denominator;
        }
        yield return null;

        // 타일 생성
        for (int i = 0; i < 6; i++)
            for (int j = 0; j < 5; j++)
            {
                InitObject(i, Group.Tile);
                _initProgress = ++numerator / denominator;
                yield return null;
            }

        // 파티클 생성
        for (int k = 0; k < 10; k++)
        {
            InitObject(27, Group.Effect);
            _initProgress = ++numerator / denominator;
            yield return null;
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
        return newObject;
    }

    // 로딩중 오브젝트 초기화
    public void InitObject(int index, Group group)
    {
        GameObject newObject = Instantiate(_prefabs[index], _groupMap[group]);
        newObject.SetActive(false);
        pools[index].Add(newObject);
    }
}