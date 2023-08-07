using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI textBox;

    public void SetMaxValue(float maxValue)
    {
        slider.maxValue = maxValue;
    }

    public void UpdateHealth(float value)
    {
        slider.value = value;
        textBox.text = "HP: " + value;
    }
}
