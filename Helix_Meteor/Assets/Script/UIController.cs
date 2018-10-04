using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIController : MonoBehaviour {

    [SerializeField] GameObject prefav_player = null;
    [SerializeField] GameObject Player_ = null;
    [SerializeField] GameObject MainPanel_ = null;
    [SerializeField] GameObject ContinueButton = null;
    [SerializeField] GameObject RetryButton = null;
    [SerializeField] Camera meteor_camera = null;
    [SerializeField] GameObject game_manager = null;

    private Vector3 dying_position;

    //スタートボタン押下時の処理
    public void PushStartButton()
    {
        if(Player_ == null)
        {
            return;
        }
        Player_.SetActive(true);
        MainPanel_.SetActive(false);
        gameObject.SetActive(false);
    }
    //コンティニューボタン押下時の処理
    public void PushContinueButton()
    {
        MainPanel_.SetActive(false);
        gameObject.SetActive(false);
        Debug.Log(dying_position);
        Vector3 continue_position = dying_position;
        GameObject new_player = Instantiate(prefav_player, continue_position, gameObject.transform.rotation) as GameObject;
        meteor_camera.GetComponent<MeteorCamera>().PlayerObjPro = new_player;
        game_manager.GetComponent<Controller>().PlayerObjProp = new_player;
        MainPanel_.SetActive(false);
        RetryButton.SetActive(false);
        //ノーダメージ期間の実装

        new_player.SetActive(true);
    }
    //リトライボタン押下時の処理
    public void PushRetryButton()
    {
        MainPanel_.SetActive(false);
        gameObject.SetActive(false);
        Vector3 retry_position = new Vector3(0,0,0);
        GameObject new_player = Instantiate(prefav_player,retry_position,gameObject.transform.rotation) as GameObject;
        meteor_camera.GetComponent<MeteorCamera>().PlayerObjPro = new_player;
        game_manager.GetComponent<Controller>().PlayerObjProp = new_player;
        MainPanel_.SetActive(false);
        ContinueButton.SetActive(false);
        new_player.SetActive(true);
    }

    public Vector3 DyingPosition
    {
        set
        {
            dying_position = value;
        }
    }
}