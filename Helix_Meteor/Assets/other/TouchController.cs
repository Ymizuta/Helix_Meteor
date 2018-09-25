using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour {

    public void Update()
    {
        TouchInfo info = AppUtil.GetTouch();
        if (info == TouchInfo.Began)
        {
            Debug.Log("Begun!");
        }
        else
        if(info == TouchInfo.Moved)
        {
            Debug.Log("Move!");
        }else
        if (info == TouchInfo.Ended)
        {
            Debug.Log("Ended");
        }
    }

}