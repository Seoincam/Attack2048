using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    void Start()
    {
        StartCoroutine(LoadSceneAsync("2048Game"));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        // 씬 전환되면서 미리 ObjectPooler에 오브젝트 생성해서 게임 중 성능 최적화
        ObjectPoolManager.Instance.Init(); 

        float timer = 1.2f;
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f * 0.2f + ObjectPoolManager.Instance._initProgress * 0.8f);
            // 너무 빨리 전환 방지
            if (timer > 0) progress *= 0.25f;
            progressBar.value = progress;

            //로딩 끝나면 전환
            if (progress >= 0.9f && ObjectPoolManager.Instance.IsInitialized) operation.allowSceneActivation = true;

            timer -= Time.deltaTime;
            yield return null;
        }
    }
    
    
}
