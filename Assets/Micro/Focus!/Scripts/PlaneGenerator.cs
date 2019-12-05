using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlaneGenerator : MonoBehaviour
{

    /* COLLECTION OF POTENTIAL IMAGES/PLANES
     *
     * Option 1 --> Create 1 list per image with each plane as seperate objects. These will have "Plane1, Plane2,..." tags
     * Option2 --> ?
     * 
     * Create a Random.Range function that selects one of the lists, listed in a list of lists (dictionary?)
     * 
     * Assign that list as the "Focal Distance" list
     * 
     * 
     */

    [ShowInInspector] List<List<GameObject>> MasterList = new List<List<GameObject>>();
    [SerializeField] List<GameObject> Plane1List = new List<GameObject>();
    [SerializeField] List<GameObject> Plane2List = new List<GameObject>();
    [SerializeField] List<GameObject> Plane3List = new List<GameObject>();
    [ShowInInspector] public static List<GameObject> targetObjects = new List<GameObject>();
    int targetFocalPlane;

    // Start is called before the first frame update
    void Awake()
    {
        MasterList.Add(Plane1List);
        MasterList.Add(Plane2List);
        MasterList.Add(Plane3List);
        AssignPlaneTags();

        targetFocalPlane = Random.Range(0, MasterList.Count);
        targetObjects = MasterList[targetFocalPlane];
        //SendMessageToTargetFocalPlane();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AssignPlaneTags()
    {
        float i = 0;
        foreach (List<GameObject> subList in MasterList)
        {
            i++;
            foreach (GameObject item in subList)
            {
                item.gameObject.name = "Plane" + i + " Object";
            }
        }
    }

    void SendMessageToTargetFocalPlane()
    {
        float targetNumber = 0;
        foreach (GameObject item in targetObjects)
        {
            targetNumber++;
            item.gameObject.name = "Target";
        }   
    }
}