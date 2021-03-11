using DG;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LifelineSelectionTool : DefaultSelectionTool
{
    public override void OnMouseButtonContinue(DiagramInputHandler context)
    {
        global::DG.UML.LifeLine lifeline = GetComponent<global::DG.UML.LifeLine>();
        foreach (RaycastResult result in context.GuiRaycast)
        {
            GameObject targetObj = result.gameObject;
            if (targetObj.tag == "Layer")
            {
                MoveLifeLineToLayerHL(lifeline, targetObj.GetComponent<DgElement>());
                //MoveLifeLineToLayer(lifeline, targetObj.GetComponent<DgElement>());
                break;
            }
        }
    }

    public override void OnDeleteKey(DiagramInputHandler context)
    {
        GameObject removed = DiagramSelection.Selected.gameObject;
        DG.UML.LifeLine lifeline = removed.GetComponent<DG.UML.LifeLine>();
        DG.UML.SequenceDiagram sequenceDiagram = lifeline.GetSequenceDiagram();

        List<DG.UML.Message> messages = sequenceDiagram.GetMessagesRec();
        foreach (DG.UML.Message message in messages)
        {

            DG.UML.LifeLine startDgLifeline = message.FromLifeLine.GetComponent<DG.UML.LifeLine>();
            DG.UML.LifeLine endDgLifeline = message.ToLifeLine.GetComponent<DG.UML.LifeLine>();

            if (removed == startDgLifeline.gameObject || removed == endDgLifeline.gameObject)
            {
                Destroy(message.gameObject, 0);
            }
        }

        Destroy(removed);
    }

    public GameObject MoveLifeLineToLayer(global::DG.UML.LifeLine lifeLine, DgElement layer)
    {
        GameObject borders = HierarchyHelper.GetChildrenWithName(layer.gameObject, "Borders")[0].gameObject;
        Vector3 worldPosition = RaycastHelper.RaycastSpecificWorldUI(borders);
        //Debug.Log("Umiestnujem LifeLine priamo pod Layer");
        lifeLine.transform.SetParent(borders.transform);
        lifeLine.transform.position = worldPosition;
        RectTransform rectTransform = lifeLine.GetComponent<RectTransform>();   // HACK!!! Try to find some solution with layouts instead. Like using grid layout with 1 cell or i dont know what else.
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, 0f);
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, 0f);
        // TODO Force update layouts
        lifeLine.LastExplicitPosition = lifeLine.transform.position;
        return lifeLine.gameObject;
    }

    public GameObject MoveLifeLineToLayerHL(global::DG.UML.LifeLine lifeLine, DgElement layer)
    {
        GameObject borders = HierarchyHelper.GetChildrenWithName(layer.gameObject, "Borders")[0].gameObject;
        Vector3 worldPosition = RaycastHelper.RaycastSpecificWorldUI(borders);
        GameObject hl = HierarchyHelper.GetChildrenWithName(borders, "HL")[0].gameObject;
        //Debug.Log("Umiestnujem LifeLine pod Horizontal Layout v Layer");
        LayoutHelper.MoveObjectToHorizontalLayout(hl.GetComponent<HorizontalLayoutGroup>(), lifeLine, worldPosition);
        return lifeLine.gameObject;
    }
}
