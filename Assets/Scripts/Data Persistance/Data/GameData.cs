using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    //cualquier variable a guardar
    public int currentPlayerMoney;
    public Vector3 playerPosition;

    //el Construct que va a iniciar al empezar una nueva partida
    public GameData()
    {
        currentPlayerMoney = 190000;
        playerPosition = new Vector3(8, 1.166f, 0);
    }
}
