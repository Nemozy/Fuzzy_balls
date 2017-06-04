using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : Photon.MonoBehaviour
{
    public bool BlockedHero = true;
    private int deltaWeight = 700000;
    private Vector3 CorrectPlayerPos;
    private Quaternion CorrectPlayerRot;

    protected Structs.MovingControl MovingStates = new Structs.MovingControl(6, 3.35);
    protected Rigidbody2D ThisRigidbody;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        CorrectPlayerPos = transform.position;
        CorrectPlayerRot = transform.rotation;
        ThisRigidbody = this.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (MovingStates.FreezeMode || BlockedHero)
            return;

        if (MovingStates.JumpMode && MovingStates.Landing)
        {
            ThisRigidbody.AddForce(this.transform.up * deltaWeight, ForceMode2D.Force);
            MovingStates.JumpMode = false;
            MovingStates.Landing = false;
        }

        if (MovingStates.GetMoveToLeftState())
        {
            MovingStates.Speed -= Time.deltaTime * MovingStates.DeltaSpeed;

            if (MovingStates.Speed < -MovingStates.MaxSpeed)
                MovingStates.Speed = -MovingStates.MaxSpeed;
        }
        else if (MovingStates.GetMoveToRightState())
        {
            MovingStates.Speed += Time.deltaTime * MovingStates.DeltaSpeed;

            if (MovingStates.Speed > MovingStates.MaxSpeed)
                MovingStates.Speed = MovingStates.MaxSpeed;
        }
        else
        {
            if (MovingStates.Speed < -0.4)
                MovingStates.Speed += Time.deltaTime * MovingStates.DeltaSpeed;
            else if (MovingStates.Speed > 0.4)
                MovingStates.Speed -= Time.deltaTime * MovingStates.DeltaSpeed;
            else
                MovingStates.Speed = 0;
        }

        //if(this.gameObject.GetPhotonView())
        //    this.gameObject.GetPhotonView().RPC("SendSpeedToOther", PhotonTargets.Others, MovingStates.Speed);
        //Vector3 testVec = this.transform.localPosition + Vector3.right * (float)MovingStates.Speed;
        //if(testVec.x < -400 && testVec.x > -400)
            this.transform.position += Vector3.right * (float)MovingStates.Speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Ground"))
        {
            MovingStates.Landing = true;
        }
        if (collision.transform.tag.Equals("Net") || collision.transform.tag.Equals("Wall"))
        {
            MovingStates.Speed = 0;
            MovingStates.StopMoving();
            if (this.gameObject.GetPhotonView())
                this.gameObject.GetPhotonView().RPC("SendSpeedToOther", PhotonTargets.Others, MovingStates.Speed);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Network player, receive data
            CorrectPlayerPos = (Vector3)stream.ReceiveNext();
            CorrectPlayerRot = (Quaternion)stream.ReceiveNext();
        }
    }

    public void UnblockHero()
    {
        BlockedHero = false;
    }

    [PunRPC]
    protected void SetParent(string[] prms)
    {
        this.gameObject.name = prms[1];
        this.transform.SetParent(GameObject.Find("Environment").transform.Find("Stage").Find("Players").Find("Player_" + prms[2]));
        Transform hero_default = GameObject.Find("Environment").transform.Find("Stage").Find("Players").Find("Player_Default").Find("Default");
        this.transform.GetComponent<RectTransform>().anchoredPosition = hero_default.GetComponent<RectTransform>().anchoredPosition;
        this.transform.GetComponent<RectTransform>().anchorMax = hero_default.GetComponent<RectTransform>().anchorMax;
        this.transform.GetComponent<RectTransform>().anchorMin = hero_default.GetComponent<RectTransform>().anchorMin;
        this.transform.GetComponent<RectTransform>().localPosition = hero_default.GetComponent<RectTransform>().localPosition;
        this.transform.GetComponent<RectTransform>().localScale = hero_default.GetComponent<RectTransform>().localScale;
        this.transform.position = GameObject.Find("Environment").transform.Find("Stage").Find("Other").Find("SpawnPoints").Find("Heroes").Find(prms[3]).Find(prms[4]).transform.position;
    }

    /*[PunRPC]
    protected void SendSpeedToOther(double spd)
    {
        MovingStates.Speed = spd;
    }*/
}