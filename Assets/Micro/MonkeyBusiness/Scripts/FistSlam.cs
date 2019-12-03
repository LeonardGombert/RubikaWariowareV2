using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistSlam : MonoBehaviour
{
    public GameObject target;
    public float speed;
    bool slam;
    float step;
    public Transform initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        step = speed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) slam = true;

        //if (Input.anyKeyDown) Debug.Log("yeet");

        if (slam)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
            if (transform.position == target.transform.position) slam = false;
        }

        else if (slam == false) transform.position = Vector3.MoveTowards(transform.position, initialPosition.position, step); 
    }

    void OnGUI()
    {
        if (Event.current.isKey && Event.current.type == EventType.KeyDown)
        {
            Debug.Log(Event.current.keyCode);
        }
    }
}
