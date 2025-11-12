using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistance 
{
    //metodos a implementar en cada script que haya cosas a guardar y cargar 
    void LoadData(GameData data);
    void SaveData(ref GameData data);
}
