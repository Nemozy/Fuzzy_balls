using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager
{
    private static string GameVersion = "ver 0.01";

    public static void SetSingletonSettings()
    {
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.autoJoinLobby = true;
        PhotonNetwork.ConnectUsingSettings(GameVersion);

        if (string.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            PhotonNetwork.playerName = "Player" + Random.Range(1, 9999);
        }
    }

    public static bool CreateRoom(RoomOptions settings)
    {
        return PhotonNetwork.CreateRoom("", settings, null);
    }

    public static bool ConnectInRandomLobby()
    {
        return PhotonNetwork.JoinRandomRoom();
    }

    public static bool InRoom()
    {
        return PhotonNetwork.inRoom;
    }

    public static int CountPlayersInRoom()
    {
        return PhotonNetwork.playerList.Length;
    }

    public static int GetPlayerIdInRoom(int id)
    {
        return PhotonNetwork.playerList[id].ID;
    }

    public static void SetTeam(PunTeams.Team team, int id)
    {
        PhotonNetwork.playerList[id].SetTeam(team);
    }

    public static int GetPing()
    {
        return PhotonNetwork.GetPing();
    }

    public static bool GetInRoomState()
    {
        return PhotonNetwork.inRoom;
    }

    public static bool LeaveRoom()
    {
        return PhotonNetwork.LeaveRoom();
    }

    public static int GetCountPlayersInRoom()
    {
        if (PhotonNetwork.inRoom)
            return PhotonNetwork.room.PlayerCount;
        else
            return 0;
    }

    //Подключение или создание комнаты для дуэли.
    public static bool ConnectInLobbyByRating_Duel()
    {
        if (!PhotonNetwork.insideLobby)
            return false;

        bool findRoom = false;
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].IsVisible && rooms[i].IsOpen && rooms[i].PlayerCount < 2)
            {
                //connect & play!
                if (PhotonNetwork.JoinRoom(rooms[i].Name))
                    findRoom = true;
            }
        }

        if (!findRoom)
        {
            RoomOptions newRoomOptions = new RoomOptions();
            newRoomOptions.IsOpen = true;
            newRoomOptions.IsVisible = true;
            newRoomOptions.MaxPlayers = 2;
            findRoom = PhotonNetwork.CreateRoom("", newRoomOptions, null);
        }
        return findRoom;
    }

    public static int GetOnline()
    {
        return PhotonNetwork.countOfPlayers;
    }

    public static int GetLobiesCount()
    {
        return PhotonNetwork.countOfRooms;
    }

    public static bool Connected()
    {
        return PhotonNetwork.connected;
    }

    public static void LoadLvL(string lvlName)
    {
        PhotonNetwork.LoadLevel(lvlName);
    }

    public static double GetServerTime()
    {
        return PhotonNetwork.time;
    }
}
