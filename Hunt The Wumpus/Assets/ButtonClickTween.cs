using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickTween : MonoBehaviour
{
    public GameObject circleToScale;
    public float clickSpeed = .2f;
    public float circleScaleAmount = 15;

    public GameObject toScale;
    public float hoverSpeed = .2f;
    public float buttonScaleAmount = 1.3f;

    IEnumerator ResetScaleAfterDelay()
    {
        yield return new WaitForSeconds(clickSpeed);
        circleToScale.LeanScaleX(0.001f, 0.01f);
        circleToScale.LeanScaleY(0.001f, 0.01f);
    }

    public void OnClick()
    {
        circleToScale.transform.position = Input.mousePosition;
        circleToScale.LeanScaleX(circleScaleAmount, clickSpeed);
        circleToScale.LeanScaleY(circleScaleAmount, clickSpeed);
        StartCoroutine(ResetScaleAfterDelay());
    }

    public void OnStartHover()
    {
        toScale.LeanScaleY(buttonScaleAmount, hoverSpeed);
        toScale.LeanScaleX(buttonScaleAmount, hoverSpeed);
    }

    public void OnEndHover()
    {
        toScale.LeanScaleY(1, hoverSpeed);
        toScale.LeanScaleX(1, hoverSpeed);
    }
}
