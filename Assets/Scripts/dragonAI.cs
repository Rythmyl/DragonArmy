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
    [Range(1, 360)][SerializeField] int faceTargetSpeed;
    [Range(1, 20)][SerializeField] int animTranSpeed;

    [Header("----- Shooting (Ranged) -----")]
    [SerializeField] GameObject bullet;
    [Range(0.1f, 10f)][SerializeField] float shootRate;
    [SerializeField] Transform shootPos;
    [Range(0.1f, 50f)][SerializeField] float bulletSpeed;
    [SerializeField] float rangedStoppingDistance = 10f;

    [Header("----- Melee Attack -----")]
    [Range(0.1f, 10f)][SerializeField] float meleeRange;
    [Range(0.5f, 5f)][SerializeField] float meleeCooldown;
    [Range(1, 20)][SerializeField] int meleeDamage;
    [SerializeField] float meleeStoppingDistance = 3f;

    [Header("----- Tower Target -----")]
    [SerializeField] GameObject tower;

    Color colorOrig;

    bool isDead = false;

    float shootTimer;
    float meleeTimer;

    void Start()
    {
        colorOrig = model != null ? model.material.color : Color.white;

        if (meleeRange > 0 && weaponCol != null)
            weaponCol.enabled = false;

        meleeTimer = meleeCooldown;
        shootTimer = shootRate;

        if (tower != null && agent != null)
        {
            agent.stoppingDistance = bullet == null ? meleeStoppingDistance : rangedStoppingDistance;
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

        if (tower == null) return;

        float distToTower = Vector3.Distance(transform.position, GetClosestPointOnTower(transform.position));

        if (bullet == null)
        {
            if (distToTower <= meleeStoppingDistance)
            {
                FaceTarget(tower.transform.position);

                if (meleeTimer >= meleeCooldown)
                    MeleeAttack();

                agent.stoppingDistance = meleeStoppingDistance;
                agent.SetDestination(GetClosestPointOnTower(transform.position));
            }
            else
            {
                agent.stoppingDistance = meleeStoppingDistance;
                agent.SetDestination(GetClosestPointOnTower(transform.position));
            }
        }
        else
        {
            if (distToTower <= rangedStoppingDistance)
            {
                FaceTarget(tower.transform.position);

                if (shootRate > 0 && shootTimer >= shootRate)
                    Shoot();

                if (meleeRange > 0 && meleeTimer >= meleeCooldown && distToTower <= meleeStoppingDistance)
                    MeleeAttack();

                agent.stoppingDistance = rangedStoppingDistance;
                agent.SetDestination(GetClosestPointOnTower(transform.position));
            }
            else
            {
                agent.stoppingDistance = rangedStoppingDistance;
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

    void FaceTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
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

    void DropBomb()
    {
        GameObject bombPickup = Resources.Load<GameObject>("BombPickup");
        if (bombPickup != null)
            Instantiate(bombPickup, transform.position + Vector3.up, Quaternion.identity);
    }

    void DropPotion()
    {
        GameObject potionPickup = Resources.Load<GameObject>("PotionPickup");
        if (potionPickup != null)
            Instantiate(potionPickup, transform.position + Vector3.up, Quaternion.identity);
    }

    public void takeDamage(int amount)
    {
        if (isDead) return;

        HP -= amount;

        if (HP <= 0)
        {
            isDead = true;

            if (ExpManager.instance != null)
                ExpManager.instance.AddExp(25);
            if (ScoreSystem.instance != null)
                ScoreSystem.instance.AddKillScore();

            buffs buffmanager = FindFirstObjectByType<buffs>();
            if (buffmanager != null)
                buffmanager.DropBuff(transform.position);

            //if (PlayerSkills.instance != null)
            //{
            //    float bombChance PlayerSkills.instance.GetBombDropChance();
            //    if (Random.value <= bombChance)
            //    {
            //        DropBomb();
            //    }
            //}

            //if (PlayerSkills.instance != null)
            //{
            //    float potionChance PlayerSkills.instance.GetPotionDropChance();
            //    if (Random.value <= potionChance)
            //    {
            //        DropPotion();
            //    }
            //}
            gamemanager.instance.updateGameGoal(-1, isDragon: true);
            agent.isStopped = true;
            Destroy(gameObject);
        }
        else
            StartCoroutine(flashRed());
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    public void SetTowerTarget(GameObject towerTarget)
    {
        tower = towerTarget;

        if (agent != null && tower != null)
        {
            agent.stoppingDistance = bullet == null ? meleeStoppingDistance : rangedStoppingDistance;
            agent.SetDestination(GetClosestPointOnTower(transform.position));
        }
    }
}
