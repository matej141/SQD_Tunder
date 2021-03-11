using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public string Title = "DropDownMenu";
    public Text Label = null;
    public Dropdown Dropdown = null;

    public virtual void Reset()
    {
        Label = HierarchyHelper.GetChildrenWithName(gameObject, "Label")[0].GetComponent<Text>();
        Dropdown = GetComponent<Dropdown>();
    }

    public virtual void Update()
    {
        // Unselect current option by selecting last option which is a placeholder
        // This is needed because selected option can not be selected again
        // and we want it to be selectable infinite times sequentially
        Dropdown.value = Dropdown.options.Count - 1;

        // Force text change
        Label.text = Title;
    }
}