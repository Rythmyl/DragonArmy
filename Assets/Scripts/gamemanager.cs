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

    [Header("----- Tower UI Elements -----")]
    public towerHealth towerHealthComponent;
    public Image towerHPBar;
    public TMP_Text towerHPText;

    [Header("----- Game State -----")]
    public bool isPaused;

    float timeScaleOrig;

    int enemyCountFromDragons;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        timeScaleOrig = Time.timeScale;

        InitializeEnemyCount();
        UpdateTowerHPUI();
    }

    void Start()
    {
        if (audioManager.Instance != null)
        {
            audioManager.Instance.musicSource.Stop();
            audioManager.Instance?.PlayGameMusic();
        }
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
        gameGoalCountText.text = enemyCountFromDragons.ToString("F0");
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

        if (audioManager.Instance != null)
            audioManager.Instance.DuckMusic();
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

        if (audioManager.Instance != null)
            audioManager.Instance.UnduckMusic();
    }

    void UpdateTowerHPUI()
    {
        if (towerHealthComponent == null || towerHPBar == null || towerHPText == null) return;

        float healthPercent = (float)towerHealthComponent.currentHealth / towerHealthComponent.maxHealth;
        towerHPBar.fillAmount = healthPercent;
        towerHPText.text = $"{towerHealthComponent.currentHealth} / {towerHealthComponent.maxHealth}";
    }

    public void RespawnTower()
    {
        if (towerHealthComponent != null)
        {
            towerHealthComponent.SetHealth(towerHealthComponent.maxHealth);
            UpdateTowerHPUI();

            towerHealthComponent.gameObject.SetActive(true);
        }
    }
}