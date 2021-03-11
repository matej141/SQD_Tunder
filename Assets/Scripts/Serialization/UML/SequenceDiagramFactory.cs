using System.Collections.Generic;
using UnityEngine;
using Data;
using DG;
using Data.MOF;
using UML.Interactions;
using DG.UML;
using MongoDB.Bson;

public class SequenceDiagramFactory : AbstractFactory
{
    public GameObject PrefabSequenceDiagram = null;
    public GameObject PrefabLayerRow = null;

    public override void RegisterFactory(FactoryRegister register)
    {
        register.RegisterXmiString("uml:Interaction", this);
        register.RegisterClass(typeof(UML.Interactions.Interaction), this);
        register.RegisterClass(typeof(DG.UML.SequenceDiagram), this);
    }
    

    public override List<global::Data.MOF.MofElement> BuildMofFromJson(BsonDocument json, XmiCollection container)
    {
        List<global::Data.MOF.MofElement> result = new List<global::Data.MOF.MofElement>();
       
        UML.Interactions.Interaction MofSequenceDiagram = new UML.Interactions.Interaction();

        string id = (string)json.GetValue("XmiId");
        MofSequenceDiagram.XmiId = id;
        container.AddMofElement(MofSequenceDiagram);
        result.Add(MofSequenceDiagram);

        return result;
    }

    
    public override void UpdateMofFromJson(BsonDocument json, XmiCollection container)
    {
        UML.Interactions.Interaction mofInteraction = (UML.Interactions.Interaction)container.GetMofElement((string)json.GetValue("XmiId"));

        //lifeline
        mofInteraction.lifeline.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "lifeline"))
        {
            mofInteraction.lifeline.Add((UML.Interactions.Lifeline)mof);
        }

        // covered
        mofInteraction.covered.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "covered"))
        {
            mofInteraction.covered.Add((UML.Interactions.Lifeline)mof);
        }

        // message
        mofInteraction.message.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "message"))
        {
            mofInteraction.message.Add((UML.Interactions.Message)mof);
        }

        // owner
        mofInteraction.owner = (UML.CommonStructure.Element)JsonConvertor.FindMofByXmiId(json, container, "owner");

        // fragment
        mofInteraction.fragment.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "fragment"))
        {
            mofInteraction.fragment.Add((UML.Interactions.InteractionFragment)mof);
        }

        // formalGate
        mofInteraction.formalGate.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "formalGate"))
        {
            mofInteraction.formalGate.Add((UML.Interactions.Gate)mof);
        }

        // generalOrdering
        mofInteraction.generalOrdering.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "generalOrdering"))
        {
            mofInteraction.generalOrdering.Add((UML.Interactions.GeneralOrdering)mof);
        }

        // name
        //mofInteraction.name = JsonConvertor.FindMofByXmiId(json, container, "name").ToString();
        mofInteraction.name = json.GetValue("name").ToString();

        // ownedElement
        mofInteraction.ownedElement.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedElement"))
        {
            mofInteraction.ownedElement.Add((UML.CommonStructure.Element)mof);
        }

        // ownedComment
        mofInteraction.ownedComment.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedComment"))
        {
            mofInteraction.ownedComment.Add((UML.CommonStructure.Comment)mof);
        }
        
    }

    public override List<MofElement> BuildMofFromDg(DgElement dg, XmiCollection container)
    {
        List<global::Data.MOF.MofElement> result = new List<global::Data.MOF.MofElement>();

        Interaction mof = new Interaction();
        mof.generateNewXmiId();
        mof.dgElement.Add(dg);
        dg.mofElement.Add(mof);
        container.AddMofElement(mof);
        result.Add(mof);

        return result;
    }

    public override void UpdateMofFromDg(DgElement dg, XmiCollection container)
    {
        Interaction mofDiagram = (Interaction)dg.mofElement[0];
        SequenceDiagram dgDiagram = (SequenceDiagram)dg;

        //lifelines 
        //covered 
        mofDiagram.lifeline.Clear();
        mofDiagram.covered.Clear();
        foreach (global::DG.UML.LifeLine dgLifeline in dgDiagram.GetLifeLines())
        {
            UML.Interactions.Lifeline mofLifeLine = (UML.Interactions.Lifeline)dgLifeline.mofElement[0];
            mofDiagram.lifeline.Add(mofLifeLine);
            mofDiagram.covered.Add(mofLifeLine);
        }

        //messages
        mofDiagram.message.Clear();
        foreach (global::DG.UML.Message dgMessage in dgDiagram.GetMessagesRec())
        {
            UML.Interactions.Message mofMessage = (UML.Interactions.Message)dgMessage.mofElement[0];
            mofDiagram.message.Add(mofMessage);
        }

        //fragments
        mofDiagram.fragment.Clear();
        foreach (global::DG.DgElement dgElement in dgDiagram.GetVlChildren())
        {
            if (dgElement.GetComponent<global::DG.UML.Message>() != null)
            {
                UML.Interactions.OccurrenceSpecification mofOccurenceSpecification1 = (UML.Interactions.OccurrenceSpecification)dgElement.mofElement[1];
                UML.Interactions.OccurrenceSpecification mofOccurenceSpecification2 = (UML.Interactions.OccurrenceSpecification)dgElement.mofElement[2];
                mofDiagram.fragment.Add(mofOccurenceSpecification1);
                mofDiagram.fragment.Add(mofOccurenceSpecification2);
            }
            else if (dgElement.GetComponent<global::DG.UML.Fragment>() != null)
            {
                UML.Interactions.CombinedFragment combinedFragment = (UML.Interactions.CombinedFragment)dgElement.mofElement[0];
                mofDiagram.fragment.Add(combinedFragment);
            }
        }
        
        //name
        mofDiagram.name = dgDiagram.GetName();

        //formalGate
        /* TODO      not now   */

        //generalOrdering 
        /* TODO      not now   */

        //enclosingInteraction
        mofDiagram.enclosingInteraction = null;

        //enclosingOperand
        mofDiagram.enclosingOperand = null;

        //ownedElement
        mofDiagram.ownedElement.Clear();
        foreach (UML.Interactions.Message mofMessage in mofDiagram.message)
        {
            mofDiagram.ownedElement.Add(mofMessage);
        }
        foreach (UML.Interactions.Lifeline mofLifeLine in mofDiagram.lifeline)
        {
            mofDiagram.ownedElement.Add(mofLifeLine);
        }
        foreach (UML.Interactions.InteractionFragment mofFragment in mofDiagram.fragment)
        {
            mofDiagram.ownedElement.Add(mofFragment);
        }

        //ownedComment
        mofDiagram.ownedComment.Clear();

        //owner
        mofDiagram.owner = null;
    }

    public override List<DgElement> BuilDgFromMof(MofElement mof, XmiCollection container)
    {
        // Build
        List<global::DG.DgElement> result = new List<global::DG.DgElement>();
        var mofIntereaction = (UML.Interactions.Interaction)mof;

        GameObject go = GameObject.Instantiate(PrefabSequenceDiagram);
        SequenceDiagram dg = go.GetComponent<SequenceDiagram>();

        result.Add(dg);


        // Map references

        dg.mofElement.Add(mofIntereaction);
        mofIntereaction.dgElement.Add(dg);

        return result;
    }

    public override void UpdateDgFromMof(DgElement dg, XmiCollection container)
    {
        Interaction mofDiagram = (UML.Interactions.Interaction)dg.mofElement[0];
        SequenceDiagram dgDiagram = (SequenceDiagram)dg;

        // Set my name
        dgDiagram.SetName(mofDiagram.name);

        // Add myself to Diagram at correct position
        
        GameObject layerRow = Instantiate(PrefabLayerRow);
        LayerRow layerRowScript = layerRow.GetComponent<LayerRow>();
        layerRow.transform.SetParent(GameObject.Find("VLLayerRow").transform);
        dg.transform.SetParent(GameObject.Find("ViewModel").transform);
        layerRowScript.MapToLayer(dgDiagram);
        layerRowScript.UpdateSceneLayerPositions();

        // Add lifelines as my children

        GameObject layer = dg.gameObject;
        GameObject borders = HierarchyHelper.GetChildrenWithName(layer, "Borders")[0].gameObject;
        GameObject hl = HierarchyHelper.GetChildrenWithName(borders, "HL")[0].gameObject;
        GameObject vl = HierarchyHelper.GetChildrenWithName(borders, "VL")[0].gameObject;

        foreach (UML.Interactions.Lifeline mofLifeline in mofDiagram.lifeline)
        {
            global::DG.UML.LifeLine dgLifeline = (global::DG.UML.LifeLine)mofLifeline.dgElement[0];
            dgLifeline.gameObject.transform.position = hl.transform.position;
            dgLifeline.gameObject.transform.SetParent(hl.transform);

        }

        // Add messages and fragment as my children
        List<UML.Interactions.Message> diagramMessages = mofDiagram.message;
        foreach (UML.Interactions.InteractionFragment interactionFragment in mofDiagram.fragment)
        {
            switch(interactionFragment.XmiType())
            {
                case "uml:OccurrenceSpecification":
                    // sendEvent -> receiveEvent
                    UML.Interactions.OccurrenceSpecification occurrence = (UML.Interactions.OccurrenceSpecification)interactionFragment;
                    foreach (UML.Interactions.Message mofMessage in diagramMessages)
                    {
                        if (occurrence == mofMessage.sendEvent || occurrence == mofMessage.receiveEvent)
                        {
                            global::DG.UML.Message dgMessage = (global::DG.UML.Message)mofMessage.dgElement[0];
                            dgMessage.gameObject.transform.position = vl.transform.position;
                            dgMessage.gameObject.transform.SetParent(vl.transform);
                            break;
                        }
                    }
                    break;
                case "uml:CombinedFragment":
                    UML.Interactions.CombinedFragment mofFragment = (UML.Interactions.CombinedFragment)interactionFragment;
                    global::DG.UML.Fragment dgFragment = (global::DG.UML.Fragment)mofFragment.dgElement[0];
                    dgFragment.gameObject.transform.position = vl.transform.position;
                    dgFragment.gameObject.transform.SetParent(vl.transform);
                    break;
            }
        }
    }
}
