using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//CAMERA MOVEMENT BY JOSIAS KOENIG

public enum CameraState {defaultSetup, followObject, screenRimMovement, bindToMouse};

//[ExecuteInEditMode]
public class CameraMovement : MonoBehaviour
{
    [Tooltip("Switch between different 2D camera states.")]
    public CameraState cameraState = CameraState.defaultSetup;
    [Tooltip("If true, <Curser.visible> and <CurserLockMode> settings in this skript get ignored")]
    public bool ignoreMouseSettings = false;
    private Vector3 startPos = new Vector3(0, 0, 0);
    private Vector3 moveToPos;


    [Header("Follow Gameobject")]
    [Tooltip("A GameObject the camera follows in X and Y space.")]
    public GameObject followGameobject;
    [Tooltip("Ignores the movement of the <Follow GameObject> on an axis.")]
    public bool ignoreXAxis = false, ignoreYAxis = false;
    [Tooltip("If true, the camera movement gets smoothed.")]
    public bool smoothFollow = true;
    [Range(0.0f, 3.0f)]
    [Tooltip("low value = very smooth camera movement \n" +
        "high value = less smooth camera movement")]
    public float smoothSpeed = 1f;

    [Header("Screen Rim Camera Controll")]
    [Tooltip("Camera movement speed.")]
    [Range(-25.0f, 25.0f)]
    public float cameraSpeed = 4f;
    [Tooltip("Size of the space on the screen-borders, where the mouse causes camera movement. \n" +
        "In % from <screen border> to <screen middle>.")]
    [Range(0, 100)]
    public int reactionZoneSize = 10;

    [Header("Camera movement with mouse")]
    [Tooltip("Camera movement speed in <bind to mouse>-state.")]
    [Range(-25.0f, 25.0f)]
    public float cameraMouseSpeed = 4;


    [Space(30)]
    [TextArea(2, 8)]
    public string functions = "Use these public functions in your skripts: \n" +
        "\n" +
        "Move the Cam to a Z-position: \n" +
        "ZoomToZPosition (Z-position you want to move to (float), Transition in seconds(float), Smooth Zoom (bool))";


    //ZoomToZPosition variables
    private bool zoomToZPos = false;
    private float zPositionOld, zPositionNew, zoomTimeInSeconds;
    private bool smoothZoom = true;
    private float zoomToZPosTimer = 0f;


    void Awake()
    {
        startPos = transform.position;
    }

    void Update()
    {
        switch (cameraState)
        {
            case CameraState.defaultSetup:
                if (!ignoreMouseSettings)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                break;

            case CameraState.followObject:
                if (!ignoreMouseSettings)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }

                if (followGameobject != null)
                {
                    if (!smoothFollow)
                    {
                        moveToPos = new Vector3(followGameobject.transform.position.x, followGameobject.transform.position.y, transform.position.z);
                    }
                    else
                    {
                        moveToPos = Vector3.Lerp(transform.position, 
                            new Vector3(followGameobject.transform.position.x, followGameobject.transform.position.y, transform.position.z), 
                            Mathf.SmoothStep(0.0f, 1.0f, Time.deltaTime * 25 * smoothSpeed));
                    }

                    if (!ignoreXAxis && !ignoreYAxis)
                    {
                        transform.position = moveToPos;
                    }
                    else if (ignoreXAxis && ignoreYAxis)
                    { }
                    else if (ignoreXAxis && !ignoreYAxis)
                    {
                        transform.position = new Vector3(startPos.x, moveToPos.y, moveToPos.z);
                    }
                    else if (!ignoreXAxis && ignoreYAxis)
                    {
                        transform.position = new Vector3(moveToPos.x, startPos.y, moveToPos.z);
                    }
                }
                else
                {
                    Debug.LogWarning("No GameObject to follow selected!");
                }
                break;

            case CameraState.screenRimMovement:
                if (!ignoreMouseSettings)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }

                if (Input.mousePosition.x > Screen.width - ((reactionZoneSize * Screen.width) / 2 / 100))
                {
                    transform.position = new Vector3(transform.position.x + Time.deltaTime * cameraSpeed * 2, transform.position.y, transform.position.z);
                }
                else if (Input.mousePosition.x < ((reactionZoneSize * Screen.width) / 100) / 2)
                {
                    transform.position = new Vector3(transform.position.x - Time.deltaTime * cameraSpeed * 2, transform.position.y, transform.position.z);
                }
                if (Input.mousePosition.y > Screen.height - ((reactionZoneSize * Screen.width) / 100) / 2)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * cameraSpeed * 2, transform.position.z);
                }
                else if (Input.mousePosition.y < ((reactionZoneSize * Screen.width) / 100) / 2)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * cameraSpeed * 2, transform.position.z);
                }
                break;
            case CameraState.bindToMouse:
                if (!ignoreMouseSettings)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }

                if (Input.GetAxis("Mouse X") < 0)
                {
                    transform.position = new Vector3(transform.position.x - Time.deltaTime * -Input.GetAxis("Mouse X") * 8 * cameraMouseSpeed, transform.position.y, transform.position.z);
                }
                else if (Input.GetAxis("Mouse X") > 0)
                {
                    transform.position = new Vector3(transform.position.x + Time.deltaTime * Input.GetAxis("Mouse X") * 8 * cameraMouseSpeed, transform.position.y, transform.position.z);
                }
                if (Input.GetAxis("Mouse Y") < 0)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * -Input.GetAxis("Mouse Y") * 12 * cameraMouseSpeed, transform.position.z);
                }
                else if (Input.GetAxis("Mouse Y") > 0)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * Input.GetAxis("Mouse Y") * 12 * cameraMouseSpeed, transform.position.z);
                }
                break;
        }

        if (zoomToZPos) //ZoomToZPosition(...) 
        {
            if (zoomToZPosTimer > zoomTimeInSeconds)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, zPositionNew);
                zoomToZPos = false;
            }
            else if (zoomTimeInSeconds == 0f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, zPositionNew);
                zoomToZPos = false;
            }
            else
            {
                zoomToZPosTimer += Time.deltaTime;

                if (smoothZoom)
                {
                    transform.position = new Vector3(transform.position.x,
                        transform.position.y, 
                        Mathf.SmoothStep(zPositionOld, zPositionNew, zoomToZPosTimer / zoomTimeInSeconds));
                }
                else if (!smoothZoom)
                {
                    transform.position = Vector3.Lerp(new Vector3(transform.position.x, transform.position.y, zPositionOld),
                        new Vector3(transform.position.x, transform.position.y, zPositionNew),
                        zoomToZPosTimer / zoomTimeInSeconds);
                }
            }
        }
    }


    public void ZoomToZPosition(float _zPosition, float _zoomTimeInSeconds, bool _smoothZoom)
    {
        zoomToZPos = true;
        zPositionOld = transform.position.z;
        zPositionNew = _zPosition;
        zoomTimeInSeconds = _zoomTimeInSeconds;
        smoothZoom = _smoothZoom;
    }
}
