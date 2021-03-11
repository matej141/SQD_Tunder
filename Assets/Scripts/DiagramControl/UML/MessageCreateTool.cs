using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MessageCreateTool : DiagramTool
{
    public GameObject PrefabMessage = null;

    protected GameObject startMessageLifeLine = null;
    protected GameObject finalMessageLifeLine = null;
    protected GameObject newMessage = null;

    public override void OnToolStart(DiagramTool previousTool, DiagramInputHandler context)
    {
        ClearSuccessfullMessageTempVariables();
    }

    public override void OnToolEnd(DiagramTool newTool, DiagramInputHandler context)
    {
        DeleteUnfinishedMessage();
    }

    public override void OnMouseButtonContinue(DiagramInputHandler context)
    {
        global::DG.UML.Message message = (newMessage != null ? newMessage.GetComponent<global::DG.UML.Message>() : null);
        if (context.MouseDelta == new Vector3(0, 0, 0))
        {
            // Do nothing
        }
        else
        {
            // Move element when user is manually controlling position

            GameObject hitLifeLine = null;
            global::DG.UML.LifeLine lifeline = null;

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

            if (hitLifeLine == null)
            {
                // Animate message target point into space when mouse is dragging and starting point exists
                if (startMessageLifeLine != null && newMessage != null)
                {
                    AnimateMessageEnd(null);
                }
            }
            else
            {
                // Set starting lifeline for message while mouse dragging
                if (startMessageLifeLine == null || newMessage == null)
                {
                    StartMessage(hitLifeLine);
                    AnimateMessageStart(lifeline);
                }
                // Set final lifeline for message while mouse dragging
                else
                {
                    AnimateMessageEnd(lifeline);
                }
            }
        }
    }

    public override void OnMouseButtonEnd(DiagramInputHandler context)
    {
        // Move message to VL when mouse buton was released
        if (finalMessageLifeLine != null && startMessageLifeLine != null && newMessage != null)
        {
            GameObject hitOperand = null;
            foreach (RaycastResult result in context.GuiRaycast)
            {
                GameObject targetObj = result.gameObject;
                if (targetObj.tag == "Operand")
                {
                    hitOperand = targetObj;
                    break;
                }
            }

            FinishSuccessfulMessage(hitOperand);
            ClearSuccessfullMessageTempVariables();
        }
        // Reset message creation when it was not finished
        else
        {
            DeleteUnfinishedMessage();
        }
    }

    public override void OnEscapeKey(DiagramInputHandler context)
    {
        // Reset message creation by ESCAPE key when some message was being created
        if (DeleteUnfinishedMessage())
        {
            // Simply restart message create tool
        }
        // Unselect tool by ESCAPE key when no message was being created
        else
        {
            DiagramInputHandler.ChooseTool(null);
        }
    }


    /// <summary>
    /// Deletes temporrary message and clears temporary fields.
    /// Returns boolean value indicating whether this method affected something.
    /// </summary>
    /// <returns>true - if this method did something; false - if no change was made</returns>
    protected virtual bool DeleteUnfinishedMessage()
    {
        bool deleted = false;
        if (newMessage != null)
        {
            Destroy(newMessage, 0);
            newMessage = null;
            deleted = true;
        }
        if (startMessageLifeLine != null)
        {
            startMessageLifeLine = null;
            deleted = true;
        }
        if (finalMessageLifeLine != null)
        {
            finalMessageLifeLine = null;
            deleted = true;
        }
        return deleted;
    }

    protected void FinishSuccessfulMessage(GameObject hitOperand)
    {
        if (newMessage == null) return;
        var msgScript = newMessage.GetComponent<DG.UML.Message>();
        if (msgScript.FromLifeLine == null || msgScript.ToLifeLine == null) return;

        GameObject vl = null;
        if (hitOperand == null)
        {
            GameObject borders = startMessageLifeLine.transform.parent.parent.gameObject;
            vl = HierarchyHelper.GetChildrenWithName(borders, "VL")[0].gameObject;
        }
        else
        {
            vl = HierarchyHelper.GetChildrenWithName(hitOperand, "VL")[0].gameObject;
        }

        MessageSelectionTool.MoveMessageInSameLayer(newMessage.GetComponent<global::DG.UML.Message>(), vl.GetComponent<VerticalLayoutGroup>());
    }

    protected virtual void ClearSuccessfullMessageTempVariables()
    {
        startMessageLifeLine = null;
        finalMessageLifeLine = null;
        newMessage = null;
    }

    protected virtual void StartMessage(GameObject fromLifeLine)
    {
        GameObject borders;

        startMessageLifeLine = fromLifeLine;

        borders = startMessageLifeLine.transform.parent.parent.gameObject;

        newMessage = Instantiate(PrefabMessage, borders.transform);
    }

    protected virtual void AnimateMessageStart(global::DG.UML.LifeLine lifeline)
    {
        global::DG.UML.Message msgScript = newMessage.GetComponent<DG.UML.Message>();
        GameObject borders = newMessage.transform.parent.gameObject;    // Special case when Message is child of Borders

        if (lifeline == null)
        {
            startMessageLifeLine = null;
            msgScript.FromLifeLine = null;
        }
        else
        {
            startMessageLifeLine = lifeline.gameObject;
            msgScript.FromLifeLine = startMessageLifeLine.transform.GetComponent<RectTransform>();
        }

        Vector3 newPosition = RaycastHelper.RaycastSpecificWorldUI(borders);
        Vector3 oldPosition = newMessage.transform.position;
        newMessage.transform.position = new Vector3(oldPosition.x, newPosition.y, oldPosition.z);

        msgScript.FromWorldPosition = newPosition;
        msgScript.ToWorldPosition.y = newPosition.y;
    }

    protected virtual void AnimateMessageEnd(global::DG.UML.LifeLine lifeline)
    {
        global::DG.UML.Message msgScript = newMessage.GetComponent<DG.UML.Message>();
        GameObject borders = newMessage.transform.parent.gameObject;    // Special case when Message is child of Borders

        if (lifeline == null)
        {
            finalMessageLifeLine = null;
            msgScript.ToLifeLine = null;
        }
        else
        {
            finalMessageLifeLine = lifeline.gameObject;
            msgScript.ToLifeLine = finalMessageLifeLine.transform.GetComponent<RectTransform>();
        }

        Vector3 newPosition = RaycastHelper.RaycastSpecificWorldUI(borders);
        Vector3 oldPosition = newMessage.transform.position;
        newMessage.transform.position = new Vector3(oldPosition.x, newPosition.y, oldPosition.z);

        msgScript.ToWorldPosition = newPosition;
        msgScript.FromWorldPosition.y = newPosition.y;
    }
}
