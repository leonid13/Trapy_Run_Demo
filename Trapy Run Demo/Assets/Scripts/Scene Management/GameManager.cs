using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EnemyWaveSpawner enemyWaveSpawner;
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
    private bool hasIncreaseThisSession = false;


    private const float ratioStepBetweenLevels = 0.125f;
    [HideInInspector] public float[] enemyRatios = new float[5];

    private void Awake()
    {
        faderScript.FadeOutImmediate();
        SettupPlayerPrefs();
        enemyRatios[4] = 1000;// just to be bigger than [3]

        int savedSceneIndex = PlayerPrefs.GetInt("currentLevel");
        if (SceneManager.GetActiveScene().buildIndex != savedSceneIndex)
        {
            SceneManager.LoadScene(savedSceneIndex);
        }

        // have to disable some stuff untill StartTheGame() is called from DisableButtonAfterTap script
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.enabled = false;
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (var e in enemies)
        {
            enemyList.Add(e);
            e.enabled = false;
        }

        if (enemyWaveSpawner == null) enemyWaveSpawner = FindObjectOfType<EnemyWaveSpawner>();
        enemyWaveSpawner.enabled = false;
    }

    private void SettupPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("currentLevel")) { PlayerPrefs.SetInt("currentLevel", 0); }
        if (!PlayerPrefs.HasKey("currentLevelTEXT")) { PlayerPrefs.SetInt("currentLevelTEXT", 1); }
        if (!PlayerPrefs.HasKey("enemyRatios[0]")) { PlayerPrefs.SetFloat("enemyRatios[0]", 1); }
        if (!PlayerPrefs.HasKey("enemyRatios[1]")) { PlayerPrefs.SetFloat("enemyRatios[1]", 0); }
        if (!PlayerPrefs.HasKey("enemyRatios[2]")) { PlayerPrefs.SetFloat("enemyRatios[2]", 0); }
        if (!PlayerPrefs.HasKey("enemyRatios[3]")) { PlayerPrefs.SetFloat("enemyRatios[3]", 0); }

        enemyRatios[0] = PlayerPrefs.GetFloat("enemyRatios[0]");
        enemyRatios[1] = PlayerPrefs.GetFloat("enemyRatios[1]");
        enemyRatios[2] = PlayerPrefs.GetFloat("enemyRatios[2]");
        enemyRatios[3] = PlayerPrefs.GetFloat("enemyRatios[3]");
    }

    private void Start()
    {
        int currentLevelTextValue = PlayerPrefs.GetInt("currentLevelTEXT");
        currentLevelText.text = $"Level: {currentLevelTextValue}";

        faderScript.FadeIn(0.5f);
        //PlayerPrefs.DeleteAll();
    }

    // when the player crosses the finish line this method will load the next scene to 90%
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

    //after every successful level the ratios switch from low level enemies to only higher ones
    //          So
    // at run, of the game, 0 the ratios are *1:*0:0:0 respectively for enemy levels 0,1,2,3
    // at the end of the level ratioStepBetweenLevels takes some amount from *1 and puts it in the next
    // level ratio, which is the first *0
    // When 1 gets to 0, we then start taking from the second ratio, and this proccess is repeated
    // after all the enemies are only level 3. This is based on the value of ratioStepBetweenLevels, if
    // its lower the difficulty curve will gradualy increase.
    private void IncreaseDifficulty()
    {
        //Take the biggest of 4 ratios
        int maxIndex = 0;
        for (int i = 0; i <= 3; i++)// get max value
        {
            if (enemyRatios[i] != 0)
            {
                maxIndex = i;
                break;
            }
        }
        //reduce it by ratioStepBetweenLevels amount
        enemyRatios[maxIndex] -= ratioStepBetweenLevels;

        //then find which is the next one to get the ratioStepBetweenLevels amount
        int chosedToIncreaseIndex = 1;
        for (int i = maxIndex; i <= 3; i++)// get min index to increase
        {
            if (enemyRatios[i] <= enemyRatios[i + 1])
            {
                chosedToIncreaseIndex = i;
                if (chosedToIncreaseIndex == maxIndex)
                {
                    continue;
                }
                break;
            }
        }
        // increase the chosen one
        enemyRatios[chosedToIncreaseIndex] += ratioStepBetweenLevels;

        // save the values
        PlayerPrefs.SetFloat("enemyRatios[0]", enemyRatios[0]);
        PlayerPrefs.SetFloat("enemyRatios[1]", enemyRatios[1]);
        PlayerPrefs.SetFloat("enemyRatios[2]", enemyRatios[2]);
        PlayerPrefs.SetFloat("enemyRatios[3]", enemyRatios[3]);
    }

    private void ActivateYouWinTextAndButtons()
    {
        endGameCanvas.gameObject.SetActive(true);
        youWinText.SetActive(true);
        tryAgainButton.gameObject.SetActive(true);
        nextLevelButton.gameObject.SetActive(true);
    }

    //PUBLIC Methods
    public void StartTheGame()// called In DisableButtonAfterTap script
    {
        enemyWaveSpawner.enabled = true;
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
        enemyWaveSpawner.StopSpawningEnemies();
        player.enabled = false;

        int currentLevelText = PlayerPrefs.GetInt("currentLevelTEXT");
        currentLevelText++;
        hasIncreaseThisSession = true;
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
        // if the player decides to try again the level, we have to reduce the currentLevelTEXT,
        // because it was previously when he CrossFinishLine()
        int currentLevelTextValue = PlayerPrefs.GetInt("currentLevelTEXT");
        if (currentLevelTextValue == 0) { PlayerPrefs.SetInt("currentLevelTEXT", 1); }
        else
        {
            if (hasIncreaseThisSession)
            {
                currentLevelTextValue--;
                PlayerPrefs.SetInt("currentLevelTEXT", currentLevelTextValue);
            }
        }
        PlayerPrefs.SetInt("currentLevel", SceneManager.GetActiveScene().buildIndex);

        faderScript.FadeOutImmediate();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ActivateNextScene()// called in NextLevelButton
    {
        IncreaseDifficulty();
        faderScript.FadeOut(0.4f);
        canActivateNextScene = true;
    }

    public void GameOver() // called in Die() in Player script
    {
        endGameCanvas.gameObject.SetActive(true);
        enemyWaveSpawner.StopSpawningEnemies();
        tryAgainButton.gameObject.SetActive(true);
        gameOverText.SetActive(true);
    }
}
