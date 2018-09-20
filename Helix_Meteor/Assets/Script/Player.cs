using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] Controller Controller_ = null; // エディターからアタッチする

    public float player_posz;           //
    public float fall_speed = 0.001f;   //前方に移動する初期速度
    public float add_speed = 0.2f;      //時間経過で加算される速度

    //円周上を移動するための変数
    float player_degree;
    float move_speed = 5f;              //左右移動の速度
    float height = 1f;
    float width = 1f;

    // Update is called once per frame  
    void Update()
    {
        //直進
        player_posz += fall_speed;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, player_posz);
        //時間経過でスピードアップ
        fall_speed = SpeedUp(add_speed);
    }

    //プレイヤーの左右移動関数
    public void Move(Controller.DIRECTION direction)
    {
        float x;
        float y;

        switch (direction)
        {
            //右
            case Controller.DIRECTION.Right:
                player_degree += move_speed;
                x = Mathf.Cos(player_degree * Mathf.Deg2Rad) * 3;
                y = Mathf.Sin(player_degree * Mathf.Deg2Rad) * 3;
                player_posz += fall_speed;
                gameObject.transform.position = new Vector3(x, y, player_posz);
                return;
            //左 
            case Controller.DIRECTION.Left:
                player_degree -= move_speed;
                x = Mathf.Cos(player_degree * Mathf.Deg2Rad) * 3;
                y = Mathf.Sin(player_degree * Mathf.Deg2Rad) * 3;
                player_posz += fall_speed;
                gameObject.transform.position = new Vector3(x, y, player_posz);
                return;
            default:
                return;
        }
    }
    
    //プレイヤー加速関数
    public float SpeedUp(float add_speed)
    {
        fall_speed += add_speed * Time.deltaTime;
        return fall_speed;
    }
    //衝突処理
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("衝突！");
    }

}
