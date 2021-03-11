using UnityEngine;
using System.Collections.Generic;
using MongoDB.Bson;
using Data.MOF;
using Data;

public class DatabaseSerializationManager : MonoBehaviour
{

    public static void SaveElementsToDB() 
    {
        if (MongoDriver.Singleton.TestConnection() == true)
        {
            // 0) - 2)
            XmiCollection elements = SerializationManager.SaveElements();
            if (elements == null)
            {
                LogManager.AddGlobalLog("Model is empty. Nothing was saved to the database.");
                return;
            }

            // 3) Store MOF elements as JSON
            MongoDriver.Singleton.DropDB();
            foreach (KeyValuePair<string, MofElement> entry in elements.MofElements())
            {
                MofElement mof = entry.Value;
                MongoDriver.Singleton.InsertMofElement(BsonDocument.Parse(JsonConvertor.ToJSON(mof).ToString()));
            }

            LogManager.AddGlobalLog("Saved " + elements.MofElements().Count + " MOF elements into the databse.");


        }
        else
        {
            LogManager.AddGlobalLog("Database was not found. Please start or connect to your DB server.");
        }
    }

    public static void LoadElementsFromDB()
    {
        
        if (MongoDriver.Singleton.TestConnection() == true)
        { 
            // 0) Delete old elements
            SerializationManager.RemoveElements();

            // 1) Load JSON from DB
            List<BsonDocument> jsonElements = MongoDriver.Singleton.GetMofElements();

            // 2) - 6)
            XmiCollection elements = SerializationManager.LoadElements(jsonElements);
            LogManager.AddGlobalLog("Loaded " + elements.MofElements().Count + " MOF elements from the database.");
        }
        else
        {
            LogManager.AddGlobalLog("Database was not found. Please start or connect to your DB server.");
        }
    }
}
