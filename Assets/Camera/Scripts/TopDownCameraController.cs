using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode {None, FollowTarget, FreeCam};


[RequireComponent(typeof(Camera))]
public class TopDownCameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] public CameraMode cameraMode = CameraMode.FollowTarget;
    [SerializeField] private bool smoothFollow = true;
    [SerializeField] private float smoothSpeed = 0.2f;
    [SerializeField] private float mousePosInfluenceOnCamMovement = 3;
    [SerializeField] private Vector2 maxOffsetForMousePosInfluenceOnCamMovement = new Vector2(3, 2);
    [SerializeField] private float ignoreOffsetForMouseInfluenceInRadius = 0;
    [Space]
    [Header("Monitoring")]
    [SerializeField] public Vector3 moveToPos = Vector3.forward;
    [SerializeField] public Vector2 currendOffset = Vector2.zero;
    [SerializeField] public float ZOffset = -1;
    [SerializeField] private bool overwrittenCamMovement = false;
    [SerializeField] public bool playerInputDisabled = false;
    [Space]
    [Header("References")]
    [SerializeField] public Camera thisCamera = null;
    [SerializeField] public Transform followTarget = null;


    void Start()
    {
        if (followTarget == null)
            followTarget = GameManager.Instance.player.transform;

        thisCamera = GetComponent<Camera>();
    }

    void Update()
    {
        switch (cameraMode)
        {
            case CameraMode.None:
                break;
            case CameraMode.FollowTarget:
                if (followTarget != null)
                {
                    if (!overwrittenCamMovement && !playerInputDisabled)
                    {
                        //MousePosition Influence on Camera Position
                        if (mousePosInfluenceOnCamMovement > 0)
                            currendOffset = MouseInfluenceOffset();

                        moveToPos = new Vector3(followTarget.position.x, followTarget.position.y, ZOffset);
                    }
                    MoveCamera(moveToPos, currendOffset, smoothFollow);
                }
                else
                    Debug.LogError("TopDownCameraController: followTarget (Transform) == null!");
                break;
            case CameraMode.FreeCam:
                break;
            default:
                break;
        }
    }

    private Vector2 MouseInfluenceOffset()
    {
        Vector2 _mouseInfluenceOffset = Vector2.zero;

        if (Vector2.Distance(thisCamera.ScreenToWorldPoint(Input.mousePosition), new Vector2(followTarget.position.x, followTarget.position.y)) > ignoreOffsetForMouseInfluenceInRadius)
        {
            _mouseInfluenceOffset = new Vector2(
                ((Input.mousePosition.x - thisCamera.WorldToScreenPoint(followTarget.position).x) / Screen.width) * mousePosInfluenceOnCamMovement,
                ((Input.mousePosition.y - thisCamera.WorldToScreenPoint(followTarget.position).y) / Screen.height) * mousePosInfluenceOnCamMovement
                );

            _mouseInfluenceOffset = new Vector3(
                Mathf.Clamp(_mouseInfluenceOffset.x, -maxOffsetForMousePosInfluenceOnCamMovement.x, maxOffsetForMousePosInfluenceOnCamMovement.x),
                Mathf.Clamp(_mouseInfluenceOffset.y, -maxOffsetForMousePosInfluenceOnCamMovement.y, maxOffsetForMousePosInfluenceOnCamMovement.y),
                0);
        }
        return _mouseInfluenceOffset;
    }

    /// <summary>
    /// Called every Update(). Updates camera position.
    /// </summary>
    public void MoveCamera(Vector3 _spezificPosition, Vector3 _addOffsetToPosition, bool _moveSmoothly = true)
    {
        Vector3 _wantedPosition = Vector3.zero;

        Debug.DrawLine(followTarget.position, _spezificPosition, Color.red, Time.deltaTime, false);
        Debug.DrawLine(_spezificPosition, _spezificPosition + _addOffsetToPosition, Color.green, Time.deltaTime, false);

        _wantedPosition = _spezificPosition + _addOffsetToPosition;

        //Teleports Camera directly to position
        if (!_moveSmoothly)
        {

        }
        //Lerps Camera smoothly to position
        else
        {
            _wantedPosition = Vector3.Lerp(transform.position,
                _wantedPosition,
                (Time.deltaTime * smoothSpeed)// * (Vector3.Distance(transform.position, _wantedPosition))
                );
        }
        transform.position = _wantedPosition;
    }


    public IEnumerator MoveCamOnPath(List<Vector2> _pathPoints, float _time, bool _smothEnds = true)
    {
        if (_pathPoints.Count == 0)
            yield break;

        overwrittenCamMovement = true;
        currendOffset = Vector2.zero;
        int _counter = 0;
        float _timeDevider;

        while (_counter <= _pathPoints.Count)
        {
            if (followTarget == null || !cameraMode.Equals(CameraMode.FollowTarget))
            {
                if (_counter == _pathPoints.Count)
                    break;
                _timeDevider = _pathPoints.Count;
            }
            else 
            {
                _timeDevider = _pathPoints.Count + 1;
                if (_counter == _pathPoints.Count)
                {
                    yield return StartCoroutine(PathMovement((Vector2)followTarget.position, _time / _timeDevider));
                    break;
                }
            }
            yield return StartCoroutine(PathMovement(_pathPoints[_counter], _time / _timeDevider));
            _counter++;
        }
        overwrittenCamMovement = false;
    }

    private IEnumerator PathMovement(Vector2 _newPos, float _time)
    {
        float _timer = 0;
        while (_timer < _time)
        {
            _timer += Time.deltaTime;
            moveToPos = Vector3.Lerp(transform.position, _newPos, _timer / _time);
            moveToPos = new Vector3(moveToPos.x, moveToPos.y, ZOffset);
            yield return Time.deltaTime;
        }
    }
}
