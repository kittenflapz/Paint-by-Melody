using System;
using Character;
using UnityEngine;

public enum WeaponType
{
    None, 
    Pistol,
    MachineGun
}



    [Serializable]
    public struct WeaponStats
    {
        public WeaponType WaeaponType;
        public string Name;

        public float FireStartDelay;
        public float FireRate;
        public float FireDistance;
        public bool Repeating;

        public LayerMask WeaponHitLayer;
    }
    
public class WeaponComponent : MonoBehaviour
 {
        public Transform HandPosition => GripIKLocation;
        [SerializeField] private Transform GripIKLocation;

        public bool Firing { get; private set; }

        [SerializeField] public WeaponStats WeaponStats;

    [SerializeField]
    AudioSource weaponFireSound;


     protected Camera ViewCamera;
     protected WeaponHolder WeaponHolder;
     protected Crosshair Crosshair;
     private Vector3 HitLocation;

    [SerializeField]
    KeyManager keyManager;

    [SerializeField]
    PlayerController player;

    private void Awake()
    {
        ViewCamera = Camera.main;
        keyManager = FindObjectOfType<KeyManager>();
        player = FindObjectOfType<PlayerController>();
    }
    public void Initialize(WeaponHolder weaponHolder, Crosshair crosshair)
     {
         WeaponHolder = weaponHolder;
         Crosshair = crosshair;
     }
        public virtual void StartFiring()
        {
            Firing = true;
            if (WeaponStats.Repeating)
            {
                InvokeRepeating(nameof(FireWeapon),WeaponStats.FireStartDelay, WeaponStats.FireRate);
            }
            else
            {
            if (player.ammo > 0)
            { 
                FireWeapon();
            }
            }
        }
        
        public virtual void StopFiring()
        {
            Firing = false;
            CancelInvoke(nameof(FireWeapon));
        }

    protected virtual void FireWeapon()
    {

        if (!WeaponHolder.Controller.isRunning)
        { 
            Ray screenRay = ViewCamera.ScreenPointToRay(new Vector3(Crosshair.mousePos.x, Crosshair.mousePos.y, 0));

            if (!Physics.Raycast(screenRay, out RaycastHit hit, WeaponStats.FireDistance, WeaponStats.WeaponHitLayer))
            { 
                return; 
            }

            if (hit.collider.gameObject.CompareTag("Key"))
            {
                Key keyHit = hit.collider.gameObject.GetComponent<Key>();
                keyHit.PlayNote();
            }

            if (hit.collider.gameObject.CompareTag("StartOverKey"))
            {
                keyManager.ClearNotesJustPlayedList();
            }

            HitLocation = hit.point;
            Vector3 hitDirection = hit.point - ViewCamera.transform.position;
            Debug.DrawRay(ViewCamera.transform.position, hitDirection.normalized * WeaponStats.FireDistance, Color.red);
           
        }
        player.ReduceAmmo();
        weaponFireSound.Play();
    }

    private void OnDrawGizmos()
    {
        if (HitLocation == Vector3.zero) return;

        Gizmos.DrawWireSphere(HitLocation, 0.2f);

    }
}

