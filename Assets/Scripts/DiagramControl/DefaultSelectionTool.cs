using UnityEngine;

public class DefaultSelectionTool : DiagramTool
{
    public override void OnDeleteKey(DiagramInputHandler context)
    {
        GameObject.Destroy(DiagramSelection.Selected.gameObject);
    }

    public override void OnEscapeKey(DiagramInputHandler context)
    {
        DiagramSelection.Select(null);
    }

    public override void OnMouseButtonStart(DiagramInputHandler context)
    {
        DiagramSelection oldSelection = DiagramSelection.Selected;
        DiagramSelection newSelection = DiagramSelection.Select(context.PhysicRaycast, context.GuiRaycast);
        if (newSelection != null && oldSelection != newSelection)
        {
            // Explicitly run starting method of next tool when selection tool has changed
            newSelection.SelectionTool.OnMouseButtonStart(context);
        }
    }

    public override void OnLoop(DiagramInputHandler context)
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            // Unselect element by ENTER key
            DiagramSelection.Select(null);
        }
    }
}
