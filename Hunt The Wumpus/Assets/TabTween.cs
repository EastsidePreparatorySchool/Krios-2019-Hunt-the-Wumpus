using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabTween : MonoBehaviour
{
    public LeanTweenType ease;
    public float distance;
    public float speed;

    public void TabChange(bool firstTab)
    {
        if (firstTab)
            LeanTween.moveLocalX(gameObject, -distance, speed).setEase(ease);
        else
            LeanTween.moveLocalX(gameObject, distance, speed).setEase(ease);
    }
}
