using CommandView;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TileBattle
{
    // Simple Mini-game as placeholder to larger one
    public class OrthogonalMove : MonoBehaviour
    {
        public GameObject hintCanvas;

        public float speed = 10.0f;

        private float _horizontalInput;
        private float _verticalInput;

        private bool _inBattle = true;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            //input
            _horizontalInput = Input.GetAxis("Horizontal");
            _verticalInput = Input.GetAxis("Vertical");

            //move
            transform.Translate(Vector3.right * (Time.deltaTime * speed * _horizontalInput));
            transform.Translate(Vector3.forward * (Time.deltaTime * speed * _verticalInput));

            //exit scene
            Planet planet = GameObject.Find("Planet").GetComponent<Planet>();
            int faceInBattle = planet.GetFaceInBattle();
            if (_inBattle && transform.position.x > 10)
            {
                print("1");
                // Mark face as colonized
                planet.SetFaceConquestStatus(faceInBattle);
                print("2");
                // planet.ColonizeFace(faceInBattle);
                print("Face number: " + faceInBattle);
                GameObject inBattleFace = planet.faces[faceInBattle];

                FaceHandler inBattleFaceHandler = inBattleFace.GetComponent<FaceHandler>();
                print("3");
                inBattleFaceHandler.SetColonized();
                print("4");
                _inBattle = false;
                print("Battle Over, You Won! Showing hints");

                inBattleFace.GetComponent<FaceHandler>().SetColorToColonized();
                planet.SetFaceInBattle(-1);
                hintCanvas.SetActive(true);
            }
            else if (transform.position.z > 3) // "Loosing" the game
            {
                _inBattle = false;
                print("Battle Over, You Lost! Going back to Command View");

                if (planet.GetComponent<Wumpus.Wumpus>().location == faceInBattle)
                {
                    //TODO: move Wumpus twice
                }

                planet.SetFaceInBattle(-1);
                SceneManager.LoadScene(0);
            }
        }
    }
}