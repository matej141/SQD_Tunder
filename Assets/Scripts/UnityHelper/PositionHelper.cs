using UnityEngine;

public class PositionHelper : MonoBehaviour
{
    public static Vector2 TransformWorldToLocalUiPosition(RectTransform target2d, Vector3 worldPosition)
    {
        Vector3 v3 = target2d.InverseTransformPoint(worldPosition);
        return new Vector2(v3.x, v3.y);
    }

    public static Vector2 TransformWorldToLocalUiPosition(GameObject target2d, Vector3 worldPosition)
    {
        return TransformWorldToLocalUiPosition((RectTransform)target2d.transform, worldPosition);
    }

    public static Vector2 TransformWorldToLocalBottomLeftPosition(RectTransform target2d, Vector3 worldPosition)
    {
        // https://answers.unity.com/questions/839773/getting-the-mouses-location-on-a-world-space-canva.html
        Vector2 localPosition = TransformWorldToLocalUiPosition(target2d, worldPosition);
        //Debug.Log("World Inverse UI hit 3D position: " + relativePoint);
        Vector2 sizeOffset = target2d.sizeDelta;
        sizeOffset.x *= target2d.pivot.x;
        sizeOffset.y *= target2d.pivot.y;
        Vector2 bottomLeftPosition = new Vector2();
        bottomLeftPosition.x = sizeOffset.x + localPosition.x;
        bottomLeftPosition.y = sizeOffset.y + localPosition.y;
        //Debug.Log("Local UI raycast hit 2D position: " + hitLocation);
        return bottomLeftPosition;
    }

    public static Vector2 TransformWorldToLocalBottomLeftPosition(GameObject target2d, Vector3 worldPosition)
    {
        return TransformWorldToLocalBottomLeftPosition((RectTransform)target2d.transform, worldPosition);
    }
}
