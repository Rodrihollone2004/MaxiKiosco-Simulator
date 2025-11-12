using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    //cualquier variable a guardar
    [Header("PlayerMoney")]
    public int currentPlayerMoney;
    public Vector3 playerPosition;

    [Header("Days")]
    public int currentDay;

    [Header("Experience")]
    public int currentExperience;
    public int currentLevel;
    public int previousLevelsExperience;
    public int nextLevelsExperience;

    //el Construct que va a iniciar al empezar una nueva partida
    public GameData()
    {
        playerPosition = new Vector3(8, 1.166f, 0);
        
        currentPlayerMoney = 190000;
        currentDay = 1;
        
        currentLevel = 0;
        currentExperience = 0;
        previousLevelsExperience = 0;
        nextLevelsExperience = 86;
    }
}
