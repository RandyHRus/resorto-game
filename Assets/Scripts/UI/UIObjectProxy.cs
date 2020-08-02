using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIObjectProxy : UIObject
{
    //Used to create a UI object out of an already existing UI instance
    public UIObjectProxy(GameObject instance): base (instance)
    {

    }
}
