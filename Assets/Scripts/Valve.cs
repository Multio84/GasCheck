using UnityEngine;


public enum Axis
{ 
    X, Y, Z
}


public class Valve : MonoBehaviour
{
    public bool isActive = false;
    public string controllerTag = "VRController";
    public Axis axis = Axis.Y;

    Transform controller;             // трансформ вошедшего контроллера
    Quaternion controllerStartRot;     // его ориентация в момент входа
    Quaternion valveStartRot;          // базовая ориентация вентиля



    void Start()
    {
        //startTransform = transform;
    }


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(controllerTag)) return;
        isActive = true;

        controller = other.transform;
        controllerStartRot = controller.rotation;  // world
        valveStartRot = transform.rotation;   // world (можно на local сменить)
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(controllerTag)) return;
        isActive = false;

        controller = null;
    }


    private void LateUpdate()
    {
        if (!isActive) return;

        // Разница между текущим и начальным поворотом контроллера
        Quaternion delta = controller.rotation * Quaternion.Inverse(controllerStartRot);

        // Из дельты берём только интересующую нас ось
        Vector3 deltaEuler = delta.eulerAngles;
        float signedAngle;
        Vector3 axisVector;

        switch (axis)
        {
            case Axis.X:
                signedAngle = Mathf.DeltaAngle(0f, deltaEuler.x);
                axisVector = Vector3.right;
                break;

            case Axis.Y:
                signedAngle = Mathf.DeltaAngle(0f, deltaEuler.y);
                axisVector = Vector3.up;
                break;

            default: // Axis.Z
                signedAngle = Mathf.DeltaAngle(0f, deltaEuler.z);
                axisVector = Vector3.forward;
                break;
        }

        // Накручиваем нужный угол на исходный поворот вентиля
        transform.rotation = valveStartRot * Quaternion.AngleAxis(signedAngle, axisVector);
        // Если нужен локальный поворот - замените rotation на localRotation,
        // а valveStartRot храните из transform.localRotation.
    }

}
