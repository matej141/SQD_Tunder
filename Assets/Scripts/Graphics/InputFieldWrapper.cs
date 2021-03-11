using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.

public class InputFieldWrapper : MonoBehaviour
{
    [Space(20)]
    [Header("InputField :")]

    public InputField MyInputField = null;
    public Text ChildPlaceholder = null;
    public Text ChildText = null;

    [Space(20)]
    [Header("Invisible parent wrapper :")]

    public Text ParentText = null;

    // Invoked when the value of the text field changes.
    public void ForwardValueToParentText(string newValue)
    {
        if (newValue.Length == 0)
        {
            ParentText.text = ChildPlaceholder.text;
            ParentText.font = ChildPlaceholder.font;
            ParentText.fontStyle = ChildPlaceholder.fontStyle;
            ParentText.fontSize = ChildPlaceholder.fontSize;
            ParentText.lineSpacing = ChildPlaceholder.lineSpacing;
            ParentText.supportRichText = ChildPlaceholder.supportRichText;
        }
        else
        {
            //parentText.text = childText.text; // Visible child text was not set by InputField at this time yet. It will probably be set after this method finishes.
            ParentText.text = newValue;
            ParentText.font = ChildText.font;
            ParentText.fontStyle = ChildText.fontStyle;
            ParentText.fontSize = ChildText.fontSize;
            ParentText.lineSpacing = ChildText.lineSpacing;
            ParentText.supportRichText = ChildText.supportRichText;
        }
    }
}