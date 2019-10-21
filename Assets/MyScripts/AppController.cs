using System;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class AppController : MonoBehaviour
{
    public GameObject HostedPointPrefab;
    public GameObject ResolvedPointPrefab;

    public ARReferencePointManager ReferencePointManager;
    public ARRaycastManager RaycastManager;
    public Text OutputText;

    public GameObject tileHighlightPrefab;
    private GameObject tileHighlight;

    private enum AppMode
    {
        // Wait for user to tap screen to begin hosting a point.
        TouchToHostCloudReferencePoint,

        // Poll hosted point state until it is ready to use.
        WaitingForHostedReferencePoint,

        //state where the players can play chess
        PlayState
    }

    private AppMode m_AppMode = AppMode.TouchToHostCloudReferencePoint;
    private ARCloudReferencePoint m_CloudReferencePoint;


    // Start is called before the first frame update
    void Start()
    {
        Vector2Int gridPoint = Geometry.GridPoint(0, 0);
        Vector3 point = Geometry.PointFromGrid(gridPoint);
        tileHighlight = Instantiate(tileHighlightPrefab, point, Quaternion.identity, gameObject.transform);
        tileHighlight.SetActive(false);
    }

    private void hostCloudReferencePoint()
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
                Pose pose = hitResults[0].pose;

                // Create a reference point at the touch.
                ARReferencePoint referencePoint =
                    ReferencePointManager.AddReferencePoint(
                        hitResults[0].pose);

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

    private void waitForHostedReferencePoint()
    {
        OutputText.text = m_AppMode.ToString();

        CloudReferenceState cloudReferenceState =
            m_CloudReferencePoint.cloudReferenceState;
        OutputText.text += " - " + cloudReferenceState.ToString();

        if (cloudReferenceState == CloudReferenceState.Success)
        {
            GameObject cloudAnchor = Instantiate(
                                         HostedPointPrefab,
                                         Vector3.zero,
                                         Quaternion.identity);
            cloudAnchor.transform.SetParent(
                m_CloudReferencePoint.transform, false);

            m_AppMode = AppMode.PlayState;
        }
       
    }

    private void Select()
    {
        int numberOfTouches = Input.touchCount;
        if (numberOfTouches > 0)
        {
            OutputText.text = numberOfTouches.ToString();
            for(int i=0; i<numberOfTouches; i++)
            {
                Touch touch = Input.GetTouch(i);
                if(touch.phase == TouchPhase.Began)
                {
                    OutputText.text = "began";
                    Ray screenRay = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(screenRay, out RaycastHit hit))
                    {
                        OutputText.text = "User tapped on game object " + hit.collider.gameObject.name;
                    }
                    else
                    {
                        OutputText.text = "no physics";
                    }
                }
            }
        }
        
          

        /*foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                OutputText.text = "Play state began";
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    OutputText.text = "Hit";
                    if (hit.collider.gameObject.tag == "BlackPawn")
                    {
                        OutputText.text = "Black Pawn hit";
                    }
                    else if (hit.collider.gameObject.tag == "WhitePawn")
                    {
                        OutputText.text = "White Pawn hit";
                    }
                    else
                    {
                        OutputText.text = "Other";
                    }
                }
                else
                {
                    OutputText.text = "Miss";
                }
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (m_AppMode == AppMode.TouchToHostCloudReferencePoint)
        {
            hostCloudReferencePoint();
        }
        else if (m_AppMode == AppMode.WaitingForHostedReferencePoint)
        {
            waitForHostedReferencePoint();
        }
        else if(m_AppMode == AppMode.PlayState)
        {
            Select();
        }
    }
}
