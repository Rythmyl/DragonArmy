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

    [Header("----- Tower UI Elements -----")]
    public towerHealth towerHealthComponent;    
    public Image towerHPBar;                     
    public TMP_Text towerHPText;  
    
    [Header("----- Player References -----")]
    public GameObject rythmyl;
    public playerController rythmylScript;
    public GameObject rythmylSpawnPos;

    [Header("----- Other UI -----")]
    public GameObject checkpointPopup;

    [Header("----- Game State -----")]
    public bool isPaused;

    float timeScaleOrig;

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
        UpdateTowerHPUI();
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
        UpdateTowerHPUI();
    }

    void InitializeEnemyCount()
    {
        var dragons = Object.FindObjectsByType<dragonAI>(FindObjectsSortMode.None);
        enemyCountFromDragons = dragons.Length;

        UpdateGameGoalUI();
    }

    void UpdateGameGoalUI()
    {
        gameGoalCountText.text = "Enemy Count: " + enemyCountFromDragons.ToString("F0");
    }

    public void updateGameGoal(int amount, bool isDragon = false)
    {
        if (isDragon)
        {
            enemyCountFromDragons += amount;
            if (enemyCountFromDragons < 0) enemyCountFromDragons = 0;
        }

        UpdateGameGoalUI();

        if (enemyCountFromDragons <= 0)
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

    void UpdateTowerHPUI()
    {
        if (towerHealthComponent == null || towerHPBar == null || towerHPText == null) return;

        float healthPercent = (float)towerHealthComponent.currentHealth / towerHealthComponent.maxHealth;
        towerHPBar.fillAmount = healthPercent;
        towerHPText.text = $"{towerHealthComponent.currentHealth} / {towerHealthComponent.maxHealth}";
    }
}
