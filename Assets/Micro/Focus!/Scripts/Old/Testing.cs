using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Focus;

public class Testing : MonoBehaviour
{
    [Range(-1, 1)] public float TestTest;
    // animate the game object from -1 to +1 and back
    public float minimum = -1.0F;
    public float maximum = 1.0F;

    float range1;
    float range2;
    float range3;


    int focalRangeMin = 0;
    int focalRangeMax = 100;
    public int totalFocalRange;
    [Range(0, 100)] public int targetFocus;
    [Range(0, 100)] public int minRequiredFocus;
    [Range(0, 100)] public int maxRequiredFocus;

    [Range(0, 100)] public float currentFocus;


    //TWEENING THEORY
    [Range(110, 220)] public float myTestValue;
    public float startPositionValue = 110;
    public float endPositionValue = 220;
    public float distanceToTravelValue;
    public float durationValue = 30;
    public float timeValue;


    // starting value for the Lerp
    static float t = 0.0f;
    
    void Update()
    {
        SetFocusValues();
        LerpFocus();
        timeValue = Time.realtimeSinceStartup;

        RunLinearTween();

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentFocus >= minRequiredFocus && currentFocus <= maxRequiredFocus)
            {
                Debug.Log("Sucess!");
            }
        }
    }

    #region // CUSTOM LINEAR FUNCTIONS
    void RunLinearTween()
    {
        distanceToTravelValue = endPositionValue - startPositionValue;
        if (timeValue < durationValue)
        {
            myTestValue = TweenManager.EaseInOutQuad(timeValue, startPositionValue, distanceToTravelValue, durationValue);
            Debug.Log(myTestValue);
        }

        if(timeValue > durationValue)
        {
            Debug.Log("I've finished Lerping");
        }
    }

    #endregion

    #region //FOCUS MOVING
    void SetFocusValues()
    {
        minRequiredFocus = targetFocus - 5 * totalFocalRange / 100;
        maxRequiredFocus = targetFocus + 5 * totalFocalRange / 100;
    }

    void LerpFocus()
    {
        currentFocus = Mathf.Lerp(focalRangeMin, focalRangeMax, t);

        t += 0.5f * Time.deltaTime;

        if (t > 1.0f)
        {
            int temp = focalRangeMax;
            focalRangeMax = focalRangeMin;
            focalRangeMin = temp;
            t = 0.0f;
        }
    }

    void LerpValueBetweenTwoPoints2()
    {
        TestTest = Mathf.Lerp(minimum, maximum, t);

        t += 0.5f * Time.deltaTime;

        if (t > 1.0f)
        {
            float temp = maximum;
            maximum = minimum;
            minimum = temp;
            t = 0.0f;
        }
    }
    #endregion

    public float EaseInOutQuint(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value * value * value * value + start;
        value -= 2;
        return end * 0.5f * (value * value * value * value * value + 2) + start;
    }
}
