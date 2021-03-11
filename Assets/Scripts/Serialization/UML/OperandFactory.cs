using System.Collections.Generic;
using Data;
using Data.MOF;
using Data.DI;
using DG;
using MongoDB.Bson;
using UnityEngine;

public class OperandFactory : AbstractFactory
{
    public GameObject PrefabOperand = null;

    public override void RegisterFactory(FactoryRegister register)
    {
        register.RegisterXmiString("uml:InteractionOperand", this);
        register.RegisterClass(typeof(UML.Interactions.InteractionOperand), this);
        register.RegisterClass(typeof(DG.UML.Operand), this);

        register.RegisterXmiString("uml:InteractionConstraint", this);
        register.RegisterClass(typeof(UML.Interactions.InteractionConstraint), this);

        register.RegisterXmiString("uml:OpaqueExpression", this);
        register.RegisterClass(typeof(UML.Values.OpaqueExpression), this);
    }
    

    public override List<MofElement> BuildMofFromJson(BsonDocument json, XmiCollection container)
    {
        List<global::Data.MOF.MofElement> result = new List<global::Data.MOF.MofElement>();

        switch ((string)json.GetValue("XmiType"))
        {
            case "uml:InteractionOperand":
                UML.Interactions.InteractionOperand operand = new UML.Interactions.InteractionOperand();
                operand.XmiId = (string)json.GetValue("XmiId");
                container.AddMofElement(operand);
                result.Add(operand);
                break;
            case "uml:InteractionConstraint":
                UML.Interactions.InteractionConstraint interactionConstraint = new UML.Interactions.InteractionConstraint();
                interactionConstraint.XmiId = (string)json.GetValue("XmiId");
                container.AddMofElement(interactionConstraint);
                result.Add(interactionConstraint);
                break;
            case "uml:OpaqueExpression":
                UML.Values.OpaqueExpression opaqueExpression = new UML.Values.OpaqueExpression();
                opaqueExpression.XmiId = (string)json.GetValue("XmiId");
                container.AddMofElement(opaqueExpression);
                result.Add(opaqueExpression);
                break;
            default:
                Debug.Log("Invalid Element!" + (string)json.GetValue("XmiType"));
                break;
        }

        return result;
    }

    public override List<MofElement> BuildMofFromDg(DgElement dg, XmiCollection container)
    {
        List<global::Data.MOF.MofElement> result = new List<global::Data.MOF.MofElement>();

        UML.Interactions.InteractionOperand mofOperand = new UML.Interactions.InteractionOperand();
        UML.Interactions.InteractionConstraint mofConstraint = new UML.Interactions.InteractionConstraint();
        UML.Values.OpaqueExpression mofExpression = new UML.Values.OpaqueExpression();

        mofOperand.generateNewXmiId();
        mofConstraint.generateNewXmiId();
        mofExpression.generateNewXmiId();

        mofOperand.dgElement.Add(dg);
        mofConstraint.dgElement.Add(dg);
        mofExpression.dgElement.Add(dg);

        dg.mofElement.Add(mofOperand);
        dg.mofElement.Add(mofConstraint);
        dg.mofElement.Add(mofExpression);

        container.AddMofElement(mofOperand);
        container.AddMofElement(mofConstraint);
        container.AddMofElement(mofExpression);

        result.Add(mofOperand);
        result.Add(mofConstraint);
        result.Add(mofExpression);

        return result;
    }

    public override List<DG.DgElement> BuilDgFromMof(MofElement mof, XmiCollection container)
    {
        List<DG.DgElement> result = new List<DG.DgElement>();

        switch (mof.XmiType())
        {
            case "uml:InteractionOperand":
                // Build
                var mofOperand = (UML.Interactions.InteractionOperand)mof;
                GameObject go = GameObject.Instantiate(PrefabOperand);
                global::DG.UML.Operand dgOperand = go.GetComponent<global::DG.UML.Operand>();
                result.Add(dgOperand);

                // Map references

                dgOperand.mofElement.Add(mofOperand);
                mofOperand.dgElement.Add(dgOperand);

                break;
            case "uml:InteractionConstraint":
                Debug.Log("Skipping BUILD of DG InteractionConstraint from MOF InteractionConstraint.");
                break;
            case "uml:OpaqueExpression":
                Debug.Log("Skipping BUILD of DG OpaqueExpression from MOF OpaqueExpression.");
                break;
            default:
                Debug.Log("Invalid Element!" + mof.XmiType());
                break;
        }        
        return result;
    }

