using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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
    [SerializeField] private TMP_Text towerLifeText;

    [Header("----- Tower UI Elements -----")]
    [SerializeField] private towerHealth towerHealthComponent;
    [SerializeField] private Image towerHPBar;
    [SerializeField] private TMP_Text towerHPText;

    [Header("----- Start Pop Up -----")]
    [SerializeField] private GameObject startPopUp;

    [Header("----- Game State -----")]
    public bool isPaused;

    [Header("----- Tower Lives Settings -----")]
    [SerializeField, Range(1, 10)] private int towerLives;

    private int enemyCountFromDragons;
    private float timeScaleOrig;

    private bool gameOver = false;

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
        UpdateTowerLifeUI();

        if (startPopUp != null)
            startPopUp.SetActive(false);

        if (PlayerSkills.instance == null)
            gameObject.AddComponent<PlayerSkills>();

        if (Upgrademanager.instance == null)
            gameObject.AddComponent<Upgrademanager>();

        if (ScoreSystem.instance == null)
            gameObject.AddComponent<ScoreSystem>();
    }

    private void Start()
    {
        if (audioManager.Instance != null)
        {
            audioManager.Instance.musicSource.Stop();
            audioManager.Instance.PlayGameMusic();
        }

        StartCoroutine(ShowStartPopUpRoutine());
    }

    private IEnumerator ShowStartPopUpRoutine()
    {
        if (startPopUp != null)
        {
            startPopUp.SetActive(true);
            yield return new WaitForSeconds(3f);
            startPopUp.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameOver) return;

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
        dragonSpawner spawner = FindFirstObjectByType<dragonSpawner>();
        if (spawner != null)
        {
            int totalQuantity = 0;
            foreach (var babySpawner in spawner.babyDragonSpawners)
                totalQuantity += babySpawner.quantityToSpawn;

            foreach (var boarSpawner in spawner.dragonBoarSpawners)
                totalQuantity += boarSpawner.quantityToSpawn;

            foreach (var bossSpawner in spawner.bossSpawners)
                totalQuantity += bossSpawner.quantityToSpawn;

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
        if (gameGoalCountText != null)
            gameGoalCountText.text = enemyCountFromDragons.ToString("F0");
    }

    private void UpdateTowerLifeUI()
    {
        if (towerLifeText != null)
        {
            towerLifeText.text = towerLives.ToString("F0");
        }
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
        updateGameGoal(-1, true);
    }

    public void OnTowerDestroyed()
    {
        if (gameOver) return;

        towerLives--;
        UpdateTowerLifeUI();

        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);

        Transform respawnButton = menuLose.transform.Find("Respawn");
        if (respawnButton != null)
        {
            if (towerLives > 0)
            {
                respawnButton.gameObject.SetActive(true);
            }
            else
            {
                respawnButton.gameObject.SetActive(false);
                gameOver = true;

                if (towerHealthComponent != null)
                    towerHealthComponent.gameObject.SetActive(false);
            }
        }
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
        if (gameOver) return;

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

        if (towerHealthComponent.currentHealth <= 0 && towerHealthComponent.gameObject.activeSelf)
        {
            towerHealthComponent.gameObject.SetActive(false);
            OnTowerDestroyed();
        }
    }

    public void RespawnTower()
    {
        if (gameOver) return;

        if (towerLives > 0 && towerHealthComponent != null)
        {
            towerHealthComponent.SetHealth(towerHealthComponent.maxHealth);
            UpdateTowerHPUI();

            towerHealthComponent.gameObject.SetActive(true);

            stateUnpause();
        }
    }
}