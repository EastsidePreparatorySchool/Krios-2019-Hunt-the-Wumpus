﻿/*
 * Copyright (c) 2016, Ivo van der Marel
 * Released under MIT License (= free to be used for anything)
 * Enjoy :)
 */

using UnityEngine;

namespace MiniGame
{
    public class Selectable : MonoBehaviour
    {
        private void Start()
        {
            this.IsSelected = false;
        }

        internal bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                //Replace this with your custom code. What do you want to happen to a Selectable when it get's (de)selected?
                Renderer r = GetComponentInChildren<Renderer>();
                if (r != null)
                    r.material.color = value ? Color.blue : Color.white;
            }
        }
 
        private bool _isSelected;
 
        void OnEnable()
        {
            RTSSelection.Selectables.Add(this);
        }
 
        void OnDisable()
        {
            RTSSelection.Selectables.Remove(this);
        }
 
    }
}