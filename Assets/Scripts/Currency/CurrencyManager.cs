using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyManager : MonoBehaviour
{
    private int totalCurrency;
    [SerializeField] private Text currencyText = null;

    private void Update()
    {
        //TODO fix only update ui when number updated
        currencyText.text = totalCurrency.ToString();
    }


    public void GainCurrency(int num)
    {
        totalCurrency += num;
    }

    public bool RemoveCurrency(int num)
    {
        if (num > totalCurrency)
            return false;
        totalCurrency -= num;
        return true;
    }

    public bool HaveCurrency(int num)
    {
        return (totalCurrency >= num);
    }

    public int GetCurrency()
    {
        return totalCurrency;
    }
}