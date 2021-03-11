using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewLayerTool : DiagramTool
{
    public GameObject PrefabLayer = null;
    public GameObject PrefabLayerRow = null;
    public static readonly float LayerOffset = 300f;

    public override void OnToolStart(DiagramTool previousTool, DiagramInputHandler context)
    {

        GameObject layer = CreateLayer();
        
        DiagramInputHandler.ChooseTool(null);
    }

    public GameObject CreateLayer()
    {

        GameObject viewModel = GameObject.Find("ViewModel");
        GameObject layer = Instantiate(PrefabLayer);
        GameObject layerRow = Instantiate(PrefabLayerRow);
        DG.UML.SequenceDiagram dgLayer = layer.GetComponent<DG.UML.SequenceDiagram>();
        LayerRow layerRowScript = layerRow.GetComponent<LayerRow>();

        // Map layer to row
        layerRowScript.MapToLayer(dgLayer);
        
        // Move Layer row to top position in menu
        Transform parent = GameObject.Find("VLLayerRow").transform;
        layerRow.transform.SetParent(parent);

        //Force rebuild layout, because it was bugged
        layerRow.transform.SetSiblingIndex(0);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)parent);
       
        // Move diagram to front position in scene
        layer.transform.SetParent(viewModel.transform);
        layer.transform.SetSiblingIndex(0);
        List<Transform> layers = HierarchyHelper.GetChildrenTransform(HierarchyHelper.GetParent(layer.gameObject));
        foreach (Transform layerx in layers)
        {
            layerx.position = new Vector3(0f, 0f, layerx.GetSiblingIndex() * LayerOffset);
        }

       
        return layer;

    }


}
