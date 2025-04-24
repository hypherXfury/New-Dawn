using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI weaponText;

    public void UpdateWeaponUI(string weaponName, int currentAmmo, int magSize)
    {
        if (weaponText != null)
        {
            weaponText.text = $"{weaponName}: {currentAmmo} / {magSize}";
        }
    }
}
