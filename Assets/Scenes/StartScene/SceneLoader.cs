using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // 전환할 씬 이름
    [SerializeField] private string sceneName = "SampleScene";

    // 버튼에서 이 메서드를 호출하면 씬 전환
    public void LoadTargetScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
