using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSlide : MonoBehaviour
{
    public float distance;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void SetToggle(bool isOn)
    {
        if (isOn)
            LeanTween.moveLocalX(gameObject, distance, speed);
        else
            LeanTween.moveLocalX(gameObject, -distance, speed);
    }
}
