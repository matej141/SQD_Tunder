using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MessageSelectionTool : DefaultSelectionTool
{
    private GameObject MessageCopy = null;

    public override void OnMouseButtonStart(DiagramInputHandler context)
    {
        global::DG.UML.Message message = GetComponent<global::DG.UML.Message>();
        GameObject line = HierarchyHelper.GetChildrenWithName(message.gameObject, "Line")[0].gameObject;
        GameObject dotLeft = HierarchyHelper.GetChildrenWithName(line, "DotLeft")[0].gameObject;
        GameObject dotRight = HierarchyHelper.GetChildrenWithName(line, "DotRight")[0].gameObject;
        message.LastExplicitPosition = message.transform.position;

        // Change selection tool of message, when message end is hit
        foreach (RaycastResult result in context.GuiRaycast)
        {
            GameObject targetObj = result.gameObject;
            if (targetObj == dotLeft || targetObj == dotRight)
            {
                DiagramTool oldSelectionTool = message.SelectionTool;
                message.SelectionTool = targetObj.GetComponent<MessageEndTool>();
                message.SelectionTool.OnToolStart(oldSelectionTool, context);
                return;
            }
        }

        // Use default select otherwise
        base.OnMouseButtonStart(context);
        if (DiagramSelection.Selected != message)
        {
            // Stop myself if other than my element was selected
            DiagramTool newTool = (DiagramSelection.Selected?.SelectionTool);
            OnToolEnd(newTool, context);
            return;
        }

        // Continue working on my copy if no special case happened
        Transform borders = message.GetSequenceDiagram().GetBorders();
        MessageCopy = Instantiate(message.gameObject, message.transform.position, message.transform.rotation);
        MessageCopy.transform.SetParent(borders);

        // Make copy not raycastable
        if (MessageCopy.GetComponent<GraphicRaycaster>() == null)
        {
            MessageCopy.AddComponent<GraphicRaycaster>();
        }
        foreach (GraphicRaycaster raycaster in MessageCopy.GetComponentsInChildren<GraphicRaycaster>())
        {
            raycaster.enabled = false;
        }
    }

    public override void OnMouseButtonContinue(DiagramInputHandler context)
    {
        if (context.MouseDelta == new Vector3(0, 0, 0))
        {
            // Do nothing
        }
        else
        {
            // Move element when user is manually controlling position

            GameObject borders = null;
        
            foreach (RaycastResult result in context.GuiRaycast)
            {
                GameObject targetObj = result.gameObject;
                if (targetObj.tag == "Layer")
                {
                    borders = HierarchyHelper.GetChildrenWithName(targetObj, "Borders")[0].gameObject;
                    break;
                }
            }
            if (borders != null)
            {
                Vector3 newPosition = RaycastHelper.RaycastSpecificWorldUI(borders);
                Vector3 oldPosition = MessageCopy.transform.position;
                MessageCopy.transform.position = new Vector3(oldPosition.x, newPosition.y, oldPosition.z);
            }
        }
    }

    public override void OnMouseButtonEnd(DiagramInputHandler context)
    {
        global::DG.UML.Message message = GetComponent<global::DG.UML.Message>();

        GameObject vl = null;

        foreach (RaycastResult result in context.GuiRaycast)
        {
            GameObject targetObj = result.gameObject;
            if (targetObj.tag == "Layer")
            {
                vl = HierarchyHelper.GetChildrenWithName(targetObj, "Borders")[0].gameObject;
                vl = HierarchyHelper.GetChildrenWithName(vl, "VL")[0].gameObject;
                break;
            }
            if (targetObj.tag == "Operand")
            {
                vl = HierarchyHelper.GetChildrenWithName(targetObj, "VL")[0].gameObject;
                break;
            }
        }
        if (vl != null)
        {
            MoveMessageInSameLayer(message, vl.GetComponent<VerticalLayoutGroup>());
        }

        ClearCopy();
    }

    public override void OnToolEnd(DiagramTool newTool, DiagramInputHandler context)
    {
        ClearCopy();
    }

    private void ClearCopy()
    {
        if (MessageCopy == null) return;
        Destroy(MessageCopy);
        MessageCopy = null;
    }

    public static void MoveMessageInSameLayer(global::DG.UML.Message message, VerticalLayoutGroup vl)
    {
        if (vl == null)
        {
            // Special Hack when ypu click in empty space between two operands
            vl = message.transform.parent.GetComponent<VerticalLayoutGroup>();
        }
        Vector3 worldHitPosition3d = RaycastHelper.RaycastSpecificWorldUI(vl.gameObject);
        LayoutHelper.MoveObjectToVerticalLayout(vl, message, worldHitPosition3d);
    }
}
