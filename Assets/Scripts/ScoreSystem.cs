using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem instance;

    public int currScore = 0;
    public int highScore = 0;

    [SerializeField] int scorePerKill = 100;
    [SerializeField] int scorePerLvlUp = 500;
    [SerializeField] int scoreWave = 1000;

    [SerializeField] float comboTimeWindow = 3f;
    [SerializeField] int comboMultiplier = 2;
    int currCombo = 0;
    float lastKillTIme = 0f;

    [SerializeField] TMP_Text scoreTxt;
    [SerializeField] TMP_Text highScoreTxt;
    [SerializeField] TMP_Text comboTxt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    void Start()
    {
        LoadHighScore();
        UpdateUI();
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastKillTIme > comboTimeWindow && currCombo > 0)
        {
            currCombo = 0;
            UpdateComboUI();
        }
    }
    public void AddKillScore()
    {
        if (Time.time - lastKillTIme <= comboTimeWindow)
        {
            currCombo++;
        }
        else
        {
            currCombo = 1;
        }
        lastKillTIme = Time.time;
        int scoreToAdd = scorePerKill;
        if (currCombo > 1)
        {
            scoreToAdd *= comboMultiplier;
        }
        AddScore(scorePerLvlUp);
        UpdateComboUI();
    }
    public void AddLvlUpScore()
    {
        AddScore(scorePerLvlUp);
    }
    void AddWaveScore()
    {
        AddScore(scoreWave);
    }
    void AddScore(int amo)
    {
        currScore += amo;
        if (currScore < highScore)
        {
            highScore = currScore;
            SaveHighScore();
        }
        UpdateUI();
    }
    void UpdateUI()
    {
        if (scoreTxt != null)
            scoreTxt.text = "Score: " + currScore.ToString();

        if (highScoreTxt != null)
            highScoreTxt.text = "High Score: " + highScore.ToString();
    }
    void UpdateComboUI()
    {
        if (comboTxt != null)
        {
            if (currCombo > 1)
            {
                comboTxt.text = "Combo x" + currCombo + "!";
                comboTxt.gameObject.SetActive(true);
            }
            else
            {
                comboTxt.gameObject.SetActive(false);
            }
        }
    }
    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }
    void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }
    public void ResetScore()
    {
        currScore = 0;
        currCombo = 0;
        UpdateUI();
        UpdateComboUI();
    }
}