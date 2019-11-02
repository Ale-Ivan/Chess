using System;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

//these are used for creating the multiplayer status of the game
using Photon.Pun;
using Photon.Realtime;

namespace com.Ale.Chess
{
    public class AppController : MonoBehaviour
    {
        public GameObject HostedPointPrefab;
        public GameObject ResolvedPointPrefab;

        public ARReferencePointManager ReferencePointManager;
        public ARRaycastManager RaycastManager;
        public Text OutputText;

        public GameObject tileHighlightPrefab;

        public bool placed = false; //boolean used for checking if the board is placed or not (if true, you cannot place a board anymore)

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
          
            PlayState //for Master Client and Simple Client
        }


        private AppMode m_AppMode = AppMode.TouchToHostCloudReferencePoint;
        private ARCloudReferencePoint m_CloudReferencePoint;
        private string m_CloudReferenceId;

        // Start is called before the first frame update
        void Start()
        {

        }

        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
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
                    if(placed == false)
                    {
                        PhotonNetwork.InstantiateSceneObject(this.HostedPointPrefab.name, pose.position, Quaternion.identity, 0);
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
                        OutputText.text = "began";
                        Ray screenRay = Camera.main.ScreenPointToRay(touch.position);
                        if (Physics.Raycast(screenRay, out RaycastHit hit))
                        {
                            OutputText.text = "User tapped on game object " + hit.collider.gameObject.tag;
                            if (hit.collider.gameObject.tag != "Board")
                            {
                                GameObject hitObject = hit.collider.gameObject;
                                Destroy(hitObject);
                            }
                            

                        }
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (placed == false && PhotonNetwork.IsMasterClient)
            {
                PlaceChessBoard();
            }
            if (placed)
            {
                if (m_AppMode == AppMode.PlayState)
                {
                    Select();
                }
            }
            

            /*if (PhotonNetwork.IsMasterClient)
            {
                if (m_AppMode == AppMode.TouchToHostCloudReferencePoint)
                {
                    TouchtoHostCloudReferencePoint();
                }
                else if (m_AppMode == AppMode.WaitingForHostedReferencePoint)
                {
                    WaitForHostedReferencePoint();
                }
                else if (m_AppMode == AppMode.PlayState)
                {
                    Select();
                }
             
            } 
            else
            {
                if (m_AppMode == AppMode.TouchToResolveCloudReferencePoint)
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
                }
            }*/
            
        }
    }

}
