using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Renderer model;
    [SerializeField] Transform headPos;
    [SerializeField] Collider weaponCol;

    [Header("----- Stats -----")]
    [Range(1, 500)] [SerializeField] int HP;
    [Range(0, 180)] [SerializeField] int FOV;
    [Range(1, 360)] [SerializeField] int faceTargetSpeed;
    [Range(1, 50)] [SerializeField] int roamDist;
    [Range(0, 10)] [SerializeField] int roamPauseTime;
    [Range(1, 20)] [SerializeField] int animTranSpeed;

    [Header("----- Shooting -----")]
    [SerializeField] GameObject bullet;
    [Range(0.1f, 10f)] [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;

    Color colorOrig;

    bool playerInTrigger;

    float shootTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;

    Vector3 playerDir;
    Vector3 startingPos;

    void Start()
    {
        colorOrig = model.material.color;
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
    }

    void Update()
    {
        shootTimer += Time.deltaTime;

        float agentSpeedCur = agent.velocity.normalized.magnitude;
        float agentSpeedAnim = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.Lerp(agentSpeedAnim, agentSpeedCur, Time.deltaTime * animTranSpeed));

        if (agent.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;

        if (playerInTrigger && !canSeePlayer())
        {
            checkRoam();
        }
        else if (!playerInTrigger)
        {
            checkRoam();
        }
    }

    void checkRoam()
    {
        if (agent.remainingDistance < 0.01f && roamTimer >= roamPauseTime)
        {
            roam();
        }
    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, NavMesh.AllAreas);
        agent.SetDestination(hit.position);
    }

    bool canSeePlayer()
    {
        playerDir = gamemanager.instance.rythmyl.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Rythmyl"))
            {
                agent.SetDestination(gamemanager.instance.rythmyl.transform.position);

                if (shootTimer >= shootRate)
                {
                    shoot();
                }

                if (agent.remainingDistance <= stoppingDistOrig)
                    faceTarget();

                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }

    void faceTarget()
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
        if (!other.CompareTag("Rythmyl"))
            return;

        playerInTrigger = false;
        agent.stoppingDistance = 0;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        agent.SetDestination(gamemanager.instance.rythmyl.transform.position);

        if (HP <= 0)
        {
            gamemanager.instance.updateGameGoal(-1);
            Destroy(gameObject);
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

    void shoot()
    {
        shootTimer = 0;
        anim.SetTrigger("Shoot");
    }

    public void createBullet()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    public void weaponColOn()
    {
        weaponCol.enabled = true;
    }

    public void weaponColOff()
    {
        weaponCol.enabled = false;
    }
}
