using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticComponentUI : ListComponentUI
{
    private OutlinedText valueText;
    Statistic statistic;

    public StatisticComponentUI(Statistic statistic, Transform parent): base(ResourceManager.Instance.StatisticComponentUI, parent)
    {
        this.statistic = statistic;

        foreach (Transform t in ObjectTransform)
        {
            if (t.tag == "Name Field")
            {
                OutlinedText text = new OutlinedText(t.gameObject);
                text.SetText(statistic.Name);
            }
            else if (t.tag == "Value Field")
            {
                valueText = new OutlinedText(t.gameObject);
                valueText.SetText(statistic.Value.ToString());
            }
        }

        statistic.OnValueChanged += RefreshDisplayValue;
    }

    private void RefreshDisplayValue(int value)
    {
        valueText.SetText(value.ToString());
    }

    public override void Destroy()
    {
        statistic.OnValueChanged -= RefreshDisplayValue;
        base.Destroy();
    }
}
