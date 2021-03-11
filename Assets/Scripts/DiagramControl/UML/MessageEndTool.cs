using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MessageEndTool : MessageCreateTool
{
    public bool LeftEnd = true;
    private bool StartEnd = false;
    private global::DG.UML.LifeLine originalLifelineStart = null;
    private global::DG.UML.LifeLine originalLifelineEnd = null;

    private GameObject OriginalMessage = null;


    public override void OnToolEnd(DiagramTool nextTool, DiagramInputHandler context)
    {
        base.OnToolEnd(nextTool, context);
        OriginalMessage.GetComponent<global::DG.UML.Message>().SelectionTool = OriginalMessage.GetComponent<MessageSelectionTool>();
    }

    public override void OnToolStart(DiagramTool previousTool, DiagramInputHandler context)
    {
        base.OnToolStart(previousTool, context);

        OriginalMessage = HierarchyHelper.GetParent(HierarchyHelper.GetParent(gameObject).gameObject).gameObject;
        global::DG.UML.Message message = OriginalMessage.GetComponent<global::DG.UML.Message>();

        // Work on my copy instead of new message
        Transform borders = message.GetSequenceDiagram().GetBorders();
        newMessage = Instantiate(message.gameObject, message.transform.position, message.transform.rotation);
        newMessage.transform.SetParent(borders);

        // Disable raycast on the copy
        if (newMessage.GetComponent<GraphicRaycaster>() == null)
        {
            newMessage.AddComponent<GraphicRaycaster>();
        }
        foreach (GraphicRaycaster raycaster in newMessage.GetComponentsInChildren<GraphicRaycaster>())
        {
            raycaster.enabled = false;
        }

        // Fill other parent variables
        RectTransform startTransform = OriginalMessage.GetComponent<global::DG.UML.Message>().FromLifeLine;
        RectTransform finalTransform = OriginalMessage.GetComponent<global::DG.UML.Message>().ToLifeLine;
        startMessageLifeLine = (startTransform?.gameObject);
        finalMessageLifeLine = (finalTransform?.gameObject);

        // Discover which message end should be moved
        bool reversed = newMessage.GetComponent<global::DG.UML.Message>().Reversed;
        StartEnd = (newMessage.GetComponent<global::DG.UML.Message>().Reversed) ^ LeftEnd;
    }
    
    public override void OnMouseButtonContinue(DiagramInputHandler context)
    {
        global::DG.UML.Message message = newMessage.GetComponent<global::DG.UML.Message>();
        if (context.MouseDelta == new Vector3(0, 0, 0))
        {
            // Do nothing
        }
        else
        {
            // Move element when user is manually controlling position

            GameObject hitLifeLine = null;
            global::DG.UML.LifeLine lifeline = null;

            // Detect hits

            foreach (RaycastResult result in context.GuiRaycast)
            {
                GameObject targetObj = result.gameObject;
                if (targetObj.tag == "LifeLine")
                {
                    hitLifeLine = targetObj;
                    lifeline = hitLifeLine.GetComponent<global::DG.UML.LifeLine>();
                    break;
                }
            }

            if (StartEnd)
            {
                AnimateMessageStart(lifeline);
            }
            else
            {
                AnimateMessageEnd(lifeline);
            }
        }
    }
    
    public override void OnMouseButtonEnd(DiagramInputHandler context)
    {
        global::DG.UML.Message message = OriginalMessage.GetComponent<global::DG.UML.Message>();

        if (finalMessageLifeLine != null && startMessageLifeLine != null && newMessage != null)
        {
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
                MessageSelectionTool.MoveMessageInSameLayer(message, vl.GetComponent<VerticalLayoutGroup>());
                message.FromLifeLine = newMessage.GetComponent<global::DG.UML.Message>().FromLifeLine;
                message.ToLifeLine = newMessage.GetComponent<global::DG.UML.Message>().ToLifeLine;
            }
        }

        // End tool immediately as button is released
        OnToolEnd(null, context);
    }

    public override void OnEscapeKey(DiagramInputHandler context)
    {
        // Return message to original state
        OnToolEnd(null, context);
    }
}