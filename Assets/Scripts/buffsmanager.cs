using UnityEngine;
using System.Collections;

public class buffs : MonoBehaviour
{
    public enum BuffType { Resistance, Heal, Speed, Strength }

    [Range(0f, 1f)][SerializeField] float dropChance = 0.33f;
    [Range(1, 50)][SerializeField] int healAmo = 15;
    [Range(0.1f, 0.5f)][SerializeField] float resistAmo = 0.25f;
    [Range(1, 5)][SerializeField] int speedBoost = 2;
    [Range(1, 10)][SerializeField] int strengthBoost = 5;

    [Range(5f, 30)][SerializeField] float resistDur = 15f;
    [Range(5f, 30)][SerializeField] float speedDur = 15f;
    [Range(5f, 30)][SerializeField] float strengthDur = 15f;

    [SerializeField] GameObject resistBuff;
    [SerializeField] GameObject healBuff;
    [SerializeField] GameObject speedBuff;
    [SerializeField] GameObject strengthBuff;

    bool hasResist;
    bool hasSpeedBst;
    bool hasStrengthBst;

    Coroutine resistCoroutine;
    Coroutine speedCoroutine;
    Coroutine strengthCoroutine;

    public void DropBuff(Vector3 dropPosition)
    {
        if (Random.value <= dropChance)
        {
            BuffType buffType = (BuffType)Random.Range(0, 4);
            SpawnBuff(buffType, dropPosition);
        }
    }

    void SpawnBuff(BuffType buffType, Vector3 position)
    {
        GameObject buff = null;

        switch (buffType)
        {
            case BuffType.Resistance:
                buff = resistBuff;
                break;
            case BuffType.Heal:
                buff = healBuff;
                break;
            case BuffType.Speed:
                buff = speedBuff;
                break;
            case BuffType.Strength:
                buff = strengthBuff;
                break;
        }
        if (buff != null)
        {
            GameObject buffed = Instantiate(buff, position + Vector3.up, Quaternion.identity);
            BuffPickup buffpickup = buffed.GetComponent<BuffPickup>();
            if (buffpickup != null)
            {
                buffpickup.Initialize(buffType, this);
            }
        }
    }

    public void ApplyBuff(BuffType buffType)
    {
        switch (buffType)
        {
            case BuffType.Resistance:
                ApplyResistance();
                break;
            case BuffType.Heal:
                ApplyHeal();
                break;
            //case BuffType.Speed:
                //ApplySpeed();
               // break;
            //case BuffType.Strength:
               // ApplyStrength();
                //break;
        }
    }

    void ApplyResistance()
    {
        if (resistCoroutine != null)
            StopCoroutine(resistCoroutine);
        hasResist = true;
        resistCoroutine = StartCoroutine(ResistBuffCoroutine());
    }

    void ApplyHeal()
    {
        // Changed from playerController to towerHealth
        towerHealth tower = GetComponent<towerHealth>();
        if (tower != null)
        {
            tower.TakeDamage(-healAmo); // Heal tower by negative damage
        }
    }

    // Commented out because speed buff currently not supported on tower
    /*
    void ApplySpeed()
    {
        playerController player = GetComponent<playerController>();
        if (player != null)
        {
            if (speedCoroutine != null)
            {
                StopCoroutine(speedCoroutine);
                player.ModifySpeed(-speedBoost);
            }
            hasSpeedBst = true;
            player.ModifySpeed(speedBoost);
            speedCoroutine = StartCoroutine(SpeedBuffCoroutine(player));
        }
    }
    */

    // Commented out because strength buff currently not supported on tower
    /*
    void ApplyStrength()
    {
        playerController player = GetComponent<playerController>();
        if (player != null)
        {
            if (strengthCoroutine != null)
            {
                StopCoroutine(strengthCoroutine);
                player.ModifyDMG(-strengthBoost);
            }
            hasStrengthBst = true;
            player.ModifyDMG(strengthBoost);
            strengthCoroutine = StartCoroutine(StrengthBuffCoroutine(player));
        }
    }
    */

    IEnumerator ResistBuffCoroutine()
    {
        yield return new WaitForSeconds(resistDur);
        hasResist = false;
    }

    // Commented out because speed buff coroutine is not used
    /*
    IEnumerator SpeedBuffCoroutine(playerController player)
    {
        yield return new WaitForSeconds(speedDur);
        player.ModifySpeed(-speedBoost);
        hasSpeedBst = false;
    }
    */

    // Commented out because strength buff coroutine is not used
    /*
    IEnumerator StrengthBuffCoroutine(playerController player)
    {
        yield return new WaitForSeconds(strengthDur);
        player.ModifyDMG(-strengthBoost);
        hasStrengthBst = false;
    }
    */

    public float GetResistMultiply()
    {
        return hasResist ? (1f - resistAmo) : 1f;
    }

    public bool hasResistance()
    {
        return hasResist;
    }
}
