
public class HandTool : DiagramTool
{

    public static HandTool Singleton { get; private set; } = null;

    public virtual void Start()
    {
        if (Singleton == null) Singleton = this;
    }

    public override void OnDeleteKey(DiagramInputHandler context)
    {
        if (DiagramSelection.Selected == null)
        {
            // Do nothing
        }
        else
        {
            DiagramSelection.Selected.SelectionTool.OnDeleteKey(context);
        }
    }

    public override void OnEscapeKey(DiagramInputHandler context)
    {
        if (DiagramSelection.Selected == null)
        {
            // Do nothing
        }
        else
        {
            DiagramSelection.Selected.SelectionTool.OnEscapeKey(context);
        }
    }

    public override void OnLoop(DiagramInputHandler context)
    {
        if (DiagramSelection.Selected == null)
        {
            // Do nothing
        }
        else
        {
            DiagramSelection.Selected.SelectionTool.OnLoop(context);
        }
    }

    public override void OnMouseButtonContinue(DiagramInputHandler context)
    {
        if (DiagramSelection.Selected == null)
        {
            // Do nothing
        }
        else
        {
            DiagramSelection.Selected.SelectionTool.OnMouseButtonContinue(context);
        }
    }

    public override void OnMouseButtonEnd(DiagramInputHandler context)
    {
        if (DiagramSelection.Selected == null)
        {
            // Do nothing
        }
        else
        {
            DiagramSelection.Selected.SelectionTool.OnMouseButtonEnd(context);
        }
    }

    public override void OnMouseButtonStart(DiagramInputHandler context)
    {
        // Pick up new SelectableDiagramElement.Selection only when nothing is selected
        if (DiagramSelection.Selected == null)
        {
            DiagramSelection.Select(context.PhysicRaycast, context.GuiRaycast);
        }
        if (DiagramSelection.Selected == null)
        {
            // Do nothing
        }
        else
        {
            DiagramSelection.Selected.SelectionTool.OnMouseButtonStart(context);
        }
    }

    public override void OnMouseLook(DiagramInputHandler context)
    {
        if (DiagramSelection.Selected == null)
        {
            // TODO Highligt object
        }
        else
        {
            DiagramSelection.Selected.SelectionTool.OnMouseLook(context);
        }
    }

    public override void OnToolEnd(DiagramTool nextTool, DiagramInputHandler context)
    {
        if (DiagramSelection.Selected == null)
        {
            // TODO Do nothing
        }
        else
        {
            DiagramSelection.Selected.SelectionTool.OnToolEnd(nextTool, context);
        }
        DiagramSelection.Select(null);
    }

    public override void OnToolStart(DiagramTool previousTool, DiagramInputHandler context)
    {
        if (DiagramSelection.Selected == null)
        {
            // TODO Do nothing
        }
        else
        {
            DiagramSelection.Selected.SelectionTool.OnToolStart(previousTool, context);
        }
        DiagramSelection.Select(null);
    }
}
