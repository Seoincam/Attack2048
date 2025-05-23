using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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
        if (EventManager.GetEvent(GameEvent.CountDownPhase) != null && EventManager.GetEvent(GameEvent.CountDownPhase).Count != 0)
        {
            EventManager.Publish(GameEvent.CountDownPhase);
        }

        // 0.3초 후 0이 된 애들이 있다면 실행
        if (EventManager.GetEvent(GameEvent.TriggerPhase) != null && EventManager.GetEvent(GameEvent.TriggerPhase).Count != 0)
        {
            List<UnityAction> actionList = EventManager.GetEvent(GameEvent.TriggerPhase).ToList();

            float waitTime = 0.3f;
            if (actionList.Count > 4) waitTime = 0.1f;


                for (int i = 0; i < actionList.Count; i++)
                {
                    yield return new WaitForSeconds(waitTime);
                    actionList[i]?.Invoke();
                }
        }

        GameManager.Instance.IsPaused = false;
        EventManager.Publish(GameEvent.NewTurn);
    }
}
