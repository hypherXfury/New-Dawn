using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] WeaponUI weaponUI;
    [SerializeField] string weaponName;
    [Header("Weapon Settings")]
    public float fireRate = 0.2f;
    public float damage = 10f;
    public float range = 100f;
    public LayerMask hitLayers;
    public Transform muzzleFlashPoint;

    [Header("Ammo Settings")]
    public int clipSize = 7;
    public int totalAmmo = 21;
    public float reloadTime = 2.8f;
    [SerializeField] private int currentAmmo;
    [SerializeField]public bool isReloading = false;

    [Header("Ammo Sounds")]
    public AudioClip emptySound;
    public AudioClip reloadSound;


    [Header("Sway Settings")]
    public float swayAmountWalk = 0.05f;
    public float swayAmountSprint = 0.08f;
    public float swayAmountCrouch = 0.03f;
    public float swaySmooth = 8f;

    [Header("Recoil")]
    public Vector2 recoilKick = new Vector2(1f, 2f);
    public float recoilRecoverySpeed = 5f;
    public float recoilX;
    public float recoilY;

    [Header("Animation")]
    public Animator animator;

    private float nextFireTime;
    private Camera cam;
    private Vector3 initialPosition;
    private Vector2 currentRecoil;
    private Vector2 recoilSmoothDampVelocity;

    [Header("Line Renderer")]
    public LineRenderer lineRenderer; // Reference to LineRenderer
    public float lineDuration = 0.1f; // Duration to show the line in seconds
    private float lineDuration1;
    private float lineTimer;

    [Header("Gun Sound")]
    [SerializeField] AudioClip gunClip;
    [SerializeField] AudioSource audioSource;

    void Start()
    {
        currentAmmo = clipSize;
        cam = Camera.main;
        initialPosition = transform.localPosition;
        lineDuration1 = lineDuration;
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned!");
        }
        lineRenderer.enabled = false;
    }

    void OnEnable()
    {
        if (animator != null)
        {
            animator.SetTrigger("Equip");
        }
        currentRecoil = Vector2.zero;
    }

    void Update()
    {
        if(weaponName == "Glock 18")
        {
            totalAmmo += GameManager.instance.totalPistolAmmo;
            GameManager.instance.totalPistolAmmo = 0;
        }
        if(weaponName == "M4A4")
        {
            totalAmmo += GameManager.instance.totalRifleAmmo;
            GameManager.instance.totalRifleAmmo = 0;
        }
         // Or store a reference
        if (weaponUI != null)
        {
            weaponUI.UpdateWeaponUI(weaponName, currentAmmo, totalAmmo);
        }

        if (isReloading) return;

        HandleSway();
        HandleShooting();
        ApplyRecoil();

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentAmmo < clipSize && totalAmmo > 0)
            {
                StartCoroutine(Reload());
                return;
            }
        }

        if (lineRenderer.enabled)
        {
            lineTimer += Time.deltaTime;
            if (lineTimer >= lineDuration1)
            {
                lineRenderer.enabled = false; // Hide the line after the duration
            }
        }
    }
    void Fire()
    {
        currentAmmo--;
        audioSource.PlayOneShot(gunClip);
    }
    void PlayEmptySound()
    {
        if (emptySound && audioSource)
            audioSource.PlayOneShot(emptySound);
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        lineRenderer.enabled = false;
        if (reloadSound && audioSource)
            audioSource.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = clipSize - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, totalAmmo);

        currentAmmo += ammoToReload;
        totalAmmo -= ammoToReload;

        isReloading = false;
        lineDuration1 = lineDuration;
        lineRenderer.enabled = true;
    }

    void HandleSway()
    {
        float swayAmount = swayAmountWalk;

        if (Input.GetKey(KeyCode.LeftShift))
            swayAmount = swayAmountSprint;
        else if (Input.GetKey(KeyCode.LeftControl))
            swayAmount = swayAmountCrouch;

        // Mouse sway
        float mouseX = Input.GetAxis("Mouse X") * swayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * swayAmount;

        Vector3 mouseSway = new Vector3(-mouseX, -mouseY, 0f);

        // Movement bob
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        float bobX = Mathf.Sin(Time.time * 6f) * (isMoving ? swayAmount * 0.5f : 0f);
        float bobY = Mathf.Cos(Time.time * 12f) * (isMoving ? swayAmount * 0.3f : 0f);
        Vector3 movementSway = new Vector3(bobX, bobY, 0f);

        // Combine both
        Vector3 finalSway = mouseSway + movementSway;

        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition + finalSway, Time.deltaTime * swaySmooth);
    }

    void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            if (animator != null)
                animator.SetTrigger("Shoot");

            if (currentAmmo > 0)
            {
                Fire();
            }
            else
            {
                lineDuration1 = 0;
                PlayEmptySound();
            }

            FireRaycast();
            ApplyRecoilKick();
        }
    }

    void FireRaycast()
    {
        // Get the direction of the ray (center of the crosshair)
        Vector3 rayDirection = cam.transform.forward;

        // Apply recoil to the ray direction by slightly altering it
        rayDirection.x += Random.Range(-recoilX, recoilX);
        rayDirection.y += Random.Range(-recoilY, recoilY);

        // Normalize the direction to make sure the ray is not too "spread"
        rayDirection.Normalize();

        // Fire the raycast
        Ray ray = new Ray(cam.transform.position, rayDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, hitLayers))
        {
            // Render the line to show bullet trajectory
            lineRenderer.SetPosition(0, muzzleFlashPoint.position);
            lineRenderer.SetPosition(1, hit.point);

            Debug.Log("Hit: " + hit.collider.name);

            ZombieAI enemy = hit.transform.GetComponent<ZombieAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(20f); // Adjust damage per shot
            }

            // Optional: instantiate impact FX
        }
        else
        {
            // If no hit, show the line going far away (just for visualization)
            lineRenderer.SetPosition(0, muzzleFlashPoint.position);
            lineRenderer.SetPosition(1, ray.origin + ray.direction * range);
        }

        // Enable the line and reset timer
        lineRenderer.enabled = true;
        lineTimer = 0f; // Reset the timer so the line will disappear after the duration
    }


    void ApplyRecoilKick()
    {
        currentRecoil += new Vector2(
            Random.Range(recoilKick.x * 0.8f, recoilKick.x),
            Random.Range(-recoilKick.y, recoilKick.y)
        );
    }

    void ApplyRecoil()
    {
        currentRecoil = Vector2.SmoothDamp(currentRecoil, Vector2.zero, ref recoilSmoothDampVelocity, 1f / recoilRecoverySpeed);

        if (MouseLook.Instance != null)
        {
            MouseLook.Instance.SetAdditionalRotation(-currentRecoil.x);
        }
    }
}
