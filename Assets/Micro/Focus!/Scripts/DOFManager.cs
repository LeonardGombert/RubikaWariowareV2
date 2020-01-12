using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Sirenix.OdinInspector;

namespace Game.Focus
{
    public class DOFManager : MicroMonoBehaviour
    {
        #region //VARIABLE DECLARATIONS
        [SerializeField] enum whereToMove { moveToFore, moveToMid, moveToBack, moveToMidb, moveToForeb };
        whereToMove moveToTarget;
        //TWEEN VALUES
        [SerializeField] float focusTweenDuration;
        float tweenTimeValue;
        Vector3 startPosition;
        Vector3 targetPosition;
        Vector3 endPosition;
        Vector3 distanceToEnd;

        //GAMEOBJECTS INFO
        GameObject foregroundGameobject;
        GameObject middlegroundGameobject;
        GameObject backgroundGameobject;
        Vector3 basePosition1;
        Vector3 basePosition2;
        Vector3 basePosition3;

        //OTHER OBJECTS TO ASSIGN
        GameObject targetPlane;
        DepthOfField generalCam;
        Transform focusPointTransform;

        //MARGIN AND DEPTH VALUES
        float foreToMidDistance;
        float midToBackDistance;
        float focusPointCurrentDepth;
        float foreGroundMargin;
        float middleGroundMargin1;
        float middleGroundMargin2;
        float backGroundMargin;
        [SerializeField][Range(0, 100)] float focusDepthPercentage;

        bool hasTime = false;
        int beatsPassed;
        #endregion

        #region //MONOBEHAVIOR CALLBACKS
        private void Awake()
        {
            foregroundGameobject = GameObject.Find("Foreground");
            middlegroundGameobject = GameObject.Find("Middleground");
            backgroundGameobject = GameObject.Find("Background");
            targetPlane = GameObject.FindGameObjectWithTag("Player");

            generalCam = GameObject.Find("DOFCamera").GetComponent<DepthOfField>();
            focusPointTransform = GameObject.Find("FocusPoint").GetComponent<Transform>();
        }

        void Start()
        {
            basePosition1 = foregroundGameobject.transform.position;
            basePosition2 = middlegroundGameobject.transform.position;
            basePosition3 = backgroundGameobject.transform.position;

            startPosition = basePosition1;
            endPosition = basePosition2;
            moveToTarget = whereToMove.moveToBack;

            foreToMidDistance = middlegroundGameobject.transform.position.z + foregroundGameobject.transform.position.z;
            midToBackDistance = backgroundGameobject.transform.position.z - middlegroundGameobject.transform.position.z;
            Macro.StartGame();
        }

        private void Update()
        {
            ConvertPositionToDOFAndOtherFunc();
            TweenLerpFocusPoint();
            ConvertCameraDepthToPercentage();
            PlayerInputs();
        }
        #endregion

        #region //MICROBEHAVIOR CALLBACKS
        protected override void OnGameStart()
        {
            Macro.DisplayActionVerb("Focus !", 3);
            Macro.StartTimer(16);
            base.OnGameStart();
        }   

        protected override void OnTimerEnd()
        {
            Macro.EndGame();
            Macro.Lose();
            base.OnTimerEnd();
        }

        protected override void OnBeat()
        {
            base.OnBeat();
            beatsPassed++;

            Debug.Log(Macro.TimeBeforeNextBeat);

            if (!hasTime)
            {
                float timeToNextBeat = (float)Macro.TimeSinceLastBeat;
                float timeToTwoBeats = timeToNextBeat * 2;
                focusTweenDuration = timeToTwoBeats;
                Debug.Log(focusTweenDuration);
                hasTime = true;
            }

            if(beatsPassed == 2)
            {
                beatsPassed = 0;
                hasTime = false;
            }
        }
        #endregion

        #region //CUSTOM FUNCTIONS

        void ConvertPositionToDOFAndOtherFunc()
        {
            generalCam.focalTransform = focusPointTransform;
            focusPointCurrentDepth = focusPointTransform.transform.position.z;
            tweenTimeValue += Time.deltaTime;
        }

        void TweenLerpFocusPoint()
        {
            distanceToEnd = endPosition - startPosition;

            if (tweenTimeValue <= focusTweenDuration)
            {
                focusPointTransform.position = 
                    new Vector3(0, TweenManager.EaseInOutQuint(tweenTimeValue, startPosition.y, distanceToEnd.y, focusTweenDuration), 
                    TweenManager.EaseInOutQuint(tweenTimeValue, startPosition.z, distanceToEnd.z, focusTweenDuration));
            }

            if (tweenTimeValue >= focusTweenDuration)
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

        void ConvertCameraDepthToPercentage()
        {
            focusDepthPercentage = focusPointCurrentDepth / backgroundGameobject.transform.position.z * 100;
            foreGroundMargin = foregroundGameobject.transform.position.z + foreToMidDistance * .1f;
            middleGroundMargin1 = middlegroundGameobject.transform.position.z - foreToMidDistance * .05f;
            middleGroundMargin2 = middlegroundGameobject.transform.position.z + midToBackDistance * .05f;
            backGroundMargin = backgroundGameobject.transform.position.z - midToBackDistance * .1f;
        }

        void PlayerInputs()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if(targetPlane == foregroundGameobject)
                {
                    if (focusPointCurrentDepth >= foregroundGameobject.transform.position.z && focusPointCurrentDepth <= foreGroundMargin)
                    {
                        Macro.EndGame();
                        Macro.Win();
                    }

                    else
                    {
                        Macro.EndGame();
                        Macro.Lose();
                    }
                }

                else if (targetPlane == middlegroundGameobject)
                {
                    if (focusPointCurrentDepth >= middleGroundMargin1 && focusPointCurrentDepth <= middleGroundMargin2)
                    {
                        Macro.EndGame(); Macro.Win();
                    }
                    else {
                        Macro.EndGame();
                        Macro.Lose();
                    }
                }

                else if (targetPlane == backgroundGameobject)
                {
                    if (focusPointCurrentDepth >= backGroundMargin && focusPointCurrentDepth <= backgroundGameobject.transform.position.z)
                    {
                        Macro.EndGame();
                        Macro.Win();
                    }

                    else
                    {
                        Macro.EndGame();
                        Macro.Lose();
                    }
                }
            }
        }
        #endregion
    }
}