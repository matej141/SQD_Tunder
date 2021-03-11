using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalManager : MonoBehaviour
{

    public GameObject[] UpperNeighbours;
    public GameObject[] LowerNeighbours;
    public GameObject[] WrapNeighbours;

    public bool WrapChildrenObjects = true;
    public bool WrapListObjects = true;

    public float Padding;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<Transform> WrapChildren = HierarchyHelper.GetChildrenTransform(gameObject);

        RectTransform myRectTransform = (RectTransform)transform;
        Vector2 myPivot = myRectTransform.pivot;
        float myWidth = myRectTransform.rect.width;
        Vector3 myWorldPosition = myRectTransform.transform.position;

        float myMinX = myWorldPosition.x - myWidth * myPivot.x;
        float myMaxX = myWorldPosition.x - myWidth * (myPivot.x - 1.0f);

        Debug.Log("H: My pivot:" + myPivot);
        Debug.Log("H: My size:" + myWidth);
        Debug.Log("H: My min/max:" + myMinX + "/" + myMaxX);

        bool wrappedObjectsExist = false;
        float minX = 0;
        float maxX = 0;

        if (WrapListObjects && WrapNeighbours.Length > 0)
        {
            for (int i = 0; i < WrapNeighbours.Length; i++)
            {
                if (WrapNeighbours[i] == null) continue;

                GameObject subObject = WrapNeighbours[i];
                RectTransform subRectTransform = (RectTransform)subObject.transform;
                Vector2 subPivot = subRectTransform.pivot;
                float subWidth = subRectTransform.rect.width;
                Vector3 subWorldPosition = subRectTransform.transform.position;

                float subMinX = subWorldPosition.x - subWidth * subPivot.x;
                float subMaxX = subWorldPosition.x - subWidth * (subPivot.x - 1.0f);

                if (!wrappedObjectsExist)
                {
                    wrappedObjectsExist = true;
                    minX = subMinX;
                    maxX = subMaxX;
                }
                if (subMinX < minX) minX = subMinX;
                if (subMaxX > maxX) maxX = subMaxX;

                Debug.Log("H: Wrap " + i + " pivot:" + subPivot);
                Debug.Log("H: Wrap " + i + " size:" + subWidth);
                Debug.Log("H: Wrap " + i + " min/max:" + subMinX + "/" + subMaxX);
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
                float subWidth = subRectTransform.rect.width;
                Vector3 subWorldPosition = subRectTransform.transform.position;

                float subMinX = subWorldPosition.x - subWidth * subPivot.x;
                float subMaxX = subWorldPosition.x - subWidth * (subPivot.x - 1.0f);

                if (!wrappedObjectsExist)
                {
                    wrappedObjectsExist = true;
                    minX = subMinX;
                    maxX = subMaxX;
                }
                if (subMinX < minX) minX = subMinX;
                if (subMaxX > maxX) maxX = subMaxX;

                Debug.Log("H: Child " + i + " pivot:" + subPivot);
                Debug.Log("H: Child " + i + " size:" + subWidth);
                Debug.Log("H: Child " + i + " min/max:" + subMinX + "/" + subMaxX);
            }
        }

        if (!wrappedObjectsExist) return;

        // Temporarily disable my children
        /*for (int i = 0; i < WrapChildren.Count; i++)
        {
            WrapChildren[i].SetParent(null);
        }*/

        // Resize myself
        
        maxX += Padding;
        minX -= Padding;
        myWidth = maxX - minX;
        myRectTransform.transform.position = new Vector3(minX + (myPivot.x) * myWidth, myRectTransform.transform.position.y, myRectTransform.transform.position.z);
        myRectTransform.sizeDelta = new Vector2(myWidth, myRectTransform.sizeDelta.y);

        // Give me my children back
        /*for (int i = 0; i < WrapChildren.Count; i++)
        {
            WrapChildren[i].SetParent(transform);
        }*/
    }
}
