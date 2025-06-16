using System;
using System.Collections;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GunData gunData;
    public Transform gunMuzzle;

    public GameObject bulletHolePrefab;
    public GameObject bulletHitParticlePrefab;
   
    
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public Transform cameraTransform;

    [SerializeField] private WeaponRecoil _weaponRecoil;

    public int currentAmo = 0;
    private float nextTimeToFire = 0f;

    private bool isReloading = false;
    
    public event Action OnAmmoChanged;

    private void Start()
    {
        currentAmo = gunData.magazineSize;
        NotifyAmmoChanged();

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
            NotifyAmmoChanged();
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
        // Verificação do countdown (nova linha adicionada)
        if (UICountdown.IsCountdownActive())
        {
            Debug.Log("Aguardando término do countdown...");
            return;
        }

        if (isReloading)
        {
            Debug.Log("Is reloading");
            return;
        }

        if (currentAmo <= 0)
        {
            Debug.Log("Sem munição! Recarregando automaticamente...");
            TryReload();
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
        NotifyAmmoChanged();
        Debug.Log(currentAmo + " balas restantes");
        Shoot();

        playerController.ApplyRecoil(gunData);
        _weaponRecoil.ApplyRecoil();
        
    }
    
    private void NotifyAmmoChanged()
    {
        OnAmmoChanged?.Invoke();
    }

    public abstract void Shoot();
    
    public virtual void OnReloadStart() {}
}
