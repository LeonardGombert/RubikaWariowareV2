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

        // Start is called before the first frame update
        void Start()
        {
            startPosition = transform.position;
            targetPosition = target.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) slamming = true;

            if (slamming)
            {
                change = targetPosition - startPosition;

                if (time <= duration)
                {
                    time += Time.deltaTime;
                    transform.position = new Vector2(targetPosition.x, TweenManager.LinearTween(time, startPosition.y, change.y, duration));
                }

                if (time >= duration && new Vector3(transform.position.x, Mathf.Round(transform.position.y)) == targetPosition)
                {
                    Vector2 temp = targetPosition;
                    targetPosition = startPosition;
                    startPosition = temp;
                    hasSlammed = true;
                    time = 0f;
                }

                if (hasSlammed && new Vector3(transform.position.x, Mathf.Round(transform.position.y)) == initialPosition.transform.position)
                {
                    Vector2 temp = startPosition;
                    startPosition = targetPosition;
                    targetPosition = temp;

                    transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y));
                    hasSlammed = false;
                    slamming = false;
                    time = 0f;
                }
            }
        }
    }
}
