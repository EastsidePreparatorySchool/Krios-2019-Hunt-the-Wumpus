using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace CommandView
{
    public class GameStarter : MonoBehaviour
    {
        public GameObject[] SceneObjects;
        public GameObject Letters;
        public CanvasGroup MainMenu;

        public VideoPlayer IntroVideo;

        private List<Vector3> _ogScales = new List<Vector3>();

        private void Awake()
        {
            foreach (GameObject sceneObject in SceneObjects)
            {
                _ogScales.Add(sceneObject.transform.localScale);
                sceneObject.transform.localScale = Vector3.zero;
            }

            for (int i = 0; i < Letters.transform.childCount; i++)
            {
                GameObject letter = Letters.transform.GetChild(i).gameObject;
                print(letter.name);

                Material letterMaterial = letter.GetComponent<Renderer>().material;
                Color letterColor = letterMaterial.color;
                letterColor.a = 0f;
                letterMaterial.color = letterColor;
            }

            MainMenu.alpha = 0f;

            IntroVideo.Play();
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}