using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class playerController : MonoBehaviour, IDamage, IPickup
{
    public enum WeaponType { Gun, Staff }

    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Transform cam;
    [SerializeField] Camera gunCam;
    [SerializeField] float lookSensitivity = 75f;

    [Header("----- Stats -----")]
    [Range(1, 100)][SerializeField] int HP;
    [Range(3, 6)][SerializeField] int speed;
    [Range(2, 5)][SerializeField] int sprintMod;
    [Range(5, 20)][SerializeField] int JumpSpeed;
    [Range(1, 3)][SerializeField] int maxJumps;
    [Range(15, 50)][SerializeField] int gravity;
    [Range(10, 50)][SerializeField] int dash = 25;
    [Range(1, 10)][SerializeField] float dashCool = 5f;
    [Range(0.1f, 0.5f)][SerializeField] float dashDur = 0.2f;

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
    [SerializeField] AudioClip[] audDash;
    [Range(0, 1)][SerializeField] float DashVol = 0.5f;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;
    int weaponListPos;

    float shootTimer;
    float pitch;

    Vector2 moveInput;
    Vector2 lookInput;

    bool jumpPressed;
    bool isSprinting;
    bool isPlayingStep;
    bool sprintHeld;

    bool isDashing = false;
    bool canDash = true;
    float dashTime = 0f;

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
            shootTimer += Time.deltaTime;
<<<<<<< HEAD
=======

            if (!canDash)
            {
                dashTime += Time.deltaTime;
                if (dashTime >= dashCool)
                {
                    canDash = true;
                    dashTime = 0f;
                }
            }
>>>>>>> Jarrett's-Branch
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

        moveDir = moveInput.x * transform.right + moveInput.y * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();
        controller.Move(playerVel * Time.deltaTime);

        if (shootTimer >= shootRate && weaponList.Count > 0 && Input.GetButton("Fire1"))
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
            yield return new WaitForSeconds(0.3f);
        else
            yield return new WaitForSeconds(0.5f);

        isPlayingStep = false;
    }

    void Dash()
    {
        if (canDash && !isDashing)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;

        if (audDash != null && audDash.Length > 0)
        {
            aud.PlayOneShot(audDash[Random.Range(0, audDash.Length)], DashVol);
        }
        Vector3 dashDir;

        if (moveInput.magnitude > 0.1f)
        {
            dashDir = (moveInput.x * transform.right + moveInput.y * transform.forward).normalized;
        }
        else
        {
            dashDir = transform.forward;
        }
        float elapsed = 0f;
        while (elapsed < dashDur)
        {
            controller.Move(dashDir * dash * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            Dash();
    }

    void sprint()
    {
        if (sprintHeld && !isSprinting)
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (!sprintHeld && isSprinting)
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    void jump()
    {
        if (jumpPressed && jumpCount < maxJumps)
        {
            jumpPressed = false;
            playerVel.y = JumpSpeed;
            jumpCount++;
            aud.pitch = Random.Range(0.9f, 1.1f);
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }
        else
        {
            jumpPressed = false;
        }
    }

    void shoot()
    {
        shootTimer = 0;
        gunStats currentWeapon = weaponList[weaponListPos];
        aud.pitch = Random.Range(0.9f, 1.1f);
        aud.PlayOneShot(currentWeapon.shootSound[Random.Range(0, currentWeapon.shootSound.Length)], currentWeapon.shootSoundVol);

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = gunCam.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, shootDist, ~ignoreLayer))
        {
            targetPoint = hit.point;
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
            Instantiate(currentWeapon.hitEffect, hit.point, Quaternion.identity);
        }
        else
        {
            targetPoint = ray.GetPoint(shootDist);
        }

        Vector3 aimDirection = (targetPoint - gunModel.transform.position).normalized;
        gunModel.transform.forward = aimDirection;
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

        if (gun.gunModel.name.ToLower().Contains("Staff"))
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
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0 && weaponListPos < weaponList.Count - 1)
        {
            weaponListPos++;
            changeWeapon();
        }
        else if (scroll < 0 && weaponListPos > 0)
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

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            jumpPressed = true;
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        sprintHeld = ctx.ReadValue<float>() > 0.5f;
    }

    void LateUpdate()
    {
        if (gamemanager.instance.isPaused)
            return;

        Vector2 li = lookInput * lookSensitivity * Time.deltaTime;

        transform.Rotate(0f, li.x, 0f);

        pitch -= li.y;
        pitch = Mathf.Clamp(pitch, -80f, 80f);
        if (cam != null)
            cam.localEulerAngles = new Vector3(pitch, 0f, 0f);

        if (gunCam != null && cam != null)
        {
            gunCam.transform.rotation = cam.rotation;
        }
    }

    public void Heal(int amo)
    {
        HP += amo;
        if (HP > HPOrig) HP = HPOrig;
        updatePlayerUI();
    }

    public int GetSpeed()
    {
        return speed;
    }

    public void ModifySpeed(int amo)
    {
        speed += amo;
    }

    public int GetShootDamage()
    {
        return shootDamage;
    }

    public void ModifyDMG(int amo)
    {
        shootDamage += amo;
    }
}
