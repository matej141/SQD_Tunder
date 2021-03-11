using DG.UML;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OperandSelectionTool : DefaultSelectionTool
{
    private GameObject OperandCopy = null;

    public override void OnMouseButtonStart(DiagramInputHandler context)
    {
        global::DG.UML.Operand operand = GetComponent<global::DG.UML.Operand>();
        operand.LastExplicitPosition = operand.transform.position;

        // Change selection tool if other object is being selected
        base.OnMouseButtonStart(context);
        if (DiagramSelection.Selected != operand)
        {
            // Stop myself if other than my element was selected
            DiagramTool newTool = (DiagramSelection.Selected?.SelectionTool);
            OnToolEnd(newTool, context);
            return;
        }

        // Continue working on my copy if no special case happened
        Transform borders = operand.GetSequenceDiagram().GetBorders();
        OperandCopy = Instantiate(operand.gameObject, operand.transform.position, operand.transform.rotation);
        OperandCopy.transform.SetParent(borders);

        // Make copy not raycastable
        if (OperandCopy.GetComponent<GraphicRaycaster>() == null)
        {
            OperandCopy.AddComponent<GraphicRaycaster>();
        }
        foreach (GraphicRaycaster raycaster in OperandCopy.GetComponentsInChildren<GraphicRaycaster>())
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
                Vector3 oldPosition = OperandCopy.transform.position;
                OperandCopy.transform.position = new Vector3(oldPosition.x, newPosition.y, oldPosition.z);
            }
        }
    }

    public override void OnMouseButtonEnd(DiagramInputHandler context)
    {
        global::DG.UML.Operand operand = GetComponent<global::DG.UML.Operand>();

        GameObject vl = null;

        foreach (RaycastResult result in context.GuiRaycast)
        {
            GameObject targetObj = result.gameObject;
            if (targetObj.tag == "Fragment")
            {
                Fragment targetFragment = targetObj.GetComponent<Fragment>();
                Fragment[] mySubFragments = operand.GetComponentsInChildren<Fragment>();
                foreach (Fragment subFragment in mySubFragments)
                {
                    if (targetFragment == subFragment)
                    {
                        // Do not move operand into one of its own fragments
                        ClearCopy();
                        return;
                    }
                }
                vl = HierarchyHelper.GetChildrenWithName(targetObj, "OperandArea")[0].gameObject;
                break;
            }
        }
        if (vl != null)
        {
            MoveOperandInSameLayer(operand, vl.GetComponent<VerticalLayoutGroup>());
        }

        ClearCopy();
    }

    public override void OnToolEnd(DiagramTool newTool, DiagramInputHandler context)
    {
        ClearCopy();
    }

    private void ClearCopy()
    {
        if (OperandCopy == null) return;
        Destroy(OperandCopy);
        OperandCopy = null;
    }

    public static void MoveOperandInSameLayer(global::DG.UML.Operand operand, VerticalLayoutGroup vl)
    {
        if (vl == null)
        {
            // Special Hack when you click in empty space between two operands
            vl = operand.transform.parent.GetComponent<VerticalLayoutGroup>();
        }
        Vector3 worldHitPosition3d = RaycastHelper.RaycastSpecificWorldUI(vl.gameObject);
        LayoutHelper.MoveObjectToVerticalLayout(vl, operand, worldHitPosition3d);
    }
}
