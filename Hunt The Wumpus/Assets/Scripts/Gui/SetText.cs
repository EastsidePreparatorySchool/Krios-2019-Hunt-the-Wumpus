using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetText : MonoBehaviour
{
    public string[] options;
   
    public void SetIt(float findex)
    {
        int index = (int)findex;
        GetComponent<Text>().text = options[index];
    }
}
