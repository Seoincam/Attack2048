using System.Collections;
using System.Linq;
using UnityEngine;

public class CountDownManager : MonoBehaviour
{
    public void CountDown()
    {
        StartCoroutine(CountDownCoroutine());
    }

    private IEnumerator CountDownCoroutine()
    {
        GameManager.Instance.IsPaused = true;

        // 0.3초 후 카운트 다운 실행
        var countDown = EventManager.GetEvent(GamePhase.CountDownPhase);
        if (countDown != null && countDown.Count != 0)
        {
            EventManager.Publish(GamePhase.CountDownPhase);
        }


        // 0이 된 슬라임 액션(벽, 삭제 등)이 있다면 실행
        var execute = EventManager.GetEvent(GamePhase.ExecutePhase);
        if (execute != null && execute.Count != 0)
        {
            var actionList = EventManager.GetEvent(GamePhase.ExecutePhase).ToList();
            float waitTime = actionList.Count == 1 ? 0.2f : 0.45f / actionList.Count;

            for (int i = 0; i < actionList.Count; i++)
            {
                yield return new WaitForSeconds(waitTime);
                actionList[i]?.Invoke();
            }
        }

        GameManager.Instance.IsPaused = false;
        EventManager.Publish(GamePhase.NewTurnPhase);
    }
}
