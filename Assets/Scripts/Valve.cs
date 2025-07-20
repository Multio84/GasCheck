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

    Transform controller;             // ��������� ��������� �����������
    Quaternion controllerStartRot;     // ��� ���������� � ������ �����
    Quaternion valveStartRot;          // ������� ���������� �������



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
        valveStartRot = transform.rotation;   // world (����� �� local �������)
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

        // ������� ����� ������� � ��������� ��������� �����������
        Quaternion delta = controller.rotation * Quaternion.Inverse(controllerStartRot);

        // �� ������ ���� ������ ������������ ��� ���
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

        // ����������� ������ ���� �� �������� ������� �������
        transform.rotation = valveStartRot * Quaternion.AngleAxis(signedAngle, axisVector);
        // ���� ����� ��������� ������� - �������� rotation �� localRotation,
        // � valveStartRot ������� �� transform.localRotation.
    }

}
