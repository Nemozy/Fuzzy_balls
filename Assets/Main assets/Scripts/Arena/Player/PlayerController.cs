using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Photon.MonoBehaviour
{
    [PunRPC]
    void SetParent(int[] id)
    {
        this.gameObject.name = "Player_" + id[1].ToString();
        this.transform.SetParent(GameObject.Find("Environment").transform.Find("Stage").Find("Players"));
        this.transform.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        this.transform.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
    }
}
