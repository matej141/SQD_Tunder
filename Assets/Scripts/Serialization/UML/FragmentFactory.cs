using System.Collections.Generic;
using Data;
using Data.DI;
using Data.MOF;
using UML.Interactions;
using DG;
using MongoDB.Bson;
using UnityEngine;

public class FragmentFactory : AbstractFactory
{
    public GameObject PrefabFragment = null;

    public override void RegisterFactory(FactoryRegister register)
    {
        register.RegisterXmiString("uml:CombinedFragment", this);
        register.RegisterClass(typeof(UML.Interactions.CombinedFragment), this);
        register.RegisterClass(typeof(DG.UML.Fragment), this);
    }


    public override List<DgElement> BuilDgFromMof(MofElement mof, XmiCollection container)
    {
        List<DG.DgElement> result = new List<DG.DgElement>();
        var mofFragment = (UML.Interactions.CombinedFragment)mof;

        GameObject go = GameObject.Instantiate(PrefabFragment);
        DG.UML.Fragment dg = go.GetComponent<DG.UML.Fragment>();
        result.Add(dg);

        // Map references

        dg.mofElement.Add(mofFragment);
        mofFragment.dgElement.Add(dg);

        return result;

    }

    public override void UpdateDgFromMof(DgElement dg, XmiCollection container)
    {
        UML.Interactions.CombinedFragment mof = (UML.Interactions.CombinedFragment)dg.mofElement[0];
        global::DG.UML.Fragment dgFragment = (global::DG.UML.Fragment)dg;

        // Set my name
        dgFragment.SetName(mof.name);
        
        // Operands
        Transform operandArea = HierarchyHelper.GetChildrenWithName(dg.gameObject, "OperandArea")[0];
        List<UML.Interactions.InteractionOperand> mofOperands = mof.operand;

        foreach (UML.Interactions.InteractionOperand mofOperand in mofOperands)
        {
            global::DG.UML.Operand dgOperand = (global::DG.UML.Operand)mofOperand.dgElement[0];
            dgOperand.gameObject.transform.position = operandArea.transform.position;
            dgOperand.gameObject.transform.SetParent(operandArea.transform);

        }

        // Operator 
        dgFragment.SetOperatorKind(mof.interactionOperator);
    }

