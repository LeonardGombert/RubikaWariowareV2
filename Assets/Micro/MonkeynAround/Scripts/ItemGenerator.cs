using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Game.MonkeynAround
{
    public class ItemGenerator : MicroMonoBehaviour
    {
        [SerializeField] List<GameObject> difficulty1List = new List<GameObject>();
        [SerializeField] List<GameObject> difficulty2List = new List<GameObject>();
        [SerializeField] List<GameObject> difficulty3List = new List<GameObject>();
        [SerializeField] float timeToWait;
        [SerializeField] float timePassed;

        [SerializeField] bool difficulty1;
        [SerializeField] bool difficulty2;
        [SerializeField] bool difficulty3;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            timePassed += Time.deltaTime;

            if (timePassed >= timeToWait)
            {
                if (difficulty1) Instantiate(difficulty1List[Random.Range(0, difficulty1List.Count)], this.transform.position, Quaternion.identity);
                else if (difficulty2) Instantiate(difficulty2List[Random.Range(0, difficulty2List.Count)], this.transform.position, Quaternion.identity);
                else if (difficulty3) Instantiate(difficulty3List[Random.Range(0, difficulty3List.Count)], this.transform.position, Quaternion.identity);

                timePassed = 0;
            }
        }
    }
}
