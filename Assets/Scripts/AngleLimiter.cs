using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class AngleLimiter : MonoBehaviour
{
    [Header("Параметры вращения")]
    [Tooltip("Локальная ось вращения")]
    public Vector3 rotationAxis = Vector3.forward;
    public float minAngle = 0f;
    public float maxAngle = 90f;
    [Tooltip("Допустимая погрешность для срабатывания переключения")]
    [Range(0f, 20f)] public float tolerance = 5f;
    
    private bool isTurnOn;
    [HideInInspector] public bool IsTurnOn => isTurnOn;

    private XRGrabInteractable grab;

    private Transform interactorTransform;
    private Quaternion initialInteractorRotation;
    private float currentAngle;

    private bool controllerInside;


    private void Awake() => grab = GetComponent<XRGrabInteractable>();

    private void Start()
    {
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        interactorTransform = args.interactorObject.transform;
        initialInteractorRotation = interactorTransform.rotation;
        controllerInside = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        interactorTransform = null;
        controllerInside = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("VRController")) return;

        interactorTransform = null;
        controllerInside = false;
    }

    private void LateUpdate()
    {
        if (!controllerInside || interactorTransform is null) return;

        // delta of controller rotation
        Quaternion delta = interactorTransform.rotation *
                           Quaternion.Inverse(initialInteractorRotation);

        delta.ToAngleAxis(out float angle, out Vector3 axis);

        // convert angle into correct sign (-/+)
        if (Vector3.Dot(axis, transform.TransformDirection(rotationAxis)) < 0)
            angle = -angle;

        float targetAngle = Mathf.Clamp(currentAngle + angle, minAngle, maxAngle);
        float appliedDelta = targetAngle - currentAngle;
        currentAngle = targetAngle;

        transform.localRotation *= Quaternion.AngleAxis(appliedDelta, rotationAxis);

        // обнуляем накопленное
        initialInteractorRotation = interactorTransform.rotation;

        UpdateIsTurnOn();
    }

    private void UpdateIsTurnOn()
    {
        if (isTurnOn && currentAngle <= minAngle + tolerance)
        {
            isTurnOn = false;
            return;
        }

        if (!isTurnOn && currentAngle >= maxAngle - tolerance)
        {
            isTurnOn = true;
        }
    }
}