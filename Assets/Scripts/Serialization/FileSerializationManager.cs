using UnityEngine;
using System.IO;
using System.Collections.Generic;
using MongoDB.Bson;
using Data.MOF;
using Data;

public class FileSerializationManager : MonoBehaviour
{
    public static void SaveElementsToFile(string path)
    {
        if (path == null || path.Length == 0) return;

        // 0) - 2)
        XmiCollection elements = SerializationManager.SaveElements();
        if (elements == null)
        {
            LogManager.AddGlobalLog("Model is empty. Nothing was saved to the file.");
            return;
        }
        
        // 3) Store MOF elements as JSON
        List<BsonDocument> collection = new List<BsonDocument>();
        foreach (KeyValuePair<string, MofElement> entry in elements.MofElements())
        {
            MofElement mof = entry.Value;
            collection.Add(BsonDocument.Parse(JsonConvertor.ToJSON(mof).ToString()));
        }
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        builder.Append("[\n");
        collection.ForEach(d => builder.Append(d.ToString() + ",\n"));
        builder.Append("]");
        File.WriteAllText(path, builder.ToString());

        LogManager.AddGlobalLog("Saved " + elements.MofElements().Count + " MOF elements into file \"" + path + "\".");
    }

    public static void LoadElementsFromFile(string path)
    {
        if (path == null || path.Length == 0) return;

        // 0) Delete old elements
        SerializationManager.RemoveElements();

        // 1) Load JSON from FILE
        string fileContent = File.ReadAllText(path);
        List<BsonDocument> bson = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<List<BsonDocument>>(fileContent);

        // 2) - 6)
        XmiCollection elements = SerializationManager.LoadElements(bson);
        LogManager.AddGlobalLog("Loaded " + elements.MofElements().Count + " MOF elements from file \"" + path + "\".");
    }
    
}
