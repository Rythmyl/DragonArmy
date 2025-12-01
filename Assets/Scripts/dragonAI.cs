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
        colorOrig = model.material.color;
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
        lastAttackPosition = startingPos;

        if (weaponCol != null)
            weaponCol.enabled = false;
        else
            Debug.LogWarning("Weapon Collider not assigned!");

        meleeTimer = meleeCooldown; // Allow immediate melee if in range
        shootTimer = shootRate;     // Allow immediate shoot if ready
    }

    void Update()
    {
        if (isDead) return;

        shootTimer += Time.deltaTime;
        meleeTimer += Time.deltaTime;

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
        if (agent.remainingDistance < 0.01f && roamTimer >= roamPauseTime)
        {
            Roam();
        }
    }

    void Roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist + startingPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, NavMesh.AllAreas);
        agent.SetDestination(hit.position);
    }

    bool CanSeePlayer()
    {
        Vector3 playerPos = gamemanager.instance.rythmyl.transform.position;
        playerDir = playerPos - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir, Color.green);

        if (Physics.Raycast(headPos.position, playerDir, out RaycastHit hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Rythmyl"))
            {
                float distToPlayer = Vector3.Distance(transform.position, playerPos);

                // Melee attack check
                if (meleeRange > 0 && distToPlayer <= meleeRange && meleeTimer >= meleeCooldown)
                {
                    agent.stoppingDistance = meleeRange;
                    agent.SetDestination(playerPos);
                    FaceTarget();
                    MeleeAttack();
                    return true;
                }

                // Ranged attack check
                if (bullet != null && shootRate > 0 && distToPlayer <= stoppingDistOrig && shootTimer >= shootRate)
                {
                    agent.stoppingDistance = stoppingDistOrig;
                    agent.SetDestination(playerPos);
                    FaceTarget();
                    Shoot();
                    return true;
                }

                // Chase player
                agent.stoppingDistance = (bullet != null && shootRate > 0) ? stoppingDistOrig : (meleeRange > 0 ? meleeRange : 0);
                agent.SetDestination(playerPos);

                if (distToPlayer <= agent.stoppingDistance)
                    FaceTarget();

                return true;
            }
        }

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
        if (other.CompareTag("Rythmyl"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Rythmyl")) return;

        playerInTrigger = false;
        agent.stoppingDistance = 0;
        agent.SetDestination(lastAttackPosition);
    }

    void Shoot()
    {
        shootTimer = 0;
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
        anim.SetTrigger("Attack"); 
        lastAttackPosition = gamemanager.instance.rythmyl.transform.position;
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
        agent.SetDestination(gamemanager.instance.rythmyl.transform.position);

        if (HP <= 0)
        {
            isDead = true;
            gamemanager.instance.updateGameGoal(-1, isDragon: true);
            anim.SetTrigger("Death");
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
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
