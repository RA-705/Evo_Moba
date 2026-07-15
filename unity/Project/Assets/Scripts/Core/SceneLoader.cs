using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Evo.UnityClient
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        public GameObject LoadingScreen;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (LoadingScreen != null)
                LoadingScreen.SetActive(false);
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        public void LoadScene(int sceneIndex)
        {
            StartCoroutine(LoadSceneAsync(sceneIndex));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            if (LoadingScreen != null)
                LoadingScreen.SetActive(true);

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                Debug.Log($"Loading {sceneName}: {Mathf.RoundToInt(progress * 100)}%");

                if (operation.progress >= 0.9f)
                {
                    yield return new WaitForSeconds(0.5f);
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

            if (LoadingScreen != null)
                LoadingScreen.SetActive(false);
        }

        private IEnumerator LoadSceneAsync(int sceneIndex)
        {
            if (LoadingScreen != null)
                LoadingScreen.SetActive(true);

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                Debug.Log($"Loading scene {sceneIndex}: {Mathf.RoundToInt(progress * 100)}%");

                if (operation.progress >= 0.9f)
                {
                    yield return new WaitForSeconds(0.5f);
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

            if (LoadingScreen != null)
                LoadingScreen.SetActive(false);
        }
    }
}
