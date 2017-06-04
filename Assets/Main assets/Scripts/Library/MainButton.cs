using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainButton : Photon.MonoBehaviour
{
    private Transform objConntrolled;

    private void Start()
    {
        if (GameObject.Find("Environment").transform.Find("Stage").Find("Players").Find("Player_" + PhotonNetwork.player.ID.ToString()) &&
            GameObject.Find("Environment").transform.Find("Stage").Find("Players").Find("Player_" + PhotonNetwork.player.ID.ToString()).GetChild(0))
            objConntrolled = GameObject.Find("Environment").transform.Find("Stage").Find("Players").Find("Player_" + PhotonNetwork.player.ID.ToString()).GetChild(0).transform;
    }

    public void SendMessageInUnit(string message)
    {
        if (!objConntrolled && GameObject.Find("Environment").transform.Find("Stage").Find("Players").Find("Player_" + PhotonNetwork.player.ID.ToString()) &&
            GameObject.Find("Environment").transform.Find("Stage").Find("Players").Find("Player_" + PhotonNetwork.player.ID.ToString()).GetChild(0))
            objConntrolled = GameObject.Find("Environment").transform.Find("Stage").Find("Players").Find("Player_" + PhotonNetwork.player.ID.ToString()).GetChild(0).transform;
        if (objConntrolled)
        {
            string[] arg = message.Split('|');
            if (arg.Length > 2)
            {
                object[] tempStorage = new object[arg.Length - 1];
                for (int i = 0; i < arg.Length - 1; i++)
                {
                    tempStorage[i] = arg[i + 1];
                }

                objConntrolled.GetComponent<UnitController>().SendMessage(arg[0], tempStorage);
            }
            else if (arg.Length > 1)
                objConntrolled.GetComponent<UnitController>().SendMessage(arg[0], arg[1]);
            else if (arg.Length > 0)
                objConntrolled.GetComponent<UnitController>().SendMessage(message);
        }
    }
}
