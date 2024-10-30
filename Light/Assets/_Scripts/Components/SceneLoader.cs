using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    // （可选）引用进度条或文本
    public Slider progressBar;
    public Text progressText;
    public int sceneIndex;

    void Start()
    {
        // 开始异步加载
        StartCoroutine(LoadGameScene());
    }

    IEnumerator LoadGameScene()
    {
        // 开始异步加载 GameScene
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        // 禁用自动跳转，等待加载完成后手动跳转
        operation.allowSceneActivation = false;

        // 更新进度条和文本
        while (!operation.isDone)
        {
            // 获取加载进度，范围在 0.0f 到 0.9f 之间
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // 更新进度条
            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            // 更新进度文本
            if (progressText != null)
            {
                progressText.text = (progress * 100f).ToString("F0") + "%";
            }

            // 当加载进度达到 0.9f 时，表示加载完成
            if (operation.progress >= 0.9f)
            {
                // （可选）添加延迟，展示加载完成的界面
                yield return new WaitForSeconds(0.5f);

                // 允许场景跳转
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}