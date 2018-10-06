using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test_circle_move : MonoBehaviour {

    float deg;
    public Button test_button;

    // Use this for initialization
    void Start () {
        deg = 180;
        //Debug.Log(0 * Mathf.Deg2Rad);
        //Debug.Log("xは" + Mathf.Cos(0 * Mathf.Deg2Rad));
        //Debug.Log("yは" + Mathf.Sin(0 * Mathf.Deg2Rad));
        //Debug.Log(90 * Mathf.Deg2Rad);
        //Debug.Log("90度のときxは" + Mathf.Cos(90 * Mathf.Deg2Rad));
        //Debug.Log("90度のときyは" + Mathf.Sin(90 * Mathf.Deg2Rad));
        //Debug.Log(180 * Mathf.Deg2Rad);
        //Debug.Log("180度のときxは" + Mathf.Cos(180 * Mathf.Deg2Rad));
        //Debug.Log("180度のときyは" + Mathf.Sin(180 * Mathf.Deg2Rad));
        //Debug.Log(270 * Mathf.Deg2Rad);
        //Debug.Log("270度のときxは" + Mathf.Cos(270 * Mathf.Deg2Rad));
        //Debug.Log("270度のときyは" + Mathf.Sin(270 * Mathf.Deg2Rad));
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetButton("Horizontal"))
        {
            deg += 10;
            float speed = 2f; 
            float x = Mathf.Cos(deg * Mathf.Deg2Rad)*speed;
            float y = Mathf.Sin(deg * Mathf.Deg2Rad)*speed;
            //Debug.Log(x);
            //Debug.Log(y);

            gameObject.transform.position = new Vector3(x, y, 0);
        }

        if (Input.GetMouseButton(0))
        {
            Debug.Log("消えよ");
            test_button.enabled = false;
        }
	}
 }
