using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] private float lerpSpeed;
    private Image content;

    private float currentFill;
    public float MaxValue { get; set; }
    private float currentValue;

    public float CurrentValue
    {
        get
        {
            return currentValue;
        }
        set
        {
            if (value > MaxValue)
            {
                currentValue = MaxValue;
            }
            else if (value < 0)
            {
                currentValue = 0;
            }
            else
            {
                currentValue = value;
            }
            currentFill = currentValue / MaxValue;
        }
    }

    void Start()
    {
        content = GetComponent<Image>();
    }

    void Update()
    {
        if (content.fillAmount != currentFill)
        {
            //content.fillAmount = Mathf.Lerp(content.fillAmount,currentFill,Time.deltaTime * lerpSpeed);
        }
        content.fillAmount = currentFill;
    }

    public void Initialize(float currentValue, float maxValue)
    {
        MaxValue = maxValue;
        CurrentValue = currentValue;
    }
}
