using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEnvironment : Photon.MonoBehaviour
{
    public Transform UI;

    private bool StartFight = false;
    private int NeedCountPlayers = 0;

    #region default methods
    void Awake()
    {
        Application.targetFrameRate = 60;
        ConnectionManager.SetSingletonSettings();
    }

    void Start ()
    {
		
	}
	
	void FixedUpdate()
    {
        if (LobbyReadyToStart())
        {
            UI.Find("BlockWaiting_StartDuel").Find("Message").gameObject.SetActive(false);
            StartFight = false;
            ConnectionManager.LoadLvL("Duel");
        }
        else if (StartFight)
        {
            StartFight = !ConnectionManager.ConnectInLobbyByRating_Duel();

            string text = "Всего онлайн: " + ConnectionManager.GetOnline().ToString() + "\n";
            text += "В комнате: " + ConnectionManager.GetCountPlayersInRoom().ToString() + "     " + "\n";
            text += "Ожидается: " + (NeedCountPlayers - ConnectionManager.GetCountPlayersInRoom()).ToString() + "   ";
            UI.Find("BlockWaiting_StartDuel").Find("Message").GetComponent<UnityEngine.UI.Text>().text = text;
        }
    }
    #endregion default methods

    //Проверка на готовность комнаты (полная комната и оба пользователя в комнате)
    private bool LobbyReadyToStart()
    {
        string text = "Всего онлайн: " + ConnectionManager.GetOnline().ToString() + "\n";
        text += "В комнате: " + ConnectionManager.GetCountPlayersInRoom().ToString() + "     " + "\n";
        text += "Ожидается: " + (NeedCountPlayers - ConnectionManager.GetCountPlayersInRoom()).ToString() + "   ";
        UI.Find("BlockWaiting_StartDuel").Find("Message").GetComponent<UnityEngine.UI.Text>().text = text;

        if (ConnectionManager.GetInRoomState() && ConnectionManager.GetCountPlayersInRoom() >= 2)
            return true;
        return false;
    }

    #region UI methods
    //Начать "дуэль". Создать/присоединиться к комнате : Отменить.
    public void StartDuel(bool start)
    {
        StartFight = start;
        if (StartFight)
        {
            NeedCountPlayers = 2;

            string text = "Всего онлайн: " + ConnectionManager.GetOnline().ToString() + "\n";
            text += "В комнате: " + ConnectionManager.GetCountPlayersInRoom().ToString() + "     " + "\n";
            text += "Ожидается: " + (NeedCountPlayers - ConnectionManager.GetCountPlayersInRoom()).ToString() + "   ";
            UI.Find("BlockWaiting_StartDuel").Find("Message").GetComponent<UnityEngine.UI.Text>().text = text;
            UI.Find("BlockWaiting_StartDuel").gameObject.SetActive(true);
        }
        else
        {
            ConnectionManager.LeaveRoom();
            UI.Find("BlockWaiting_StartDuel").gameObject.SetActive(false);
        }
    }

    public void ConsoleActivation()
    {
        if (GameObject.Find("Interface") && GameObject.Find("Interface").transform.Find("Console"))
        {
            GameObject obj = GameObject.Find("Interface").transform.Find("Console").gameObject;
            obj.SetActive(!obj.activeSelf);
        }
    }
    #endregion UI methods
}
