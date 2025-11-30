using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [Header("----- UI Menus -----")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [Header("----- UI Elements -----")]
    public TMP_Text gameGoalCountText;
    public Image RythmylHPBar;
    public GameObject playerDamagePanel;

    [Header("----- Player References -----")]
    public GameObject rythmyl;
    public playerController rythmylScript;
    public GameObject rythmylSpawnPos;

    [Header("----- Other UI -----")]
    public GameObject checkpointPopup;

    [Header("----- Game State -----")]
    public bool isPaused;

    float timeScaleOrig;

    int enemyCountFromEnemies;
    int enemyCountFromGolems;
    int enemyCountFromBugs;
    int enemyCountFromDragons;

    private Vector3 lastCheckpointPosition;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        timeScaleOrig = Time.timeScale;

        rythmyl = GameObject.FindWithTag("Rythmyl");
        rythmylScript = rythmyl.GetComponent<playerController>();

        rythmylSpawnPos = GameObject.FindWithTag("Rythmyl Spawn Pos");

        if (rythmylSpawnPos != null)
            lastCheckpointPosition = rythmylSpawnPos.transform.position;
        else
            lastCheckpointPosition = rythmyl.transform.position;

        InitializeEnemyCount();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
                menuActive = null;
            }
        }
    }

    void InitializeEnemyCount()
    {
        var enemies = Object.FindObjectsByType<enemyAI>(FindObjectsSortMode.None);
        enemyCountFromEnemies = enemies.Length;

        var golems = Object.FindObjectsByType<golemAI>(FindObjectsSortMode.None);
        enemyCountFromGolems = golems.Length;

        var bugs = Object.FindObjectsByType<bugAI>(FindObjectsSortMode.None);
        enemyCountFromBugs = bugs.Length;

        var dragons = Object.FindObjectsByType<dragonAI>(FindObjectsSortMode.None);
        enemyCountFromDragons = dragons.Length;

        UpdateGameGoalUI();
    }

    void UpdateGameGoalUI()
    {
        int totalCount = enemyCountFromEnemies + enemyCountFromGolems + enemyCountFromBugs + enemyCountFromDragons;
        gameGoalCountText.text = "Enemy Count: " + totalCount.ToString("F0");
    }

    public void updateGameGoal(int amount, bool isGolem = false, bool isBug = false, bool isDragon = false)
    {
        if (isBug)
        {
            enemyCountFromBugs += amount;
            if (enemyCountFromBugs < 0) enemyCountFromBugs = 0;
        }
        else if (isGolem)
        {
            enemyCountFromGolems += amount;
            if (enemyCountFromGolems < 0) enemyCountFromGolems = 0;
        }
        else if (isDragon)
        {
            enemyCountFromDragons += amount;
            if (enemyCountFromDragons < 0) enemyCountFromDragons = 0;
        }
        else
        {
            enemyCountFromEnemies += amount;
            if (enemyCountFromEnemies < 0) enemyCountFromEnemies = 0;
        }

        UpdateGameGoalUI();

        if (enemyCountFromEnemies + enemyCountFromGolems + enemyCountFromBugs + enemyCountFromDragons <= 0)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void DecrementDragonCount()
    {
        updateGameGoal(-1, isDragon: true);
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    public void UpdateCheckpoint(Vector3 checkpointPos)
    {
        lastCheckpointPosition = checkpointPos;
        if (checkpointPopup != null)
        {
            checkpointPopup.SetActive(true);
            Invoke(nameof(HideCheckpointPopup), 2f);
        }
    }

    void HideCheckpointPopup()
    {
        if (checkpointPopup != null)
            checkpointPopup.SetActive(false);
    }

    public void RespawnPlayer()
    {
        if (rythmyl != null)
        {
            rythmyl.transform.position = lastCheckpointPosition;

            if (rythmylScript != null)
            {
                rythmylScript.ResetHealth();
            }

            Rigidbody rb = rythmyl.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero; 
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
