using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    private enum Orientation
    {
        Horizontal,
        Vertical
    }

    [SerializeField] Transform valueMeter;

    private Vector3 originalScale;
    private float originalValue;
    private float value;

    [SerializeField] private bool withText;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] Orientation orientation;

    //Can really only be called ONCE, else it will change it's originalScale and always reset to an empty bar
    public void InitializeBar(float maxValue)
    {
        originalValue = maxValue;
        originalScale = valueMeter.localScale;
    }
    public void ResetValue()
    {
        valueMeter.localScale = originalScale;
        value = originalValue;
    }
    public void UpdateBar(float newValue)
    {
        value = newValue;

        float ratio = 0;
        if (originalValue != 0) ratio = value / originalValue;

        if (ratio != 0 && orientation == Orientation.Horizontal) valueMeter.localScale = new Vector3(ratio * originalScale.x, originalScale.y, originalScale.z);

        if (ratio != 0 && orientation == Orientation.Vertical) valueMeter.localScale = new Vector3(originalScale.x, ratio * originalScale.y, originalScale.z);

        if (withText)
        {
            text.text = newValue.ToString() + "/" + originalValue.ToString();
        }
    }
}

