using DG;
using DG.UML;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootTool : DiagramTool
{
    public override void OnMouseButtonStart(DiagramInputHandler context)
    {
        base.OnMouseButtonStart(context);
        foreach (RaycastResult result in context.GuiRaycast)
        {
            DgElement dg = result.gameObject.GetComponent<DgElement>();
            if (dg == null) continue;

            if (result.gameObject.GetComponent<SequenceDiagram>() != null) continue;

            Rigidbody rigidbody = dg.gameObject.GetComponent<Rigidbody>();
            if (rigidbody != null) break;

            // Add gravity
            rigidbody = dg.gameObject.AddComponent<Rigidbody>();
            rigidbody.mass = 100f;

            // Common destroy
            dg.transform.SetParent(GameObject.Find("ViewModel").transform);
            Destroy(dg.gameObject, 10f);
            dg.enabled = false;
            Destroy(dg);

            break;
        }
    }
}
