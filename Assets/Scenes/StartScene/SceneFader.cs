using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;   // 페이드용 CanvasGroup
    public float fadeDuration = 1f;   // 페이드 걸리는 시간(초)

    private void Awake()
    {
        // 혹시 씬을 열 때부터 화면이 가려져 있으면 안 되므로,
        // 초기에 alpha=0으로 해두고 시작해도 좋습니다.
        canvasGroup.alpha = 0f;
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutIn(sceneName));
    }

    private IEnumerator FadeOutIn(string sceneName)
    {
        // (1) Fade Out (alpha: 0 -> 1)
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }

        // (2) 씬 전환
        SceneManager.LoadScene(sceneName);

        // (선택) 새 씬에서도 이 오브젝트가 유지되어 페이드 인을 하고 싶다면
        // DontDestroyOnLoad(gameObject) 등을 고려해야 합니다.
        // 만약 StartScene에서만 쓰고, GameScene에서 페이드 인은 필요없다면 생략 가능.

        // (3) Fade In (alpha: 1 -> 0) - 새 씬에서도 페이드 아웃 화면에서 서서히 보여주려면
        //      아래 코드를 그대로 사용해야 합니다. 다만, 새 씬에도 SceneFader가 존재하거나
        //      DontDestroyOnLoad로 이 오브젝트가 넘어와야 가능.

        /*
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }
        */
    }
}
