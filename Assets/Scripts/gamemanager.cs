using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [Header("----- UI Menus -----")]
    [SerializeField] private GameObject menuActive;
    [SerializeField] private GameObject menuPause;
    [SerializeField] private GameObject menuWin;
    [SerializeField] private GameObject menuLose;

    [Header("----- UI Elements -----")]
    [SerializeField] private TMP_Text gameGoalCountText;

    [Header("----- Tower UI Elements -----")]
    [SerializeField] private towerHealth towerHealthComponent;
    [SerializeField] private Image towerHPBar;
    [SerializeField] private TMP_Text towerHPText;

    [Header("----- Game State -----")]
    public bool isPaused;

    private int enemyCountFromDragons;

    private float timeScaleOrig;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        timeScaleOrig = Time.timeScale;

        InitializeEnemyCountFromSpawner();
        UpdateTowerHPUI();
    }

    private void Start()
    {
        if (audioManager.Instance != null)
        {
            audioManager.Instance.musicSource.Stop();
            audioManager.Instance.PlayGameMusic();
        }
    }

    private void Update()
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

    private void InitializeEnemyCountFromSpawner()
    {
        dragonSpawner spawner = Object.FindFirstObjectByType<dragonSpawner>();
        if (spawner != null)
        {
            int totalQuantity = 0;

            foreach (var babySpawner in spawner.babyDragonSpawners)
            {
                totalQuantity += babySpawner.quantityToSpawn;
            }
            foreach (var boarSpawner in spawner.dragonBoarSpawners)
            {
                totalQuantity += boarSpawner.quantityToSpawn;
            }
            foreach (var bossSpawner in spawner.bossSpawners)
            {
                totalQuantity += bossSpawner.quantityToSpawn;
            }

            enemyCountFromDragons = totalQuantity;
        }
        else
        {
            enemyCountFromDragons = 0;
        }
        UpdateGameGoalUI();
    }

    private void UpdateGameGoalUI()
    {
        gameGoalCountText.text = enemyCountFromDragons.ToString("F0");
    }

    public void updateGameGoal(int amount, bool isDragon = false)
    {
        if (isDragon)
        {
            enemyCountFromDragons += amount;
            if (enemyCountFromDragons < 0) enemyCountFromDragons = 0;
            UpdateGameGoalUI();

            if (enemyCountFromDragons <= 0)
            {
                statePause();
                menuActive = menuWin;
                menuActive.SetActive(true);
            }
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

    private void UpdateTowerHPUI()
    {
        if (towerHealthComponent == null || towerHPBar == null || towerHPText == null) return;

        float healthPercent = (float)towerHealthComponent.currentHealth / towerHealthComponent.maxHealth;
        towerHPBar.fillAmount = healthPercent;
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