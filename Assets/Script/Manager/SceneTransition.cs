using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // 切换场景
    public void LoadSceneWithFade(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        // TODO: 可能有转场动画
    }

    
}
