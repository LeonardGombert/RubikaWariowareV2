using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Focus;

namespace Game.MonkeynAround
{
    public class FistSlam : MicroMonoBehaviour
    {
        [SerializeField] GameObject target;
        [SerializeField] bool slamming = false;
        [SerializeField] bool hasSlammed = false;
        [SerializeField] GameObject initialPosition;

        //TWEEN VALUES
        [SerializeField] float time;
        [SerializeField] Vector3 change;
        [SerializeField] Vector3 startPosition;
        [SerializeField] Vector3 targetPosition;
        [SerializeField] float duration;

        [SerializeField] Sprite armsUp;
        [SerializeField] Sprite armsDown;
        SpriteRenderer gorillaSr;
        [SerializeField] SpriteRenderer slamSr;
        [SerializeField] AudioSource slamSFX;

        // Start is called before the first frame update
        void Start()
        {
            startPosition = transform.position;
            targetPosition = target.transform.position;
            gorillaSr = GetComponentInChildren<SpriteRenderer>();
            slamSFX = GetComponentInChildren<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                slamming = true;
                slamSFX.Play();
            }

                if (slamming)
            {
                gorillaSr.sprite = armsDown;
                gorillaSr.sortingOrder = 9;
                change = targetPosition - startPosition;

                if (time <= duration)
                {
                    time += Time.deltaTime;
                    transform.position = new Vector2(targetPosition.x, TweenManager.LinearTween(time, startPosition.y, change.y, duration));
                    slamSr.enabled = true;
                }

                if (time >= duration)
                {
                    slamming = false;
                    transform.position = startPosition;
                    time = 0f;
                }
            }

            else
            {

                //slamSFX.Stop();
                gorillaSr.sprite = armsUp;
                gorillaSr.sortingOrder = 1;
                slamSr.enabled = false;
            }
        }
    }
}
