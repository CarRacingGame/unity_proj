using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;   // ���̵�� CanvasGroup
    public float fadeDuration = 1f;   // ���̵� �ɸ��� �ð�(��)

    private void Awake()
    {
        // Ȥ�� ���� �� ������ ȭ���� ������ ������ �� �ǹǷ�,
        // �ʱ⿡ alpha=0���� �صΰ� �����ص� �����ϴ�.
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

        // (2) �� ��ȯ
        SceneManager.LoadScene(sceneName);

        // (����) �� �������� �� ������Ʈ�� �����Ǿ� ���̵� ���� �ϰ� �ʹٸ�
        // DontDestroyOnLoad(gameObject) ���� ����ؾ� �մϴ�.
        // ���� StartScene������ ����, GameScene���� ���̵� ���� �ʿ���ٸ� ���� ����.

        // (3) Fade In (alpha: 1 -> 0) - �� �������� ���̵� �ƿ� ȭ�鿡�� ������ �����ַ���
        //      �Ʒ� �ڵ带 �״�� ����ؾ� �մϴ�. �ٸ�, �� ������ SceneFader�� �����ϰų�
        //      DontDestroyOnLoad�� �� ������Ʈ�� �Ѿ�;� ����.

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
