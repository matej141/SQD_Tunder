using UnityEngine;

public class ExitPopUp : MonoBehaviour
{
    // 200x100 px window will apear in the center of the screen.
    private Rect windowRect = new Rect((Screen.width - 200) / 2, (Screen.height - 100) / 2, 200, 100);
    // Only show it if needed.
    private bool show = true;

    void OnGUI()
    {
        if (show)
            windowRect = GUI.Window(0, windowRect, DialogWindow, "Are you sure you want to exit?");
    }

    // This is the actual window.
    void DialogWindow(int windowID)
    {
        //GUI.Label(new Rect(5, 20, 30, 20), "Quit?");

        if (GUI.Button(new Rect(15, 60, 60, 20), "Yes"))
        {
            Application.Quit();
            //EditorApplication.Exit(0);
            show = false;
        }

        if (GUI.Button(new Rect(200 - 75, 60, 60, 20), "No"))
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
