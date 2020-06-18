using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetSpin : MonoBehaviour
{
    public float beginningSpin = 0; //500.0f;
    public float spinSlowFactor = 0.99f; //beginningSpin is multiplied by this every frame to slow it down

    public float minSpinSpeedBeforeZero = 10.0f; //how slow it needs to be spinning before it stops itself
    //public float beginningDistance = 30.0f;
    //public float targetDistance = 17.0f; //will be at this most of the game
    //public float zoomSpeed = 5.0f; //how fast it zooms in at the beginning of the game

    public float rotationSpeed = 80.0f; //player controlled
    public float distance = 17.0f;

    private float _horizontalInput;
    private float _verticalInput;

    private float _verticalTilt; //I don't want to deal with transform.eulerAngles so I made my own variable - Taras

    // Start is called before the first frame update
    void Start()
    {
        //distance = beginningDistance;
        //transform.position = new Vector3(0, 0, -distance);
    }

    // Update is called once per frame
    void Update()
    {
        // Get input from keyboard
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        UpdateSpin(); //for the fun animation at the beginning


        if (SceneManager.GetActiveScene().buildIndex == 0)
        {

            float vertDelta = Time.deltaTime * rotationSpeed * -_verticalInput;
            if (_verticalTilt > 50)
            {
                vertDelta = Mathf.Min(vertDelta, 0);
            }

            if (_verticalTilt < -50)
            {
                vertDelta = Mathf.Max(vertDelta, 0);
            }

            transform.Rotate(vertDelta,
                Time.deltaTime * rotationSpeed * _horizontalInput, 0, Space.World);
            _verticalTilt += vertDelta;


            // print(vertDelta + ", " + _verticalTilt);

        }
    }

    void UpdateSpin()
    {
        //transform.Translate(Vector3.forward * distance);
        //spin at the beginning
        transform.Rotate(Vector3.up, Time.deltaTime * beginningSpin, Space.World);
        if (beginningSpin > minSpinSpeedBeforeZero)
        {
            beginningSpin *= spinSlowFactor;
        }
        else
        {
            beginningSpin = 0;
        }

        /*//zoom to targetDistance
        if (distance > targetDistance)
        {
            distance -= zoomSpeed * Time.deltaTime;
        }

        if (distance < targetDistance)
        {
            distance = targetDistance; //in case you go too far
        }

        transform.Translate(Vector3.back * distance);*/
    }
}