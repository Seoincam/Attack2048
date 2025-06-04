using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    Image _image;

    void Awake()
    {
        _image = GetComponent<Image>();
    }


    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInCoroutine());
    }
    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        Color origin = _image.color;
        float alpha = 0;
        origin.a = alpha;

        while (alpha < 0.9f)
        {
            alpha += Time.deltaTime / 0.35f;
            origin.a = alpha;
            _image.color = origin;
            yield return null;
        }

        origin.a = 0.9f;
        _image.color = origin;
    }

    private IEnumerator FadeOutCoroutine()
    {
        Color origin = _image.color;
        float alpha = origin.a;
        origin.a = alpha;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime / 0.35f;
            origin.a = alpha;
            _image.color = origin;
            yield return null;
        }

        origin.a = 0;
        _image.color = origin;
    }
}
