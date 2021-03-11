using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.Linq;

using Data;
using Data.DI.UML;
using UML.Interactions;

namespace DG.UML
{

    public class Message : DgElement
    {

        [Space(20)]
        [Header("Primary priority :")]

        public RectTransform FromLifeLine = null;
        public RectTransform ToLifeLine = null;

        [Space(20)]
        [Header("Secondary priority :")]

        //public Vector2 FromPointAnchored; // When from lifeline is missing
        //public Vector2 ToPointAnchored;   // When  to  lifeline is missing
        public Vector3 FromWorldPosition;
        public Vector3 ToWorldPosition;

        [Space(20)]
        [Header("Graphics :")]

        public Color Color = new Color(0.25f, 0.31f, 0.57f, 1.00f);
        public bool Dashed = false;
        public float DashWidth = 5;

        private RectTransform MessageTransform = null;
        private RectTransform LineTransform = null;
        private RectTransform LabelTransform = null;
        private RectTransform ArrowTransform = null;


        public virtual Vector2 FromPoint
        {
            get
            {
                Vector3 v3 = (FromLifeLine == null ? FromWorldPosition : FromLifeLine.position);
                Vector2 v2 = PositionHelper.TransformWorldToLocalUiPosition(MessageTransform, v3);
                v2.x += (MessageTransform.sizeDelta.x * MessageTransform.pivot.x);
                return v2;
                /*
                if (FromLifeLine == null) return FromPointAnchored;
                return FromLifeLine.anchoredPosition - new Vector2(FromLifeLine.parent.GetComponent<HorizontalLayoutGroup>().padding.left, 0f);
                // Both codes return the same value.
                // But for some magical reasons, lower code does not work for newly created Messages where you are still moving its ends.
                //return PositionHelper.TransformWorldToLocalBottomLeftPosition((RectTransform)(transform), FromLifeLine.position);
                */
            }
        }

        public virtual Vector2 ToPoint
        {
            get
            {
                Vector3 v3 = (ToLifeLine == null ? ToWorldPosition : ToLifeLine.position);
                Vector2 v2 = PositionHelper.TransformWorldToLocalUiPosition(MessageTransform, v3);
                v2.x += (MessageTransform.sizeDelta.x * MessageTransform.pivot.x);
                return v2;
                /*
                if (ToLifeLine == null) return ToPointAnchored;
                return ToLifeLine.anchoredPosition - new Vector2(ToLifeLine.parent.GetComponent<HorizontalLayoutGroup>().padding.left, 0f);
                // Both codes return the same value.
                // But for some magical reasons, lower code does not work for newly created Messages where you are still moving its ends.
                //return PositionHelper.TransformWorldToLocalBottomLeftPosition((RectTransform)(transform), ToLifeLine.position);
                */
            }
        }

        protected virtual float Width
        {
            get { return (FromPoint == ToPoint ? 80f : Mathf.Abs(FromPoint.x - ToPoint.x)); }
        }

        protected virtual float Height
        {
            get { return (FromPoint == ToPoint ? 25f : Mathf.Abs(FromPoint.y - ToPoint.y)); }
        }

        public virtual bool Reversed
        {
            get { return (FromPoint.x > ToPoint.x) || FromPoint == ToPoint; }
        }

        protected virtual int Orientation
        {
            get { return Reversed ? 0 : 180; }
        }

        public virtual int Direction
        {
            get { return Reversed ? -1 : 1; }
        }

        protected virtual float StartPosX
        {
            get { return Reversed ? ToPoint.x : FromPoint.x; }
        }

        protected virtual List<Vector2> LinePoints(Vector2 start, Vector2 end, int chunks = 1)
        {
            Vector2 diff = end - start;
            Vector2 chunk = diff / ((float)chunks);
            Vector2 current = start;

            var points = new List<Vector2>();

            for (int i = 0; i <= chunks; i++, current += chunk)
            {
                points.Add(current);
            }

            // Always use even number of points
            if (points.Count % 2 == 1)
            {
                points.RemoveAt(points.Count - 1);
            }

            return points;
        }

