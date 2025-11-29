using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerController : MonoBehaviour, IDamage, IPickup
{
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

    [Header("----- Guns -----")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
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
    int gunListPos;

    float shootTimer;

    bool isSprinting;
    bool isPlayingStep;

    void Start()
    {
        HPOrig = HP;
        respawnRythmyl();
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

        if (Input.GetButton("Fire1") && gunList.Count > 0 && shootTimer >= shootRate)
        {
            shoot();
        }
        selectGun();
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
        gunStats gunPos = gunList[gunListPos];
        aud.pitch = Random.Range(0.9f, 1.1f);
        aud.PlayOneShot(gunPos.shootSound[Random.Range(0, gunPos.shootSound.Length)], gunPos.shootSoundVol);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }

            Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(screenFlashDamage());

        aud.pitch = Random.Range(0.9f, 1.1f);
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        Debug.Log($"Player took {amount} damage, HP left: {HP}");

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
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;

        changeGun();
    }

    void changeGun()
    {
        shootDamage = gunList[gunListPos].shootDamage;
        shootDist = gunList[gunListPos].shootDist;
        shootRate = gunList[gunListPos].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            changeGun();
        }
    }

    public void respawnRythmyl()
    {
        HP = HPOrig;
        updatePlayerUI();
        controller.transform.position = gamemanager.instance.rythmylSpawnPos.transform.position;
    }

    // New method to reset health and update UI on respawn
    public void ResetHealth()
    {
        HP = HPOrig;
        updatePlayerUI();
    }
}
