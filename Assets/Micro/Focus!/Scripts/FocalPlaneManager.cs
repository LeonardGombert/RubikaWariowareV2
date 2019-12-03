using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FocalPlaneManager : MonoBehaviour
{   
    // FIND WHICH PLANES ARE WHICH
    [SerializeField] GameObject Plane1;
    [SerializeField] GameObject Plane2;
    [SerializeField] GameObject Plane3;

    const int focalLength = 100;

    int baseFocalRangeMin = 0;
    int baseFocalRangeMax= focalLength;
    [SerializeField] [Range(0, focalLength)] int targetFocus;
    [SerializeField] [Range(0, focalLength)] int minFocusMargin;
    [SerializeField] [Range(0, focalLength)] int maxFocusMargin;
    [SerializeField] [Range(0, focalLength)] float currentFocus;

    float focalStartPosition;
    float focalEndPosition;
    float distanceToFocalMax;
    [SerializeField] float totalTweenDuration;
    float tweenTimeValue;

    #region //POST PROCESSING
    [SerializeField] List<PostProcessVolume> m_VolumeList = new List<PostProcessVolume>();
    DepthOfField depthOfFieldLayer = null;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        focalStartPosition = baseFocalRangeMin;
        focalEndPosition = baseFocalRangeMax;

        Plane1 = GameObject.Find("Plane1 Object");
        Plane2 = GameObject.Find("Plane2 Object");
        Plane3 = GameObject.Find("Plane3 Object");

    }

    // Update is called once per frame
    void Update()
    {
        tweenTimeValue += Time.deltaTime;

        SetFocusValues();
        EaseInOutQuadFocus();
        FocusPostProcessing();
        CheckPlayerInput();
    }

    void SetFocusValues()
    {
        minFocusMargin = targetFocus - 5 * baseFocalRangeMax / 100;
        maxFocusMargin = targetFocus + 5 * baseFocalRangeMax / 100;
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
        foreach (PostProcessVolume m_Volume in m_VolumeList)
        {
            m_Volume.profile.TryGetSettings(out depthOfFieldLayer);

            float a = depthOfFieldLayer.focusDistance.value;
            float b = 300f;

            float c = b - a;

            //Bloom Values
            depthOfFieldLayer.enabled.value = true;
            depthOfFieldLayer.focusDistance.value = TweenManager.EaseInOutQuint(tweenTimeValue, a, c, totalTweenDuration);
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
             */
        }
    }
}
