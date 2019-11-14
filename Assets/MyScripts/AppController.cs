using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

//these are used for creating the multiplayer status of the game
using Photon.Pun;
using Photon.Realtime;
using Google.XR.ARCoreExtensions;

namespace com.Ale.Chess
{
    public class AppController : MonoBehaviour
    {
        public GameObject Board;
        public GameObject ResolvedPointPrefab;

        public ARReferencePointManager ReferencePointManager;
        public ARRaycastManager RaycastManager;
        public Text OutputText;
        public InputField InputField;

        public GameObject tileHighlightPrefab;
        private GameObject tileHighlight;

        //black pieces
        public GameObject blackPawn;
        public GameObject blackRook;
        public GameObject blackKnight;
        public GameObject blackBishop;
        public GameObject blackKing;
        public GameObject blackQueen;

        //white pieces
        public GameObject whitePawn;
        public GameObject whiteRook;
        public GameObject whiteKnight;
        public GameObject whiteBishop;
        public GameObject whiteKing;
        public GameObject whiteQueen;

        private enum AppMode
        {
            // Wait for user to tap screen to begin hosting a point. Master Client
            TouchToHostCloudReferencePoint,

            // Poll hosted point state until it is ready to use. Master Client
            WaitingForHostedReferencePoint,

            // Wait for user to tap screen to begin resolving the point. SimpleClient
            TouchToResolveCloudReferencePoint,

            // Poll resolving point state until it is ready to use. SimpleClient
            WaitingForResolvedReferencePoint,

            PlayState 
        }

        private Pose pose;
        private bool placed = false;

        private AppMode m_AppMode = AppMode.TouchToHostCloudReferencePoint;
        private ARCloudReferencePoint m_CloudReferencePoint;
        private string m_CloudReferenceId;

        // Start is called before the first frame update
        void Start()
        {
            tileHighlight = Instantiate(tileHighlightPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);
            tileHighlight.SetActive(false);
            InputField.onEndEdit.AddListener(OnInputEndEdit);
        }

