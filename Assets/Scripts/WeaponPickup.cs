using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public Transform weaponHolder; // Assign the empty object under the camera
    public KeyCode pickupKey = KeyCode.E;

    private Camera playerCamera;
    [SerializeField] private GameObject weaponText;

    void Start()
    {
        weaponText.SetActive(false);
        playerCamera = Camera.main;
    }

    void Update()
    {
        CheckForPickup();
    }

    void CheckForPickup()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            if (hit.collider.CompareTag("Weapon")) // Set your weapons' tag to "Weapon"
            {
                if (Input.GetKeyDown(pickupKey))
                {
                    PickupWeapon(hit.collider.gameObject);
                }
            }
        }
    }

    void PickupWeapon(GameObject weaponObj)
    {
        weaponText.SetActive(true);
        // Reset transform & parent
        weaponObj.transform.SetParent(weaponHolder);
        weaponObj.transform.localPosition = Vector3.zero;
        weaponObj.transform.localRotation = Quaternion.identity;

        // Optionally add logic to auto-select the weapon if it's the only one
        WeaponManager wm = weaponHolder.GetComponent<WeaponManager>();
        if (wm != null)
        {
            wm.selectedWeapon = weaponHolder.childCount - 1; // Assume new weapon added at end
            wm.SelectWeapon();
        }

        // Disable physics if any
        if (weaponObj.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        if (weaponObj.TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = false;
        }

        // Enable weapon script
        Weapon weaponScript = weaponObj.GetComponent<Weapon>();
        if (weaponScript != null)
        {
            weaponScript.enabled = true;
        }

        Debug.Log("Picked up: " + weaponObj.name);
    }
}
