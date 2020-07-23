using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTween : MonoBehaviour
{
    public LeanTweenType ease;
    public float distance;
    public float speed;

    public void SetToggle(bool isOn)
    {
        if (isOn)
            LeanTween.moveLocalX(gameObject, distance, speed).setEase(ease);
        else
            LeanTween.moveLocalX(gameObject, -distance, speed).setEase(ease);
    }
}
