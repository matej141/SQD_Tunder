using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
using UML.Interactions;
using DG;
using Data.MOF;
using MongoDB.Bson;

public class MessageFactory : AbstractFactory 
{
    public GameObject PrefabMessage = null;

    public override void RegisterFactory(FactoryRegister register)
    {
        register.RegisterXmiString("uml:Message", this);
        register.RegisterClass(typeof(UML.Interactions.Message), this);
        register.RegisterClass(typeof(DG.UML.Message), this);

        register.RegisterXmiString("uml:OccurrenceSpecification", this);
        register.RegisterClass(typeof(UML.Interactions.OccurrenceSpecification), this);
    }
    
    
    public override List<global::Data.MOF.MofElement> BuildMofFromJson(BsonDocument json, XmiCollection container)
    {
        List<global::Data.MOF.MofElement> result = new List<global::Data.MOF.MofElement>();

        switch ((string)json.GetValue("XmiType"))
        {
            case "uml:Message":
                UML.Interactions.Message mofMessage = new UML.Interactions.Message();
                mofMessage.XmiId = (string)json.GetValue("XmiId");
                container.AddMofElement(mofMessage);
                result.Add(mofMessage);
                break;
            case "uml:OccurrenceSpecification":
                UML.Interactions.OccurrenceSpecification mofOccurrenceSpecification = new UML.Interactions.OccurrenceSpecification();
                mofOccurrenceSpecification.XmiId = (string)json.GetValue("XmiId");
                container.AddMofElement(mofOccurrenceSpecification);
                result.Add(mofOccurrenceSpecification);
                break;
            default:
                Debug.Log("Invalid ELement!" + (string)json.GetValue("XmiType"));
                break;
        }

        return result;
    }

    public override void UpdateMofFromJson(BsonDocument json, XmiCollection container)
    {
        switch ((string)json.GetValue("XmiType"))
        {
            case "uml:Message":
                UML.Interactions.Message message = (UML.Interactions.Message)container.GetMofElement((string)json.GetValue("XmiId"));
                message.messageSort = (UML.Interactions.MessageSort)(json.GetValue("messageSort").AsInt32);
                message.owner = (UML.CommonStructure.Element)(JsonConvertor.FindMofByXmiId(json, container, "owner"));

                message.ownedElement.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedElement"))
                    message.ownedElement.Add((UML.CommonStructure.Element)mof);

                message.ownedComment.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedComment"))
                    message.ownedComment.Add((UML.CommonStructure.Comment)mof);

                message.name = json.GetValue("name").ToString();
                message.receiveEvent = (UML.Interactions.OccurrenceSpecification)JsonConvertor.FindMofByXmiId(json, container, "receiveEvent");
                message.sendEvent = (UML.Interactions.OccurrenceSpecification)JsonConvertor.FindMofByXmiId(json, container, "sendEvent");
                message.interaction = (UML.Interactions.Interaction)JsonConvertor.FindMofByXmiId(json, container, "interaction");

                break;
            case "uml:OccurrenceSpecification":
                UML.Interactions.OccurrenceSpecification occurrenceSpecification = (UML.Interactions.OccurrenceSpecification)container.GetMofElement((string)json.GetValue("XmiId"));
                occurrenceSpecification.owner = (UML.CommonStructure.Element)JsonConvertor.FindMofByXmiId(json, container, "owner");

                occurrenceSpecification.ownedElement.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedElement"))
                    occurrenceSpecification.ownedElement.Add((UML.CommonStructure.Element)mof);

                occurrenceSpecification.ownedComment.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedComment"))
                    occurrenceSpecification.ownedComment.Add((UML.CommonStructure.Comment)mof);

                occurrenceSpecification.name = json.GetValue("name").ToString();

                occurrenceSpecification.covered.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "covered"))
                    occurrenceSpecification.covered.Add((UML.Interactions.Lifeline)mof);

