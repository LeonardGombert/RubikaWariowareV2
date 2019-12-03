using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ItemGenerator : MonoBehaviour
{
    [SerializeField] List<GameObject> itemsToSpawnList = new List<GameObject>();
    [SerializeField] float timeToWait;
    float timePassed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;

        if(timePassed >= timeToWait)
        {
            Instantiate(itemsToSpawnList[Random.Range(0, itemsToSpawnList.Count)], this.transform.position, Quaternion.identity);
            timePassed = 0;
        }
    }
}
