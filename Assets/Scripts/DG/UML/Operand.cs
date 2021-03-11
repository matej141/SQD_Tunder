using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG.UML
 {
    public class Operand : DgElement
    {
    
        public List<GameObject> GetChildren()
        {
            List<GameObject> children = new List<GameObject>();
            int childrenCount = this.gameObject.transform.childCount;
            for (int i = 0; i < childrenCount; i++)
            {
                if (this.gameObject.transform.GetChild(i).GetComponent<GameObject>() != null)
                {
                    children.Add((GameObject)(this.gameObject.transform.GetChild(i).GetComponent<GameObject>()));
                    Debug.Log("Operand children: " + this.gameObject.transform.GetChild(i).GetComponent<GameObject>());
                }
                else
                    Debug.Log("NO CHILDREN UNDER OBJECT!");
            }

            return children;
        }

        public global::DG.UML.Fragment GetParent()
        {
            GameObject operandArea = HierarchyHelper.GetParent(this.gameObject);
            if (operandArea == null) return null;

            GameObject fragment = HierarchyHelper.GetParent(operandArea);
            if (fragment == null) return null;

            return fragment.GetComponent<DG.UML.Fragment>();
        }


        public GameObject GetVnoreneDG()
        {
            List<Transform> vl = HierarchyHelper.GetChildrenWithName(this.gameObject, "VL");
            GameObject vlObject = vl[0].gameObject;
            if (vlObject == null) return null;

            List<Transform> fragment = HierarchyHelper.GetChildrenWithName(vlObject, "Fragment");
            if (fragment == null) return null;
            return fragment[0].gameObject;
        }

        public Transform GetVL()
        {
            return HierarchyHelper.GetChildrenWithName(this.gameObject, "VL")[0];
        }
        
        public SequenceDiagram GetSequenceDiagram() //TODO CYKLUS AK JE VNORENY OPERAND 
        {
            global::DG.UML.Fragment dgFragment = GetParent();
            return dgFragment.GetSequenceDiagram();
        }

        public string getGuardExpression()
        {
            GameObject guard = HierarchyHelper.GetChildrenWithName(this.gameObject, "Guard")[0].gameObject;
            GameObject expression = HierarchyHelper.GetChildrenWithName(guard, "Expression")[0].gameObject;
            GameObject inputField = HierarchyHelper.GetChildrenWithName(expression, "InputField")[0].gameObject;

            return inputField.GetComponent<InputField>().text;
        }

        public void setGuardExpression(string expressionString)
        {
            GameObject guard = HierarchyHelper.GetChildrenWithName(this.gameObject, "Guard")[0].gameObject;
            GameObject expression = HierarchyHelper.GetChildrenWithName(guard, "Expression")[0].gameObject;
            GameObject inputField = HierarchyHelper.GetChildrenWithName(expression, "InputField")[0].gameObject;

            inputField.GetComponent<InputField>().text = expressionString;

        }
        
        public List<global::DG.DgElement> GetVlChildren()
        {  
            GameObject vl = HierarchyHelper.GetChildrenWithName(this.gameObject, "VL")[0].gameObject;
            List<DG.DgElement> children = new List<DG.DgElement>();
            int childrenCount = vl.transform.childCount;
            for (int i = 0; i < childrenCount; i++)
            {
                if (vl.transform.GetChild(i).GetComponent<DG.UML.Message>() != null)
                {
                    children.Add((DG.DgElement)(vl.transform.GetChild(i).GetComponent<DG.UML.Message>()));
                }
                if (vl.transform.GetChild(i).GetComponent<DG.UML.Fragment>() != null)
                {
                    children.Add((DG.DgElement)(vl.transform.GetChild(i).GetComponent<DG.UML.Fragment>()));
                }

                
            }

            return children;
        }


        public List<global::DG.UML.Message> GetMessages()
        {
            List<DG.UML.Message> result = new List<DG.UML.Message>();
            GameObject vl = GetVL().gameObject;
            if (vl == null) return null;
            foreach (DG.DgElement VlChild in GetVlChildren())
            {
                if (VlChild.transform.GetComponent<DG.UML.Message>() != null)
                {
                    result.Add(VlChild.gameObject.GetComponent<DG.UML.Message>());
                }
            }
            return result;
        }

        public List<global::DG.UML.Message> GetMessagesRec()
        {
            List<global::DG.UML.Message> result = new List<DG.UML.Message>();
            GameObject vl = GetVL().gameObject;
            
            if (vl == null) return null;

            foreach (DG.DgElement VlChild in GetVlChildren())
            {
                if (VlChild.transform.GetComponent<DG.UML.Message>() != null)
                {
                    result.Add(VlChild.gameObject.GetComponent<DG.UML.Message>());
                    
                }
                
                    if (VlChild.transform.GetComponent<DG.UML.Fragment>() != null)
                    {
                    result.AddRange(VlChild.transform.GetComponent<DG.UML.Fragment>().GetMessagesRec());
                    
                }
            }
            return result;
        }
        
        public List<global::DG.UML.Fragment> GetFragments()
        {
            List<DG.UML.Fragment> result = new List<DG.UML.Fragment>();
            GameObject vl = GetVL().gameObject;
            if (vl == null) return null;

            foreach (DG.DgElement VlChild in GetVlChildren())
            {
                if (vl.transform.GetComponent<DG.UML.Fragment>() != null)
                {
                    result.Add(VlChild.gameObject.GetComponent<DG.UML.Fragment>());
                }
            }

            return result;
        }

        public List<global::DG.UML.Fragment> GetFragmentsRec()
        {
            List<DG.UML.Fragment> result = new List<DG.UML.Fragment>();
            GameObject vl = GetVL().gameObject;
            if (vl == null) return null;

            foreach (DG.DgElement VlChild in GetVlChildren())
            {
                if (vl.transform.GetComponent<DG.UML.Fragment>() != null)
                {
                    result.Add(VlChild.gameObject.GetComponent<DG.UML.Fragment>());
                    result.AddRange(vl.transform.GetComponent<DG.UML.Fragment>().GetFragmentsRec());
                }
            }
            return result;
        }

        public List<global::DG.UML.LifeLine> GetLifeLinesRec()
        {


            List<global::DG.UML.Message> messages = this.gameObject.GetComponent<DG.UML.Operand>().GetMessagesRec();
            HashSet<global::DG.UML.LifeLine> lifeLines = new HashSet<global::DG.UML.LifeLine>();
            foreach (global::DG.UML.Message dgMessage in messages)
            {
                global::DG.UML.LifeLine dgLifelineFrom = dgMessage.FromLifeLine.GetComponent<global::DG.UML.LifeLine>();
                global::DG.UML.LifeLine dgLifelineTo = dgMessage.ToLifeLine.GetComponent<global::DG.UML.LifeLine>();

                lifeLines.Add((global::DG.UML.LifeLine)dgLifelineFrom.mofElement[0].dgElement[0]);
                lifeLines.Add((global::DG.UML.LifeLine)dgLifelineTo.mofElement[0].dgElement[0]);
            }

            List<global::DG.UML.LifeLine> listLifeLines = new List<global::DG.UML.LifeLine> (lifeLines);
            return listLifeLines;

        }




    }
}