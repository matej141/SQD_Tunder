using System.Collections.Generic;
using UnityEngine;

public class HierarchyHelper : MonoBehaviour
{
    public static GameObject GetParent(GameObject target)
    {
        if (target.transform.parent != null)
        {
            target = target.transform.parent.gameObject;
        }
        return target;
    }

    public static List<Transform> GetChildrenTransform(GameObject target)
    {
        List<Transform> children = new List<Transform>();
        int childrenCount = target.transform.childCount;
        for (int i = 0; i < childrenCount; i++)
        {
            children.Add(target.transform.GetChild(i));
        }
        return children;
    }

    public static List<Transform> GetChildrenWithName(GameObject target, string name)
    {
        List<Transform> allChildren = GetChildrenTransform(target);
        List<Transform> filteredChildren = new List<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.gameObject.name.Equals(name))
            {
                filteredChildren.Add(child);
            }
        }
        return filteredChildren;
    }
}
