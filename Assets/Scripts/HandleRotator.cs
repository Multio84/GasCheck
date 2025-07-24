using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System;


public class HandleRotator : MonoBehaviour
{
    public enum RotationDirection
    {
        Clockwise,
        CounterClockwise
    }

    [Header("Параметры вращения")]
    [Tooltip("Локальная ось, вокруг которой крутится рукоятка")]
    public Vector3 rotationAxis = Vector3.forward;

    [Tooltip("Направление, которое считаем «положительным»")]
    public RotationDirection workDirection = RotationDirection.Clockwise;

    public float minAngle = 0f;
    public float maxAngle = 90f;

    [Tooltip("Допустимая погрешность для срабатывания переключения")]
    [Range(0f, 20f)] public float tolerance = 10f;

    private bool isOpen;
    [HideInInspector] public bool IsOpen
    { 
        get => isOpen;
        private set
        {
            isOpen = value;
            //Debug.Log($"{name} sent message '{IsOpen}'. Current angle = {currentAngle}");
            StateChanged?.Invoke(isOpen);
        }
    }

    public event Action<bool> StateChanged;

    private XRGrabInteractable grab;
    private Transform interactorTransform;
    private Quaternion initialInteractorRotation;

    private float currentAngle;     // current rotation angle in work direction
    private bool controllerInside;  // is handle gripped
    private int dirSign;            // the sign of work direction



    private void Awake() => grab = GetComponent<XRGrabInteractable>();

    private void Start()
    {
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);

        dirSign = workDirection == RotationDirection.Clockwise ? 1 : -1;
    }

    private void LateUpdate()
    {
        if (!controllerInside || interactorTransform == null) return;

        Quaternion delta = interactorTransform.rotation *
                           Quaternion.Inverse(initialInteractorRotation);

        delta.ToAngleAxis(out float rawAngle, out Vector3 axis);

        // задаём знак в зависимости от того, совпадают ли оси
        if (Vector3.Dot(axis, transform.TransformDirection(rotationAxis)) < 0)
            rawAngle = -rawAngle;

        // Меняем знак, если рабочим считается направление ПРОТИВ часовой стрелки
        float logicAngleDelta = rawAngle * dirSign;

        // накапливаем угол в нужном диапазоне
        float targetAngle = Mathf.Clamp(currentAngle + logicAngleDelta, minAngle, maxAngle);
        float appliedDeltaLogic = targetAngle - currentAngle;
        currentAngle = targetAngle;

        // вращаем рукоятку
        float appliedDeltaVisual = appliedDeltaLogic * dirSign;   // возвращаем исходный знак
        transform.localRotation *= Quaternion.AngleAxis(appliedDeltaVisual, rotationAxis);

        // обнуляем накопление для следующего кадра
        initialInteractorRotation = interactorTransform.rotation;

        UpdateIsOpen();
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

    private void UpdateIsOpen()
    {
        if (IsOpen && currentAngle <= minAngle + tolerance)
        {
            IsOpen = false;
            //Debug.Log($"{name} is turned OFF. Current angle = {currentAngle}");
            return;
        }

        if (!IsOpen && currentAngle >= maxAngle - tolerance)
        {
            IsOpen = true;
            //Debug.Log($"{name} is turned ON. Current angle = {currentAngle}");
        }
    }
}