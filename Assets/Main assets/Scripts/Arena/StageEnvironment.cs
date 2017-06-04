using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Добавить сюда всю механику подключения игроков и прогрузку. А также создание новых игровых объектов.
/// </summary>
public class StageEnvironment : Photon.MonoBehaviour
{
    public Transform UI;

    private bool GamePvPMode = true;
    private bool GameStart = false;
    private bool RoundStart = false;
    private double TimeMessageDisable = 0;
    private double TimeGameStart = 0;
    private bool InitPlayers = false;

    protected int RoundNumber = 0;
    protected Transform UIBlockScreen;
    protected Transform UIMessage;
    protected bool GameOver = false;

    protected int CountPlayers = 0;
    #region default methods

    protected void Awake()
    {
        Application.targetFrameRate = 60;
        if (!ConnectionManager.Connected())
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
            //TODO: Потом в меню выводить кнопку что-то типа "вернуться в бой", которая будет тебя возвращать в тот "бой", из которого выкинуло.
            return;
        }
    }

    void Start ()
    {
        Init();
    }
	
	void FixedUpdate()
    {
        if(UIMessage && UIMessage.gameObject.activeSelf && GetTime() > TimeMessageDisable)
        {
            HideMessage();
        }

        if (!GameStart && GamePvPMode)
        {
            if (ConnectionManager.InRoom() && ConnectionManager.CountPlayersInRoom() < 2)
            {
                BlockScreen(true);
                ShowMessage("Waiting all players..." + TimeFormatString(GetTime(), 0));
                //30 сек на подключение игроков. Ожидание всех игроков.
                if (GetTime() < 30)
                    return;
            }
            if (TimeGameStart == 0 && ConnectionManager.InRoom() && ConnectionManager.CountPlayersInRoom() >= 2)
            {
                //Для прогрузки сервера
                if (GetTime() < 2)
                {
                    return;
                }

                if (!InitPlayers)
                {
                    InitPlayers = ConnectionPlayers();
                    return;
                }
                if (InitPlayers)
                {
                    if (InstantiatePlayersObjects())
                    {
                        TimeGameStart = GetTime() + 1;
                    }
                    else
                        return;
                }
            }
            if (GetTime() - TimeGameStart > 4)
            {
                HideMessage();
                GameStart = true;
                BlockScreen(false);
            }
            else
            {
                BlockScreen(true);
                ShowMessage(TimeFormatString(3, TimeGameStart));
            }
        }
        
        if(GameStart)
        {
            if(!RoundStart)
            {
                RoundStart = true;
                RoundNumber += 1;
                StartRound();
                UnblockAllHeroes();
            }
            if(RoundStart)
            {
                CheckLose();
            }
        }
        FixedUpdte();
    }

    protected virtual void Init()
    {

    }
    protected virtual void FixedUpdte()
    {

    }
    protected virtual void StartRound()
    {

    }
    protected virtual void CheckLose()
    {

    }
    #endregion default methods

    #region usage methods
    public void UnblockAllHeroes()
    {
        for (int i = 0; i < ConnectionManager.CountPlayersInRoom(); i++)
        {
            Transform player = GameObject.Find("Environment").transform.Find("Stage").Find("Players").Find(string.Concat("Player_", ConnectionManager.GetPlayerIdInRoom(i)));

            if (player && player.childCount > 0)
            {
                player.GetChild(0).GetComponent<HeroController>().UnblockHero();
            }
        }
    }

    protected bool SetStartGame()
    {
        return GameStart = true;
    }

    public void BlockScreen(bool lockState)
    {
        if (UIBlockScreen)
        {
            UIBlockScreen.gameObject.SetActive(lockState);
        }
    }

    public double GetTime()
    {
        return ConnectionManager.GetServerTime();
    }

    #region Message
    protected void ShowMessage(string mess)
    {
        if (UIMessage)
        {
            UIMessage.GetComponent<UnityEngine.UI.Text>().text = mess;
            UIMessage.gameObject.SetActive(true);
        }
    }

    protected void ShowMessage(string mess, double duration)
    {
        if (UIMessage)
        {
            TimeMessageDisable = GetTime() + duration;
            UIMessage.GetComponent<UnityEngine.UI.Text>().text = mess;
            UIMessage.gameObject.SetActive(true);
        }
    }

    protected void HideMessage()
    {
        if (UIMessage)
        {
            UIMessage.gameObject.SetActive(false);
        }
    }
    #endregion Message

    private string TimeFormatString(double dur, double pointTime)
    {
        return "0:" + string.Format("{0:00}", Mathf.FloorToInt((float)(dur - (ConnectionManager.GetServerTime() - pointTime))));
    }

    private bool ConnectionPlayers()
    {
        Transform players = GameObject.Find("Environment").transform.Find("Stage").Find("Players");
        GameObject inst = PhotonNetwork.Instantiate("Player_", Vector3.zero, Quaternion.identity, 0);
        inst.name = "Player_" + PhotonNetwork.player.ID.ToString();
        inst.transform.SetParent(players);
        inst.transform.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        inst.transform.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        int[] parameters = { photonView.viewID, PhotonNetwork.player.ID };
        inst.GetPhotonView().RPC("SetParent", PhotonTargets.All, parameters);

        CountPlayers = ConnectionManager.CountPlayersInRoom();
        return true;
    }

    private bool InstantiatePlayersObjects()
    {
        if(ConnectionManager.CountPlayersInRoom() < 2)
        {
            return false;
        }
        for (int i = 0; i < ConnectionManager.CountPlayersInRoom(); i++)
        {
            Transform tr_tmp = GameObject.Find("Environment").transform.Find("Stage").Find("Players").Find(string.Concat("Player_",ConnectionManager.GetPlayerIdInRoom(i)));
            if (!tr_tmp)
            {
                return false;
            }
            Transform tr_default = GameObject.Find("Environment").transform.Find("Stage").Find("Players").Find("Player_Default");
            if (tr_default)
            {
                tr_tmp.transform.GetComponent<RectTransform>().anchoredPosition = tr_default.GetComponent<RectTransform>().anchoredPosition;
                tr_tmp.transform.GetComponent<RectTransform>().anchorMax = tr_default.GetComponent<RectTransform>().anchorMax;
                tr_tmp.transform.GetComponent<RectTransform>().anchorMin = tr_default.GetComponent<RectTransform>().anchorMin;
                tr_tmp.transform.GetComponent<RectTransform>().localPosition = tr_default.GetComponent<RectTransform>().localPosition;
                tr_tmp.transform.GetComponent<RectTransform>().localScale = tr_default.GetComponent<RectTransform>().localScale;
            }
        }

        Transform trsfrm_tmp = null;
        if(GameObject.Find("Environment").transform.Find("Stage").Find("Players").Find(string.Concat("Player_", PhotonNetwork.player.ID.ToString())) &&
             GameObject.Find("Environment").transform.Find("Stage").Find("Players").Find(string.Concat("Player_", PhotonNetwork.player.ID.ToString())).childCount > 0)
            trsfrm_tmp = GameObject.Find("Environment").transform.Find("Stage").Find("Players").Find(string.Concat("Player_", PhotonNetwork.player.ID.ToString())).GetChild(0);
        if (!trsfrm_tmp)
        {
            if (PhotonNetwork.player.ID == 1)
            {
                GameObject inst_hero = PhotonNetwork.Instantiate("Models/Heroes/Dumbass/Dumbass", Vector3.zero, Quaternion.identity, 0);
                inst_hero.name = "Hero_1";

                string[] parameters = { photonView.viewID.ToString(), inst_hero.name, PhotonNetwork.player.ID.ToString(), "Left", "Middle" };
                inst_hero.GetPhotonView().RPC("SetParent", PhotonTargets.All, parameters);
            }
            else if (PhotonNetwork.player.ID == 2)
            {
                GameObject inst_hero = PhotonNetwork.Instantiate("Models/Heroes/Angry/Angry", Vector3.zero, Quaternion.identity, 0);
                inst_hero.name = "Hero_2";

                string[] parameters = { photonView.viewID.ToString(), inst_hero.name, PhotonNetwork.player.ID.ToString(), "Right", "Middle" };
                inst_hero.GetPhotonView().RPC("SetParent", PhotonTargets.All, parameters);
            }

            return false;
        }
        //Удаление "шаблонов"
        //Destroy(GameObject.Find("Environment").transform.Find("Stage").Find("Players").Find("Player_Default").gameObject);

        //Выставляем команды игроков
        for (int i = 0; i < ConnectionManager.CountPlayersInRoom(); i++)
        {
            Transform player = GameObject.Find("Environment").transform.Find("Stage").Find("Players").Find(string.Concat("Player_", ConnectionManager.GetPlayerIdInRoom(i)));

            if (!player || player.childCount == 0)
                return false;
            if (player.GetChild(0).transform.position.x > 0)
                ConnectionManager.SetTeam(PunTeams.Team.blue, i);
            else
                ConnectionManager.SetTeam(PunTeams.Team.red, i);
        }
        
        return true;
    }
    #endregion usage methods
}
