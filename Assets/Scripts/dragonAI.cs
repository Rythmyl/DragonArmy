using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class dragonAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Renderer model;
    [SerializeField] Transform headPos;
    [SerializeField] Collider weaponCol;

    [Header("----- Stats -----")]
    [Range(50, 500)][SerializeField] int HP;
    [Range(0, 180)][SerializeField] int FOV;
    [Range(1, 360)][SerializeField] int faceTargetSpeed;
    [Range(1, 20)][SerializeField] int animTranSpeed;

    [Header("----- Shooting (Ranged) -----")]
    [SerializeField] GameObject bullet;
    [Range(0.1f, 10f)][SerializeField] float shootRate;
    [SerializeField] Transform shootPos;
    [Range(0.1f, 50f)][SerializeField] float bulletSpeed;

    [Header("----- Melee Attack -----")]
    [Range(0.1f, 10f)][SerializeField] float meleeRange;
    [Range(0.5f, 5f)][SerializeField] float meleeCooldown;
    [Range(1, 20)][SerializeField] int meleeDamage;

    [Header("----- Tower Target -----")]
    public GameObject tower;

    Color colorOrig;

    bool playerInTrigger;
    bool isDead = false;
    bool isEngagedByPlayer = false;

    float shootTimer;
    float meleeTimer;
    float angleToPlayer;
    Vector3 playerDir;
    Vector3 lastAttackPosition;
    float stoppingDistOrig;

    float lastHitTime = -10f;
    [SerializeField] float engageTimeout = 5f;

    void Start()
    {
        colorOrig = model != null ? model.material.color : Color.white;
        stoppingDistOrig = agent != null ? agent.stoppingDistance : 0;

        lastAttackPosition = transform.position;

        if (meleeRange > 0 && weaponCol != null)
            weaponCol.enabled = false;

        meleeTimer = meleeCooldown;
        shootTimer = shootRate;

        if (tower != null && agent != null)
        {
            agent.stoppingDistance = 10f;
            agent.SetDestination(GetClosestPointOnTower(transform.position));
        }
    }

    void Update()
    {
        if (isDead) return;

        shootTimer += Time.deltaTime;
        meleeTimer += Time.deltaTime;

        if (agent == null || anim == null) return;

        float agentSpeedCur = agent.velocity.magnitude;
        float agentSpeedAnim = anim.GetFloat("Speed");
        anim.SetFloat("Speed", Mathf.Lerp(agentSpeedAnim, agentSpeedCur, Time.deltaTime * animTranSpeed));

        if (isEngagedByPlayer)
        {
            if (Time.time - lastHitTime > engageTimeout)
            {
                isEngagedByPlayer = false;
            }
            else if (playerInTrigger && !CanSeePlayer())
            {
                isEngagedByPlayer = false;
            }
            else if (!playerInTrigger)
            {
                isEngagedByPlayer = false;
            }
        }

        if (isEngagedByPlayer)
        {
            if (gamemanager.instance?.rythmyl != null)
            {
                Vector3 playerPos = gamemanager.instance.rythmyl.transform.position;
                float distToPlayer = Vector3.Distance(transform.position, playerPos);

                if (distToPlayer <= meleeRange && meleeTimer >= meleeCooldown)
                {
                    agent.stoppingDistance = meleeRange;
                    agent.SetDestination(playerPos);
                    FaceTarget(playerPos);
                    MeleeAttack();
                }
                else if (bullet != null && shootRate > 0 && distToPlayer <= stoppingDistOrig && shootTimer >= shootRate)
                {
                    agent.stoppingDistance = stoppingDistOrig;
                    agent.SetDestination(playerPos);
                    FaceTarget(playerPos);
                    Shoot();
                }
                else
                {
                    agent.SetDestination(playerPos);
                    FaceTarget(playerPos);
                }
            }
        }
        else
        {
            if (tower == null) return;

            float distToTower = Vector3.Distance(transform.position, GetClosestPointOnTower(transform.position));

            if (distToTower <= 10f)
            {
                FaceTarget(tower.transform.position);

                if (bullet != null && shootRate > 0 && shootTimer >= shootRate)
                    Shoot();

                if (meleeRange > 0 && meleeTimer >= meleeCooldown && distToTower <= meleeRange)
                    MeleeAttack();

                agent.stoppingDistance = 10f;
                agent.SetDestination(GetClosestPointOnTower(transform.position));
            }
            else
            {
                agent.stoppingDistance = 10f;
                agent.SetDestination(GetClosestPointOnTower(transform.position));
            }
        }
    }

    Vector3 GetClosestPointOnTower(Vector3 fromPosition)
    {
        if (tower == null) return Vector3.zero;

        CapsuleCollider cap = tower.GetComponent<CapsuleCollider>();
        if (cap != null)
            return cap.ClosestPoint(fromPosition);

        return tower.transform.position;
    }

    bool CanSeePlayer()
    {
        if (headPos == null || agent == null) return false;

        Vector3 playerPos = gamemanager.instance?.rythmyl?.transform.position ?? Vector3.zero;
        playerDir = playerPos - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        if (Physics.Raycast(headPos.position, playerDir, out RaycastHit hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Rythmyl"))
            {
                return true;
            }
        }

        return false;
    }

    void FaceTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        if (other.CompareTag("Rythmyl"))
            playerInTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null) return;

        if (!other.CompareTag("Rythmyl")) return;

        playerInTrigger = false;
    }

    void Shoot()
    {
        shootTimer = 0;
        anim?.SetTrigger("Shoot");
    }

    public void createBullet()
    {
        if (bullet == null || shootPos == null) return;

        GameObject newBullet = Instantiate(bullet, shootPos.position, shootPos.rotation);

        Rigidbody rb = newBullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = shootPos.forward * bulletSpeed;
    }

    void MeleeAttack()
    {
        meleeTimer = 0;
        anim?.SetTrigger("Attack");

        if (isEngagedByPlayer)
            lastAttackPosition = gamemanager.instance?.rythmyl?.transform.position ?? lastAttackPosition;
        else if (tower != null)
            lastAttackPosition = tower.transform.position;
    }

    public void weaponColOn()
    {
        if (weaponCol != null)
            weaponCol.enabled = true;
    }

    public void weaponColOff()
    {
        if (weaponCol != null)
            weaponCol.enabled = false;
    }

    public void takeDamage(int amount)
    {
        if (isDead) return;

        HP -= amount;

        if (!isEngagedByPlayer)
            isEngagedByPlayer = true;

        lastHitTime = Time.time; 

        if (agent != null && gamemanager.instance?.rythmyl != null && isEngagedByPlayer)
            agent.SetDestination(gamemanager.instance.rythmyl.transform.position);

        if (HP <= 0)
        {
            isDead = true;
            gamemanager.instance?.updateGameGoal(-1, isDragon: true);
            if (agent != null)
                agent.isStopped = true;
            Destroy(gameObject, 5f);
        }
        else
            StartCoroutine(flashRed());
    }

    IEnumerator flashRed()
    {
        if (model != null)
        {
            model.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            model.material.color = colorOrig;
        }
    }
}
