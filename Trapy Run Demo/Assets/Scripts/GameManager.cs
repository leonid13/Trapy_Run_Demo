using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
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
    private List<Enemy> enemyList = new List<Enemy>();
    private Player player;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("currentLevel"))
        {
            PlayerPrefs.SetInt("currentLevel", 0);
        }
        if (!PlayerPrefs.HasKey("currentLevelTEXT"))
        {
            PlayerPrefs.SetInt("currentLevelTEXT", 1);
        }
        int savedSceneIndex = PlayerPrefs.GetInt("currentLevel");
        if (SceneManager.GetActiveScene().buildIndex != savedSceneIndex)
        {
            SceneManager.LoadScene(savedSceneIndex);
        }

        faderScript.FadeOutImmediate();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.enabled = false;
        player.GetComponent<BoxCollider>().enabled = false;
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (var e in enemies)
        {
            enemyList.Add(e);
            e.enabled = false;
        }
    }

    private void Start()
    {
        int currentLevelTextValue = PlayerPrefs.GetInt("currentLevelTEXT");

        currentLevelText.text = $"Level: {currentLevelTextValue}";

        faderScript.FadeIn(0.5f);
        //PlayerPrefs.DeleteAll();
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

    //PUBLIC Methods
    public void StartTheGame()// called In DisableButtonAfterTap game object
    {
        player.enabled = true;
        player.GetComponent<BoxCollider>().enabled = true;
        foreach (var e in enemyList)
        {
            e.enabled = true;
        }
        tutorialCanvas.gameObject.SetActive(false);
    }

    public void CrossFinishLine()
    {
        player.enabled = false;
        player.GetComponent<NavMeshAgent>().isStopped = true;

        int currentLevelText = PlayerPrefs.GetInt("currentLevelTEXT");
        currentLevelText++;
        PlayerPrefs.SetInt("currentLevelTEXT", currentLevelText);

        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCountInBuildSettings - 1 == SceneManager.GetActiveScene().buildIndex)
        {
            nextScene = 0;
        }
        PlayerPrefs.SetInt("currentLevel", nextScene);

        Debug.Log("scene to load    " + nextScene);
        Invoke(nameof(ActivateYouWinTextAndButtons), 3f);
        StartCoroutine(StartNextSceneInBackground(nextScene));
    }

    public void TryAgain()// called In TryAgainButton
    {
        int currentLevelTextValue = PlayerPrefs.GetInt("currentLevelTEXT");
        if (currentLevelTextValue == 0) { PlayerPrefs.SetInt("currentLevelTEXT", 0); }
        else
        {
            currentLevelTextValue--;
            PlayerPrefs.SetInt("currentLevelTEXT", currentLevelTextValue);
        }
        PlayerPrefs.SetInt("currentLevel", SceneManager.GetActiveScene().buildIndex);

        faderScript.FadeOut(0.4f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
    }
}
