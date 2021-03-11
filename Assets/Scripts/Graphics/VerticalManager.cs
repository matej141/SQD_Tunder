using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalManager : MonoBehaviour
{

    public GameObject[] UpperNeighbours;
    public GameObject[] LowerNeighbours;
    public GameObject[] WrapNeighbours;

    public bool WrapChildrenObjects = true;
    public bool WrapListObjects = true;

    public float Padding;
    
    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        List<Transform> WrapChildren = HierarchyHelper.GetChildrenTransform(gameObject);

        RectTransform myRectTransform = (RectTransform)transform;
        Vector2 myPivot = myRectTransform.pivot;
        float myHeight = myRectTransform.rect.height;
        Vector3 myWorldPosition = myRectTransform.transform.position;

        float myMinY = myWorldPosition.y - myHeight * myPivot.y;
        float myMaxY = myWorldPosition.y - myHeight * (myPivot.y - 1.0f);

        Debug.Log("V: My pivot:" + myPivot);
        Debug.Log("V: My size:" + myHeight);
        Debug.Log("V: My min/max:" + myMinY + "/" + myMaxY);

        bool wrappedObjectsExist = false;
        float minY = 0;
        float maxY = 0;

        if (WrapListObjects && WrapNeighbours.Length > 0)
        {
            for (int i = 0; i < WrapNeighbours.Length; i++)
            {
                if (WrapNeighbours[i] == null) continue;

                GameObject subObject = WrapNeighbours[i];
                RectTransform subRectTransform = (RectTransform)subObject.transform;
                Vector2 subPivot = subRectTransform.pivot;
                float subHeight = subRectTransform.rect.height;
                Vector3 subWorldPosition = subRectTransform.transform.position;

                float subMinY = subWorldPosition.y - subHeight * subPivot.y;
                float subMaxY = subWorldPosition.y - subHeight * (subPivot.y - 1.0f);

                if (!wrappedObjectsExist)
                {
                    wrappedObjectsExist = true;
                    minY = subMinY;
                    maxY = subMaxY;
                }
                if (subMinY < minY) minY = subMinY;
                if (subMaxY > maxY) maxY = subMaxY;

                Debug.Log("V: Wrap " + i + " pivot:" + subPivot);
                Debug.Log("V: Wrap " + i + " size:" + subHeight);
                Debug.Log("V: Wrap " + i + " min/max:" + subMinY + "/" + subMaxY);
            }
        }

        if (WrapChildrenObjects && WrapChildren != null && WrapChildren.Count > 0)
        {
            for (int i = 0; i < WrapChildren.Count; i++)
            {
                if (WrapChildren[i] == null) continue;

                GameObject subObject = WrapChildren[i].gameObject;
                RectTransform subRectTransform = (RectTransform)subObject.transform;
                Vector2 subPivot = subRectTransform.pivot;
                float subHeight = subRectTransform.rect.height;
                Vector3 subWorldPosition = subRectTransform.transform.position;

                float subMinY = subWorldPosition.y - subHeight * subPivot.y;
                float subMaxY = subWorldPosition.y - subHeight * (subPivot.y - 1.0f);

                if (!wrappedObjectsExist)
                {
                    wrappedObjectsExist = true;
                    minY = subMinY;
                    maxY = subMaxY;
                }
                if (subMinY < minY) minY = subMinY;
                if (subMaxY > maxY) maxY = subMaxY;

                Debug.Log("V: Child " + i + " pivot:" + subPivot);
                Debug.Log("V: Child " + i + " size:" + subHeight);
                Debug.Log("V: Child " + i + " min/max:" + subMinY + "/" + subMaxY);
            }
        }

        if (!wrappedObjectsExist) return;

        // Temporarily disable my children
        /*for (int i = 0; i < WrapChildren.Count; i++)
        {
            WrapChildren[i].SetParent(null);
        }*/

        // Resize myself

        maxY += Padding;
        minY -= Padding;
        myHeight = maxY - minY;
        myRectTransform.transform.position = new Vector3(myRectTransform.transform.position.x, minY + (myPivot.y) * myHeight, myRectTransform.transform.position.z);
        myRectTransform.sizeDelta = new Vector2(myRectTransform.sizeDelta.x, myHeight);

        // Give me my children back
        /*for (int i = 0; i < WrapChildren.Count; i++)
        {
            WrapChildren[i].SetParent(transform);
        }*/
    }
}
