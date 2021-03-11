using UnityEngine;
using UnityEngine.UI;

public class DatabaseMenu : Menu
{
    public Text OpenedFilePath = null;

    private GameObject DbPanel = null;

    private GameObject Mid = null;

    private InputField IPAddr = null;

    private InputField Port = null;

    private InputField DBname = null;

    private InputField Username = null;

    private InputField Password = null;

    private Text Log = null;

    public override void Reset()
    {
        base.Reset();
        Title = "Database";
        OpenedFilePath = GameObject.Find("OpenedFilePath").GetComponent<Text>();
    }

    public void Start()
    {
        Dropdown.onValueChanged.AddListener(OnDatabaseOptionSelected);
        DbPanel = GameObject.Find("DB Panel");
        DbPanel.SetActive(false);

        Mid = HierarchyHelper.GetChildrenWithName(DbPanel, "Middle panel")[0].gameObject;

        GameObject parent = null;

        parent = HierarchyHelper.GetChildrenWithName(Mid, "IPAddress")[0].gameObject;
        IPAddr = HierarchyHelper.GetChildrenWithName(parent, "InputField")[0].GetComponent<InputField>();

        parent = HierarchyHelper.GetChildrenWithName(Mid, "Port")[0].gameObject;
        Port = HierarchyHelper.GetChildrenWithName(parent, "InputField")[0].GetComponent<InputField>();

        parent = HierarchyHelper.GetChildrenWithName(Mid, "DBName")[0].gameObject;
        DBname = HierarchyHelper.GetChildrenWithName(parent, "InputField")[0].GetComponent<InputField>();

        parent = HierarchyHelper.GetChildrenWithName(Mid, "Username")[0].gameObject;
        Username = HierarchyHelper.GetChildrenWithName(parent, "InputField")[0].GetComponent<InputField>();

        parent = HierarchyHelper.GetChildrenWithName(Mid, "Password")[0].gameObject;
        Password = HierarchyHelper.GetChildrenWithName(parent, "InputField")[0].GetComponent<InputField>();

        parent = HierarchyHelper.GetChildrenWithName(Mid, "Log")[0].gameObject;
        Log = HierarchyHelper.GetChildrenWithName(parent, "Text")[0].GetComponent<Text>();
    }

    public void OnDatabaseOptionSelected(int value)
    {
        switch (value)
        {
            case 0:
                ShowDialog();
                break;
            case 1:
                Import();
                break;
            case 2:
                Export();
                break;
            default:
                break;
        }
    }

    public void ShowDialog()
    {
        DbPanel.SetActive(true);
        Log.color = Color.black;
        if (MongoDriver.Singleton.TestConnection())
        {
            Log.text = "Successfully connected to database `"
                + (DBname.text == null || DBname.text == "" ? DBname.transform.Find("Placeholder").GetComponent<Text>().text : DBname.text)
                + "`\non the server:\n"
                + MongoDriver.Singleton.BuildUrl(IPAddr.text, Port.text, Username.text, Password.text, true);
        }
        else
        {
            Log.text = ("You are currently not connected to any database.");

        }
    }

    public void CloseDialogButton()
    {
        DbPanel.SetActive(false);
    }


    public void ConnectButton()
    {
        if (MongoDriver.Singleton.Connect(
            IPAddr.text, Port.text, DBname.text, Username.text, Password.text))
        {
            // Close window on success
            DbPanel.SetActive(false);
        }
        else
        {
            Log.color = Color.red;
            Log.text = "Unable to connect to database `"
                + (DBname.text == null || DBname.text == "" ? DBname.transform.Find("Placeholder").GetComponent<Text>().text : DBname.text)
                + "`\non the server:\n"
                + MongoDriver.Singleton.BuildUrl(IPAddr.text, Port.text, Username.text, Password.text, true)
                + "\nYou are currently not connected to any database.";
        }
    }

    public void Import()
    {
        // TODO Warning, that current model will be removed
        OpenedFilePath.text = null;
        // TODO Check if connection was estabilished before
        DatabaseSerializationManager.LoadElementsFromDB();
    }

    public void Export()
    {
        // TODO Check if connection was estabilished before
        DatabaseSerializationManager.SaveElementsToDB();
    }
}
