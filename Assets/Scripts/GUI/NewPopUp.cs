using UnityEngine;
using UnityEngine.UI;

public class NewPopUp : MonoBehaviour
{
    // 200x100 px window will apear in the center of the screen.
    private Rect windowRect = new Rect((Screen.width - 260) / 2, (Screen.height - 100) / 2, 260, 100);
    // Only show it if needed.
    private bool show = true;
    private Text FilePath = null;

    public void SetFilePath(Text text)
    {
        FilePath = text;
    }

    public Text GetFilePath()
    {
        return FilePath;
    }

    void OnGUI()
    {
        if (show)
            windowRect = GUI.Window(0, windowRect, DialogWindow, "Create new model?");
    }

    // This is the actual window.
    void DialogWindow(int windowID)
    {
        GUI.Label(new Rect(5, 20, 240, 50), "All unsaved data will be lost. Are you sure you want to create new model?");

        if (GUI.Button(new Rect(15, 70, 60, 20), "Yes"))
        {
            FilePath.text = "";
            SerializationManager.RemoveElements();
            FindObjectOfType<NewLayerTool>().CreateLayer();
            show = false;
        }

        if (GUI.Button(new Rect(260 - 75, 70, 60, 20), "No"))
        {
            show = false;
        }
    }

    // To open the dialogue from outside of the script.
    public void Open()
    {
        show = true;
    }
    public void Close()
    {
        show = false;
    }
}
