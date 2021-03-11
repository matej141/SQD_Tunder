using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastHelper : MonoBehaviour
{
    public static int RayCastDistance = 10000;

    public static RaycastHit[] RaycastAllWorldPhysics(/*bool ignoreUiLayer = true*/)
    {
        RaycastHit[] hits = new RaycastHit[0];
        //if (ignoreUiLayer && RaycastWorldUI(true) == null) return hits;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hits = Physics.RaycastAll(ray, RayCastDistance);
        //Debug.Log("FIRE: " + hits.Length + " hits");
        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.transform.gameObject.name);
        }
        return hits;
    }

    public static List<RaycastResult> RaycastAllWorldUI(bool ignoreUiLayer = true)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        // Debug.Log("FIRE: " + results.Count + " hits");
        foreach (RaycastResult result in results)
        {
            //Debug.Log(result.gameObject.name);
            if (result.gameObject.layer == 5)   // UI layer has number 5
            {
                results.Clear();
                break;
            }
        }
        return results;
    }

    public static Vector3 RaycastSpecificWorldUI(GameObject target2d)
    {
        // https://answers.unity.com/questions/839773/getting-the-mouses-location-on-a-world-space-canva.html
        Plane plane = new Plane();
        plane.Set3Points(
            target2d.transform.TransformPoint(new Vector3(0, 0)),
            target2d.transform.TransformPoint(new Vector3(0, 1)),
            target2d.transform.TransformPoint(new Vector3(1, 0))
        );
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayHitDistance;
        if (plane.Raycast(ray, out rayHitDistance))
        {
            Vector3 worldHitPositionOfUiObject = ray.GetPoint(rayHitDistance);
            //Debug.Log("World UI raycast hit 3D position: " + worldHitPositionOfUiObject);
            return worldHitPositionOfUiObject;
        }
        return Vector3.zero;
    }

    public static List<RaycastResult> Reorder(List<RaycastResult> raycast)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        Dictionary<float, List<RaycastResult>> dd = new Dictionary<float, List<RaycastResult>>();
        List<float> distances = new List<float>();

        foreach (RaycastResult element in raycast)
        {
            float distance = element.distance;
            if (!dd.ContainsKey(distance))
            {
                distances.Add(distance);
                dd.Add(distance, new List<RaycastResult>());
            }
            dd[distance].Add(element);
        }
        distances.Sort();

        foreach (float distance in distances)
        {
            results.AddRange(dd[distance]);
        }
        return results;
    }

    public static List<RaycastResult> FilterNearest(List<RaycastResult> raycast)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        if (raycast.Count < 1) return results;

        List<RaycastResult> sorted = Reorder(raycast);
        double closestDistance = sorted[0].distance;

        foreach (RaycastResult result in sorted)
        {
            if (result.distance != closestDistance) break;
            results.Add(result);
        }

        return results;
    }
}
