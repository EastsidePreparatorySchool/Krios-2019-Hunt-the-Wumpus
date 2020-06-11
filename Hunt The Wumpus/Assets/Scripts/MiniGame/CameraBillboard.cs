/*
 * Rotates an object towards the currently active camera
 * 
 * 1. Attach CameraBillboard component to a canvas or a game object
 * 2. Specify the offset and you're done
 *
 *credit to this github for the code
 * https://gist.github.com/ditzel/6ca74cd88765b98dfffebc2aafce667b
 * 
 **/

using UnityEngine;

namespace MiniGame
{
    public class CameraBillboard : MonoBehaviour
    {
        public Camera minigameCamera;
        
        public bool billboardX = true;
        public bool billboardY = true;
        public bool billboardZ = true;
        public float offsetToCamera;
        protected Vector3 LocalStartPosition;

        private Transform _myTrans;

        // Use this for initialization
        void Start()
        {
            LocalStartPosition = transform.localPosition;
            minigameCamera = Camera.current;
        }

        // Update is called once per frame
        void Update()
        {
            _myTrans = transform;

            if (minigameCamera == null) { return; }
            Quaternion mainCameraRot = minigameCamera.transform.rotation;
            _myTrans.LookAt(_myTrans.position + mainCameraRot * Vector3.forward,
                mainCameraRot * Vector3.up);
            if (!billboardX || !billboardY || !billboardZ)
            {
                Quaternion myRot = _myTrans.rotation;
                myRot = Quaternion.Euler(billboardX ? myRot.eulerAngles.x : 0f,
                    billboardY ? myRot.eulerAngles.y : 0f,
                    billboardZ ? myRot.eulerAngles.z : 0f);
                _myTrans.rotation = myRot;
            }

            _myTrans.localPosition = LocalStartPosition;
            _myTrans.position = _myTrans.position + _myTrans.rotation * Vector3.forward * offsetToCamera;
        }
    }
}