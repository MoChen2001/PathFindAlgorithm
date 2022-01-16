using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetControl : MonoBehaviour
{
    private GameObject target_preafab;
    private Ray screenRay;
    private RaycastHit hit;
    private GameObject currTarget;

    public Action action;
    public GameObject CurrTarget {  get => currTarget; }

    private void Start()
    {
        target_preafab = Resources.Load<GameObject>("Target");
        currTarget = GameObject.Instantiate(target_preafab, new Vector3(0, 0.5f, 0), 
            Quaternion.identity, gameObject.transform);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(screenRay,out hit))
            {
                if(hit.transform.tag == "Ground")
                {
                    Vector3 pos = new Vector3((int)hit.point.x, 0.5f, (int)hit.point.z);
                    currTarget.transform.position = pos;
                }
            }
        }
    }
}
