using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;
using Game.Focus;

public class FocalPlaneManagerOld : MonoBehaviour
{   
    // FIND WHICH PLANES ARE WHICH
    [SerializeField] GameObject Plane1;
    [SerializeField] GameObject Plane2;
    [SerializeField] GameObject Plane3;

    BlurOptimized Plane1Blur;
    BlurOptimized Plane2Blur;
    BlurOptimized Plane3Blur;

    const int focalLength = 100;

    int baseFocalRangeMin = 0;
    int baseFocalRangeMax= focalLength;
    [SerializeField] [Range(0, focalLength)] int targetFocus;
    [SerializeField] [Range(0, focalLength)] int minFocusMargin;
    [SerializeField] [Range(0, focalLength)] int maxFocusMargin;
    [SerializeField] [Range(0, focalLength)] float currentFocus;

    [SerializeField] [Range(0, 100)] int focalPlane1FocusLevel;
    [SerializeField] [Range(0, 100)] int focalPlane2FocusLevel;
    [SerializeField] [Range(0, 100)] int focalPlane3FocusLevel;


    //TWEEN VALUES
    float focalStartPosition;
    float focalEndPosition;
    float distanceToFocalMax;
    [SerializeField] float totalTweenDuration;
    float tweenTimeValue;

    #region //POST PROCESSING
    [SerializeField] List<PostProcessVolume> m_VolumeList = new List<PostProcessVolume>();
    UnityStandardAssets.ImageEffects.DepthOfField depthOfFieldLayer = null;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        focalStartPosition = baseFocalRangeMin;
        focalEndPosition = baseFocalRangeMax;

        Plane1 = GameObject.Find("Plane1 Object");
        Plane2 = GameObject.Find("Plane2 Object");
        Plane3 = GameObject.Find("Plane3 Object");

        Plane1Blur = Plane1.GetComponentInChildren<BlurOptimized>();
        Plane2Blur = Plane2.GetComponentInChildren<BlurOptimized>();
        Plane3Blur = Plane3.GetComponentInChildren<BlurOptimized>();
    }

    // Update is called once per frame
    void Update()
    {
        tweenTimeValue += Time.deltaTime;

        SetFocusValues();
        UpdateFocalPlaneValues();
        //EaseInOutQuadFocus();
        EaseInQuadFocus();
        FocusPostProcessing();
        CheckPlayerInput();
    }

    void SetFocusValues()
    {
        minFocusMargin = targetFocus - 5 * baseFocalRangeMax / focalLength;
        maxFocusMargin = targetFocus + 5 * baseFocalRangeMax / focalLength;
    }

    void UpdateFocalPlaneValues()
    {
        if (minFocusMargin < currentFocus && currentFocus < maxFocusMargin) Plane1Blur.enabled = false;
        else Plane1Blur.enabled = true;
        Plane1Blur.blurIterations = (int)currentFocus;
        //focalPlane1 = (int)currentFocus;
        //if (focalPlane1 < 50) focalPlane2 += focalPlane1 / 2;
        //else if (focalPlane1 > 50) focalPlane2 -= focalPlane3 / 2;
        focalPlane3FocusLevel = 100 - focalPlane1FocusLevel;
    }

    void EaseInOutQuadFocus()
    {
        distanceToFocalMax = focalEndPosition - focalStartPosition;

        if (tweenTimeValue < totalTweenDuration)
        {
            currentFocus = TweenManager.EaseInOutQuint(tweenTimeValue, focalStartPosition, distanceToFocalMax, totalTweenDuration);
        }

        if (tweenTimeValue > totalTweenDuration)
        {
            Debug.Log("I've finished Easing");
            float temp = focalEndPosition;
            focalEndPosition = focalStartPosition;
            focalStartPosition = temp;
            tweenTimeValue = 0.0f;
        }
    }

    void EaseInQuadFocus()
    {
        distanceToFocalMax = focalEndPosition - focalStartPosition;

        if (tweenTimeValue < totalTweenDuration)
        {
            currentFocus = TweenManager.EaseInQuad(tweenTimeValue, focalStartPosition, distanceToFocalMax, totalTweenDuration);
            if (currentFocus == focalLength) currentFocus = TweenManager.EaseOutQuad (tweenTimeValue, focalStartPosition, distanceToFocalMax, totalTweenDuration);
        }
        if (tweenTimeValue > totalTweenDuration)
        {
            Debug.Log("I've finished Easing");
            float temp = focalEndPosition;
            focalEndPosition = focalStartPosition;
            focalStartPosition = temp;
            tweenTimeValue = 0.0f;
        }
    }

    void EaseOutQuadFocus()
    {

    }

    void CheckPlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentFocus >= minFocusMargin && currentFocus <= maxFocusMargin)
            {
                Debug.Log("Sucess!");
            }
        }
    }

    void FocusPostProcessing()
    {
        /*foreach (PostProcessVolume m_Volume in m_VolumeList)
        {
            m_Volume.profile.TryGetSettings(out depthOfFieldLayer);

            float a = depthOfFieldLayer.focusDistance.value;
            float b = 300f;

            float c = b - a;

            //Bloom Values
            depthOfFieldLayer.enabled.value = true;
            depthOfFieldLayer.focusDistance.value = currentFocus; //TweenManager.EaseInOutQuint(tweenTimeValue, a, c, totalTweenDuration);
            if (depthOfFieldLayer.focusDistance.value == 300f)
            {
                float temp = b;
                b = a;
                a = temp;
                a = 0.1f;
            }

            /*
             * PLUS LA VALEUR CURRENT FOCUS SE RAPPROCHE DE TARGET FOCUS, PLUS LA VALEUR FOCAL LENGTH DOIT SE RAPPROCHER DE FOCUS DISTANCE
             * focus distance = 150
             * focal length = Lerp(300, 150, totalTweenDuration)
             * if startValue = 150, *flip*
             
        }*/
    }
}
