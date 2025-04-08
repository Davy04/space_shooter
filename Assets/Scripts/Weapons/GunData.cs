using UnityEngine;

[CreateAssetMenu (fileName = "NewGunData", menuName = "Gun/GunData")]
public class GunData : ScriptableObject
{

    public string gunName;

    public LayerMask targetLayerMask;

    [Header("Fire config")]
    public float shootingRange;
    public float fireRate;

    [Header("Reload config")]
    public int magazineSize;
    public float reloadTime;

    [Header("Recoil settings")]
    public float recoilAmount;
    public Vector2 maxRecoil;
    public float recoilSpeed;
    public float resetRecoilSpeed;

    [Header("VFX")] 
    public GameObject bulletTrailPrefab;
    public float bulletSpeed;

}
