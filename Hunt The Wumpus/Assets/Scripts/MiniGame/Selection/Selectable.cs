/*
 * Copyright (c) 2016, Ivo van der Marel
 * Released under MIT License (= free to be used for anything)
 * Enjoy :)
 */

using UnityEngine;

namespace MiniGame.Selection
{
    public class Selectable : MonoBehaviour
    {
        private void Start()
        {
            this.IsSelected = false;
        }

        internal bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                //Replace this with your custom code. What do you want to happen to a Selectable when it get's (de)selected?
                //GameObject matParent = GameObject.Find("Soldier/Cube");
                Renderer r = GetComponentInChildren<SkinnedMeshRenderer>();
                if (r != null)
                    r.material.color = value ? Color.HSVToRGB(230f / 360, .3f, .6f) : new Color(.8f, .8f, .8f);
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