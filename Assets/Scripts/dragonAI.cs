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
    [Range(1, 50)][SerializeField] int roamDist;
    [Range(0, 10)][SerializeField] int roamPauseTime;
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

    Color colorOrig;

    bool playerInTrigger;
    bool isDead = false;

    float shootTimer;
    float meleeTimer;
    float roamTimer;
    float angleToPlayer;
    Vector3 playerDir;
    Vector3 startingPos;
    Vector3 lastAttackPosition;
    float stoppingDistOrig;

    void Start()
    {
        if (model == null)
            Debug.LogError("Renderer (model) not assigned!");
        else
            colorOrig = model.material.color;

        if (agent == null)
            Debug.LogError("NavMeshAgent (agent) not assigned!");
        else
            stoppingDistOrig = agent.stoppingDistance;

        startingPos = transform.position;
        lastAttackPosition = startingPos;

        // Disable weapon collider only if meleeRange > 0 and weaponCol assigned
        if (meleeRange > 0)
        {
            if (weaponCol != null)
            {
                weaponCol.enabled = false;
            }
            else
            {
                // No weapon collider assigned but meleeRange is set; melee won't work properly,
                // but no debug logs as per request.
            }
        }

        meleeTimer = meleeCooldown; // Allow immediate melee if in range
        shootTimer = shootRate;     // Allow immediate shoot if ready
    }

    void Update()
    {
        if (isDead) return;

        shootTimer += Time.deltaTime;
        meleeTimer += Time.deltaTime;

        if (agent == null || anim == null)
            return; // Safety check

        float agentSpeedCur = agent.velocity.magnitude;
        float agentSpeedAnim = anim.GetFloat("Speed");
        anim.SetFloat("Speed", Mathf.Lerp(agentSpeedAnim, agentSpeedCur, Time.deltaTime * animTranSpeed));

        if (agent.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;

        if (playerInTrigger && !CanSeePlayer())
            CheckRoam();
        else if (!playerInTrigger)
            CheckRoam();
    }

    void CheckRoam()
    {
        if (agent != null && agent.remainingDistance < 0.01f && roamTimer >= roamPauseTime)
        {
            Roam();
        }
    }

    void Roam()
    {
        roamTimer = 0;
        if (agent == null) return;

        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist + startingPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, NavMesh.AllAreas);
        agent.SetDestination(hit.position);
    }

    bool CanSeePlayer()
    {
        if (headPos == null || agent == null)
            return false;

        Vector3 playerPos = gamemanager.instance?.rythmyl?.transform.position ?? Vector3.zero;
        playerDir = playerPos - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir, Color.green);

        if (Physics.Raycast(headPos.position, playerDir, out RaycastHit hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Rythmyl"))
            {
                float distToPlayer = Vector3.Distance(transform.position, playerPos);

                // Melee attack only if meleeRange > 0 AND weaponCol assigned
                if (meleeRange > 0 && weaponCol != null && distToPlayer <= meleeRange && meleeTimer >= meleeCooldown)
                {
                    agent.stoppingDistance = meleeRange;
                    agent.SetDestination(playerPos);
                    FaceTarget();
                    MeleeAttack();
                    return true;
                }

                // Ranged attack only if bullet prefab assigned
                if (bullet != null && shootRate > 0 && distToPlayer <= stoppingDistOrig && shootTimer >= shootRate)
                {
                    agent.stoppingDistance = stoppingDistOrig;
                    agent.SetDestination(playerPos);
                    FaceTarget();
                    Shoot();
                    return true;
                }

                // Chase player with appropriate stopping distance (handle both cases safely)
                if (bullet != null && shootRate > 0)
                    agent.stoppingDistance = stoppingDistOrig;
                else if (meleeRange > 0)
                    agent.stoppingDistance = meleeRange;
                else
                    agent.stoppingDistance = 0;

                agent.SetDestination(playerPos);

                if (distToPlayer <= agent.stoppingDistance)
                    FaceTarget();

                return true;
            }
        }

        if (agent != null)
            agent.stoppingDistance = 0;

        return false;
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
            return;

        if (other.CompareTag("Rythmyl"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null) return;

        if (!other.CompareTag("Rythmyl")) return;

        playerInTrigger = false;
        if (agent != null)
        {
            agent.stoppingDistance = 0;
            agent.SetDestination(lastAttackPosition);
        }
    }

    void Shoot()
    {
        shootTimer = 0;
        if (anim != null)
            anim.SetTrigger("Shoot");
    }

    public void createBullet()
    {
        if (bullet == null || shootPos == null) return;

        GameObject newBullet = Instantiate(bullet, shootPos.position, shootPos.rotation);

        Rigidbody rb = newBullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = shootPos.forward * bulletSpeed;
        }
    }

    void MeleeAttack()
    {
        meleeTimer = 0;
        if (anim != null)
            anim.SetTrigger("Attack");
        lastAttackPosition = gamemanager.instance?.rythmyl?.transform.position ?? lastAttackPosition;
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
        if (agent != null && gamemanager.instance?.rythmyl != null)
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
        {
            StartCoroutine(flashRed());
        }
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
