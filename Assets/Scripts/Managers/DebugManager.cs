using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    [Header("Slider00")]
    public Slider slider00;
    public TextMeshProUGUI text00;

    [Header("Slider01")]
    public Slider slider01;
    public TextMeshProUGUI text01;

    [Header("Slider02")]
    public Slider slider02;
    public TextMeshProUGUI text02;

    [Header("Slider03")]
    public Slider slider03;
    public TextMeshProUGUI text03;

    [Header("Debug Text 00")]
    public TextMeshProUGUI debugText00;

    [Header("Debug Text 01")]
    public TextMeshProUGUI debugText01;

    [Header("Debug Text 02")]
    public TextMeshProUGUI debugText02;

    [Header("Debug Text 03")]
    public TextMeshProUGUI debugText03;


    public static DebugManager Instance;




    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Enter a value to tweak via the DebuggerCanvas (ie : this function needs to be call on UPDATE)
    /// </summary>
    /// <param name="sliderID">Which slider you are using (check if this slider isn't used before)</param>
    /// <param name="title">Title upon your slider ("slider name" + sliderValue)</param>
    /// <param name="value">The value you want to tweak</param>
    /// <param name="minValue">The min of your slider</param>
    /// <param name="maxValue">The max of your slider</param>
    /// <returns></returns>
    public float Tweak(int sliderID, string title, float value, float minValue, float maxValue)
    {
        float sliderValue;

        switch (sliderID)
        {
            case 0:
                {
                    text00.text = title;
                    slider00.minValue = minValue;
                    slider00.maxValue = maxValue;
                    sliderValue = slider00.value;
                    return sliderValue;
                }

            case 1:
                {
                    text01.text = title;
                    slider01.minValue = minValue;
                    slider01.maxValue = maxValue;
                    sliderValue = slider01.value;
                    return sliderValue;
                }

            case 2:
                {
                    text02.text = title;
                    slider02.minValue = minValue;
                    slider02.maxValue = maxValue;
                    sliderValue = slider02.value;
                    return sliderValue;
                }

            case 3:
                {
                    text03.text = title;
                    slider03.minValue = minValue;
                    slider03.maxValue = maxValue;
                    sliderValue = slider03.value;
                    return sliderValue;
                }
        }

        return value;
    }


    /// <summary>
    /// What value you wish to display on the DebuggerCanvas
    /// </summary>
    /// <param name="debugTextID"></param>
    /// <param name="valueToDisplay"></param>
    public void DisplayValue(int debugTextID, string valueToDisplay)
    {
        switch (debugTextID)
        {
            case 0:
                {
                    debugText00.text = valueToDisplay;
                    break;
                }

            case 1:
                {
                    debugText01.text = valueToDisplay;
                    break;
                }

            case 2:
                {
                    debugText02.text = valueToDisplay;
                    break;
                }

            case 3:
                {
                    debugText03.text = valueToDisplay;
                    break;
                }
        }
    }
}
