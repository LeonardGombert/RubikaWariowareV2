using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Focus;

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

        [SerializeField] itemType myType;

        // Start is called before the first frame update
        void Start()
        {
            target = GameObject.Find("Target");
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
            float step = speed * Time.deltaTime;

            time += Time.deltaTime;

            change = targetPosition - startPosition;

            if (time <= duration)
            {
                transform.position = new Vector2(TweenManager.LinearTween(time, startPosition.x, change.x, duration), transform.position.y);
            }

            if (Mathf.Round(transform.position.x) == target.transform.position.x) Destroy(gameObject);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.name == "Fists" && this.gameObject.name == "COCONUT") Destroy(gameObject);
            if (collision.gameObject.name == "Fists" && gameObject.name == "DONT DESTROY")
            {
                Debug.Log("You Lose");
                Macro.Lose();
                Macro.EndGame();
            }
        }
    }
}