using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class ExpManager : MonoBehaviour
{

    public static ExpManager instance;
    [SerializeField] AnimationCurve ExpCurve;

    int currLvl, totalExp;
    int prevLvlExp, nextLvlExp;

    [SerializeField] TextMeshProUGUI lvlText;
    [SerializeField] TextMeshProUGUI expText;
    [SerializeField] Image expFill;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        currLvl = 0;
        totalExp = Mathf.Max(0, totalExp);
        UpdateLvl();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddExp (int amount)
    {
        amount = Mathf.Max(0, amount);
        totalExp += amount;
        CheckForLevelUp();
        UpdateInterface();
    }

    void CheckForLevelUp()
    {
        while (totalExp >= nextLvlExp && nextLvlExp > 0)
        {
            currLvl++;
            UpdateLvl();
        }
    
    }

    void UpdateLvl()
    {
        prevLvlExp = Mathf.Max(0, (int)ExpCurve.Evaluate(currLvl));
        nextLvlExp = Mathf.Max(prevLvlExp + 1, (int)ExpCurve .Evaluate(currLvl + 1));
        UpdateInterface();
    }

    void UpdateInterface()
    {
        int start = Mathf.Max(0, totalExp - prevLvlExp);
        int end = Mathf.Max(1, nextLvlExp - prevLvlExp);

        lvlText.text = currLvl.ToString();
        expText.text = $"{start} exp / {end} exp";
        expFill.fillAmount = (float)start / end;
    }
}
