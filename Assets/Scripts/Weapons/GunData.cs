using UnityEngine;

[CreateAssetMenu(fileName = "NewGunData", menuName = "Gun/GunData")]
public class GunData : ScriptableObject
{
    [Header("General Settings")]
    public string gunName;
    public LayerMask targetLayerMask;

    [Header("Fire Config")]
    public float shootingRange = 100f;
    public float fireRate = 0.5f; 
    public int damage = 25;

    [Header("Ammo & Reload")]
    public int magazineSize = 10;
    public float reloadTime = 2f;

    [Header("Recoil Settings")]
    public float recoilAmount = 1f;
    public Vector2 maxRecoil = new Vector2(0.5f, 0.5f);
    public float recoilSpeed = 10f;
    public float resetRecoilSpeed = 5f; 

    [Header("Visual Effects (VFX)")]
    public GameObject bulletTrailPrefab;
    public float bulletSpeed = 100f;
    public GameObject bulletImpactEffect;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
}