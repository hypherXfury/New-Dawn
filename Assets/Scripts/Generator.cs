using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Generator : MonoBehaviour
{
    [Header("Generator Settings")]
    public int requiredBlood = 100;
    public float fuelRate = 10f; // blood per second

    private float currentBlood = 0;
    private bool isPlayerNear = false;
    private bool isFullyFueled = false;

    [Header("UI")]
    public GameObject promptUI; // "Hold F to fuel"
    public Slider generatorBar;

    private void Update()
    {
        generatorBar.gameObject.SetActive(isPlayerNear);

        if (isPlayerNear && !isFullyFueled && Input.GetKey(KeyCode.F))
        {
            FuelGenerator();
        }

        if (promptUI != null)
        {
            promptUI.SetActive(isPlayerNear && !isFullyFueled);
        }
    }

    void FuelGenerator()
    {
        float bloodToTransfer = fuelRate * Time.deltaTime;

        if (GameManager.instance.HasBlood(1))
        {
            GameManager.instance.ConsumeBlood(bloodToTransfer);
            currentBlood += bloodToTransfer;

            if (generatorBar != null)
                generatorBar.value = (float)currentBlood / requiredBlood;

            if (currentBlood >= requiredBlood)
            {
                currentBlood = requiredBlood;
                isFullyFueled = true;
                TriggerFinale();
            }
        }
    }

    void TriggerFinale()
    {
        Debug.Log("🛸 Generator fully fueled! LIGHT BEAM INITIATED!");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
