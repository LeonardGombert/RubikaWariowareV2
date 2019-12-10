using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Sirenix.OdinInspector;
using System;

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
        [FoldoutGroup("AssignObjects")] [SerializeField] GameObject targetPlane;

        //GameObject Positions on Graph
        [FoldoutGroup("Positions")] [SerializeField] Vector2 foreGoPosition;
        [FoldoutGroup("Positions")] [SerializeField] Vector2 midGoPosition;
        [FoldoutGroup("Positions")] [SerializeField] Vector2 backGoPosition;
        [FoldoutGroup("Positions")] [SerializeField] bool shouldMove;

        //BLUR VALUES
        [FoldoutGroup("AssignObjects")] [SerializeField] DepthOfField generalCam;

        [FoldoutGroup("BlurValuesPercentages")] [SerializeField] [Range(0, 100)] float foreGoBlurPercentage;
        [FoldoutGroup("BlurValuesPercentages")] [SerializeField] [Range(0, 100)] float middGoBlurPercentage;
        [FoldoutGroup("BlurValuesPercentages")] [SerializeField] [Range(0, 100)] float backGoBlurPercentage;

        Vector3 basePosition1;
        Vector3 basePosition2;
        Vector3 basePosition3;

        Vector3 startPosition;
        Vector3 targetPosition;
        Vector3 endPosition;
        Vector3 distanceToEnd;

        [SerializeField] float foreToMidDistance;
        [SerializeField] float midToBackDistance;
        [SerializeField] float focusPointCurrentDepth;
        [SerializeField] float foreGroundMargin;
        [SerializeField] float middleGroundMargin1;
        [SerializeField] float middleGroundMargin2;
        [SerializeField] float backGroundMargin;
        [SerializeField][Range(0, 100)] float focusDepthPercentage;

        [FoldoutGroup("AssignObjects")][SerializeField] Transform focusPointTransform;

        [SerializeField] enum whereToMove { moveToFore, moveToMid, moveToBack, moveToMidb, moveToForeb };

        whereToMove moveToTarget;

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


            startPosition = basePosition1;
            endPosition = basePosition2;
            moveToTarget = whereToMove.moveToBack;

            foreToMidDistance = middlegroundGameobject.transform.position.z + foregroundGameobject.transform.position.z;
            midToBackDistance = backgroundGameobject.transform.position.z - middlegroundGameobject.transform.position.z;
        }

        private void Update()
        {
            focusPointCurrentDepth = focusPointTransform.transform.position.z;
            tweenTimeValue += Time.deltaTime;
            TweenLerpFocusPoint();
            ConvertPositionToDOF();
            ConvertCameraDepthToPercentage();
            PlayerInputs();
        }

        void ConvertCameraDepthToPercentage()
        {
            focusDepthPercentage = focusPointCurrentDepth / backgroundGameobject.transform.position.z * 100;

            foreGroundMargin = foregroundGameobject.transform.position.z + foreToMidDistance * .1f;
            middleGroundMargin1 = middlegroundGameobject.transform.position.z - foreToMidDistance * .05f;
            middleGroundMargin2 = middlegroundGameobject.transform.position.z + midToBackDistance * .05f;
            backGroundMargin = backgroundGameobject.transform.position.z - midToBackDistance * .1f;
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
        void EaseInOutQuintFocus()
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

        void TweenLerpFocusPoint()
        {
            distanceToEnd = endPosition - startPosition;

            if (tweenTimeValue <= totalTweenDuration)
            {
                focusPointTransform.position = 
                    new Vector3(0, TweenManager.EaseInOutQuint(tweenTimeValue, startPosition.y, distanceToEnd.y, totalTweenDuration), 
                    TweenManager.EaseInOutQuint(tweenTimeValue, startPosition.z, distanceToEnd.z, totalTweenDuration));
            }

            if (tweenTimeValue >= totalTweenDuration)
            {
                switch (moveToTarget)
                {
                    case whereToMove.moveToMid:
                        startPosition = basePosition1;
                        endPosition = basePosition2;
                        moveToTarget = whereToMove.moveToBack;
                        tweenTimeValue = 0.0f;
                        break;
                    case whereToMove.moveToBack:
                        startPosition = basePosition2;
                        endPosition = basePosition3;
                        moveToTarget = whereToMove.moveToMidb;
                        tweenTimeValue = 0.0f;
                        break;
                    case whereToMove.moveToMidb:
                        startPosition = basePosition3;
                        endPosition = basePosition2;
                        moveToTarget = whereToMove.moveToForeb;
                        tweenTimeValue = 0.0f;
                        break;
                    case whereToMove.moveToForeb:
                        startPosition = basePosition2;
                        endPosition = basePosition1;
                        moveToTarget = whereToMove.moveToMid;
                        tweenTimeValue = 0.0f;
                        break;
                    default:
                        break;
                }
            }
        }

        void ConvertPositionToDOF()
        {
            generalCam.focalTransform = focusPointTransform;
            /*generalCam.focalLength = (float)CustomScaler.Scale((int)currentFocus, minFocus, maxFocus, 5, 15);
            foreGoBlurPercentage = 100 - foreGoPosition.y;
            middGoBlurPercentage = 100 - midGoPosition.y;
            backGoBlurPercentage = 100 - backGoPosition.y;*/
        }

        void PlayerInputs()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if(targetPlane == foregroundGameobject)
                {
                    if (focusPointCurrentDepth >= foregroundGameobject.transform.position.z && focusPointCurrentDepth <= foreGroundMargin) Debug.Log("Sucess!");
                    else Debug.Log("Fail");
                }

                if (targetPlane == middlegroundGameobject)
                {
                    if (focusPointCurrentDepth >= middleGroundMargin1 && focusPointCurrentDepth <= middleGroundMargin2) Debug.Log("Sucess!");
                    else Debug.Log("Fail");
                }

                else if (targetPlane == backgroundGameobject)
                {
                    if (focusPointCurrentDepth >= backGroundMargin && focusPointCurrentDepth <= backgroundGameobject.transform.position.z) Debug.Log("Sucess!");
                    else Debug.Log("Fail");
                }
            }
        }

        #endregion
    }
}