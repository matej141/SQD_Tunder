using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiagramSelection : MonoBehaviour
{

    public static DiagramSelection Selected { get; private set; } = null;

    public DiagramTool SelectionTool = null;

    /// <summary>
    /// Used in selection dragging tools to detect
    /// whether actual position was generated automatically by Unity layouts
    /// or it was assigned by our dragging scripts.
    /// </summary>
    public Vector3 LastExplicitPosition = new Vector3(0f, 0f, 0f);
    /// <summary>
    /// Used in selection dragging tools
    /// to move scene when element was moved by automatic Unity layouts
    /// instead of being moved by our dragging tools.
    /// </summary>
    public static readonly bool SceneMoveEnabled = false;

    public virtual void Start()
    {
        if (SelectionTool == null) SelectionTool = GameObject.Find("DefaultSelectionTool").GetComponent<DiagramTool>();
    }

    /// <summary>
    /// GameObject you wish to select must have 'Canvas' and 'Outline' components.
    /// Use 'null' object to simply deselect current object without selecting new one.
    /// </summary>
    /// <param name="newSelection"></param>
    /// <returns>Newly selected object.</returns>
    public static DiagramSelection Select(DiagramSelection newSelection)
    {
        DiagramSelection oldSelection = Selected;
        if (oldSelection == newSelection) return oldSelection;
        Selected = newSelection;

        if (oldSelection != null)
        {
            oldSelection.OnUnselected(newSelection);

            Outline outline = oldSelection.GetComponent<Outline>();
            if (outline != null) outline.enabled = false;
        }
        if (newSelection != null)
        {
            Outline outline = newSelection.GetComponent<Outline>();
            if (outline != null) outline.enabled = true;

            newSelection.OnSelected(oldSelection);
        }
        return newSelection;
    }

    public static DiagramSelection Select(RaycastHit[] physicRaycast, List<RaycastResult> guiRaycast)
    {
        DiagramSelection newSelection = RaycastSelectable(physicRaycast, guiRaycast);
        return Select(newSelection);
    }

    public static DiagramSelection RaycastSelectable(RaycastHit[] physicRaycast, List<RaycastResult> guiRaycast)
    {
        // TODO FIX raycast order.
        // Elements inside layer are raycasted in correct order.
        // But layers themselves are raycasted in wrong order.
        // Layer in the background has higher priority than layer in the front.
        foreach (RaycastResult result in guiRaycast)
            if (result.gameObject.GetComponent<DiagramSelection>() != null)
                return result.gameObject.GetComponent<DiagramSelection>();
        foreach (RaycastHit result in physicRaycast)
            if (result.transform.GetComponent<DiagramSelection>() != null)
                return result.transform.GetComponent<DiagramSelection>();
        return null;
    }

    /// <summary>
    /// Moves scene vertically to make sure, that mouse always points to element, when element is relocated by automatic layouts.
    /// Otherwise element keeps flashing as it constatntly teleports between other parent elements.
    /// </summary>
    /// <returns>`true` if scene was moved, `false` otherwise</returns>
    public bool ShiftSceneWhenNeeded()
    {
        if (!DiagramSelection.SceneMoveEnabled || LastExplicitPosition == transform.position) return false;

        Transform root = GameObject.Find("ViewModel").transform;
        root.position = root.position + (LastExplicitPosition - transform.position);
        LastExplicitPosition = transform.position;

        return true;
    }

    // TODO Reduce protection level
    public virtual void OnSelected(DiagramSelection previouslySelected)
    {
        // Override in child
    }

    public virtual void OnUnselected(DiagramSelection newlySelected)
    {
        // Override in child
    }

}
