using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItem : MonoBehaviour
{
    GameObject target;
    Rigidbody2D rb2D;
    BoxCollider2D boxcoll2D;

    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Trash");
        rb2D = GetComponent<Rigidbody2D>();
        boxcoll2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);

        if (transform.position == target.transform.position) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Fists") Destroy(gameObject);
    }
}