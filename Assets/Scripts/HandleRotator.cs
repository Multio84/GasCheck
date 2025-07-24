using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


[RequireComponent(typeof(XRGrabInteractable))]
public class HandleRotator : MonoBehaviour
{
    public enum RotationDirection
    {
        Clockwise = 1,
        CounterClockwise = -1
    }

    [Header("ѕараметры вращени€")]
    [Tooltip("Ћокальна€ ось, вокруг которой крутитс€ руко€тка")]
    public Vector3 rotationAxis = Vector3.forward;

    [Tooltip("Ќаправление вращени€ от стартовой позиции: по часовой, или против")]
    public RotationDirection workDirection = RotationDirection.Clockwise;

    public float minAngle = 0f;
    public float maxAngle = 90f;

    [Tooltip("”гол, после поворота на который, газ считаетс€ поданным")]
    [Range(0f, 20f)] public float tolerance = 10f;

    public event Action<bool> StateChanged;

    private XRGrabInteractable _grab;
    private Transform _t;
    private Transform _interactor;
    private Quaternion _baseHandleRot;      // start angle of a handle in scene
    private Quaternion _startInteractorRot; // controller's orientation in a moment of grab
    private float _currentAngle;            // current orientation
    private float _angleAtGrab;             // handle rotation in a moment of grab
    private bool _isOpen;

    private int DirSign => (int) workDirection;


    private void Awake()
    {
        _t = transform;
        _grab = GetComponent<XRGrabInteractable>();
        _baseHandleRot = _t.localRotation;

        _grab.selectEntered.AddListener(OnGrab);
        _grab.selectExited.AddListener(OnRelease);

        enabled = false;
    }

    private void OnDestroy()
    {
        if (!_grab) return;
        _grab.selectEntered.RemoveListener(OnGrab);
        _grab.selectExited.RemoveListener(OnRelease);
    }

    #region XR events
    private void OnGrab(SelectEnterEventArgs args)
    {
        _startInteractorRot = args.interactorObject.transform.rotation;
        _angleAtGrab = _currentAngle;

        enabled = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (_interactor is not null && other.transform == _interactor)
            OnRelease(default);
    }
    #endregion


    void Update()
    {
        if (!_grab.isSelected) return;

        // delta between current and start controller's rotation
        Quaternion delta = _grab.interactorsSelecting[0].transform.rotation * Quaternion.Inverse(_startInteractorRot);


        delta.ToAngleAxis(out float rawAngle, out Vector3 rawAxis);

        Vector3 worldAxis = _t.rotation * rotationAxis;
        if (Vector3.Dot(rawAxis, worldAxis) < 0f)
            rawAngle = -rawAngle;

        float logicDelta = rawAngle * DirSign;  // accounting direction

        float targetAngle = Mathf.Clamp(_angleAtGrab + logicDelta,
                                        minAngle, maxAngle);

        // apply rotation on a base of start angle
        _t.localRotation = _baseHandleRot * Quaternion.AngleAxis(targetAngle * DirSign, rotationAxis);

        _currentAngle = targetAngle;
        UpdateIsOpen();
    }

    private void UpdateIsOpen()
    {
        bool newState = _currentAngle >= maxAngle - tolerance;
        if (!newState)
            newState = !(_currentAngle <= minAngle + tolerance);

        if (_isOpen == newState) return;
        _isOpen = newState;
        StateChanged?.Invoke(_isOpen);
    }
}