        public Transform GetVL()
        {
            // TODO FIXME This returns layer VL, not parent VL
            GameObject borders = HierarchyHelper.GetChildrenWithName(this.GetSequenceDiagram().gameObject, "Borders")[0].gameObject;
            return HierarchyHelper.GetChildrenWithName(borders, "VL")[0];
        }

        public Transform GetHL()
        {
            GameObject borders = HierarchyHelper.GetChildrenWithName(this.GetSequenceDiagram().gameObject, "Borders")[0].gameObject;
            return HierarchyHelper.GetChildrenWithName(borders, "HL")[0];
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

        public string GetName()
        {
            GameObject message = gameObject;
            GameObject objectName = message.transform.Find("Label").Find("MethodName").GetChild(0).gameObject;
            string name = objectName.GetComponent<InputField>().text;
            return name;
        }

        public void Awake()
        {
            MessageTransform = GetComponent<RectTransform>();
            LineTransform = (RectTransform)transform.Find("Line");
            LabelTransform = (RectTransform)transform.Find("Label");
            ArrowTransform = (RectTransform)LineTransform.Find("Arrow");

            // Disable line end tools in the beginning
            Transform line = this.transform.GetChild(1);
            foreach (Transform child in line)
            {
                if (child.CompareTag ("Dot")) 
                {
                    child.gameObject.SetActive(false);
                }
            }
        }


        // Update is called once per frame
        void Update()
        {
            // Set colors
            UILineRenderer lineRenderer = LineTransform.GetComponent<UILineRenderer>();
            lineRenderer.color = Color;
            UIPolygon arrowPolygon = ArrowTransform.GetComponent<UIPolygon>();
            arrowPolygon.color = Color;

            // Set arrow geometry
            // Arrow anchor position is counted from bottom of line renderer
            Vector2 arrowSize = ArrowTransform.sizeDelta;
            ArrowTransform.anchoredPosition = new Vector2(Direction * (Width - arrowSize.x) * 0.5f, arrowSize.y * 0.5f);
            ArrowTransform.localEulerAngles = new Vector3(0, Orientation, 0);

            // Set line geometry
            // Line anchor position is counted from center of line renderer
            LayoutElement lineLayout = LineTransform.GetComponent<LayoutElement>();
            lineLayout.minHeight = arrowSize.y;
            lineLayout.preferredHeight = (FromPoint != ToPoint ? arrowSize.y : Height + arrowSize.y);
            LineTransform.sizeDelta = new Vector2(Width, lineLayout.preferredHeight);
            var points = new List<Vector2>();
            if (FromPoint == ToPoint)
            {
                // Self message
                points.AddRange(LinePoints(new Vector2(0f, Height * -0.5f), new Vector2(Width, Height * -0.5f)));
                points.AddRange(LinePoints(new Vector2(Width, Height * -0.5f), new Vector2(Width, Height * +0.5f)));
                points.AddRange(LinePoints(new Vector2(Width, Height * +0.5f), new Vector2(0f, Height * +0.5f)));
            }
            else
            {
                // Normal horizontal message
                points.AddRange(LinePoints(new Vector2(0f, 0f), new Vector2(Width, 0f)));
            }
            lineRenderer.Points = points.ToArray();

            // Set label geometry
            LabelTransform.sizeDelta = new Vector2(Width, 0f); // Height of label is resized by layouts

            // Set root message geometry
            GetComponent<VerticalLayoutGroup>().padding.left = (int)StartPosX;
        }

        public override void OnSelected(DiagramSelection selected)
        {
                Transform line = this.transform.GetChild(1);
                foreach (Transform child in line)
                {
                    if (child.CompareTag ("Dot")) 
                    {
                        child.gameObject.SetActive(true);

                    }
                }
        }

        public override void OnUnselected(DiagramSelection selected)
        {
                Transform line = this.transform.GetChild(1);
                foreach (Transform child in line)
                {
                    if (child.CompareTag ("Dot")) 
                    {
                        child.gameObject.SetActive(false);
                    }
                }
        }
    }
}
