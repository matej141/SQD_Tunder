using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DG.UML
{
    public class Fragment : DgElement
    {
            public GameObject prefabOperand;

            // Start is called before the first frame update
            public override void Start()
            {
                base.Start();
                GameObject operatorWrapper = HierarchyHelper.GetChildrenWithName(gameObject, "OperatorWrapper")[0].gameObject;
                Button fragmentButton = HierarchyHelper.GetChildrenWithName(operatorWrapper, "AddButton")[0].GetComponent<Button>();
                fragmentButton.onClick.AddListener(delegate { AddNewOperand(); });
            }

            // Update is called once per frame
            void Update()
            {

            }

            public List<global::DG.UML.Operand> GetOperands()
            { 
                GameObject operandArea = HierarchyHelper.GetChildrenWithName(this.gameObject, "OperandArea")[0].gameObject;
                // GameObject vl = HierarchyHelper.getChildrenWithName(borders, "VL")[0].gameObject;
                List<DG.UML.Operand> children = new List<DG.UML.Operand>();
                int childrenCount = operandArea.transform.childCount;
                for (int i = 0; i < childrenCount; i++)
                {
                    if (operandArea.transform.GetChild(i).GetComponent<DG.UML.Operand>() != null)
                    {
                        children.Add((DG.UML.Operand)(operandArea.transform.GetChild(i).GetComponent<DG.UML.Operand>()));
                        Debug.Log("Fragment children: " + operandArea.transform.GetChild(i).GetComponent<DG.UML.Operand>());
                    }
                    else
                        Debug.Log("NOT A MESSAGE INSIDE VL OF LAYER!");
                }
           
                return children;
            
            }

        public string GetName() 
        {
            GameObject operatorWrapper = HierarchyHelper.GetChildrenWithName(this.gameObject, "OperatorWrapper")[0].gameObject;
            GameObject fragmentName = HierarchyHelper.GetChildrenWithName(operatorWrapper, "FragmentName")[0].gameObject;
            GameObject inputField = HierarchyHelper.GetChildrenWithName(fragmentName, "InputField")[0].gameObject;
            return inputField.GetComponent<InputField>().text;
        }

        public void SetName(string name) 
        {
            GameObject operatorWrapper = HierarchyHelper.GetChildrenWithName(this.gameObject, "OperatorWrapper")[0].gameObject;
            GameObject fragmentName = HierarchyHelper.GetChildrenWithName(operatorWrapper, "FragmentName")[0].gameObject;
            GameObject inputField = HierarchyHelper.GetChildrenWithName(fragmentName, "InputField")[0].gameObject;
            inputField.GetComponent<InputField>().text = name;
        }


        public global::UML.Interactions.InteractionOperatorKind GetOperatorKind() {
            GameObject operatorWrapper = HierarchyHelper.GetChildrenWithName(this.gameObject, "OperatorWrapper")[0].gameObject;
            GameObject operator_ = HierarchyHelper.GetChildrenWithName(operatorWrapper, "Operator")[0].gameObject;
            //GameObject label = HierarchyHelper.GetChildrenWithName(operator_, "Label")[0].gameObject;
            return (global::UML.Interactions.InteractionOperatorKind)operator_.GetComponent<Dropdown>().value;
            //return label.GetComponent<Text>().text;
        }

        public void SetOperatorKind(global::UML.Interactions.InteractionOperatorKind setThis)
        {
            GameObject operatorWrapper = HierarchyHelper.GetChildrenWithName(this.gameObject, "OperatorWrapper")[0].gameObject;
            GameObject operator_ = HierarchyHelper.GetChildrenWithName(operatorWrapper, "Operator")[0].gameObject;
            
            operator_.GetComponent<Dropdown>().value = (int)setThis;
        }

        public global::DG.DgElement GetParentDg()
        {
            GameObject vl = HierarchyHelper.GetParent(this.gameObject);
            if (vl == null || vl.name != "VL") return null;
            GameObject vlParent = HierarchyHelper.GetParent(vl);
            if (vlParent == null) return null;
            if (vlParent.tag == "Operand")
            {
                return vlParent.GetComponent<global::DG.UML.Operand>();
            }
            else if (vlParent.name == "Borders")
            {
                return HierarchyHelper.GetParent(vlParent).GetComponent<SequenceDiagram>();
            }
            Debug.Log("Unknown parent of Message: " + vlParent.name);
            return null;
        }

        public SequenceDiagram GetSequenceDiagram()
        {
            global::DG.DgElement myParent = GetParentDg();
            if ((myParent.GetComponent<global::DG.UML.SequenceDiagram>()) != null)
            {
                return myParent.GetComponent<global::DG.UML.SequenceDiagram>();
            }
            else if (myParent.GetComponent<global::DG.UML.Operand>() != null)
            {
                return myParent.GetComponent<global::DG.UML.Operand>().GetSequenceDiagram();
            }
            Debug.Log("Unknown parent of Message: " + myParent.GetType());
            return null;
        }
            
        public void AddNewOperand(bool select = true)
        {
            //Debug.Log("Adding operand.");
            GameObject operatorWrapper = HierarchyHelper.GetParent(gameObject);
            GameObject fragment = HierarchyHelper.GetParent(operatorWrapper);
            GameObject operandArea = HierarchyHelper.GetChildrenWithName(gameObject, "OperandArea")[0].gameObject;
            GameObject newOperand = GameObject.Instantiate(prefabOperand);
            newOperand.transform.position = transform.position;
            newOperand.transform.SetParent(operandArea.transform);
            // TODO Force update layouts
            newOperand.GetComponent<global::DG.UML.Operand>().LastExplicitPosition = newOperand.transform.position;


            if (select)
            {
                GameObject guard = HierarchyHelper.GetChildrenWithName(newOperand, "Guard")[0].gameObject;
                GameObject expression = HierarchyHelper.GetChildrenWithName(guard, "Expression")[0].gameObject;
                GameObject inputField = HierarchyHelper.GetChildrenWithName(expression, "InputField")[0].gameObject;
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(inputField);
                DiagramInputHandler.ChooseTool(null);
                DiagramSelection.Select(newOperand.GetComponent<DiagramSelection>());
            }
        }


        public List<DG.UML.Message> GetMessagesRec()
        {
            List<DG.UML.Message> result = new List<DG.UML.Message>();
            List<DG.UML.Operand> operands = new List<DG.UML.Operand>();
            operands = GetOperands();

            foreach (global::DG.UML.Operand operand in operands)
            {
                result.AddRange( operand.GetMessagesRec());
            }
            
            return result;
        }

        public List<global::DG.UML.Fragment> GetFragmentsRec()
        {
            List<DG.UML.Fragment> result = new List<DG.UML.Fragment>();
            List<DG.UML.Operand> operands = new List<DG.UML.Operand>();
            operands = GetOperands();

            foreach (DG.UML.Operand operand in operands)
            {
                result.AddRange(operand.GetFragmentsRec());
            }

            return result;
        }
    }
}
