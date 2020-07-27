using UnityEngine;

namespace CommandView
{
    public class ButtonClickTween : MonoBehaviour
    {
        public GameObject circleToScale;
        public float clickSpeed = .2f;
        public float circleScaleAmount = 15;

        public GameObject toScale;
        public float hoverSpeed = .2f;
        public float buttonScaleAmount = 1.3f;

        public void OnClick()
        {
            circleToScale.transform.position = Input.mousePosition;
            circleToScale.LeanScaleX(circleScaleAmount, clickSpeed);
            circleToScale.LeanScaleY(circleScaleAmount, clickSpeed);
            //StartCoroutine(ResetScaleAfterDelay());
            circleToScale.LeanScaleX(0.001f, 0.01f).setDelay(clickSpeed + 0.05f);
            circleToScale.LeanScaleY(0.001f, 0.01f).setDelay(clickSpeed + 0.05f);
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
}