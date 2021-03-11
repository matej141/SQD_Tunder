using MongoDB.Bson;
using MongoDB.Driver;
using UnityEngine;
using System.Collections.Generic;
using System;

public class MongoDriver : MonoBehaviour
{
    public static MongoDriver Singleton { get; private set; } = null;

    private IMongoDatabase Db = null;
    public MongoClient Client { get; private set; } = null; // Should be `null` if connection is not successfull
    private IMongoCollection<BsonDocument> MofElements = null;
    private IMongoCollection<BsonDocument> DiElements = null;

    protected void Start()
    {
        if (Singleton == null) Singleton = this;

        Connect(null, null, null, null, null);
        //Connect("127.0.0.1", "27017", "local", null, null);
        //Connect("0.0.0.0", "27017", "local", null, null);
        //Connect("127.0.0.1", "27018", "local", null, null);
        //Connect("127.0.0.1", "27017", "local", "test", "test");
    }

    public string BuildUrl(string ip, string port, string username, string password, bool hideCredentials)
    {
        string url;
        if (ip == "" || ip == null) ip = "127.0.0.1";
        if (port == "" || port == null) port = "27017";
        if (hideCredentials) password = "****";
        
        if (username == null || username == "")
            url = "mongodb://" + ip + ":" + port;
        else
            url = "mongodb://" + username + ":" + password + "@" + ip + ":" + port;

        return url;
    }


    public bool Connect(string ip, string port, string databaseName, string username, string password)
    {
        try
        {
            // TODO Surround with try catch
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(BuildUrl(ip, port, username, password, false)));
            
            Client = new MongoClient(settings);
            
            if (databaseName == "" || databaseName == null) databaseName = "local";
            Db = Client.GetDatabase(databaseName);
            
            MofElements = Db.GetCollection<BsonDocument>("mof");
            DiElements = Db.GetCollection<BsonDocument>("di");
            
            return TestConnection();
        }
        catch (Exception)
        {
            ClearConnection();
            return false;
        }
    }

    public bool TestConnection()
    {
        if (Client == null) goto fail;
        if (Db == null) goto fail;
        if (MofElements == null) goto fail;
        if (DiElements == null) goto fail;
        try
        {
            // https://stackoverflow.com/a/44757956
            if (!Db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(4000)) goto fail;

            // https://stackoverflow.com/a/41350598
            // This does not working in initial connect
            //Client.GetServer().Ping();

        }
        catch (Exception)
        {
            goto fail;
        }
        return true;

    fail:
        ClearConnection();
        return false;
    }

    public void ClearConnection()
    {
        // Closing existing connections is not needed because C# MongoClient is super good.
        // https://stackoverflow.com/questions/32703051/properly-shutting-down-mongodb-database-connection-from-c-sharp-2-1-driver

        Client = null;
        Db = null;
        MofElements = null;
        DiElements = null;
    }

    //  private void InsertRandomDocument()
    //  {
    //      UnityThreadHelper.TaskDistributor.Dispatch(() =>
    //      {
    //          orders.InsertOne(order);
    //          System.Threading.Thread.Sleep(10);
    //          if (System.Threading.Interlocked.Increment(ref counter) < 100000 && work)
    //              UnityThreadHelper.Dispatcher.Dispatch(InsertRandomDocument);
    //      });
    //  }

    public void DropDB()
    {
        Db.DropCollection("mof");
        Db.DropCollection("di");
    }

    public bool InsertMofElement(BsonDocument document)
    {

        UnityThreadHelper.TaskDistributor.Dispatch(() =>
        {
            //System.Threading.Thread.Sleep(1000);

            MofElements.InsertOne(document);
        });
        return true;
    }

    public bool InsertDiElement(BsonDocument document)
    {
        UnityThreadHelper.TaskDistributor.Dispatch(() =>
        {
                //System.Threading.Thread.Sleep(1000);

                DiElements.InsertOne(document);
        });
        return true;
    }

    public List<BsonDocument> GetMofElements()
    {
        List<BsonDocument> list = new List<BsonDocument>();
        var filter = Builders<BsonDocument>.Filter.Empty;
        foreach (BsonDocument item in MofElements.Find(filter).ToListAsync().Result)
        {
            list.Add(item);
        }
        return list;
    }

    public List<BsonDocument> GetDiElements()
    {
        List<BsonDocument> list = new List<BsonDocument>();
        var filter = Builders<BsonDocument>.Filter.Empty;

        foreach (BsonDocument item in DiElements.Find(filter).ToListAsync().Result)
        {
            list.Add(item);
        }
        return list;
    }
}