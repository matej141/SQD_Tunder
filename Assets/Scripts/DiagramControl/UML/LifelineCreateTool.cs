using DG;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LifelineCreateTool : DiagramTool
{
    public GameObject PrefabLifeline = null;

    public override void OnEscapeKey(DiagramInputHandler context)
    {
        DiagramInputHandler.ChooseTool(null);
    }
    
    public override void OnMouseButtonStart(DiagramInputHandler context)
    {
        foreach (RaycastResult result in context.GuiRaycast)
        {
            GameObject targetObj = result.gameObject;
            if (targetObj.tag == "Layer")
            {
                // Create lifeline
                GameObject lifeline = CreateLifeLineToLayerHL(targetObj.GetComponent<DgElement>());
                //GameObject lifeline = createLifeLineToLayer(targetObj.GetComponent<DgElement>());

                // Select lifeline object name
                DiagramInputHandler.ChooseTool(null);
                DiagramSelection.Select(lifeline.GetComponent<DiagramSelection>());
                GameObject input;
                input = HierarchyHelper.GetChildrenWithName(lifeline, "Header")[0].gameObject;
                input = HierarchyHelper.GetChildrenWithName(input, "ObjectName")[0].gameObject;
                input = HierarchyHelper.GetChildrenWithName(input, "InputField")[0].gameObject;
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(input);
                break;
            }
        }
    }

    public GameObject CreateLifeLineToLayer(DgElement layer)
    {
        GameObject borders = HierarchyHelper.GetChildrenWithName(layer.gameObject, "Borders")[0].gameObject;
        Vector3 worldPosition = RaycastHelper.RaycastSpecificWorldUI(borders);
        //Debug.Log("Vytvaram LifeLine priamo pod Layer");
        GameObject lifeLine = GameObject.Instantiate(PrefabLifeline, worldPosition, Quaternion.identity, borders.transform);
        RectTransform rectTransform = lifeLine.GetComponent<RectTransform>();
        // HACK!!! Try to find some solution with layouts instead. Like using grid layout with 1 cell or i dont know what else.
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, 0f);
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, 0f);
        // TODO Force update layouts
        lifeLine.GetComponent<global::DG.UML.LifeLine>().LastExplicitPosition = lifeLine.transform.position;
        return lifeLine;
    }

    public GameObject CreateLifeLineToLayerHL(DgElement layer)
    {
        GameObject lifeLine = GameObject.Instantiate(PrefabLifeline);
        GameObject borders = HierarchyHelper.GetChildrenWithName(layer.gameObject, "Borders")[0].gameObject;
        Vector3 worldPosition = RaycastHelper.RaycastSpecificWorldUI(borders);
        GameObject hl = HierarchyHelper.GetChildrenWithName(borders, "HL")[0].gameObject;
        //Debug.Log("Vytvaram LifeLine pod Horizontal Layout v Layer");
        LayoutHelper.MoveObjectToHorizontalLayout(hl.GetComponent<HorizontalLayoutGroup>(), lifeLine.GetComponent<global::DG.UML.LifeLine>(), worldPosition);
        return lifeLine;
    }
}
