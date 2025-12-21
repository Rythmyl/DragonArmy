using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkills : MonoBehaviour
{
    public static PlayerSkills instance;

    public int skillPoints = 0;
    [SerializeField] TMP_Text skillPointstxt;

    public int luckLvl = 0;
    [SerializeField] int luckMaxLvl = 5;
    [SerializeField] float luckBonus = 0.05f;

    public int bombDropLvl = 0;
    [SerializeField] int bombDropMaxLvl = 5;
    [SerializeField] float bombDropChance = 0.05f;

    public int potionDropLvl = 0;
    [SerializeField] int potionDropMaxLvl = 5;
    [SerializeField] float potionDropChance = 0.05f;
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
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddSkillPoint()
    {
        skillPoints++;
        UpdateUI();
        AutoAllocatePoints();
    }

    void AutoAllocatePoints()
    {
        while (skillPoints > 0)
        {
            bool allocated = false;
            if (luckLvl < luckMaxLvl)
            {
                luckMaxLvl++;
                skillPoints--;
                allocated = true;
                UpdateBuffDropChance();
            }
            else if (bombDropLvl < bombDropMaxLvl)
            {
                bombDropLvl++;
                skillPoints--;
                allocated = true;
            }
            else if (potionDropLvl < potionDropMaxLvl)
            { 
                potionDropLvl++;
                skillPoints--;
                allocated = true;
            }
            if (!allocated)
                break;
        }
        UpdateUI();
    }
    void UpdateBuffDropChance()
    {
        buffs buffSystem = FindFirstObjectByType<buffs>();
        if (buffSystem != null && luckLvl > 0)
        {
            float totalBonus = luckLvl * luckBonus;
            buffSystem.dropChance = Mathf.Min(0.33f + totalBonus, 1f);
        }
    }

    public float GetBombDropChance()
    {
        return bombDropLvl * bombDropLvl;
    }

    public float GetPotionDropChance()
    {
        return potionDropLvl * potionDropLvl;
    }

    void UpdateUI()
    {
        if (skillPointstxt != null)
            skillPointstxt.text = "Skill Points: " + skillPoints;
    }
}
