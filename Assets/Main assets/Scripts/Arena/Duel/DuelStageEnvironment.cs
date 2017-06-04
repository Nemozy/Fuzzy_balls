using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelStageEnvironment : StageEnvironment
{
    protected override void Init()
    {
        //FIXME:Поправить ссылки на айтемы.
        UIBlockScreen = UI.Find("BlockScreen");
        UIMessage = UI.Find("Message");
    }

    protected override void FixedUpdte()
    {

    }

    protected override void StartRound()
    {
        GameObject inst_ball = PhotonNetwork.Instantiate("Models/Balls/Simple_ball/Simple_ball", Vector3.zero, Quaternion.identity, 0);
        Transform tmp_tr = GameObject.Find("Environment").transform.Find("Stage").Find("Other");
        inst_ball.transform.SetParent(tmp_tr);

        Transform tr_default = null;
        if (RoundNumber % 4 < 2)
        {
            tr_default = tmp_tr.Find("SpawnPoints").Find("Ball").Find("Right");
        }
        else
        {
            tr_default = tmp_tr.Find("SpawnPoints").Find("Ball").Find("Left");
        }
        if(tr_default)
        {
            inst_ball.transform.GetComponent<RectTransform>().anchoredPosition = tr_default.GetComponent<RectTransform>().anchoredPosition;
            inst_ball.transform.GetComponent<RectTransform>().anchorMax = tr_default.GetComponent<RectTransform>().anchorMax;
            inst_ball.transform.GetComponent<RectTransform>().anchorMin = tr_default.GetComponent<RectTransform>().anchorMin;
            inst_ball.transform.GetComponent<RectTransform>().localPosition = tr_default.GetComponent<RectTransform>().localPosition;
            inst_ball.transform.GetComponent<RectTransform>().localScale = tr_default.GetComponent<RectTransform>().localScale;
        }
    }
}
