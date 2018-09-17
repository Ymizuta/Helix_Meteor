using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float player_posz;           //
    public float fall_speed = 0.01f;    //前方に移動するスピード

    public float move_speed = 2f;
    public float width = 3f;
    public float hieght = 3f;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    //プレイヤーが前方に移動
    void Move()
    {
        float x = Mathf.Cos(Time.time * speed) * width;
        float y = Mathf.Sin(Time.time * speed) * height;
        gameObject.transform.Translate(x, y, player_posz);
        player_posz += fall_speed;
    }
}
