using System;
using UnityEngine;

namespace MiniGame
{
    public class CameraController : MonoBehaviour
    {
        public float flySpeed = 15;
        public float minX = -2;
        public float maxX = 2;
        public float minZ = -1;
        public float maxZ = 3;
        public float minY = 6;
        public float maxY = 25;
        private float _zoom = 1;
        private float _originalY;

        // Start is called before the first frame update
        void Start()
        {
            _originalY = transform.position.y;
        }

        // Update is called once per frame
        void Update()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            float deltaX = horizontalInput * flySpeed * Time.deltaTime;
            float deltaZ = verticalInput * flySpeed * Time.deltaTime;

            float scrollWheelChange = Input.GetAxis("Mouse ScrollWheel");

            Transform myTrans;
            (myTrans = transform).Translate(new Vector3(deltaX, 0, deltaZ), Space.World);
            var position = myTrans.position;
            var posY = position.y + myTrans.forward.y * (scrollWheelChange * 15);
            if (posY < maxY && posY > minY)
            {
                position += myTrans.forward * (scrollWheelChange * 15);
                _zoom = 1 / (position.y / _originalY);
            }

            myTrans.position = position;

            Vector3 pos = myTrans.position;
            float curX = pos.x;
            float curY = pos.y;
            float curZ = pos.z;

            float adjMaxX = maxX + (maxX - minX) * _zoom / 4;
            float adjMinX = minX - (maxX - minX) * _zoom / 4;
            if (curX > adjMaxX)
            {
                curX = adjMaxX;
            }
            else if (curX < adjMinX)
            {
                curX = adjMinX;
            }

            if (curY > maxY)
            {
                curY = maxY;
            }
            else if (curY < minY)
            {
                curY = minY;
            }

            float adjMaxZ = maxZ + (maxZ - minZ) * _zoom / 10;
            float adjMinZ = minZ - (maxZ - minZ) * _zoom / 40;
            if (curZ > adjMaxZ)
            {
                curZ = adjMaxZ;
            }
            else if (curZ < adjMinZ)
            {
                curZ = adjMinZ;
            }


            /*
            if (curZ > maxZ)
            {
                curZ = maxZ;
            } else if (curZ < minZ)
            {
                curZ = minZ;
            }
            */
            myTrans.position = new Vector3(curX, curY, curZ);

            /*
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
            
            curX = transform.position.x; // in case it changed
            curZ = transform.position.z;
            curY = transform.position.y;
            if (curY > maxY)
            {
                myTrans.position = new Vector3(curX, maxY, curZ);
            }

            if (curY < minY)
            {
                myTrans.position = new Vector3(curX, minY, curZ);
            }
            */
        }

        public void GoTo(float x, float z)
        {
            LeanTween.move(gameObject, new Vector3(
                Math.Min(maxX, Math.Max(minX, x)),
                transform.position.y,
                Math.Min(maxZ, Math.Max(minZ, z))), 1f).setEase(LeanTweenType.easeInOutQuad);
        }
    }
}