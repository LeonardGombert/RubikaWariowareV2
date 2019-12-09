using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using UnityEngine.Rendering.PostProcessing;
using Sirenix.OdinInspector;

public class TestAbsGraphTheory : MonoBehaviour
{
    //Camera Base Values
    [FoldoutGroup("CameraValues")][SerializeField] [Range(0, 200)] float currentFocus;
    [FoldoutGroup("CameraValues")][SerializeField] [Range(0, 200)] int minFocus;
    [FoldoutGroup("CameraValues")][SerializeField] [Range(0, 200)] int maxFocus;

    //TWEEN VALUES
    float focalStartPosition;
    float focalEndPosition;
    float distanceToFocalMax;
    [FoldoutGroup("TweenValues")][SerializeField] float totalTweenDuration;
    [FoldoutGroup("TweenValues")][SerializeField] float tweenTimeValue;

    //GameObjects to Blur
    [FoldoutGroup("AssignObjects")][SerializeField] GameObject foregroundGameobject;
    [FoldoutGroup("AssignObjects")][SerializeField] GameObject middlegroundGameobject;
    [FoldoutGroup("AssignObjects")][SerializeField] GameObject backgroundGameobject;

    //GameObject Positions on Graph
    [FoldoutGroup("Positions")][SerializeField] Vector2 foreGoPosition;
    [FoldoutGroup("Positions")][SerializeField] Vector2 midGoPosition;
    [FoldoutGroup("Positions")][SerializeField] Vector2 backGoPosition;
    [FoldoutGroup("Positions")][SerializeField] bool shouldMove;

    //BLUR VALUES
    [FoldoutGroup("AssignObjects")][SerializeField] BlurOptimized foreGrBlur;
    [FoldoutGroup("AssignObjects")][SerializeField] BlurOptimized middGrBlur;
    [FoldoutGroup("AssignObjects")][SerializeField] BlurOptimized backGrBlur;

    [FoldoutGroup("BlurValues")][SerializeField] float convertedForeBlurSize;
    [FoldoutGroup("BlurValues")][SerializeField] int convertedForeBlurIte;

    [FoldoutGroup("BlurValues")][SerializeField] float convertedMiddBlurSize;
    [FoldoutGroup("BlurValues")][SerializeField] int convertedMiddBlurIte;

    [FoldoutGroup("BlurValues")][SerializeField] float convertedBackBlurSize;
    [FoldoutGroup("BlurValues")][SerializeField] int convertedBackBlurIte;

    [FoldoutGroup("BlurValuesPercentages")][SerializeField][Range(0, 100)] float foreGoBlurPercentage;
    [FoldoutGroup("BlurValuesPercentages")] [SerializeField] [Range(0, 100)] float middGoBlurPercentage;
    [FoldoutGroup("BlurValuesPercentages")] [SerializeField] [Range(0, 100)] float backGoBlurPercentage;

    Vector3 basePosition1;
    Vector3 basePosition2;
    Vector3 basePosition3;

    #region //POST PROCESSING
    List<PostProcessVolume> m_VolumeList = new List<PostProcessVolume>();
    UnityEngine.Rendering.PostProcessing.DepthOfField depthOfFieldLayer = null;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        focalStartPosition = minFocus;
        focalEndPosition = maxFocus;

        basePosition1 = foregroundGameobject.transform.position;
        basePosition2 = middlegroundGameobject.transform.position;
        basePosition3 = backgroundGameobject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        tweenTimeValue += Time.deltaTime;
        EaseInOutQuadFocus();
        UpdateAbsoluteValue();
        ConvertValuesToBlur();
    }

    void UpdateAbsoluteValue()
    {
        foreGoPosition = new Vector2(currentFocus, (-Mathf.Abs(currentFocus) + 100));
        if (foreGoPosition.y <= 0f) foreGoPosition.y = 0f;

        midGoPosition = new Vector2(currentFocus, (-Mathf.Abs(currentFocus  -100) + 100));
        if (midGoPosition.y <= 0f) midGoPosition.y = 0f;

        backGoPosition = new Vector2(currentFocus, (-Mathf.Abs(currentFocus - 200) + 100));
        if (backGoPosition.y <= 0f) backGoPosition.y = 0f;

        if(shouldMove)
        {
            foregroundGameobject.transform.position = foreGoPosition;
            middlegroundGameobject.transform.position = midGoPosition;
            backgroundGameobject.transform.position = backGoPosition;
        }

        if (!shouldMove)
        {
            foregroundGameobject.transform.position = basePosition1;
            middlegroundGameobject.transform.position = basePosition2;
            backgroundGameobject.transform.position = basePosition3;
        }
    }

    void ConvertValuesToBlur()
    {
        /*
         * Take Y value, which is the focus percentage
         * store the remaining percentage as the value to convert for blurring
         * scale this value to apply it to blur size/iterations
         */

        foreGoBlurPercentage = 100 - foreGoPosition.y;
        if (foreGoPosition.y >= 90 && foreGoPosition.y <= 100) foreGrBlur.enabled = false;
            else foreGrBlur.enabled = true;

        convertedForeBlurSize = (float)CustomScaler.Scale((int)foreGoBlurPercentage, 0, 100, 0, 3);
        foreGrBlur.blurSize = convertedForeBlurSize;
        convertedForeBlurIte = (int)CustomScaler.Scale((int)foreGoBlurPercentage, 0, 100, 1, 2);
        foreGrBlur.blurIterations = convertedForeBlurIte;

        middGoBlurPercentage = midGoPosition.y;
        if (middGoBlurPercentage >= 90 && middGoBlurPercentage <= 100) middGrBlur.enabled = false;
            else middGrBlur.enabled = true;

        convertedMiddBlurSize = (float)CustomScaler.Scale((int)middGoBlurPercentage, minFocus, maxFocus, 0, 3);
        middGrBlur.blurSize = convertedMiddBlurSize;
        convertedMiddBlurIte = (int)CustomScaler.Scale((int)middGoBlurPercentage, minFocus, maxFocus, 1, 2);
        middGrBlur.blurIterations = convertedMiddBlurIte;

        backGoBlurPercentage = backGoPosition.y;
        if (backGoBlurPercentage >= 90 && backGoBlurPercentage <= 100) backGrBlur.enabled = false;
            else backGrBlur.enabled = true;

        convertedBackBlurSize = (float)CustomScaler.Scale((int)backGoBlurPercentage, minFocus, maxFocus, 0, 3);
        backGrBlur.blurSize = convertedBackBlurSize;
        convertedBackBlurIte = (int)CustomScaler.Scale((int)backGoBlurPercentage, minFocus, maxFocus, 1, 2);
        backGrBlur.blurIterations = convertedBackBlurIte;
    }

    void EaseInOutQuadFocus()
    {
        distanceToFocalMax = focalEndPosition - focalStartPosition;

        if (tweenTimeValue <= totalTweenDuration)
        {
            currentFocus = TweenManager.EaseInOutQuad(tweenTimeValue, focalStartPosition, distanceToFocalMax, totalTweenDuration);
        }


        if (tweenTimeValue >= totalTweenDuration)
        {
            Debug.Log("I've finished Easing");
            float temp = focalEndPosition;
            focalEndPosition = focalStartPosition;
            focalStartPosition = temp;
            tweenTimeValue = 0.0f;
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

            //Depth of field
            depthOfFieldLayer.enabled.value = true;
            depthOfFieldLayer.focusDistance.value = currentFocus;

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
