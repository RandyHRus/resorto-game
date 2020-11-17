using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionPanel<T> : ComponentsListPanel<T> where T : ListComponentUI
{
    private ListComponentUI currentSelected;

    //public SelectionPanel(Transform parent, Vector2 position) : base(parent, position)
    //{
    //
    //}

    public SelectionPanel(GameObject instance) : base(instance) {

    }


    public override void OnComponentDestroyed(UIObject comp)
    {
        base.OnComponentDestroyed((T)comp);

        if (comp == currentSelected)
            currentSelected = null;
    }

    public override void OnComponentSelectedHandler(ListComponentUI selection)
    {
        if (currentSelected is CollapsibleComponentUI oldCollapsible)
        {
            void OnOldCollapseEnd(CollapsibleComponentUI sender)
            {
                Coroutines.Instance.StartCoroutine(TryOpenCollapsible(selection));
                oldCollapsible.OnCollapseEnd -= OnOldCollapseEnd;
            }

            if (oldCollapsible.Collapsed)
            {
                Coroutines.Instance.StartCoroutine(TryOpenCollapsible(selection));
            }
            else
            {
                oldCollapsible.Collapse(true);

                if (selection != currentSelected)
                    oldCollapsible.OnCollapseEnd += OnOldCollapseEnd;
            }         
        }
        else if (selection is CollapsibleComponentUI newCollapsible)
        {
            newCollapsible.Collapse(false);
        }

        currentSelected?.Highlight(false);
        selection.Highlight(true);
        currentSelected = selection;
    }

    private IEnumerator TryOpenCollapsible(ListComponentUI selection)
    {
        yield return 0;

        if (selection is CollapsibleComponentUI compToOpenCollapsible)
        {
            compToOpenCollapsible.Collapse(false);
        }
    }
}
