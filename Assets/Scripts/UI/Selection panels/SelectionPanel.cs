using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionPanel<T> : ComponentsListPanel<T> where T : ListComponentUI
{
    private ListComponentUI currentSelected;

    public SelectionPanel(Transform parent, Vector2 position) : base(parent, position)
    {

    }

    public SelectionPanel(GameObject instance) : base(instance) {

    }

    public override void InsertListComponent(T comp)
    {
        base.InsertListComponent(comp);
        comp.OnSelect += ChangeSelectedSelection;
    }

    public override void RemoveListComponent(T comp)
    {
        base.RemoveListComponent(comp);
        comp.OnSelect -= ChangeSelectedSelection;
    }

    public void ChangeSelectedSelection(ListComponentUI selection)
    {
        currentSelected?.Highlight(false);
        selection.Highlight(true);
        currentSelected = selection;
    }
}
