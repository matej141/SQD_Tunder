using DG.UML;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FragmentCreateTool : DiagramTool
{
    public GameObject PrefabFragment = null;

    public override void OnEscapeKey(DiagramInputHandler context)
    {
        DiagramInputHandler.ChooseTool(null);
    }

    public override void OnMouseButtonStart(DiagramInputHandler context)
    {
        foreach (RaycastResult result in context.GuiRaycast)
        {
            GameObject targetObj = result.gameObject;
            if (targetObj.tag == "Layer" || targetObj.tag == "Operand")
            {
                // Create fragment
                GameObject fragment = CreateFragmentToVL(targetObj);

                // Select fragment name
                DiagramInputHandler.ChooseTool(null);
                DiagramSelection.Select(fragment.GetComponent<DiagramSelection>());
                GameObject input;
                input = HierarchyHelper.GetChildrenWithName(fragment, "OperatorWrapper")[0].gameObject;
                input = HierarchyHelper.GetChildrenWithName(input, "FragmentName")[0].gameObject;
                input = HierarchyHelper.GetChildrenWithName(input, "InputField")[0].gameObject;
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(input);
                break;
            }
        }
    }

    public GameObject CreateFragmentToVL(GameObject dg)
    {
        Transform vl = null;
        switch (dg.tag)
        {
            case "Layer":
                vl = dg.GetComponent<SequenceDiagram>().GetVL();
                break;
            case "Operand":
                vl = dg.GetComponent<Operand>().GetVL();
                break;
            default:
                throw new Exception("Incorrect parent for Fragment!");
        }

        Vector3 worldPosition = RaycastHelper.RaycastSpecificWorldUI(dg);
        //Debug.Log("Vytvaram Fragment pod Vertical Layout v " + dg.tag);
        GameObject fragment = Instantiate(PrefabFragment);
        LayoutHelper.MoveObjectToVerticalLayout(vl.GetComponent<VerticalLayoutGroup>(), fragment.GetComponent<global::DG.UML.Fragment>(), worldPosition);
        fragment.GetComponent<global::DG.UML.Fragment>().AddNewOperand(false);
        // TODO Force update layouts
        fragment.GetComponent<global::DG.UML.Fragment>().LastExplicitPosition = fragment.transform.position;
        return fragment;
    }
}
