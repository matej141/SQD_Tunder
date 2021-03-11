using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG.UML
{
    public class SequenceDiagram : DgElement
    {

        public LayerRow LayerRow = null;
                
        public override void OnSelected(DiagramSelection previouslySelected)
        {
            // It should not be possible to select layer.
            Select(null);
        }

        public string GetName()
        {
            GameObject diagramName = this.transform.Find("NameWrapper").Find("Name").Find("InputField").gameObject;
            string name = diagramName.GetComponent<InputField>().text;
            return name;
        }

        public void SetName(string name)
        {
            GameObject diagramName = this.transform.Find("NameWrapper").Find("Name").Find("InputField").gameObject;
            diagramName.GetComponent<InputField>().text = name;
        }

        public Transform GetBorders()
        {
            return HierarchyHelper.GetChildrenWithName(this.gameObject, "Borders")[0];
        }

        public Transform GetVL()
        {
            GameObject borders = HierarchyHelper.GetChildrenWithName(this.gameObject, "Borders")[0].gameObject;
            return HierarchyHelper.GetChildrenWithName(borders, "VL")[0];
        }

        public Transform GetHL()
        {
            GameObject borders = HierarchyHelper.GetChildrenWithName(this.gameObject, "Borders")[0].gameObject;
            return HierarchyHelper.GetChildrenWithName(borders, "HL")[0];
        }

        public List<DG.UML.LifeLine> GetLifeLines() {
            GameObject borders = HierarchyHelper.GetChildrenWithName(this.gameObject, "Borders")[0].gameObject;
            GameObject hl = HierarchyHelper.GetChildrenWithName(borders, "HL")[0].gameObject;
            List<DG.UML.LifeLine> children = new List<LifeLine>();
            int childrenCount = hl.transform.childCount;
            for (int i = 0; i < childrenCount; i++)
            {
                children.Add((DG.UML.LifeLine)(hl.transform.GetChild(i).GetComponent<DG.UML.LifeLine>()));
            }
            return children;
        }

        private void GetMessagesRec(Fragment fragment, List<DG.UML.Message> messages)
        {
            Transform operandArea = fragment.gameObject.transform.Find("OperandArea");
            int count = operandArea.childCount;
            for (int i = 0; i < count; i++)
            {
                if (operandArea.GetChild(i).Find("VL") != null)
                {
                    int countChildren = operandArea.GetChild(i).Find("VL").childCount;
                    for (int j = 0; j < countChildren; j++)
                    {
                        if (operandArea.GetChild(i).Find("VL").GetChild(j).GetComponent<DG.UML.Message>() != null)
                        {
                            messages.Add((DG.UML.Message)(operandArea.GetChild(i).Find("VL").GetChild(j).GetComponent<DG.UML.Message>()));
                        }
                        if (operandArea.GetChild(i).Find("VL").GetChild(j).GetComponent<DG.UML.Fragment>() != null)
                        {
                            GetMessagesRec(operandArea.GetChild(i).Find("VL").GetChild(j).GetComponent<DG.UML.Fragment>(), messages);
                        }
                    }
                }
            }
        }

        public List<DG.UML.Message> GetMessages()
        {
            GameObject borders = HierarchyHelper.GetChildrenWithName(this.gameObject, "Borders")[0].gameObject;
            GameObject vl = HierarchyHelper.GetChildrenWithName(borders, "VL")[0].gameObject;
            List<DG.UML.Message> children = new List<DG.UML.Message>();
            int childrenCount = vl.transform.childCount;
            for (int i = 0; i < childrenCount; i++)
            {
                if (vl.transform.GetChild(i).GetComponent<DG.UML.Message>() != null)
                    children.Add((DG.UML.Message)(vl.transform.GetChild(i).GetComponent<DG.UML.Message>()));
                else
                    Debug.Log("NOT A MESSAGE INSIDE VL OF LAYER!");
            }
            return children;
        }

        public List<DG.UML.Message> GetMessagesRec()
        { 
            GameObject borders = HierarchyHelper.GetChildrenWithName(this.gameObject, "Borders")[0].gameObject;
            GameObject vl = HierarchyHelper.GetChildrenWithName(borders, "VL")[0].gameObject;
            List<DG.UML.Message> children = new List<DG.UML.Message>();
            int childrenCount = vl.transform.childCount;
            for (int i = 0; i < childrenCount; i++)
            {
                if (vl.transform.GetChild(i).GetComponent<DG.UML.Message>() != null)
                {
                    children.Add((DG.UML.Message)(vl.transform.GetChild(i).GetComponent<DG.UML.Message>()));
                }
                if (vl.transform.GetChild(i).GetComponent<DG.UML.Fragment>() != null)
                {
                    GetMessagesRec(vl.transform.GetChild(i).GetComponent<DG.UML.Fragment>(), children);
                }
            }
            return children;
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
        public List<DG.DgElement> GetVlChildren()
        {
            GameObject borders = HierarchyHelper.GetChildrenWithName(this.gameObject, "Borders")[0].gameObject;
            GameObject vl = HierarchyHelper.GetChildrenWithName(borders, "VL")[0].gameObject;
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

        public void Update()
        {
            RectTransform hl = (RectTransform)GetHL();
            RectTransform vl = (RectTransform)GetVL();

            // Resize Message objects to full width

            if (hl.hasChanged)
            {
                foreach (global::DG.UML.Message message in GetMessagesRec())
                {
                    LayoutElement messageLayout = message.GetComponent<LayoutElement>();
                    HorizontalLayoutGroup diagramHl = hl.GetComponent<HorizontalLayoutGroup>();
                    float newWidth = hl.sizeDelta.x - diagramHl.padding.left - diagramHl.padding.right;
                    messageLayout.minWidth = newWidth;
                    messageLayout.preferredWidth = newWidth;
                }
            }

            // Resize Borders object according to its children
            // Borders have to be explicitly wrapped, because ContentSizeFitter is not working for some reason.

            if  (hl.hasChanged || vl.hasChanged)
            {
                LayoutElement borderLayout = HierarchyHelper.GetChildrenWithName(gameObject, "Borders")[0].GetComponent<LayoutElement>();
                Vector2 childSize;
                float maxWidth = borderLayout.minWidth;
                float maxHeight = borderLayout.minHeight;

                childSize = hl.sizeDelta;
                if (childSize.x > maxWidth) maxWidth = childSize.x;
                if (childSize.y > maxHeight) maxHeight = childSize.y;

                childSize = vl.sizeDelta;
                if (childSize.x > maxWidth) maxWidth = childSize.x;
                if (childSize.y > maxHeight) maxHeight = childSize.y;

                borderLayout.preferredWidth = maxWidth;
                borderLayout.preferredHeight = maxHeight;
            }
        }
    }
}
