using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    private GameObject player_Prefab;
    private GameObject player;

    public GameObject Player { get => player; }


    void Awake()
    {
        player_Prefab = Resources.Load<GameObject>("Player");
        player = GameObject.Instantiate(player_Prefab,gameObject.transform);
    }



}
