using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerController : MonoBehaviour, IDamage, IPickup
{
    public enum WeaponType { Gun, Staff }

    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Header("----- Stats -----")]
    [Range(1, 100)][SerializeField] int HP;
    [Range(3, 6)][SerializeField] int speed;
    [Range(2, 5)][SerializeField] int sprintMod;
    [Range(5, 20)][SerializeField] int JumpSpeed;
    [Range(1, 3)][SerializeField] int maxJumps;
    [Range(15, 50)][SerializeField] int gravity;

    [Header("----- Weapons -----")]
    [SerializeField] List<gunStats> weaponList = new List<gunStats>();
    [SerializeField] List<WeaponType> weaponTypes = new List<WeaponType>();
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject staffModel;

    int shootDamage;
    int shootDist;
    float shootRate;

    [Header("----- Audio -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audStep;
    [Range(0, 1)][SerializeField] float audStepVol;
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;
    int weaponListPos;

    float shootTimer;

    bool isSprinting;
    bool isPlayingStep;

    void Start()
    {
        HPOrig = HP;
        respawnRythmyl();

        if (gunModel != null) gunModel.SetActive(true);
        if (staffModel != null) staffModel.SetActive(false);
    }

    void Update()
    {
        if (!gamemanager.instance.isPaused)
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

            shootTimer += Time.deltaTime;

            movement();
        }
        sprint();
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            if (moveDir.normalized.magnitude > 0.3f && !isPlayingStep)
            {
                StartCoroutine(playStep());
            }
            playerVel = Vector3.zero;
            jumpCount = 0;
        }
        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();
        controller.Move(playerVel * Time.deltaTime);

        if (Input.GetButton("Fire1") && weaponList.Count > 0 && shootTimer >= shootRate)
        {
            shoot();
        }
        selectWeapon();
    }

    IEnumerator playStep()
    {
        isPlayingStep = true;
        aud.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVol);

        if (isSprinting)
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
        isPlayingStep = false;
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            playerVel.y = JumpSpeed;
            jumpCount++;
            aud.pitch = Random.Range(0.9f, 1.1f);
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }
    }

    void shoot()
    {
        shootTimer = 0;
        gunStats currentWeapon = weaponList[weaponListPos];
        aud.pitch = Random.Range(0.9f, 1.1f);
        aud.PlayOneShot(currentWeapon.shootSound[Random.Range(0, currentWeapon.shootSound.Length)], currentWeapon.shootSoundVol);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                int damageToApply = shootDamage;
                buffs buffed = GetComponent<buffs>();
                if (buffed != null)
                {
                    damageToApply = Mathf.RoundToInt(shootDamage * buffed.GetResistMultiply());
                }
                dmg.takeDamage(damageToApply);
            }

            Instantiate(currentWeapon.hitEffect, hit.point, Quaternion.identity);
        }
    }

    public void takeDamage(int amount)
    {
        buffs buffed = GetComponent<buffs>();
        if (buffed != null)
        {
            amount = Mathf.RoundToInt(amount * buffed.GetResistMultiply());
        }

        HP -= amount;
        updatePlayerUI();
        StartCoroutine(screenFlashDamage());

        aud.pitch = Random.Range(0.9f, 1.1f);
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        if (HP <= 0)
        {
            gamemanager.instance.youLose();
        }
    }

    public void updatePlayerUI()
    {
        gamemanager.instance.RythmylHPBar.fillAmount = (float)HP / HPOrig;
    }

    IEnumerator screenFlashDamage()
    {
        gamemanager.instance.playerDamagePanel.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerDamagePanel.SetActive(false);
    }

    public void getGunStats(gunStats gun)
    {
        weaponList.Add(gun);

        if (gun.gunModel.name.ToLower().Contains("staff"))
            weaponTypes.Add(WeaponType.Staff);
        else
            weaponTypes.Add(WeaponType.Gun);

        weaponListPos = weaponList.Count - 1;

        changeWeapon();
    }

    void changeWeapon()
    {
        shootDamage = weaponList[weaponListPos].shootDamage;
        shootDist = weaponList[weaponListPos].shootDist;
        shootRate = weaponList[weaponListPos].shootRate;

        if (weaponTypes[weaponListPos] == WeaponType.Staff)
        {
            if (staffModel != null)
            {
                staffModel.SetActive(true);
                var staffMeshFilter = staffModel.GetComponent<MeshFilter>();
                var staffMeshRenderer = staffModel.GetComponent<MeshRenderer>();

                var sourceMeshFilter = weaponList[weaponListPos].gunModel.GetComponent<MeshFilter>();
                var sourceMeshRenderer = weaponList[weaponListPos].gunModel.GetComponent<MeshRenderer>();

                if (staffMeshFilter != null && sourceMeshFilter != null)
                    staffMeshFilter.sharedMesh = sourceMeshFilter.sharedMesh;
                if (staffMeshRenderer != null && sourceMeshRenderer != null)
                    staffMeshRenderer.sharedMaterial = sourceMeshRenderer.sharedMaterial;
            }
            if (gunModel != null)
                gunModel.SetActive(false);
        }
        else
        {
            if (gunModel != null)
            {
                gunModel.SetActive(true);
                var gunMeshFilter = gunModel.GetComponent<MeshFilter>();
                var gunMeshRenderer = gunModel.GetComponent<MeshRenderer>();

                var sourceMeshFilter = weaponList[weaponListPos].gunModel.GetComponent<MeshFilter>();
                var sourceMeshRenderer = weaponList[weaponListPos].gunModel.GetComponent<MeshRenderer>();

                if (gunMeshFilter != null && sourceMeshFilter != null)
                    gunMeshFilter.sharedMesh = sourceMeshFilter.sharedMesh;
                if (gunMeshRenderer != null && sourceMeshRenderer != null)
                    gunMeshRenderer.sharedMaterial = sourceMeshRenderer.sharedMaterial;
            }
            if (staffModel != null)
                staffModel.SetActive(false);
        }
    }

    void selectWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && weaponListPos < weaponList.Count - 1)
        {
            weaponListPos++;
            changeWeapon();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && weaponListPos > 0)
        {
            weaponListPos--;
            changeWeapon();
        }
    }

    public void respawnRythmyl()
    {
        HP = HPOrig;
        updatePlayerUI();
        controller.transform.position = gamemanager.instance.rythmylSpawnPos.transform.position;
    }

    public void ResetHealth()
    {
        HP = HPOrig;
        updatePlayerUI();
    }

    public void ModifySpeed(int amount)
    {
        speed += amount;
    }

    public void ModifyDMG(int amount)
    {
        shootDamage += amount;
    }

    public void Heal(int amount)
    {
        HP += amount;
        if (HP > HPOrig)
            HP = HPOrig;
        updatePlayerUI();
    }
}