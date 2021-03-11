using SFB;
using UnityEngine;
using UnityEngine.UI;

public class FileMenu : Menu
{
    public Text OpenedFilePath = null;
    private NewPopUp NewMenu = null;
    private ExitPopUp ExitMenu = null;

    private static readonly ExtensionFilter[] ExtensionList = new ExtensionFilter[]
    {
        new ExtensionFilter("Json", "json"),
    };

    public override void Reset()
    {
        base.Reset();
        Title = "File";
        OpenedFilePath = GameObject.Find("OpenedFilePath").GetComponent<Text>();
    }

    public void Start()
    {
        gameObject.AddComponent<NewPopUp>();
        NewMenu = GetComponent<NewPopUp>();
        NewMenu.Close();
        gameObject.AddComponent<ExitPopUp>();
        ExitMenu = GetComponent<ExitPopUp>();
        ExitMenu.Close();
        Dropdown.onValueChanged.AddListener(OnFileOptionSelected);
        NewMenu.SetFilePath(OpenedFilePath);
    }

    public void OnFileOptionSelected(int value)
    {
        switch (value)
        {
            case 0:
                New();
                break;
            case 1:
                Open();
                break;
            case 2:
                Save();
                break;
            case 3:
                SaveAs();
                break;
            case 4:
                Exit();
                break;
            default:
                break;
        }
    }

    public void New()
    {
        NewMenu.Open();
    }

    public void Open()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "./Examples/", ExtensionList, false);

        if (paths.Length > 0)
        {
            OpenedFilePath.text = paths[0];
            FileSerializationManager.LoadElementsFromFile(paths[0]);
        }
    }

    public void Save()
    {
        if (OpenedFilePath.text == null || OpenedFilePath.text.Length == 0) SaveAs();
        FileSerializationManager.SaveElementsToFile(OpenedFilePath.text);
    }

    public void SaveAs()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Save File", "./Examples/", "New Model", ExtensionList);

        if (path != null)
        {
            FileSerializationManager.SaveElementsToFile(path);
            OpenedFilePath.text = path;
        }
    }

    public void Exit()
    {
        ExitMenu.Open();        // Ask before closing
    }

}
