using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public int selectedWeapon = 0;
    public GameObject weaponText;
    public Weapon[] weapon1;
    void Start()
    {
        SelectWeapon();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // Number Keys
        if (Input.GetKeyDown(KeyCode.Alpha1)) { selectedWeapon = 0; SelectWeapon(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { selectedWeapon = 1; SelectWeapon(); }
        // Add more if needed
    }

    public void SelectWeapon()
    {
        if(weapon1[0].isReloading)
        {
            weapon1[0].isReloading = false;   
        }
        if(weapon1[1].isReloading)
        {
            weapon1[1].isReloading = false;   
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            bool isSelected = (i == selectedWeapon);
            Transform weapon = transform.GetChild(i);
            weapon.gameObject.SetActive(isSelected);
            weaponText.SetActive(isSelected);
            if (weapon.TryGetComponent<Weapon>(out Weapon weaponScript))
            {
                weaponScript.enabled = isSelected;
            }
        }
    }
}
