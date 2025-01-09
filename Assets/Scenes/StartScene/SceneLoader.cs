using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // ��ȯ�� �� �̸�
    [SerializeField] private string sceneName = "SampleScene";

    // ��ư���� �� �޼��带 ȣ���ϸ� �� ��ȯ
    public void LoadTargetScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
