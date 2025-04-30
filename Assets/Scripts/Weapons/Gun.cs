using System.Collections;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GunData gunData;
    public Transform gunMuzzle;

    public GameObject bulletHolePrefab;//adicionar novamente
    public GameObject bulletHitParticlePrefab;
   
    
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public Transform cameraTransform;

    [SerializeField] private WeaponRecoil _weaponRecoil;

    private int currentAmo = 0;
    private float nextTimeToFire = 0f;

    private bool isReloading = false;

    private void Start()
    {
        currentAmo = gunData.magazineSize;

        playerController = transform.root.GetComponent<PlayerController>();
        cameraTransform = playerController.virtualCamera.transform;
    }

    public virtual void Update()
    {
        playerController.ResetRecoil(gunData);
    }

    public void TryReload()
    {
        if (!isReloading && currentAmo < gunData.magazineSize)
        {
            isReloading = true;
            OnReloadStart();
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log(gunData.gunName + "is Reloading");
        
        yield return new WaitForSeconds(gunData.reloadTime);

        currentAmo = gunData .magazineSize;
        isReloading = false;

        Debug.Log(gunData.gunName + "is Reloaded");
    }

    public void TryShoot()
    {
        if (isReloading)
        {
            Debug.Log("Is reloading");
            return;
        }

        if (currentAmo <= 0)
        {
            Debug.Log("Sem munição! Recargando automaticamente...");
            TryReload(); // ← recarrega automático
            return;
        }

        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + (1f / gunData.fireRate);
            HandleShoot();
        }
    }


    private void HandleShoot()
    {
        currentAmo--;
        Debug.Log(currentAmo + "balas restantes");
        Shoot();

        playerController.ApplyRecoil(gunData);
        _weaponRecoil.ApplyRecoil();
        
    }

    public abstract void Shoot();
    
    public virtual void OnReloadStart() {}
}
