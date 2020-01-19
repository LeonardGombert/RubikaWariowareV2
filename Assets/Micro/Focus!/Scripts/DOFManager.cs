using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Sirenix.OdinInspector;
using System;
using UnityEngine.Rendering.PostProcessing;

namespace Game.Focus
{
    public class DOFManager : MicroMonoBehaviour
    {
        #region //VARIABLE DECLARATIONS
        [SerializeField] enum whereToMove { moveToFore, moveToMid, moveToBack, moveToMidb, moveToForeb };
        whereToMove moveToTarget;
        //TWEEN VALUES
        float focusTweenDuration;
        float tweenTimeValue;
        Vector3 startPosition;
        Vector3 targetPosition;
        Vector3 endPosition;
        Vector3 distanceToEnd;

        //VIGNETTE TWEEN VALUES
        bool takePicture;
        bool gameOver;
        float vignetteDuration = 0.1f;
        float vignetteTimeValue;
        float vignetteTimeValue2;
        float startVignetteValue = 0f;
        float endVignetteValue = 1f;
        [SerializeField] PostProcessVolume m_Volume;
        UnityEngine.Rendering.PostProcessing.Vignette vignetteLayer = null;

        //GAMEOBJECTS INFO
        GameObject foregroundGameobject;
        GameObject middlegroundGameobject;
        GameObject backgroundGameobject;
        Vector3 basePosition1;
        Vector3 basePosition2;
        Vector3 basePosition3;
        bool isOnTarget;

        //OTHER OBJECTS TO ASSIGN
        GameObject targetPlane;
        GameObject cameraHUDOverlay;
        UnityStandardAssets.ImageEffects.DepthOfField generalCam;
        GameObject focusPoint;
        AudioSource cameraAudioSource;
        [SerializeField] UnityEngine.UI.Image cameraHudCenter;
        [SerializeField] AudioClip cameraShutter;
        [SerializeField] AudioClip cameraFocus;

        [SerializeField] GameObject Picture1;
        [SerializeField] GameObject Picture2;
        [SerializeField] GameObject Picture3;

        [SerializeField] GameObject WinSprite;
        [SerializeField] GameObject LoseSprite;

        //MARGIN AND DEPTH VALUES
        float foreToMidDistance;
        float midToBackDistance;
        float focusPointCurrentDepth;
        float foreGroundMargin;
        float middleGroundMargin1;
        float middleGroundMargin2;
        float backGroundMargin;
        [SerializeField][Range(0, 100)] float focusDepthPercentage;
        float test;

        float timeLeftUI = 0.5f;
        float timeLeft = 2f;
        bool hasTime = false;
        int beatsPassed;
        #endregion

        #region //MONOBEHAVIOR CALLBACKS
        private void Awake()
        {
            int whichLayer = UnityEngine.Random.Range(0, 3);
            if (whichLayer == 0) Picture1.SetActive(true);
            if (whichLayer == 1) Picture2.SetActive(true);
            if (whichLayer == 2) Picture3.SetActive(true);

            foregroundGameobject = GameObject.Find("Foreground");
            middlegroundGameobject = GameObject.Find("Middleground");
            backgroundGameobject = GameObject.Find("Background");
            targetPlane = GameObject.FindGameObjectWithTag("Player");

            cameraHUDOverlay = GameObject.Find("Camera HUD");

            generalCam = GameObject.Find("DOFCamera").GetComponent<UnityStandardAssets.ImageEffects.DepthOfField>();
            focusPoint = GameObject.Find("FocusPoint");

            cameraAudioSource = focusPoint.GetComponent<AudioSource>();
            cameraAudioSource.clip = cameraFocus;
            cameraAudioSource.loop = true;

            basePosition1 = foregroundGameobject.transform.position;
            basePosition2 = middlegroundGameobject.transform.position;
            basePosition3 = backgroundGameobject.transform.position;

            startPosition = basePosition1;
            endPosition = basePosition2;
            moveToTarget = whereToMove.moveToBack;

            foreToMidDistance = middlegroundGameobject.transform.position.z + foregroundGameobject.transform.position.z;
            midToBackDistance = backgroundGameobject.transform.position.z - middlegroundGameobject.transform.position.z;
            test = cameraAudioSource.pitch;
            cameraAudioSource.Play();
        }

        void Start()
        {
            Macro.StartGame();
            cameraAudioSource.pitch = 1.5f / focusTweenDuration;
        }

