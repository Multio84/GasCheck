using UnityEngine;


public class Match : MonoBehaviour
{
    [SerializeField] private float strikeDistance = 0.02f;
    [SerializeField] private GameObject fire;
    public bool isLit = false;

    private bool touching = false;
    private Transform lighter;      // a part of matchbox to light up a match
    private Vector3 startLocalPos;  // tip position when first touched lighter
    private Vector3 surfaceNormal;  // lighter normal


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Entered trigger {other.name}");
        if (isLit || !other.CompareTag("Lighter")) return;

        touching = true;
        lighter = other.transform;
        startLocalPos = lighter.InverseTransformPoint(transform.position);

        surfaceNormal = lighter.forward;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!touching || other.transform != lighter) return;

        // current tip position in lighter's local pos
        Vector3 curLocal = lighter.InverseTransformPoint(transform.position);
        Vector3 delta = curLocal - startLocalPos;

        // check the way along lighter's tangent
        Vector3 tangential =
            Vector3.ProjectOnPlane(lighter.TransformVector(delta), surfaceNormal);

        if (tangential.magnitude >= strikeDistance)
            LightUp();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == lighter)
            touching = false;
    }

    public void LightUp()
    {
        isLit = true;
        touching = false;
        fire.SetActive(true);
    }
}