using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("--- Gun Stats ---")]
    public gunStats gunStatsData;

    [Header("--- Shooting Setup ---")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("--- Turn Management ---")]
    public int turretID;
    public static int currentTurn = 0;
    public static int turretCount = 2;

    private float shootCooldown;

    private void Update()
    {
        shootCooldown -= Time.deltaTime;

        bool canShoot = (gunStatsData.unlimitedAmmo || gunStatsData.ammoCur > 0);

        if (shootCooldown <= 0f && (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && currentTurn == turretID && canShoot)
        {
            Shoot();
            shootCooldown = gunStatsData.shootRate;

            currentTurn = (currentTurn + 1) % turretCount;
        }
    }

    private void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            Projectile projectileScript = proj.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Init(gunStatsData.shootDamage);
                projectileScript.hitEffectPrefab = gunStatsData.hitEffect?.gameObject;
            }
        }

        if (gunStatsData.shootSound != null && gunStatsData.shootSound.Length > 0)
        {
            AudioSource.PlayClipAtPoint(gunStatsData.shootSound[0], transform.position, gunStatsData.shootSoundVol);
        }

        if (!gunStatsData.unlimitedAmmo)
        {
            gunStatsData.ammoCur = Mathf.Max(gunStatsData.ammoCur - 1, 0);
        }
    }
}