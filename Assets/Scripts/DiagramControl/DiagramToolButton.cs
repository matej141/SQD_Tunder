using UnityEngine;
using UnityEngine.UI;

public class DiagramToolButton : MonoBehaviour
{
    // Chooses tool on current game object by default
    public DiagramTool MyTool = null;
    
    public void Reset()
    {
        MyTool = GetComponent<DiagramTool>();
    }

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ChooseMyTool);
        // Choose tool on the same game object when no tool was specified
        if (MyTool == null) MyTool = GetComponent<DiagramTool>();
    }
    
    void Update()
    {
        Color c = new Color(0.702f, 0.702f, 0.702f);
        // Change color of button when my tool is selected
        DiagramTool currentTool = DiagramInputHandler.Singleton.CurrentTool;
        GetComponent<Image>().color = (currentTool == MyTool ? c : Color.white);
    }

    public void ChooseMyTool()
    {
        DiagramInputHandler.ChooseTool(MyTool);
    }
}
