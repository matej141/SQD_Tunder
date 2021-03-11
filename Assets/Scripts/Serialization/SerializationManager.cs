using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using MongoDB.Bson;

using Data;
using Data.MOF;
using DG;
public class SerializationManager : MonoBehaviour
{
    public static XmiCollection SaveElements()
    {
        // Get root elements
        XmiCollection model = new XmiCollection();
        
        GameObject viewModel = GameObject.Find("ViewModel");

        // Get all DG elements
        // https://docs.unity3d.com/ScriptReference/GameObject.GetComponentsInChildren.html
        Component[] dgElements;
        dgElements = viewModel.GetComponentsInChildren(typeof(DgElement));
        if (dgElements == null || dgElements.Length == 0)
        {
            Debug.Log("There are no elements in model that could be saved.");
            return null;
        }

        // 0) Remove old MOF - DI -DG mappings
        foreach (DgElement dgElement in dgElements)
        {
            dgElement.mofElement.Clear();
        }

        // 1) Create empty MOF elements
        foreach (DgElement dgElement in dgElements)
        {
            try
            {
                AbstractFactory factory = FactoryRegister.Singleton.RegisteredClass[dgElement.GetType()];
                factory.BuildMofFromDg(dgElement, model);
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log("Unable to create empty MOF from JSON " + dgElement.GetType());
                Debug.Log(e);
            }
        }

        // 2) Fill data of MOF elements
        foreach (DgElement dgElement in dgElements)
        {
            try
            {
                AbstractFactory factory = FactoryRegister.Singleton.RegisteredClass[dgElement.GetType()];
                factory.UpdateMofFromDg(dgElement, model);
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log("Unable to fill data of MOF from JSON " + dgElement.GetType());
                Debug.Log(e);
            }
        }

        return model;
  
    }

    public static XmiCollection LoadElements(List<BsonDocument> jsonElements)
    {
        XmiCollection container = new XmiCollection();

        // 2) Build MOF from JSON
        foreach (BsonDocument element in jsonElements)
        {
            string str = null;
            try
            {
                str = (string)element.GetValue("XmiType");
                FactoryRegister.Singleton.RegisteredXmiString[str].BuildMofFromJson(element, container);
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log("LOAD 2) \t Unable to build MOF from JSON " + str);
                Debug.Log(e);
            }
        }

        // 3)  Update MOF from JSON
        foreach (BsonDocument element in jsonElements)
        {
            string str = null;
            try
            {
                str = (string)element.GetValue("XmiType");
                FactoryRegister.Singleton.RegisteredXmiString[str].UpdateMofFromJson(element, container);
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log("LOAD 3) \t Unable to update data of MOF from JSON " + str);
                Debug.Log(e);
            }
        }

        // 4) Build DG from MOF
        List<DgElement> createdDgElements = new List<DgElement>();
        foreach (KeyValuePair<string, MofElement> element in container.MofElements())
        {
            MofElement mof = null;
            try
            {
                mof = element.Value;
                createdDgElements.AddRange(FactoryRegister.Singleton.RegisteredClass[mof.GetType()].BuilDgFromMof(element.Value, container));
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log("LOAD 4) \t Unable to build DG from MOF " + mof.GetType());
                Debug.Log(e);
            }
        }

        // 5) Update DG from MOF
        foreach (DgElement dg in createdDgElements)
        {
            try
            {
                FactoryRegister.Singleton.RegisteredClass[dg.GetType()].UpdateDgFromMof(dg, container);
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log("LOAD 5) \t Unable to update data of DG from MOF " + dg.GetType());
                Debug.Log(e);
            }
        }

        // 6) Refresh layer menu
        LayoutRebuilder.ForceRebuildLayoutImmediate(GameObject.Find("VLLayerRow").GetComponent<RectTransform>());

        return container;
    }

    public static void RemoveElements()
    {

        foreach (Transform childTransform in HierarchyHelper.GetChildrenTransform(GameObject.Find("ViewModel")))
        {
            childTransform.SetParent(null);
            Destroy(childTransform.gameObject);
        }

        foreach (Transform childTransform in HierarchyHelper.GetChildrenTransform(GameObject.Find("VLLayerRow")))
        {
            childTransform.SetParent(null);
            Destroy(childTransform.gameObject);
        }

    }

}
