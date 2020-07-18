/*
 * Copyright (c) 2016, Ivo van der Marel
 * Released under MIT License (= free to be used for anything)
 * Enjoy :)
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGame.Selection
{
    public class RTSSelection : MonoBehaviour
    {
        public static readonly List<Selectable> Selectables = new List<Selectable>();

        public Canvas canvas;
        public Image selectionBox;
        private KeyCode selectionToggleKey = KeyCode.LeftShift;

        private class Hotkey
        {
            public KeyCode keyCode;
            public List<Selectable> selectables;

            public Hotkey(KeyCode key, List<Selectable> selectables)
            {
                this.keyCode = key;
                this.selectables = selectables;
            }
        }

        private Hotkey[] _armyHotkeys;
        //private KeyCode selectAllArmyKey = KeyCode.Alpha1;

        public static readonly float DoubleClickCooldown = 0.5f;
        private float _buttonCooler;

        private KeyCode _lastButton;
        //private int _buttonCount;

        public Camera minigameCamera;
        public CameraController cameraController;
        private Vector3 _startScreenPos;
        private BoxCollider _worldCollider;
        private RectTransform _rt;
        private bool _isSelecting;

        void Awake()
        {
            if (canvas == null)
                canvas = FindObjectOfType<Canvas>();

            if (selectionBox != null)
            {
                //We need to reset anchors and pivot to ensure proper positioning
                _rt = selectionBox.GetComponent<RectTransform>();
                _rt.pivot = Vector2.one * .5f;
                _rt.anchorMin = Vector2.one * .5f;
                _rt.anchorMax = Vector2.one * .5f;
                selectionBox.gameObject.SetActive(false);
            }

            cameraController = minigameCamera.gameObject.GetComponent<CameraController>();
            InitializeHotkeys();
        }

        void Update()
        {
            HandleClicks();
            HandleHotkeys();
        }

        private void HandleClicks()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray mouseToWorldRay = minigameCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                //Shoots a ray into the 3D world starting at our mouseposition
                if (Physics.Raycast(mouseToWorldRay, out hitInfo))
                {
                    //We check if we clicked on an object with a Selectable component
                    Selectable s = hitInfo.collider.GetComponentInParent<Selectable>();
                    if (s != null)
                    {
                        //While holding the copyKey, we can add and remove objects from our selection
                        //print(Input.GetKey(KeyCode.LeftShift));
                        if (Input.GetKey(selectionToggleKey))
                        {
                            //Toggle the selection
                            UpdateSelection(s, !s.IsSelected);
                        }
                        else
                        {
                            //If the copyKey was not held, we clear our current selection and select only this unit
                            ClearSelected();
                            UpdateSelection(s, true);
                        }

                        //If we clicked on a Selectable, we don't want to enable our SelectionBox
                        return;
                    }
                }

                if (selectionBox == null)
                    return;
                //Storing these variables for the selectionBox
                _startScreenPos = Input.mousePosition;
                _isSelecting = true;
            }

            //If we never set the selectionBox variable in the inspector, we are simply not able to drag the selectionBox to easily select multiple objects. 'Regular' selection should still work
            if (selectionBox == null)
                return;

            //We finished our selection box when the key is released
            if (Input.GetMouseButtonUp(0))
            {
                _isSelecting = false;
            }

            selectionBox.gameObject.SetActive(_isSelecting);

            if (_isSelecting)
            {
                Bounds b = new Bounds();
                //The center of the bounds is inbetween startpos and current pos
                b.center = Vector3.Lerp(_startScreenPos, Input.mousePosition, 0.5f);
                //We make the size absolute (negative bounds don't contain anything)
                b.size = new Vector3(Mathf.Abs(_startScreenPos.x - Input.mousePosition.x),
                    Mathf.Abs(_startScreenPos.y - Input.mousePosition.y),
                    0);

                //To display our selectionbox image in the same place as our bounds
                _rt.position = b.center;
                _rt.sizeDelta = canvas.transform.InverseTransformVector(b.size);

                //Looping through all the selectables in our world (automatically added/removed through the Selectable OnEnable/OnDisable)
                foreach (Selectable selectable in Selectables)
                {
                    //If the screenPosition of the worldobject is within our selection bounds, we can add it to our selection
                    Vector3 screenPos = minigameCamera.WorldToScreenPoint(selectable.transform.position);
                    screenPos.z = 0;
                    UpdateSelection(selectable, (b.Contains(screenPos)));
                }
            }
        }

        private void InitializeHotkeys()
        {
            KeyCode[] hotkeyOptions = {KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5};
            _armyHotkeys = new Hotkey[hotkeyOptions.Length];
            for (int i = 0; i < hotkeyOptions.Length; i++)
            {
                _armyHotkeys[i] = new Hotkey(hotkeyOptions[i], new List<Selectable>());
            }
        }

        private void HandleHotkeys()
        {
            if (_armyHotkeys[0].selectables.Count == 0)
            {
                foreach (Selectable s in Selectables)
                {
                    _armyHotkeys[0].selectables.Add(s);
                }
            }

            foreach (Hotkey hotkey in _armyHotkeys)
            {
                if (Input.GetKeyDown(hotkey.keyCode))
                {
                    //print(hotkey.keyCode + ": " + hotkey.selectables.Count);
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        hotkey.selectables.Clear();
                        foreach (Selectable s in Selectables)
                        {
                            if (s.IsSelected)
                            {
                                hotkey.selectables.Add(s);
                            }
                        }
                    }
                    else
                    {
                        foreach (Selectable s in Selectables)
                        {
                            s.IsSelected = false;
                        }

                        foreach (Selectable s in hotkey.selectables)
                        {
                            s.IsSelected = true;
                        }

                        if (_lastButton == hotkey.keyCode)
                        {
                            print("double click " + _lastButton);
                            MoveCameraToArmy(hotkey);
                        }

                        _lastButton = hotkey.keyCode;
                        _buttonCooler = DoubleClickCooldown;
                    }
                }
            }

            if (_buttonCooler > 0)
            {
                _buttonCooler -= 1 * Time.deltaTime;
            }
            else
            {
                //_buttonCount = 0;
                _lastButton = KeyCode.None;
            }
        }

        private void MoveCameraToArmy(Hotkey hotkey)
        {
            if (hotkey.selectables.Count == 0)
            {
                return;
            }

            Vector3 soldierPosition = hotkey.selectables[0].gameObject.transform.position;
            cameraController.GoTo(soldierPosition.x, soldierPosition.z - 10);
        }

        /// <summary>
        /// Add or remove a Selectable from our selection
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        void UpdateSelection(Selectable s, bool value)
        {
            if (s.IsSelected != value)
                s.IsSelected = value;
        }

        /// <summary>
        /// Returns all Selectable objects with isSelected set to true
        /// </summary>
        /// <returns></returns>
        private List<Selectable> GetSelected()
        {
            return new List<Selectable>(Selectables.Where(x => x.IsSelected));
        }

        /// <summary>
        /// Clears the full selection
        /// </summary>
        void ClearSelected()
        {
            Selectables.ForEach(x => x.IsSelected = false);
        }
    }
}