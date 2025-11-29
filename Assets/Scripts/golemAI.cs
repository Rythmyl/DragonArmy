using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class golemAI : MonoBehaviour, IDamage
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

    [Header("----- Attack -----")]
    [Range(.1f, 10f)][SerializeField] int attackRange;
    [Range(0.5f, 5f)][SerializeField] float attackCooldown;
    [Range(1, 10)][SerializeField] int damageAmount;

    Color colorOrig;

    bool playerInTrigger;
    bool isDead = false;

    float attackTimer;
    float roamTimer;
    float angleToPlayer;
    Vector3 playerDir;
    Vector3 startingPos;
    Vector3 lastAttackPosition;
    float stoppingDistOrig;

    void Start()
    {
        colorOrig = model.material.color;
        startingPos = transform.position;
        lastAttackPosition = startingPos;
        stoppingDistOrig = agent.stoppingDistance;
        agent.speed = 3f;

        if (weaponCol == null)
        {
            Debug.LogError("Weapon Collider is not assigned!");
        }
        else
        {
            weaponCol.enabled = false;
        }
    }

    void Update()
    {
        if (isDead) return;

        attackTimer += Time.deltaTime;

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

        Debug.DrawRay(headPos.position, playerDir, Color.red);

        if (Physics.Raycast(headPos.position, playerDir, out RaycastHit hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Rythmyl"))
            {
                agent.stoppingDistance = attackRange;
                agent.SetDestination(playerPos);

                float distToPlayer = Vector3.Distance(transform.position, playerPos);
                if (distToPlayer <= attackRange && attackTimer >= attackCooldown)
                {
                    Attack();
                }
                if (distToPlayer <= attackRange)
                {
                    FaceTarget();
                }

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
            Debug.Log($"Weapon Collider triggered by: {other.gameObject.name}");

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

    void Attack()
    {
        attackTimer = 0;
        anim.SetTrigger("StompTrigger");
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
            gamemanager.instance.updateGameGoal(-1, isGolem: true);
            anim.SetTrigger("Death");
            agent.isStopped = true;
            Destroy(gameObject, 5f);
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}