        private void Update()
        {
            if (!gameOver)
            {
                CheckFocusPointPosition();
                ConvertPositionToDOF();
                TweenLerpFocusPoint();
                ConvertCameraDepthToPercentage();
            }

            else if(gameOver)
            {
                timeLeftUI -= Time.deltaTime;
                if (timeLeftUI < 0)
                {
                    LoseSprite.gameObject.SetActive(false);
                    WinSprite.gameObject.SetActive(false);
                }
                cameraAudioSource.Stop();
                timeLeft -= Time.deltaTime;
                if (timeLeft < 0)
                {
                    Macro.EndGame();
                }
            }
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
            base.OnTimerEnd();
            gameOver = true;
            Macro.EndGame();
            Macro.Lose();
        }

        protected override void OnBeat()
        {
            base.OnBeat();
            beatsPassed++;
            
            if (!hasTime)
            {
                float timeToNextBeat = (float)Macro.TimeSinceLastBeat;
                float timeToTwoBeats = timeToNextBeat * 2;
                focusTweenDuration = timeToTwoBeats;
                hasTime = true;
            }

            if(beatsPassed == 2)
            {
                beatsPassed = 0;
                hasTime = false;
            }
        }
        #endregion

        #region //CUSTOM FUNCTION
        private void CheckFocusPointPosition()
        {
            if (targetPlane == foregroundGameobject)
            {
                if (focusPointCurrentDepth >= foregroundGameobject.transform.position.z && focusPointCurrentDepth <= foreGroundMargin)
                {
                    cameraHudCenter.color = Color.green;
                    isOnTarget = true;
                }

                else
                {
                    cameraHudCenter.color = Color.white;
                    isOnTarget = false;
                }
            }

            else if (targetPlane == middlegroundGameobject)
            {
                if (focusPointCurrentDepth >= middleGroundMargin1 && focusPointCurrentDepth <= middleGroundMargin2)
                {
                    cameraHudCenter.color = Color.green;
                    isOnTarget = true;
                }
                else
                {
                    cameraHudCenter.color = Color.white;
                    isOnTarget = false;
                }
            }

            else if (targetPlane == backgroundGameobject)
            {
                if (focusPointCurrentDepth >= backGroundMargin && focusPointCurrentDepth <= backgroundGameobject.transform.position.z)
                {
                    cameraHudCenter.color = Color.green;
                    isOnTarget = true;
                }

                else
                {
                    cameraHudCenter.color = Color.white;
                    isOnTarget = false;
                }
            }
        }

        void ConvertPositionToDOF()
        {
            generalCam.focalTransform = focusPoint.transform;
            focusPointCurrentDepth = focusPoint.transform.position.z;
            tweenTimeValue += Time.deltaTime;
        }

        void TweenLerpFocusPoint()
        {
            distanceToEnd = endPosition - startPosition;

            if (tweenTimeValue <= focusTweenDuration)
            {
                focusPoint.transform.position = 
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
                cameraAudioSource.clip = cameraShutter;
                cameraAudioSource.loop = false;
                cameraAudioSource.Play();

                takePicture = true;

                if (isOnTarget == true)
                {
                    WinSprite.gameObject.SetActive(true);
                    Macro.Win();
                    gameOver = true;
                }
                
                else
                {
                    LoseSprite.gameObject.SetActive(true);
                    Macro.Lose();
                    gameOver = true;
                }
            }
            
            //VIGNETTE EFFECT
            m_Volume.profile.TryGetSettings(out vignetteLayer);
            float distanceToMax = endVignetteValue - startVignetteValue;
            float distanceToMax2 = startVignetteValue - endVignetteValue;
            
            if (takePicture)
            {
                vignetteLayer.enabled.value = true;

                if (vignetteTimeValue <= vignetteDuration)
                {
                    vignetteLayer.intensity.value = TweenManager.EaseInOutQuad(vignetteTimeValue, startVignetteValue, distanceToMax, vignetteDuration);
                    vignetteTimeValue += Time.deltaTime;
                }

                if (vignetteTimeValue >= vignetteDuration)
                {
                    vignetteLayer.intensity.value = TweenManager.EaseInOutQuad(vignetteTimeValue2, endVignetteValue, distanceToMax2, vignetteDuration);
                    vignetteTimeValue2 += Time.deltaTime;
                }

                if (vignetteTimeValue2 >= vignetteDuration)
                {
                    vignetteTimeValue = 0f;
                    vignetteTimeValue2 = 0f;
                    takePicture = false;
                    gameOver = true;
                    cameraHUDOverlay.SetActive(false);
                }
            }
        }
        #endregion
    }
}