    public override void UpdateDgFromMof(DgElement dg, XmiCollection container)
    {
        global::DG.UML.Operand dgOperand = (global::DG.UML.Operand)dg;
        UML.Interactions.InteractionOperand mofOperand = (UML.Interactions.InteractionOperand)(dgOperand.mofElement[0]);

        UML.Interactions.Interaction mofDiagram = mofOperand.enclosingInteraction;

        GameObject vl = dgOperand.GetVL().gameObject;

        // Add messages and fragment as my children
        List<UML.Interactions.Message> diagramMessages = mofDiagram.message;
        foreach (UML.Interactions.InteractionFragment interactionFragment in mofOperand.fragment)
        {
            switch (interactionFragment.XmiType())
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
        
        UML.Interactions.InteractionConstraint mofInteractionConstraint = (UML.Interactions.InteractionConstraint)mofOperand.guard;
        UML.Values.OpaqueExpression mofOpaqueExpression = (UML.Values.OpaqueExpression)mofInteractionConstraint.specification;
        dgOperand.setGuardExpression(mofOpaqueExpression.body);

    }

    public override void UpdateMofFromDg(DgElement dg, XmiCollection container)
    {
        UML.Interactions.InteractionOperand mofOperand = (UML.Interactions.InteractionOperand)dg.mofElement[0];
        global::DG.UML.Operand dgOperand = (global::DG.UML.Operand)dg;
        UML.CommonStructure.Constraint mofConstraint = (UML.CommonStructure.Constraint)dg.mofElement[1];
        UML.Values.OpaqueExpression mofExpression = (UML.Values.OpaqueExpression)dg.mofElement[2];

        global::DG.UML.SequenceDiagram dgDiagram = dgOperand.GetSequenceDiagram();
        UML.Interactions.Interaction mofDiagram = (UML.Interactions.Interaction)dgDiagram.mofElement[0];


        // OPERAND


        // fragment
        mofOperand.fragment.Clear();
        foreach (global::DG.DgElement dgElement in dgOperand.GetVlChildren())
        {
            if (dgElement.GetComponent<global::DG.UML.Message>() != null)
            {
                UML.Interactions.OccurrenceSpecification mofOccurenceSpecification1 = (UML.Interactions.OccurrenceSpecification)dgElement.mofElement[1];
                UML.Interactions.OccurrenceSpecification mofOccurenceSpecification2 = (UML.Interactions.OccurrenceSpecification)dgElement.mofElement[2];
                mofOperand.fragment.AddLast(mofOccurenceSpecification1);
                mofOperand.fragment.AddLast(mofOccurenceSpecification2);
            }
            else if (dgElement.GetComponent<global::DG.UML.Fragment>() != null)
            {
                UML.Interactions.CombinedFragment combinedFragment = (UML.Interactions.CombinedFragment)dgElement.mofElement[0];
                mofOperand.fragment.AddLast(combinedFragment);
            }
        }

        // guard
        mofOperand.guard = (UML.Interactions.InteractionConstraint)mofConstraint;

        // covered
        mofOperand.covered.Clear();
        foreach (DG.UML.LifeLine dgLifeLine in dgOperand.GetLifeLinesRec())
        {
            mofOperand.covered.Add((global::UML.Interactions.Lifeline)dgLifeLine.mofElement[0]);
        }


        // generalOrdering
        // SKIP

        // enclosingInteraction
        mofOperand.enclosingInteraction = (UML.Interactions.Interaction)dgOperand.GetSequenceDiagram().mofElement[0];

        // enclosingOperand
        mofOperand.enclosingOperand = null;

        // name
        mofOperand.name = "";

        // owner
        mofOperand.owner = (UML.CommonStructure.Element)dgOperand.GetParent().mofElement[0];

        // ownedElement 
        mofOperand.ownedElement.Clear();
        mofOperand.ownedElement.AddRange(mofOperand.fragment);

        // ownedComment
        mofOperand.ownedComment.Clear();


        // INTERACTION CONSTRAINT


        // minint
        // SKIP

        // maxint
        // SKIP

        // constrainedElement


        // context
        // Skip

        // specification
        mofConstraint.specification = mofExpression;


        // OPAQUE EXPRESSION


        // body
        mofExpression.body = dgOperand.getGuardExpression();

        // language
        // SKIP

        // behavior
        // SKIP

        // result
        // SKIP



    }

public override void UpdateMofFromJson(BsonDocument json, XmiCollection container)
    {
        switch ((string)json.GetValue("XmiType"))
        {
            case "uml:InteractionOperand":
                UML.Interactions.InteractionOperand operand = (UML.Interactions.InteractionOperand)container.GetMofElement((string)json.GetValue("XmiId"));
                
                operand.covered.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "covered"))
                    operand.covered.Add((UML.Interactions.Lifeline)mof);
                
                operand.diElement.Clear();
                foreach (DiElement di in JsonConvertor.FindDiArrayByXmiId(json, container, "diElement"))
                    operand.diElement.Add((DiElement)di);

                operand.enclosingInteraction = (UML.Interactions.Interaction)JsonConvertor.FindMofByXmiId(json, container, "enclosingInteraction");

                operand.enclosingOperand = (UML.Interactions.InteractionOperand)JsonConvertor.FindMofByXmiId(json, container, "enclosingOperand");

                operand.fragment.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "fragment"))
                    operand.fragment.AddLast((UML.Interactions.InteractionFragment)mof);

                operand.generalOrdering.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "generalOrdering"))
                    operand.generalOrdering.Add((UML.Interactions.GeneralOrdering)mof);

                operand.guard = (UML.Interactions.InteractionConstraint)JsonConvertor.FindMofByXmiId(json, container, "guard");

                operand.name = json.GetValue("name").ToString();

                operand.ownedComment.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedComment"))
                    operand.ownedComment.Add((UML.CommonStructure.Comment)mof);

                operand.ownedElement.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedElement"))
                    operand.ownedElement.Add((UML.CommonStructure.Element)mof);

                operand.owner = (UML.CommonStructure.Element)JsonConvertor.FindMofByXmiId(json, container, "owner");

                break;
            case "uml:InteractionConstraint":
                UML.Interactions.InteractionConstraint interactionConstraint = (UML.Interactions.InteractionConstraint)container.GetMofElement((string)json.GetValue("XmiId"));
                
                interactionConstraint.constrainedElement.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "constrainedElement"))
                    interactionConstraint.constrainedElement.AddLast((UML.CommonStructure.Element)mof);

                interactionConstraint.diElement.Clear();
                foreach (DiElement di in JsonConvertor.FindDiArrayByXmiId(json, container, "diElement"))
                    interactionConstraint.diElement.Add((DiElement)di);

                interactionConstraint.name = json.GetValue("name").ToString();

                interactionConstraint.ownedComment.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedComment"))
                    interactionConstraint.ownedComment.Add((UML.CommonStructure.Comment)mof);

                interactionConstraint.ownedElement.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedElement"))
                    interactionConstraint.ownedElement.Add((UML.CommonStructure.Element)mof);

                interactionConstraint.owner = (UML.CommonStructure.Element)JsonConvertor.FindMofByXmiId(json, container, "owner");

                interactionConstraint.specification = (UML.Values.ValueSpecification)JsonConvertor.FindMofByXmiId(json, container, "specification");

                break;
            case "uml:OpaqueExpression":
                UML.Values.OpaqueExpression opaqueExpression = (UML.Values.OpaqueExpression)container.GetMofElement((string)json.GetValue("XmiId"));

                opaqueExpression.body = json.GetValue("body").ToString();

                opaqueExpression.diElement.Clear();
                foreach (DiElement di in JsonConvertor.FindDiArrayByXmiId(json, container, "diElement"))
                    opaqueExpression.diElement.Add((DiElement)di);

                opaqueExpression.language = json.GetValue("language").ToString();

                opaqueExpression.name = json.GetValue("name").ToString();

                opaqueExpression.ownedComment.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedComment"))
                    opaqueExpression.ownedComment.Add((UML.CommonStructure.Comment)mof);

                opaqueExpression.ownedElement.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedElement"))
                    opaqueExpression.ownedElement.Add((UML.CommonStructure.Element)mof);

                opaqueExpression.owner = (UML.CommonStructure.Element)JsonConvertor.FindMofByXmiId(json, container, "owner");

                break;
            default:
                Debug.Log("Invalid ELement! " + (string)json.GetValue("XmiType"));
                break;
        }
        //return result;
    }
}
