using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Canvas tutorialCanvas;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private Canvas endGameCanvas;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private GameObject youWinText;
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Fader faderScript;
    private bool canActivateNextScene = false;
    private bool canRealoadCurrentScene = false;

    private void Awake()
    {
        int savedSceneIndex = PlayerPrefs.GetInt("currentLevel");
        if (SceneManager.GetActiveScene().buildIndex != savedSceneIndex)
        {
            SceneManager.LoadScene(savedSceneIndex);
        }
        int currentLevelTextValue = PlayerPrefs.GetInt("currentLevelTEXT");
        currentLevelText.text = $"Level: {currentLevelTextValue + 1}";

        faderScript.FadeOutImmediate();
    }

    private void Start()
    {
        faderScript.FadeIn(0.3f);
        Time.timeScale = 0f;
    }

    private void ActivateYouWinTextAndButtons()
    {
        endGameCanvas.gameObject.SetActive(true);
        youWinText.SetActive(true);
        tryAgainButton.gameObject.SetActive(true);
        nextLevelButton.gameObject.SetActive(true);
    }

    private IEnumerator StartNextSceneInBackground(int sceneToLoad)
    {
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                if (canActivateNextScene)
                    asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    private IEnumerator RealoadThisSceneInBackground()
    {
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                if (canRealoadCurrentScene)
                    asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    //PUBLIC Methods
    public void StartTheGame()// called In WholeScreenButton game object
    {
        Time.timeScale = 1f;
        tutorialCanvas.gameObject.SetActive(false);
    }

    public void CrossFinishLine()
    {
        int currentLevelText = PlayerPrefs.GetInt("currentLevelTEXT");
        PlayerPrefs.SetInt("currentLevelTEXT", currentLevelText++);

        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.GetSceneByBuildIndex(nextScene) == null)
        {
            nextScene = 0;
        }
        PlayerPrefs.SetInt("currentLevel", nextScene);

        Invoke(nameof(ActivateYouWinTextAndButtons), 3.5f);
        Debug.Log("called 000");
        StartCoroutine(StartNextSceneInBackground(nextScene));
        Debug.Log("Called 111");
        StartCoroutine(RealoadThisSceneInBackground());
        Debug.Log("Called 222");
    }

    public void TryAgain()// called In TryAgainButton
    {
        int currentLevelTextValue = PlayerPrefs.GetInt("currentLevelTEXT");
        if (currentLevelTextValue == 0) { PlayerPrefs.SetInt("currentLevelTEXT", 0); }
        else
        { PlayerPrefs.SetInt("currentLevelTEXT", currentLevelTextValue--); }
        PlayerPrefs.SetInt("currentLevel", SceneManager.GetActiveScene().buildIndex);

        faderScript.FadeOut(0.2f);
        canRealoadCurrentScene = true;
    }

    public void ActivateNextScene()// called in NextLevelButton
    {
        faderScript.FadeOut(0.2f);
        canActivateNextScene = true;
    }

    public void GameOver() // called in Die() in Player script
    {
        tryAgainButton.gameObject.SetActive(true);
        gameOverText.SetActive(true);
        StartCoroutine(RealoadThisSceneInBackground());
    }
}
