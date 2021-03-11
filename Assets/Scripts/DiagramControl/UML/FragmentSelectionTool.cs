using DG.UML;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FragmentSelectionTool : DefaultSelectionTool
{
    private GameObject FragmentCopy = null;

    public override void OnMouseButtonStart(DiagramInputHandler context)
    {
        global::DG.UML.Fragment fragment = GetComponent<global::DG.UML.Fragment>();
        fragment.LastExplicitPosition = fragment.transform.position;

        // Change selection tool if other object is being selected
        base.OnMouseButtonStart(context);
        if (DiagramSelection.Selected != fragment)
        {
            // Stop myself if other than my element was selected
            DiagramTool newTool = (DiagramSelection.Selected?.SelectionTool);
            OnToolEnd(newTool, context);
            return;
        }

        // Continue working on my copy if no special case happened
        Transform borders = fragment.GetSequenceDiagram().GetBorders();
        FragmentCopy = Instantiate(fragment.gameObject, fragment.transform.position, fragment.transform.rotation);
        FragmentCopy.transform.SetParent(borders);
        
        // Make copy not raycastable
        if (FragmentCopy.GetComponent<GraphicRaycaster>() == null)
        {
            FragmentCopy.AddComponent<GraphicRaycaster>();
        }
        foreach (GraphicRaycaster raycaster in FragmentCopy.GetComponentsInChildren<GraphicRaycaster>())
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
                Vector3 oldPosition = FragmentCopy.transform.position;
                FragmentCopy.transform.position = new Vector3(oldPosition.x, newPosition.y, oldPosition.z);
            }
        }
    }

    public override void OnMouseButtonEnd(DiagramInputHandler context)
    {
        global::DG.UML.Fragment fragment = GetComponent<global::DG.UML.Fragment>();

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
                Operand targetOperand = targetObj.GetComponent<Operand>();
                Operand[] mySubOperands = fragment.GetComponentsInChildren<Operand>();
                foreach (Operand subOperand in mySubOperands)
                {
                    if (targetOperand == subOperand)
                    {
                        // Do not move fragment into one of its own operands
                        ClearCopy();
                        return;
                    }
                }
                vl = HierarchyHelper.GetChildrenWithName(targetObj, "VL")[0].gameObject;
                break;
            }
        }
        if (vl != null)
        {
            MoveFragmentInSameLayer(fragment, vl.GetComponent<VerticalLayoutGroup>());
        }

        ClearCopy();
    }

    public override void OnToolEnd(DiagramTool newTool, DiagramInputHandler context)
    {
        ClearCopy();
    }

    private void ClearCopy()
    {
        if (FragmentCopy == null) return;
        Destroy(FragmentCopy);
        FragmentCopy = null;
    }

    public static void MoveFragmentInSameLayer(global::DG.UML.Fragment fragment, VerticalLayoutGroup vl)
    {
        if (vl == null)
        {
            // Special Hack when you click in empty space between two operands
            vl = fragment.transform.parent.GetComponent<VerticalLayoutGroup>();
        }
        Vector3 worldHitPosition3d = RaycastHelper.RaycastSpecificWorldUI(vl.gameObject);
        LayoutHelper.MoveObjectToVerticalLayout(vl, fragment, worldHitPosition3d);
    }
}
