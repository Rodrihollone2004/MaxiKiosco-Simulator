using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        //Usamos Path.Combine para no tener problemas con diferentes sistemas operativos que utilizan distintos signos de separación como /
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                //Cargar la data serializada (JSON) del archivo
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //Deserializar la data del JSON devuelta en un C# script
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error al intentar cargar la data de: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        //Usamos Path.Combine para no tener problemas con diferentes sistemas operativos que utilizan distintos signos de separación como /
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            //crear el archivo que va a ser subscripto si es que no existe 
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //Serializar la data C# de Game Data dentro de un JSON
            string dataToStore = JsonUtility.ToJson(data, true); //crear un JSON en base a poner true como segundo parametro

            //Escribir la data serializada dentro del archivo que va a estar en la pc
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error al intentar guardar la data en: " + fullPath + "\n" + e);
        }
    }
}
