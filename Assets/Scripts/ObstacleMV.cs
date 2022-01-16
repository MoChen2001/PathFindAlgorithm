using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMV : MonoBehaviour
{

    private GameObject obstacle_Prefab;
    
    


    public GameObject Obstacle_Prefab
    {
        get =>  obstacle_Prefab;
        set => obstacle_Prefab = value;
    }


    private void Awake()
    {
        obstacle_Prefab = Resources.Load<GameObject>("Obstacle");
    }




}
