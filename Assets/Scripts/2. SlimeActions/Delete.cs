// - - - - - - - - - - - - - - - - - -
// Delete.cs
//  - 삭제 클래스.
// - - - - - - - - - - - - - - - - - -

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Delete : SlimeActionBase
{
    
    private int _x, _y; // Square 배열 상의 현재 위치
    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] Text lifeText;



    void Update()
    {
        float alpha = Mathf.Min(Mathf.Abs(Mathf.Sin(Time.time)), 0.7f);
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, alpha);
    }



    public void Init(int x, int y) {
        _x = x; _y = y;
        transform.position = GameManager.Instance.LocateTile(_x,_y);
        lifeText.text = _lifeCounter.ToString();
    }


    protected override void Execute()
    {
        StartCoroutine(DelayExecute()); // 타일 이동하고 삭제 (시각적으로)
    }

    private IEnumerator DelayExecute()
    {
        yield return new WaitForSeconds(0.3f);
        GameManager.Instance.DestroyTile(_x,_y);
        Destroy(gameObject);
    }

    public override void OnTurnChanged()
    {
        base.OnTurnChanged();
        lifeText.text = _lifeCounter.ToString();
    }
}
