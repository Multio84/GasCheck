using UnityEngine;


public class Match : MonoBehaviour
{
    [SerializeField] float strikeDistance = 0.025f;   // 2�3 ��
    [SerializeField] GameObject fire;
    public bool isLit = false;

    bool touching = false;
    Transform lighter;      // �������
    Vector3 startLocalPos;  // ��� ��� ������ ��� ������ �������
    Vector3 surfaceNormal;  // ������� �������� (��� ������������ ������������)


    void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Entered trigger {other.name}");

        if (isLit || !other.CompareTag("Lighter")) return;

        Debug.Log("Entered Lighter");

        touching = true;
        lighter = other.transform;
        startLocalPos = lighter.InverseTransformPoint(transform.position);

        /*  � �������� ��� ContactPoint, ������� ������� ����� �������.
         *  ����� ������� �������� � ��� ��������� +Z  light-era.
         *  ��� ������ ������������ �������� ������ ��� ���
         *  ������� � ����� light-��� ������ � ������ Vector3.              */
        surfaceNormal = lighter.forward;
    }

    void OnTriggerStay(Collider other)
    {
        if (!touching || other.transform != lighter) return;

        // ������� ������� ������� � ��������� ����������� �������
        Vector3 curLocal = lighter.InverseTransformPoint(transform.position);
        Vector3 delta = curLocal - startLocalPos;

        // ����������� ������������ ����� �������
        Vector3 tangential =
            Vector3.ProjectOnPlane(lighter.TransformVector(delta), surfaceNormal);

        if (tangential.magnitude >= strikeDistance)
            LightUp();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == lighter)
            touching = false;
    }

    public void LightUp()
    {
        isLit = true;
        touching = false;
        fire.SetActive(true);
        Debug.Log("Match is lighted!");
    }
}


//{
//    [Header("Setup")]
//    [SerializeField] Collider tipCollider;            // ��������� ������� ������
//    [SerializeField] float strikeDistance = 0.025f;   // 2.5 �� � ������� ����� ���������

//    [Header("Runtime")]
//    public bool IsLighted { get; private set; }

//    // --- ���������� ��������� ---
//    bool touching;      // ������ ������ ����� � �������?
//    float traveled;     // ����������� ����������
//    Transform lighter;    // ��� ����� Lighter, �� �������� �������
//    Vector3 lastLocalPos;   // ������� ������� � ��������� �� �������
//    Vector3 surfaceNormalWorld; // ������� ��������, ����������� ��� ������ ��������


//    void OnCollisionEnter(Collision col)
//    {
//        if (IsLighted || !col.collider.CompareTag("Lighter")) return;

//        BeginStrike(col.collider.transform, col.GetContact(0).normal);
//    }


//    void OnCollisionStay(Collision col)
//    {
//        if (touching && col.collider.transform == lighter)
//            ContinueStrike();
//    }

//    void OnCollisionExit(Collision col)
//    {
//        if (touching && col.collider.transform == lighter)
//            ResetStrike();
//    }

//    /* -------------- ������ -------------- */

//    void BeginStrike(Transform lighter, Vector3 normalWorld)
//    {
//        this.lighter = lighter;
//        surfaceNormalWorld = normalWorld.normalized;

//        lastLocalPos = this.lighter.InverseTransformPoint(tipCollider.transform.position);
//        traveled = 0f;
//        touching = true;
//    }

//    void ContinueStrike()
//    {
//        Vector3 curLocal = lighter.InverseTransformPoint(transform.position);
//        Vector3 deltaLocal = curLocal - lastLocalPos;
//        lastLocalPos = curLocal;

//        // ��������� ���������� � ���, ����� ���������� ��������� ����� �������
//        Vector3 deltaWorld = lighter.TransformVector(deltaLocal);
//        Vector3 tangential = Vector3.ProjectOnPlane(deltaWorld, surfaceNormalWorld);
//        traveled += tangential.magnitude;

//        if (traveled >= strikeDistance)
//            LightUp();
//    }

//    void ResetStrike()
//    {
//        touching = false;
//        traveled = 0f;
//        lighter = null;
//    }

//    void LightUp()
//    {
//        IsLighted = true;
//        touching = false;

//        Debug.Log("Match is lighted!");
//    }
//}