        void Awake()
        {
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void InstantiateBoardPieces()
        {
            Vector3 boardPosition = this.Board.transform.position;
            float x = boardPosition.x;
            float y = boardPosition.y;
            float z = boardPosition.z;
            x += 0.25f;
            y -= 1f;
            z += 0.7f;
            AddPiece(whiteRook, x - 0.35f, y, z - 0.35f);
            AddPiece(whiteKnight, x - 0.25f, y, z - 0.35f);
            AddPiece(whiteBishop, x - 0.15f, y, z - 0.35f);
            AddPiece(whiteQueen, x - 0.05f, y, z - 0.35f);
            AddPiece(whiteKing, x + 0.05f, y, z - 0.35f);
            AddPiece(whiteBishop, x + 0.15f, y, z - 0.35f);
            AddPiece(whiteKnight, x + 0.25f, y, z - 0.35f);
            AddPiece(whiteRook, x + 0.35f, y, z - 0.35f);

            for (int i = -7; i <= 7; i+=2)
            {
                AddPiece(whitePawn, x + i*0.05f, y, z - 0.25f);
            }

            AddPiece(blackRook, x - 0.35f,y, z + 0.35f);
            AddPiece(blackKnight, x - 0.25f, y, z + 0.35f);
            AddPiece(blackBishop, x - 0.15f, y, z + 0.35f);
            AddPiece(blackQueen, x - 0.05f, y, z + 0.35f);
            AddPiece(blackKing, x + 0.05f, y, z + 0.35f);
            AddPiece(blackBishop, x + 0.15f, y, z + 0.35f);
            AddPiece(blackKnight, x + 0.25f, y, z + 0.35f);
            AddPiece(blackRook, x + 0.35f, y, z + 0.35f);

            for (int i = -7; i <= 7; i+=2)
            {
                AddPiece(blackPawn, x + i*0.05f, y, z + 0.25f);
            }

        }

        private void AddPiece(GameObject piece, float x, float y, float z)
        {
            
            Vector3 place = new Vector3(x, y, z);
            GameObject newPiece = PhotonNetwork.Instantiate(piece.name, place, Quaternion.identity);
            newPiece.transform.parent = gameObject.transform;
        }

         private void PlaceChessBoard()
        {
            OutputText.text = "Please touch the screen to place the chess board";

            if (Input.touchCount >= 1
                && Input.GetTouch(0).phase == TouchPhase.Began
                && !EventSystem.current.IsPointerOverGameObject(
                        Input.GetTouch(0).fingerId))
            {
                List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
                RaycastManager.Raycast(Input.GetTouch(0).position, hitResults);
                if (hitResults.Count > 0)
                {
                    Pose pose = hitResults[0].pose;
                    if (placed == false)
                    {
                        PhotonNetwork.Instantiate(this.Board.name, pose.position, Quaternion.identity);
                        InstantiateBoardPieces();
                        placed = true;
                        m_AppMode = AppMode.PlayState;
                    }
                    else
                    {
                        OutputText.text = "Chess Board already placed, cannot place another one";
                    }

                }
            }
        }
        private void TouchtoHostCloudReferencePoint()
        {
            OutputText.text = m_AppMode.ToString();

            if (Input.touchCount >= 1
                && Input.GetTouch(0).phase == TouchPhase.Began
                && !EventSystem.current.IsPointerOverGameObject(
                        Input.GetTouch(0).fingerId))
            {
                List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
                RaycastManager.Raycast(Input.GetTouch(0).position, hitResults);
                if (hitResults.Count > 0)
                {
                    pose = hitResults[0].pose;

                    // Create a reference point at the touch.
                    ARReferencePoint referencePoint =
                        ReferencePointManager.AddReferencePoint(
                            pose);

                    // Create Cloud Reference Point.
                    m_CloudReferencePoint =
                        ReferencePointManager.AddCloudReferencePoint(
                            referencePoint);
                    if (m_CloudReferencePoint == null)
                    {
                        OutputText.text = "Create Failed!";
                        return;
                    }

                    // Wait for the reference point to be ready.
                    m_AppMode = AppMode.WaitingForHostedReferencePoint;
                }
            }
        }

        private void WaitForHostedReferencePoint()
        {
            OutputText.text = m_AppMode.ToString();

            CloudReferenceState cloudReferenceState = m_CloudReferencePoint.cloudReferenceState;
            OutputText.text += " - " + cloudReferenceState.ToString();

            if (cloudReferenceState == CloudReferenceState.Success)
            {
                GameObject cloudAnchor = Instantiate(
                                             Board,
                                             Vector3.zero,
                                             Quaternion.identity);
                cloudAnchor.transform.SetParent(
                    m_CloudReferencePoint.transform, false);

                m_CloudReferenceId = m_CloudReferencePoint.cloudReferenceId;
                m_CloudReferencePoint = null;

                OutputText.text = m_CloudReferenceId;

                InstantiateBoardPieces();
                m_AppMode = AppMode.PlayState;
            }
            else
            {
                OutputText.text = cloudReferenceState.ToString();
            }
        }

        private void TouchToResolveCloudReferencePoint()
        {
            OutputText.text = m_CloudReferenceId;

            if (Input.touchCount >= 1
                && Input.GetTouch(0).phase == TouchPhase.Began
                && !EventSystem.current.IsPointerOverGameObject(
                        Input.GetTouch(0).fingerId))
            {
                m_CloudReferencePoint =
                    ReferencePointManager.ResolveCloudReferenceId(
                        m_CloudReferenceId);
                if (m_CloudReferencePoint == null)
                {
                    OutputText.text = "Resolve Failed!";
                    m_CloudReferenceId = string.Empty;
                    m_AppMode = AppMode.TouchToHostCloudReferencePoint;
                    return;
                }

                m_CloudReferenceId = string.Empty;

                // Wait for the reference point to be ready.
                m_AppMode = AppMode.WaitingForResolvedReferencePoint;
            }
        }

        private void WaitForResolvedReferencePoint()
        {
            OutputText.text = m_AppMode.ToString();

            CloudReferenceState cloudReferenceState =
                m_CloudReferencePoint.cloudReferenceState;
            OutputText.text += " - " + cloudReferenceState.ToString();

            if (cloudReferenceState == CloudReferenceState.Success)
            {
                GameObject cloudAnchor = Instantiate(
                                             ResolvedPointPrefab,
                                             Vector3.zero,
                                             Quaternion.identity);
                cloudAnchor.transform.SetParent(
                    m_CloudReferencePoint.transform, false);

                m_CloudReferencePoint = null;

                m_AppMode = AppMode.PlayState;
            }
        }

        private void OnInputEndEdit(string text)
        {
            m_CloudReferenceId = string.Empty;

            m_CloudReferencePoint =
                ReferencePointManager.ResolveCloudReferenceId(text);
            if (m_CloudReferencePoint == null)
            {
                OutputText.text = "Resolve Failed!";
                m_AppMode = AppMode.TouchToHostCloudReferencePoint;
                return;
            }

            // Wait for the reference point to be ready.
            m_AppMode = AppMode.WaitingForResolvedReferencePoint;
        }

        private void Select()
        {
            int numberOfTouches = Input.touchCount;
            if (numberOfTouches > 0)
            {
                for (int i = 0; i < numberOfTouches; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    if (touch.phase == TouchPhase.Began)
                    {
                        //OutputText.text = "began";
                        Ray screenRay = Camera.main.ScreenPointToRay(touch.position);
                        if (Physics.Raycast(screenRay, out RaycastHit hit))
                        {
                            GameObject hitObject = hit.collider.gameObject;
                            Vector3 hitPosition = hitObject.transform.position;
                            tileHighlight.SetActive(true);
                            tileHighlight.transform.position = hitPosition;

                            if (hitObject.tag != "Board")
                            {
                                OutputText.text = "User tapped on game object " + hit.collider.gameObject.tag;
                                PhotonNetwork.Destroy(hitObject);
                            }
                            
                        }
                    }
                }
            }
        }
        

        // Update is called once per frame
        void Update()
        {
            if(PhotonNetwork.IsMasterClient)
            {
                if(placed == false)
                {
                    PlaceChessBoard();
                } 
                else if(m_AppMode == AppMode.PlayState)
                {
                    Select();
                }
                
            }
            /*if (m_AppMode == AppMode.TouchToHostCloudReferencePoint)
            {
                TouchtoHostCloudReferencePoint();
            }
            else if (m_AppMode == AppMode.WaitingForHostedReferencePoint)
            {
                WaitForHostedReferencePoint();
            }
            else if (m_AppMode == AppMode.TouchToResolveCloudReferencePoint)
            {
                TouchToResolveCloudReferencePoint();
            }
            else if (m_AppMode == AppMode.WaitingForResolvedReferencePoint)
            {
                WaitForResolvedReferencePoint();
            }
            else if (m_AppMode == AppMode.PlayState)
            {
                Select();
            }*/

        }
    }

}
