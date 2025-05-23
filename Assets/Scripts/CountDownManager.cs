using System.Collections;
using UnityEngine;

public class CountDownManager : MonoBehaviour
{
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private Fade _darkBackground;

    public void CountDown()
    {
        StartCoroutine(CountDownCoroutine());
    }

    private IEnumerator CountDownCoroutine()
    {
        // 0.3초 후 카운트 다운 실행
        if (EventManager.GetEvent(GameEvent.CountDownPhase) != null && EventManager.GetEvent(GameEvent.CountDownPhase).Count != 0)
        {
            EventManager.Publish(GameEvent.CountDownPhase);
        }

        // 0.3초 후 0이 된 애들이 있다면 실행
        if (EventManager.GetEvent(GameEvent.TriggerPhase) != null && EventManager.GetEvent(GameEvent.TriggerPhase).Count != 0)
        {
            yield return new WaitForSeconds(0.2f);
            EventManager.Publish(GameEvent.TriggerPhase);
            yield return new WaitForSeconds(0.2f);
        }

        // _mainCanvas.sortingOrder = 6;
        // _darkBackground.FadeIn();

        // _mainCanvas.sortingOrder = 3;
        // _darkBackground.FadeOut();

        // yield return new WaitForSeconds(0.1f);
        EventManager.Publish(GameEvent.NewTurn);
    }

}