                occurrenceSpecification.toBefore.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "toBefore"))
                    occurrenceSpecification.toBefore.AddLast((UML.Interactions.GeneralOrdering)mof);

                occurrenceSpecification.toAfter.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "toAfter"))
                    occurrenceSpecification.toAfter.AddLast((UML.Interactions.GeneralOrdering)mof);

                occurrenceSpecification.generalOrdering.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "generalOrdering"))
                    occurrenceSpecification.generalOrdering.Add((UML.Interactions.GeneralOrdering)mof);

                occurrenceSpecification.enclosingInteraction = (UML.Interactions.Interaction)JsonConvertor.FindMofByXmiId(json, container, "enclosingInteraction");

                break;
            default:
                Debug.Log("Invalid Element!" + (string)json.GetValue("XmiType"));
                break;
        }
    }

    public override List<DgElement> BuilDgFromMof(MofElement mof, XmiCollection container)
    {
        List<global::DG.DgElement> result = new List<global::DG.DgElement>();
        switch (mof.XmiType())
        {
            case "uml:Message":
                // Build
                var mofMessage = (UML.Interactions.Message)mof;
                GameObject go = GameObject.Instantiate(PrefabMessage);
                global::DG.UML.Message dg = go.GetComponent<global::DG.UML.Message>();
                result.Add(dg);

                // Map references

                dg.mofElement.Add(mofMessage);
                mofMessage.dgElement.Add(dg);

                break;
            case "uml:OccurrenceSpecification":
                Debug.Log("Skipping BUILD of DG OccurrenceSpecification from MOF OccurrenceSpecification.");
                break;
            default:
                Debug.Log("Invalid Element!" + mof.XmiType());
                break;
        }
        return result;
    }
    public override void UpdateDgFromMof(DgElement dg, XmiCollection container)
    {
        UML.Interactions.Message mof = (UML.Interactions.Message)dg.mofElement[0];
        global::DG.UML.Message dgMessage = (global::DG.UML.Message) dg;

        // Message name
        Transform header = HierarchyHelper.GetChildrenWithName(dg.gameObject, "Label")[0];
        Transform objectName = HierarchyHelper.GetChildrenWithName(header.gameObject, "MethodName")[0];
        Transform inputField = HierarchyHelper.GetChildrenWithName(objectName.gameObject, "InputField")[0];
        inputField.GetComponent<InputField>().text = mof.name;

        // Message Lifelines
        UML.Interactions.Lifeline startMofLifeline = mof.sendEvent.covered[0]; // toto by mala byt MOF lifeline 
        global::DG.UML.LifeLine startDgLifeline = (global::DG.UML.LifeLine)startMofLifeline.dgElement[0];
        UML.Interactions.Lifeline endMofLifeline = mof.receiveEvent.covered[0]; // toto by mala byt MOF lifeline
        global::DG.UML.LifeLine endDgLifeline = (global::DG.UML.LifeLine)endMofLifeline.dgElement[0];
        
        dgMessage.FromLifeLine = startDgLifeline.transform.GetComponent<RectTransform>(); // toto by malo byt asi v update Message
        dgMessage.ToLifeLine = endDgLifeline.transform.GetComponent<RectTransform>(); // toto by malo byt asi v update Message
    }
    
    public override List<MofElement> BuildMofFromDg(DgElement dg, XmiCollection container)
    {
        List<global::Data.MOF.MofElement> result = new List<global::Data.MOF.MofElement>();

        UML.Interactions.Message mofMessage = new UML.Interactions.Message();
        OccurrenceSpecification mofOccurence1 = new UML.Interactions.OccurrenceSpecification();
        OccurrenceSpecification mofOccurence2 = new  UML.Interactions.OccurrenceSpecification();

        mofMessage.generateNewXmiId();
        mofOccurence1.generateNewXmiId();
        mofOccurence2.generateNewXmiId();

        mofMessage.dgElement.Add(dg);
        //mofOccurence1.dgElement.Add(dg);
        //mofOccurence2.dgElement.Add(dg);

        dg.mofElement.Add(mofMessage);
        dg.mofElement.Add(mofOccurence1);
        dg.mofElement.Add(mofOccurence2);

        container.AddMofElement(mofMessage);
        container.AddMofElement(mofOccurence1);
        container.AddMofElement(mofOccurence2);

        result.Add(mofMessage);
        result.Add(mofOccurence1);
        result.Add(mofOccurence2);

        return result;
    }

    public override void UpdateMofFromDg(DgElement dg, XmiCollection container)
    {
        UML.Interactions.Message mofMessage = (UML.Interactions.Message)dg.mofElement[0];
        global::DG.UML.Message dgMessage = (global::DG.UML.Message)dg;
        UML.Interactions.OccurrenceSpecification mofOccurrenceSpecification1 = (UML.Interactions.OccurrenceSpecification)dg.mofElement[1]; // odkial
        UML.Interactions.OccurrenceSpecification mofOccurrenceSpecification2 = (UML.Interactions.OccurrenceSpecification)dg.mofElement[2]; // kam
        global::DG.UML.SequenceDiagram dgDiagram = dgMessage.GetSequenceDiagram();
        UML.Interactions.Interaction mofDiagram = (UML.Interactions.Interaction)dgDiagram.mofElement[0];

        // TO JE MESSAGE
        // messageSort
        /* TODO         */

        // sendEvent - odkial lifeline
        mofMessage.sendEvent = mofOccurrenceSpecification1;

        // receiveEvent - kam lifeline
        mofMessage.receiveEvent = mofOccurrenceSpecification2;

        //name
        /* TODO         */
        mofMessage.name = dgMessage.GetName();

        //ownedElement
        /* TODO     empty array    */
        mofMessage.ownedElement.Clear();

        //ownedComment
        /* TODO     empty array    */
        mofMessage.ownedComment.Clear();

        //2x (owner == Interaction)
        mofMessage.interaction = mofDiagram;
        mofMessage.owner = mofDiagram;

        // argument
        /* TODO     do not implement yet    */

        // connector
        /* TODO     do not implement yet    */

        // signature
        /* TODO     do not implement yet    */

        // messageKind 
        /* TODO     we discuss it if we should use it or discard it  */



        // TO JE OCCURENCE S.

        // covered 
        mofOccurrenceSpecification1.covered.Clear();
        mofOccurrenceSpecification2.covered.Clear();
        global::DG.UML.LifeLine dgLifelineFrom = dgMessage.FromLifeLine.GetComponent<global::DG.UML.LifeLine>();
        mofOccurrenceSpecification1.covered.Add((UML.Interactions.Lifeline)dgLifelineFrom.mofElement[0]);
        global::DG.UML.LifeLine dgLifelineTo = dgMessage.ToLifeLine.GetComponent<global::DG.UML.LifeLine>();
        mofOccurrenceSpecification2.covered.Add((UML.Interactions.Lifeline)dgLifelineTo.mofElement[0]);
        /* TODO lifeline zistit ktora */

        // enclosingInteraction == Owner
        mofOccurrenceSpecification1.owner = mofDiagram;
        mofOccurrenceSpecification2.owner = mofDiagram;

        // enclosingOperand
        /* TODO     complete?    */

        //name
        /* TODO         */
        mofOccurrenceSpecification1.name = "";
        mofOccurrenceSpecification2.name = "";

        //ownedElement
        /* TODO      empty array */
        mofOccurrenceSpecification1.ownedElement.Clear();
        mofOccurrenceSpecification2.ownedElement.Clear();

        //ownedComment
        /* TODO      empty array */
        mofOccurrenceSpecification1.ownedComment.Clear();
        mofOccurrenceSpecification2.ownedComment.Clear();

        //2x (owner == Interaction)
        /* TODO         */
        mofOccurrenceSpecification1.enclosingInteraction = mofDiagram;
        mofOccurrenceSpecification1.owner = mofDiagram;
        mofOccurrenceSpecification2.enclosingInteraction = mofDiagram;
        mofOccurrenceSpecification2.owner = mofDiagram;

        // toBefore 
        /* TODO     do not implement yet  */

        // toAfter
        /* TODO     do not implement yet    */

        // generalOrdering
        /* TODO     do not implement yet    */

    }
}
