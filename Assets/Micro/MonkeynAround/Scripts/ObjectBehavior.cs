using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Focus;
using System;

namespace Game.MonkeynAround
{
    public class ObjectBehavior : MicroMonoBehaviour
    {
        enum itemType { Coconut, Monkey };

        [SerializeField] GameObject target;
        [SerializeField] Rigidbody2D rb2D;
        [SerializeField] BoxCollider2D boxcoll2D;

        [SerializeField] float speed;

        //TWEEN VALUES
        [SerializeField] float time;
        [SerializeField] Vector3 change;
        [SerializeField] Vector3 startPosition;
        [SerializeField] Vector3 targetPosition;
        [SerializeField] float duration;
        [SerializeField] float durationForTrash;

        [SerializeField] itemType myType;        
        [SerializeField] GameObject intactCoconut;
        GameObject trashExit;

        GameObject generator;
        ItemGenerator itemGen;

        bool _moveToTrash;

        // Start is called before the first frame update
        void Start()
        {
            target = GameObject.Find("Target");
            trashExit = GameObject.Find("TrashExit");
            generator = GameObject.Find("Generator");

            itemGen = GetComponent<ItemGenerator>();
            rb2D = GetComponent<Rigidbody2D>();
            boxcoll2D = GetComponent<BoxCollider2D>();
            
            startPosition = transform.position;
            targetPosition = target.transform.position;

            if (myType == itemType.Coconut) this.gameObject.name = "COCONUT";
            else if (myType == itemType.Monkey) this.gameObject.name = "DONT DESTROY";
        }

        // Update is called once per frame
        void Update()
        {
            MoveToTarget();
            if (_moveToTrash) MoveToTrash();
        }

        private void MoveToTrash()
        {
            time += Time.deltaTime;
            targetPosition = trashExit.transform.position;

            change = targetPosition - startPosition;

            if (time <= duration)
            {
                transform.position = new Vector2(TweenManager.LinearTween(time, startPosition.x, change.x, durationForTrash), TweenManager.LinearTween(time, startPosition.y, change.y, durationForTrash));
            }

            if (time >= duration)
            {
                if (Mathf.Round(transform.position.x) == targetPosition.x) Destroy(gameObject);
            }
        }

        void MoveToTarget()
        {
            time += Time.deltaTime;

            change = targetPosition - startPosition;

            if (time <= duration)
            {
                transform.position = new Vector2(TweenManager.LinearTween(time, startPosition.x, change.x, duration), transform.position.y);
            }

            if (Mathf.Round(transform.position.x) == target.transform.position.x) Destroy(gameObject);
        }

        private void MoveOffScreen()
        {
            time += Time.deltaTime;

            change = targetPosition - startPosition;

            if (time <= duration)
            {
                transform.position = new Vector2(TweenManager.LinearTween(time, startPosition.x, change.x, durationForTrash), TweenManager.LinearTween(time, startPosition.y, change.y, durationForTrash));
            }

            if (time >= duration)
            {
                if (Mathf.Round(transform.position.x) == targetPosition.x) Destroy(gameObject);
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            //SLAMMING OBJECTS
            if (collision.gameObject.name == "Fists" && this.gameObject.name == "COCONUT")
            {
                this.gameObject.name = "BROKEN COCONUT";
                intactCoconut.SetActive(false);
            }

            if (collision.gameObject.name == "Fists" && gameObject.name == "DONT DESTROY")
            {
                generator.gameObject.SendMessage("Lost");
                Debug.Log("You Lose");
                Macro.Lose();
                Macro.EndGame();
            }

            //CHECKS OR CROSSES
            if(collision.gameObject.name == "Grinder" && this.gameObject.name == "COCONUT")
            {
                target.gameObject.SendMessage("Cross");

                //Launch coconut enter anim
                startPosition = transform.position;
                time = 0f;
                _moveToTrash = true;

                //itemGen.hasLost = true;

                generator.gameObject.SendMessage("Lost");
                Debug.Log("You Lose");
                Macro.Lose();
                Macro.EndGame();
            }

            if (collision.gameObject.name == "Grinder" && this.gameObject.name == "BROKEN COCONUT")
            {
                target.gameObject.SendMessage("Check");

                //Launch coconut enter anim
                startPosition = transform.position;
                time = 0f;
                _moveToTrash = true;
            }

            if (collision.gameObject.name == "Grinder" && this.gameObject.name == "DONT DESTROY")
            {
                //Launch monkey anim
                startPosition = transform.position;
                targetPosition = new Vector2(transform.position.x + 50, transform.position.y);
                time = 0f;
                MoveOffScreen();
            }
        }
    }
}