    public override List<global::Data.MOF.MofElement> BuildMofFromJson(BsonDocument json, XmiCollection container)
    {
        List<global::Data.MOF.MofElement> result = new List<global::Data.MOF.MofElement>();

        switch ((string)json.GetValue("XmiType"))
        {
            case "uml:CombinedFragment":
                UML.Interactions.CombinedFragment mofFragment = new UML.Interactions.CombinedFragment();
                mofFragment.XmiId = (string)json.GetValue("XmiId");
                container.AddMofElement(mofFragment);
                result.Add(mofFragment);
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
        global::DG.UML.Fragment fragment = (global::DG.UML.Fragment)dg;
        
        UML.Interactions.CombinedFragment mofFragment = new UML.Interactions.CombinedFragment();
        
        mofFragment.generateNewXmiId();
        mofFragment.dgElement.Add(dg);
        dg.mofElement.Add(mofFragment);
        container.AddMofElement(mofFragment);
        result.Add(mofFragment);

        return result;
    }

    public override void UpdateMofFromDg(DgElement dg, XmiCollection container)
    {
        UML.Interactions.CombinedFragment mofFragment = (UML.Interactions.CombinedFragment)dg.mofElement[0];
        global::DG.UML.Fragment dgFragment = (global::DG.UML.Fragment)dg;

        global::DG.UML.SequenceDiagram dgDiagram = dgFragment.GetSequenceDiagram();
        UML.Interactions.Interaction mofDiagram = (UML.Interactions.Interaction)dgDiagram.mofElement[0];

        // InteractionOperatorKind
        mofFragment.interactionOperator =(UML.Interactions.InteractionOperatorKind)dgFragment.GetOperatorKind();
       
        


       // operand
        mofFragment.operand.Clear();
        foreach (global::DG.UML.Operand dgOperand in dgFragment.GetOperands())
        {
            UML.Interactions.InteractionOperand mofOperand = (InteractionOperand)(dgOperand.mofElement[0]);
            mofFragment.operand.Add(mofOperand);
        }

        // fragmentGate
        // SKIP

        // covered TODO spravit toto cez GetLifeLinesRec funkciu v fragmente
        mofFragment.covered.Clear();
        XmiCollection uniqueLifelines = new XmiCollection();
        foreach (global::DG.UML.Message dgMessage in dgFragment.GetMessagesRec())
        {
            uniqueLifelines.AddMofElement(dgMessage.FromLifeLine.GetComponent<global::DG.UML.LifeLine>().mofElement[0]);
            uniqueLifelines.AddMofElement(dgMessage.ToLifeLine.GetComponent<global::DG.UML.LifeLine>().mofElement[0]);
        }
        foreach (KeyValuePair<string, MofElement> element in uniqueLifelines.MofElements())
        {
            mofFragment.covered.Add((UML.Interactions.Lifeline)element.Value);
        }

        // generalOrdering
        // SKIP

        //name
        mofFragment.name = dgFragment.GetName();

        // owner;
        mofFragment.owner = (UML.CommonStructure.Element)dgFragment.GetParentDg().mofElement[0];

        // ownedElement
        mofFragment.ownedElement.Clear();
        foreach (UML.Interactions.InteractionOperand mofOperand in mofFragment.operand)
        {
            mofFragment.ownedElement.Add(mofOperand);
        }

        // ownedComment
        mofFragment.ownedComment.Clear();

        // enclosingInteraction
        mofFragment.enclosingInteraction = mofDiagram;

        // eclosingOperand
        if (mofFragment.owner.XmiType() == "uml:SequenceDiagram") 
        {
            mofFragment.enclosingOperand = null;
        }
        else if (mofFragment.owner.XmiType() == "uml:InteractionOperand")
        {
            mofFragment.enclosingOperand = (UML.Interactions.InteractionOperand)mofFragment.owner;
        }

    }

    public override void UpdateMofFromJson(BsonDocument json, XmiCollection container)
    {
        switch ((string)json.GetValue("XmiType"))
        {
            case "uml:CombinedFragment":
                UML.Interactions.CombinedFragment fragment = (UML.Interactions.CombinedFragment)container.GetMofElement((string)json.GetValue("XmiId"));

                fragment.cfragmentGate.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "cfragmentGate"))
                    fragment.cfragmentGate.Add((UML.Interactions.Gate)mof);

                fragment.covered.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "covered"))
                    fragment.covered.Add((UML.Interactions.Lifeline)mof);

                fragment.diElement.Clear();
                foreach (DiElement di in JsonConvertor.FindDiArrayByXmiId(json, container, "diElement"))
                    fragment.diElement.Add((DiElement)di);

                fragment.enclosingInteraction = (UML.Interactions.Interaction)JsonConvertor.FindMofByXmiId(json, container, "enclosingInteraction");

                fragment.enclosingOperand = (UML.Interactions.InteractionOperand)JsonConvertor.FindMofByXmiId(json, container, "enclosingOperand");

                fragment.generalOrdering.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "generalOrdering"))
                    fragment.generalOrdering.Add((UML.Interactions.GeneralOrdering)mof);

                fragment.interactionOperator = (UML.Interactions.InteractionOperatorKind)(json.GetValue("interactionOperator").AsInt32);

                fragment.name = json.GetValue("name").ToString();

                fragment.operand.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "operand"))
                    fragment.operand.Add((UML.Interactions.InteractionOperand)mof);

                fragment.ownedComment.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedComment"))
                    fragment.ownedComment.Add((UML.CommonStructure.Comment)mof);

                fragment.ownedElement.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedElement"))
                    fragment.ownedElement.Add((UML.CommonStructure.Element)mof);

                fragment.owner = (UML.CommonStructure.Element)JsonConvertor.FindMofByXmiId(json, container, "owner");

                break;
            default:
                Debug.Log("Invalid ELement! " + (string)json.GetValue("XmiType"));
                break;
        }
    }
    
}
