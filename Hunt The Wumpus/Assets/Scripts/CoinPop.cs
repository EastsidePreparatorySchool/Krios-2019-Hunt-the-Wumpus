using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.rotate(gameObject, new Vector3(0, 360 * 5, 0), 1f);

        float initY = transform.position.y;

        var seq = LeanTween.sequence();
        seq.append(LeanTween.moveY(gameObject, initY + 5, 0.6f));
        seq.append(LeanTween.moveY(gameObject, initY, 0.4f));
        Destroy(gameObject, 1f);
    }
}