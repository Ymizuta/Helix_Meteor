using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float player_posz;           //
    public float fall_speed = 0.001f;    //前方に移動するスピード
    public float add_speed = 0.5f;

    float player_degree;
    float move_speed;
    float height = 1f;
    float width = 1f;
    enum DIRECTION
    {
        Right,
        Left
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame  
    void Update()
    {
        //直進
        player_posz += fall_speed;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, player_posz);

        //左右入力取得
        DIRECTION direction;
        //右
        if(Input.GetAxis("Horizontal")>0)
        {
            direction = DIRECTION.Right;
            Move(direction);
        }
        else
        //左
        if(Input.GetAxis("Horizontal") < 0)
        {
            direction = DIRECTION.Left;
            Move(direction);
        }

        //時間経過でスピードアップ
        fall_speed = SpeedUp(add_speed);

    }

    void Move(DIRECTION direction)
    {
        float x;
        float y;

        switch (direction)
        {
            //右
            case DIRECTION.Right:
                player_degree += 1;
                x = Mathf.Cos(player_degree * Mathf.Deg2Rad) * 3;
                y = Mathf.Sin(player_degree * Mathf.Deg2Rad) * 3;
                player_posz += fall_speed;
                gameObject.transform.position = new Vector3(x, y, player_posz);
                return;       
            //左 
            case DIRECTION.Left:
                player_degree -= 1;
                x = Mathf.Cos(player_degree * Mathf.Deg2Rad) * 3;
                y = Mathf.Sin(player_degree * Mathf.Deg2Rad) * 3;
                player_posz += fall_speed;
                gameObject.transform.position = new Vector3(x, y, player_posz);
                return;
            default:
                return;
        }
    }

    public float SpeedUp(float add_speed)
    {
        fall_speed += add_speed * Time.deltaTime;
        return fall_speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("衝突！");
    }

}
