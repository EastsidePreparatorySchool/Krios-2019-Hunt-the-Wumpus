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
            gameObject.LeanMoveLocalX(-distance, speed).setEase(ease);
        else
            gameObject.LeanMoveLocalX(distance, speed).setEase(ease);
    }
}
