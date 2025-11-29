using UnityEngine;

[CreateAssetMenu]
public class gunStats : ScriptableObject
{
    [Header("----- Gun Model -----")]
    public GameObject gunModel;

    [Header("----- Shooting Stats -----")]
    [Range(1, 10)] public int shootDamage;
    [Range(15, 1000)] public int shootDist;
    [Range(0.1f, 2)] public float shootRate;

    [Header("----- Ammo -----")]
    public int ammoCur;
    [Range(5, 50)] public int ammoMax;

    [Header("----- Effects -----")]
    public ParticleSystem hitEffect;

    [Header("----- Audio -----")]
    public AudioClip[] shootSound;
    [Range(0, 1)] public float shootSoundVol;
}
