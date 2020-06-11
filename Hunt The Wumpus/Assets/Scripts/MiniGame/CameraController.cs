using UnityEngine;

namespace MiniGame
{
    public class CameraController : MonoBehaviour
    {
        public float flySpeed = 15;
        public float minX = -5;
        public float maxX = 5;
        public float minZ = -27;
        public float maxZ = 17;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            float deltaX = horizontalInput * flySpeed * Time.deltaTime;
            float deltaZ = verticalInput * flySpeed * Time.deltaTime;
            Transform myTrans;
            (myTrans = transform).Translate(new Vector3(deltaX, 0, deltaZ), Space.World);

            Vector3 pos = myTrans.position;
            float curX = pos.x;
            float curY = pos.y;
            float curZ = pos.z;

            if (curX > maxX)
            {
                myTrans.position = new Vector3(maxX, curY, curZ);
            }

            if (curX < minX)
            {
                myTrans.position = new Vector3(minX, curY, curZ);
            }

            curX = transform.position.x; // in case it changed
            if (curZ > maxZ)
            {
                myTrans.position = new Vector3(curX, curY, maxZ);
            }

            if (curZ < minZ)
            {
                myTrans.position = new Vector3(curX, curY, minZ);
            }
        }
    }
}