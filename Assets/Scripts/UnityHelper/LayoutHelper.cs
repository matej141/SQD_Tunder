using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutHelper : MonoBehaviour
{
    /// <summary>
    /// WARNING:
    /// This check behaves as frequency cap for LayoutHelper.
    /// It is used to make sure, that LayoutHelper will not be used very often.
    /// If you want to use LayoutHelper in script or in some loop, than you should set this time to zero.
    /// </summary>
    private static readonly TimeSpan MinTimeDelta = TimeSpan.FromSeconds(0.05f);

    // This substraction is here to make sure, that first created element in the scene will not be blocked by frequency cap
    private static DateTime PreviousMoveTick = DateTime.Now - MinTimeDelta - MinTimeDelta;

    private static bool ValidateTick()
    {
        DateTime CurrentTick;
        TimeSpan TimeDelta;

        CurrentTick = DateTime.Now;
        TimeDelta = CurrentTick - PreviousMoveTick;
        if (TimeDelta < MinTimeDelta) return false;

        PreviousMoveTick = CurrentTick;
        return true;
    }

    public static void SetColor(GameObject target, Color color)
    {
        target = HierarchyHelper.GetParent(target);

        MeshRenderer renderer = target.GetComponent<MeshRenderer>();
        Material[] materials = renderer.materials;
        materials[0].color = color;

        List<Transform> children = HierarchyHelper.GetChildrenTransform(target);

        int childrenCount = children.Count;
        for (int i = 0; i < childrenCount; i++)
        {
            renderer = children[i].GetComponent<MeshRenderer>();
            materials = renderer.materials;
            materials[0].color = color;
        }
    }

    static public void MoveObjectToHorizontalLayout(HorizontalLayoutGroup hl, DiagramSelection newChild, Vector3 worldPosition)
    {
        if (!ValidateTick()) return;

        RectTransform parent = hl.GetComponent<RectTransform>();
        Vector2 localHitPosition2d = PositionHelper.TransformWorldToLocalBottomLeftPosition(parent, worldPosition);
        newChild.transform.SetParent(null);

        List<Transform> children = HierarchyHelper.GetChildrenTransform(parent.gameObject);
        int newIndex;
        for (newIndex = 0; newIndex < children.Count; newIndex++)
        {
            GameObject child = children[newIndex].gameObject;
            Vector2 localChildPosition = PositionHelper.TransformWorldToLocalBottomLeftPosition(parent, child.transform.position);
            // Debug.Log("index: " + newIndex + "\thit: " + localHitPosition2d + "\tchild: " + localChildPosition);
            if (localHitPosition2d.x < localChildPosition.x) break;
        }

        newChild.transform.position = worldPosition;
        newChild.transform.SetParent(parent);
        newChild.transform.rotation = Quaternion.identity;
        newChild.transform.SetSiblingIndex(newIndex);
        //newChild.LastExplicitPosition = newChild.transform.position;
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
        newChild.LastExplicitPosition = newChild.transform.position;
    }

    static public void MoveObjectToVerticalLayout(VerticalLayoutGroup vl, DiagramSelection newChild, Vector3 worldPosition)
    {
        if (!ValidateTick()) return;

        RectTransform parent = vl.GetComponent<RectTransform>();
        Vector2 localHitPosition2d = PositionHelper.TransformWorldToLocalBottomLeftPosition(parent, worldPosition);
        newChild.transform.SetParent(null);

        List<Transform> children = HierarchyHelper.GetChildrenTransform(parent.gameObject);
        int newIndex;
        for (newIndex = 0; newIndex < children.Count; newIndex++)
        {
            GameObject child = children[newIndex].gameObject;
            Vector2 localChildPosition = PositionHelper.TransformWorldToLocalBottomLeftPosition(parent, child.transform.position);
            // Debug.Log("index: " + newIndex + "\thit: " + localHitPosition2d + "\tchild: " + localChildPosition);
            if (localHitPosition2d.y > localChildPosition.y) break;
        }

        newChild.transform.position = worldPosition;
        newChild.transform.SetParent(parent);
        newChild.transform.rotation = Quaternion.identity;
        newChild.transform.SetSiblingIndex(newIndex);
        //newChild.LastExplicitPosition = newChild.transform.position;
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
        newChild.LastExplicitPosition = newChild.transform.position;
    }
}

