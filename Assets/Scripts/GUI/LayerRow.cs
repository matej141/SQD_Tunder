using DG;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerRow : MonoBehaviour
{
    public static readonly float LayerOffset = 300f;

    public DgElement Layer = null;
    public InputField DiagramLayerName = null;
    public Text RowLayerName = null;

    public Button DeleteButton = null;
    public Button MoveForwardButton = null;
    public Button MoveBackwardButton = null;

    void Start()
    {
        DeleteButton.onClick.AddListener(delegate { DeleteLayer(); });
        MoveForwardButton.onClick.AddListener(delegate { MoveLayerForward(); });
        MoveBackwardButton.onClick.AddListener(delegate { MoveLayerBackward(); });

        // Event handler for text update is done in create layer script
    }


    public void UpdateMyNameAccordingToDiagram(string newValue)
    {
        RowLayerName.text = DiagramLayerName.transform.parent.GetComponent<Text>().text;
    }

    private void DeleteLayer()
    {
        GameObject parent = HierarchyHelper.GetParent(Layer.gameObject);
        // We need to set the parent of the Layer to null, because Destroy is executed on the end of the Update cycle
        // and the Layer is not removed from the list of game objects before the recalculation of positions.
        Layer.transform.SetParent(null);

        GameObject.Destroy(Layer.gameObject);
        GameObject.Destroy(gameObject);
        
        List<Transform> layers = HierarchyHelper.GetChildrenTransform(parent);
        foreach (Transform layer in layers)
        {
           layer.position = new Vector3(0f, 0f, layer.GetSiblingIndex() * LayerOffset);
        }
 
    }


    public void MapToLayer(DG.UML.SequenceDiagram dgLayer)
    {
        LayerRow layerRowScript = gameObject.GetComponent<LayerRow>();
        // Map row to layer
        dgLayer.LayerRow = layerRowScript;
        layerRowScript.Layer = dgLayer;

        GameObject nameWrapper = HierarchyHelper.GetChildrenWithName(dgLayer.gameObject, "NameWrapper")[0].gameObject;
        GameObject layerNameText = HierarchyHelper.GetChildrenWithName(nameWrapper, "Name")[0].gameObject;
        GameObject layerNameInput = HierarchyHelper.GetChildrenWithName(layerNameText, "InputField")[0].gameObject;
        layerRowScript.DiagramLayerName = layerNameInput.GetComponent<InputField>();

        // Make text auto update
        layerRowScript.DiagramLayerName.onValueChanged.AddListener(layerRowScript.UpdateMyNameAccordingToDiagram);

        // Set initial text
        layerRowScript.UpdateMyNameAccordingToDiagram("");


    }

    public void MoveLayerForward()
    {
        //if (Layer.gameObject.transform.GetSiblingIndex() !=0)
        {
            gameObject.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex() - 1);
            Layer.gameObject.transform.SetSiblingIndex(Layer.gameObject.transform.GetSiblingIndex() - 1);
        }

        UpdateSceneLayerPositions();
    }

    public void MoveLayerBackward()
    {
        List<Transform> layers = HierarchyHelper.GetChildrenTransform(HierarchyHelper.GetParent(Layer.gameObject));

        if (Layer.gameObject.transform.GetSiblingIndex() == layers.Count - 1)
        {
            gameObject.transform.SetSiblingIndex(0);
            Layer.gameObject.transform.SetSiblingIndex(0);
        }
        else
        {
            gameObject.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex() + 1);
            Layer.gameObject.transform.SetSiblingIndex(Layer.gameObject.transform.GetSiblingIndex() + 1);
        }

        UpdateSceneLayerPositions();
    }

    public void UpdateSceneLayerPositions()
    {
        foreach (Transform layer in HierarchyHelper.GetChildrenTransform(HierarchyHelper.GetParent(Layer.gameObject)))
        {
            layer.position = new Vector3(0f, 0f, layer.GetSiblingIndex() * LayerOffset);
        }
    }

}
