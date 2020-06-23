using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace CommandView
{
    public class GameStarter : MonoBehaviour
    {
        public Planet planetHandler;
        public GameObject[] sceneObjects;
        public GameObject letters;
        public CanvasGroup mainMenu;

        public GameObject introVideo;
        public VideoPlayer introVideoPlayer;
        public AudioSource introMusicStart;
        public AudioSource introMusicLoop;
        public Animator postVideoCameraWhip;

        public Material titleTextOpaque;
        public Material titleTextFade;
        private Renderer[] _letterRenderers = new Renderer[4];
        private Color _opaqueColor;
        private Color _transparentColor;

        private readonly List<Vector3> _ogScales = new List<Vector3>();

        private bool _postVideo;
        private static readonly int IntroVideoFinished = Animator.StringToHash("IntroVideoComplete");

        private void Awake()
        {
            foreach (GameObject sceneObject in sceneObjects)
            {
                _ogScales.Add(sceneObject.transform.localScale);
                sceneObject.transform.localScale = Vector3.zero;
            }

            for (int i = 0; i < letters.transform.childCount; i++)
            {
                GameObject letter = letters.transform.GetChild(i).gameObject;
                Renderer letterRenderer = letter.GetComponent<Renderer>();
                _letterRenderers[i] = letterRenderer;
                letterRenderer.material = titleTextFade;
                Material letterMaterial = letterRenderer.material;

                Color letterColor = letterMaterial.color;
                _opaqueColor = letterColor;
                letterColor.a = 0f;
                _transparentColor = letterColor;
                letterMaterial.color = letterColor;
            }

            mainMenu.alpha = 0f;

            introVideoPlayer.Play();
            introMusicLoop.PlayDelayed(introMusicStart.clip.length);
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (!introVideoPlayer.isPlaying && !_postVideo)
            {
                for (int i = 0; i < sceneObjects.Length; i++)
                {
                    sceneObjects[i].transform.localScale = _ogScales[i];
                }

                introVideo.SetActive(false);
                postVideoCameraWhip.SetBool(IntroVideoFinished, true);
                _postVideo = true;
            }

            if (_postVideo && !_letterRenderers[0].material.color.Equals(_opaqueColor))
            {
                foreach (Renderer letterRenderer in _letterRenderers)
                {
                    letterRenderer.material.color =
                        Color.Lerp(letterRenderer.material.color, _opaqueColor, Time.deltaTime * 2);
                }
            }
            else if (_letterRenderers[0].material.color.Equals(_opaqueColor))
            {
                foreach (Renderer letterRenderer in _letterRenderers)
                {
                    letterRenderer.material = titleTextOpaque;
                }
            }
        }
    }
}