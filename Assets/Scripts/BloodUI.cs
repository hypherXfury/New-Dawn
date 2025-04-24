using UnityEngine;
using UnityEngine.UI;

public class BloodUI : MonoBehaviour
{
    [Header("UI Components")]
    public Slider bloodSlider; // OR you can use Image fillAmount instead

    public void UpdateBloodBar(float current, int max)
    {
        if (bloodSlider != null)
        {
            bloodSlider.value = (float)current / max;
        }
    }
}
