using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Sirenix.OdinInspector;

namespace Game.Focus
{
    public class FocusManager : MicroMonoBehaviour
    {
        #region //VARIABLE DECLARATIONS
        //Camera Base Values
        [FoldoutGroup("CameraValues")] [SerializeField] [Range(0, 200)] float currentFocus;
        [FoldoutGroup("CameraValues")] [SerializeField] [Range(0, 200)] int minFocus;
        [FoldoutGroup("CameraValues")] [SerializeField] [Range(0, 200)] int maxFocus;

        //TWEEN VALUES
        [FoldoutGroup("TweenValues")] [SerializeField] float totalTweenDuration;
        [FoldoutGroup("TweenValues")] [SerializeField] float tweenTimeValue;
        float focalStartPosition;
        float focalEndPosition;
        float distanceToFocalMax;

        //GameObjects to Blur
        [FoldoutGroup("AssignObjects")] [SerializeField] GameObject foregroundGameobject;
        [FoldoutGroup("AssignObjects")] [SerializeField] GameObject middlegroundGameobject;
        [FoldoutGroup("AssignObjects")] [SerializeField] GameObject backgroundGameobject;

        //GameObject Positions on Graph
        [FoldoutGroup("Positions")] [SerializeField] Vector2 foreGoPosition;
        [FoldoutGroup("Positions")] [SerializeField] Vector2 midGoPosition;
        [FoldoutGroup("Positions")] [SerializeField] Vector2 backGoPosition;
        [FoldoutGroup("Positions")] [SerializeField] bool shouldMove;

        //BLUR VALUES
        [FoldoutGroup("AssignObjects")] [SerializeField] Camera foreGrDOF;
        [FoldoutGroup("AssignObjects")] [SerializeField] Camera middGrDOF;
        [FoldoutGroup("AssignObjects")] [SerializeField] Camera backGrDOF;
        [FoldoutGroup("AssignObjects")] [SerializeField] DepthOfField generalCam;

        [FoldoutGroup("BlurValues")] [SerializeField] float convertedForeFocalLength;
        [FoldoutGroup("BlurValues")] [SerializeField] int convertedForeBlurIte;

        [FoldoutGroup("BlurValues")] [SerializeField] float convertedMiddBlurSize;
        [FoldoutGroup("BlurValues")] [SerializeField] int convertedMiddBlurIte;

        [FoldoutGroup("BlurValues")] [SerializeField] float convertedBackBlurSize;
        [FoldoutGroup("BlurValues")] [SerializeField] int convertedBackBlurIte;

        [FoldoutGroup("BlurValuesPercentages")] [SerializeField] [Range(0, 100)] float foreGoBlurPercentage;
        [FoldoutGroup("BlurValuesPercentages")] [SerializeField] [Range(0, 100)] float middGoBlurPercentage;
        [FoldoutGroup("BlurValuesPercentages")] [SerializeField] [Range(0, 100)] float backGoBlurPercentage;

        Vector3 basePosition1;
        Vector3 basePosition2;
        Vector3 basePosition3;

        //Macro Variables
        private bool isGameStarted = false;
        #endregion

        #region //MONOBEHAVIOR CALLBACKS
        private void Start()
        {
            focalStartPosition = minFocus;
            focalEndPosition = maxFocus;

            basePosition1 = foregroundGameobject.transform.position;
            basePosition2 = middlegroundGameobject.transform.position;
            basePosition3 = backgroundGameobject.transform.position;
        }

        private void Update()
        {
            tweenTimeValue += Time.deltaTime;
            EaseInOutQuadFocus();
            UpdateAbsoluteValue();
            ConvertPositionToDOF();
        }
        #endregion

        #region //MICROBEHAVIOR CALLBACKS
        protected override void OnGameStart() => Macro.DisplayActionVerb("Catch !", 3);
        protected override void OnActionVerbDisplayEnd() => Macro.StartTimer(16);

        protected override void OnTimerEnd()
        {
            Macro.EndGame();
        }
        #endregion

        #region //CUSTOM FUNCTIONS
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

        void UpdateAbsoluteValue()
        {
            foreGoPosition = new Vector2(currentFocus, (-Mathf.Abs(currentFocus) + 100));
            if (foreGoPosition.y <= 0f) foreGoPosition.y = 0f;

            midGoPosition = new Vector2(currentFocus, (-Mathf.Abs(currentFocus - 100) + 100));
            if (midGoPosition.y <= 0f) midGoPosition.y = 0f;

            backGoPosition = new Vector2(currentFocus, (-Mathf.Abs(currentFocus - 200) + 100));
            if (backGoPosition.y <= 0f) backGoPosition.y = 0f;

            if (shouldMove)
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

        void ConvertPositionToDOF()
        {
            generalCam.focalLength = (float)CustomScaler.Scale((int)currentFocus, minFocus, maxFocus, 5, 15);
            foreGoBlurPercentage = 100 - foreGoPosition.y;
            middGoBlurPercentage = 100 - midGoPosition.y;
            backGoBlurPercentage = 100 - backGoPosition.y;
            /*
            foreGoBlurPercentage = 100 - foreGoPosition.y;
            if (foreGoPosition.y >= 90 && foreGoPosition.y <= 100) foreGrDOF.enabled = false;
                else foreGrDOF.enabled = true;
            convertedForeFocalLength = (float)CustomScaler.Scale((int)foreGoBlurPercentage, 0, 100, 0, 5);
            foreGrDOF.depth = convertedForeFocalLength;

            middGoBlurPercentage = 100 - midGoPosition.y;
            if (middGoBlurPercentage >= 90 && middGoBlurPercentage <= 100) middGrDOF.enabled = false;
                else middGrDOF.enabled = true;
            convertedMiddBlurSize = (float)CustomScaler.Scale((int)middGoBlurPercentage, 0, 100, 5, 10);
            middGrDOF.depth = convertedMiddBlurSize;

            backGoBlurPercentage = 100 - backGoPosition.y;
            if (backGoBlurPercentage >= 90 && backGoBlurPercentage <= 100) backGrDOF.enabled = false;
                else backGrDOF.enabled = true;
            convertedBackBlurSize = (float)CustomScaler.Scale((int)backGoBlurPercentage, 0, 100, 10, 15);
            backGrDOF.depth = convertedBackBlurSize;*/
        }
        #endregion
    }
}