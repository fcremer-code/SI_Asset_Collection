using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPath : MonoBehaviour
{
    public List<Transform> CameraWaypointTransforms = new List<Transform>();
    [SerializeField] public float inTime = 1;
    [SerializeField] public bool smoothOutPathEnds = true;
    [SerializeField] private TopDownCameraController topDownCameraController;

    public void StartThisCameraPath()
    {
        List<Vector2> CamWaypointVector2 = new List<Vector2>();

        for (int i = 0; i < CameraWaypointTransforms.Count; i++)
        {
            CamWaypointVector2.Add((Vector2)CameraWaypointTransforms[i].position);
        }
        StartCoroutine(topDownCameraController.MoveCamOnPath(CamWaypointVector2, inTime, smoothOutPathEnds));
    }
}
