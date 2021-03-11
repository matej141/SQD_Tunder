using UnityEngine;

public class DiagramTool : MonoBehaviour
{
    public virtual void OnToolStart(DiagramTool previousTool, DiagramInputHandler context)
    {
        // Override in child
    }

    public virtual void OnToolEnd(DiagramTool newTool, DiagramInputHandler context)
    {
        // Override in child
    }

    public virtual void OnLoop(DiagramInputHandler context)
    {
        // Override in child
    }

    public virtual void OnMouseButtonStart(DiagramInputHandler context)
    {
        // Override in child
    }

    public virtual void OnMouseButtonContinue(DiagramInputHandler context)
    {
        // Override in child
    }

    public virtual void OnMouseButtonEnd(DiagramInputHandler context)
    {
        // Override in child
    }

    public virtual void OnMouseLook(DiagramInputHandler context)
    {
        // Override in child
    }

    public virtual void OnEscapeKey(DiagramInputHandler context)
    {
        // Override in child
    }

    public virtual void OnDeleteKey(DiagramInputHandler context)
    {
        // Override in child
    }

}
