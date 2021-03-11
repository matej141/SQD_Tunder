using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG.UML
{

    public class LifeLine : DgElement
    {

        public DG.UML.SequenceDiagram GetSequenceDiagram()
        {
            GameObject hl = HierarchyHelper.GetParent(gameObject);
            if (hl == null) return null;
            GameObject borders = HierarchyHelper.GetParent(hl);
            if (borders == null) return null;
            return HierarchyHelper.GetParent(borders).GetComponent<SequenceDiagram>();
        }

        public List<DG.UML.Message> GetMessages()
        {
            List<DG.UML.Message> messages = new List<DG.UML.Message>();
            DG.UML.SequenceDiagram sequenceDiagram = this.GetSequenceDiagram();

            List<DG.UML.Message> allMessages = sequenceDiagram.GetMessagesRec();
            foreach (DG.UML.Message message in allMessages)
            {
                DG.UML.LifeLine startDgLifeline = message.FromLifeLine.GetComponent<DG.UML.LifeLine>();
                DG.UML.LifeLine endDgLifeline = message.ToLifeLine.GetComponent<DG.UML.LifeLine>();

                if (this == startDgLifeline || this == endDgLifeline)
                {
                    messages.Add(message);
                }
            }

            return messages;
        }

        public Transform GetVL()
        {
            GameObject borders = HierarchyHelper.GetChildrenWithName(this.GetSequenceDiagram().gameObject, "Borders")[0].gameObject;
            return HierarchyHelper.GetChildrenWithName(borders, "VL")[0];
        }

        public Transform GetHL()
        {
            GameObject borders = HierarchyHelper.GetChildrenWithName(this.GetSequenceDiagram().gameObject, "Borders")[0].gameObject;
            return HierarchyHelper.GetChildrenWithName(borders, "HL")[0];
        }

        public DG.UML.SequenceDiagram GetParentDg()
        {
            return this.GetSequenceDiagram();
        }

        public DG.UML.SequenceDiagram GetRootDg()
        {
            return this.GetSequenceDiagram();
        }

        public string GetName() 
        {
            GameObject lifeline = gameObject;
            GameObject objectName = lifeline.transform.Find("Header").Find("ObjectName").GetChild(0).gameObject;
            string name = objectName.GetComponent<InputField>().text;
            return name;
        }

        public void SetName(string name)
        {
            GameObject lifeline = gameObject;
            GameObject objectName = lifeline.transform.Find("Header").Find("ObjectName").GetChild(0).gameObject;
            objectName.GetComponent<InputField>().text = name;
        }
        
    }

}
