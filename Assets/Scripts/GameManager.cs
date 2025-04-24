using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Blood Settings")]
    public float currentBlood = 0;
    public int maxBloodCapacity = 100;

    [Header("Ammo Storage")]
    public int totalPistolAmmo = 0;
    public int totalRifleAmmo = 0;

    [Header("UI Reference")]
    public BloodUI bloodUI; // We'll create this script next

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CollectResources(int blood, int pistolAmmo, int rifleAmmo)
    {
        // Clamp blood collection
        currentBlood = Mathf.Clamp(currentBlood + blood, 0, maxBloodCapacity);

        // Add ammo
        totalPistolAmmo += pistolAmmo;
        totalRifleAmmo += rifleAmmo;

        // Update UI
        if (bloodUI != null)
        {
            bloodUI.UpdateBloodBar(currentBlood, maxBloodCapacity);
        }
    }

    public bool HasBlood(int amount)
    {
        return currentBlood >= amount;
    }

    public void ConsumeBlood(float amount)
    {
        currentBlood = Mathf.Clamp(currentBlood - amount, 0, maxBloodCapacity);

        if (bloodUI != null)
        {
            bloodUI.UpdateBloodBar(currentBlood, maxBloodCapacity);
        }
    }
}
