using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;
using Game.Focus;

public class FocalPlaneManager : MonoBehaviour
{
    /*GAME LOGIC
     * 
     * 3 Plane Objects, plane manager assigns sprites to them
     * 1 Plane is the "Target" plane
     * Focus starts on a random plane (One of three int values)
     * 
     * FocusManager eases a currentFocus value between 0 and 100%
     * The 3 planes individually change their blurAmount based on the currentFocus value
     */


    //FOCUS VALUES
    //[SerializeField] float startFocus;
    [SerializeField] [Range(0, 100)] float currentFocus;
    [SerializeField] [Range(0, 100)] int minFocus = 0;
    [SerializeField] [Range(0, 100)] int maxFocus = 100;

    [SerializeField] GameObject test;
    [SerializeField] BlurOptimized testBlur;
    public float convertedCurrentFocus;
    public int convertedCurrentFocus2;

    //TWEEN VALUES
    float focalStartPosition;
    float focalEndPosition;
    float distanceToFocalMax;
    [SerializeField] float totalTweenDuration;
    float tweenTimeValue;

    // Start is called before the first frame update
    void Awake()
    {
        focalStartPosition = minFocus;
        focalEndPosition = maxFocus;
    }

    // Update is called once per frame
    void Update()
    {
        tweenTimeValue += Time.deltaTime;

        VectorDrawer();

        UpdateBlurValues();
        EaseInQuadFocus();
        CheckPlayerInput();
    }

    void VectorDrawer()
    {
        Vector2 currentFocusVector = new Vector2(0, 0);

        Vector2 Center = new Vector2(0, 0);
        Vector2 Ord = new Vector2(0, 5);
        Vector2 Abs = new Vector2(5, 0);

        Vector2 testVector1 = new Vector2(0, 0);
        Vector2 testVector2 = new Vector2(1.5f, 1);
        Vector2 testVector3 = new Vector2(3,0);

        Debug.DrawLine(Center, Ord, Color.green);
        Debug.DrawLine(Center, Abs, Color.green);

        Debug.DrawLine(testVector1, testVector2, Color.red);
        Debug.DrawLine(testVector2, testVector3, Color.red);

        if(currentFocusVector.x <= Abs.x)
        {
            currentFocusVector.x += 0.1f;
            bool moveBack = false;

            if (currentFocusVector.x >= Abs.x) moveBack = true;

            if (moveBack)
            {
                currentFocusVector.x -= 0.1f;
                if (currentFocusVector.x <= 0) moveBack = false;
            }
        }

        test.transform.position = currentFocusVector;
    }

    void UpdateBlurValues()
    {
        if (currentFocus <= 10) testBlur.enabled = false;
        else testBlur.enabled = true;

        convertedCurrentFocus = (float)CustomScaler.Scale((int)currentFocus, minFocus, maxFocus, 0, 3);
        testBlur.blurSize = convertedCurrentFocus;

        convertedCurrentFocus2 = (int)CustomScaler.Scale((int)currentFocus, minFocus, maxFocus, 1, 2);
        testBlur.blurIterations = convertedCurrentFocus2;
    }

    void EaseInOutQuadFocus()
    {
        distanceToFocalMax = focalEndPosition - focalStartPosition;

        if (tweenTimeValue <= totalTweenDuration)
        {
            currentFocus = TweenManager.EaseInOutQuint(tweenTimeValue, focalStartPosition, distanceToFocalMax, totalTweenDuration);
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

    void EaseInQuadFocus()
    {
        distanceToFocalMax = focalEndPosition - focalStartPosition;

        if (tweenTimeValue < totalTweenDuration)
        {
            currentFocus = TweenManager.EaseInQuad(tweenTimeValue, focalStartPosition, distanceToFocalMax, totalTweenDuration);
            if (currentFocus == maxFocus) currentFocus = TweenManager.EaseOutQuad (tweenTimeValue, focalStartPosition, distanceToFocalMax, totalTweenDuration);
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
            /*if (currentFocus >= minFocusMargin && currentFocus <= maxFocusMargin)
            {
                Debug.Log("Sucess!");
            }*/
        }
    }
}