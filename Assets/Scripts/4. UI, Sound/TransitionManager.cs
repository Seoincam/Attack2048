using DG.Tweening;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] Transform transitionCircleA;
    [SerializeField] Transform transitionCircleB;

    public void StartTransition()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(transitionCircleA.DOScale(new Vector3(1.2f, 1.2f, 1.2f), .5f)
            .SetEase(Ease.OutBack));
        sequence.Join(transitionCircleB.DOScale(new Vector3(1.7f, 1.7f, 1.7f), .7f)
            .SetEase(Ease.OutBack));

        
    }
}
