﻿using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class CursorController : MonoBehaviour
    {
        private Image _img;
        public Sprite[] cursors;

        // Start is called before the first frame update
        void Start()
        {
            Cursor.visible = false;
            _img = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = Input.mousePosition;
        }

        public void SetCursor(int index)
        {
            _img.sprite = cursors[index];
        }
    }
}