using UnityEngine;


public class Burner : MonoBehaviour
{
    [SerializeField] GameObject jet;    // a single jet of gas prefab
    [SerializeField] [Min(1)] int jetsAmount;
    [SerializeField] float offsetAngle; // angle to shift rotation of all jets
    float deltaAngle;                   // angle from one jet to another
    Vector3 rotationAxis = Vector3.up;
    //GasJet[] jets;


    void Start()
    {
        SpawnGasJets();
    }

    void SpawnGasJets()
    {
        if (!jet)
        {
            Debug.LogWarning("Prefab not assigned!");
            return;
        }

        deltaAngle = 360 / jetsAmount;
        Quaternion currentRotation = Quaternion.AngleAxis(offsetAngle, rotationAxis);

        for (int i = 0; i <= jetsAmount; i++)
        {
            Instantiate(
                jet,
                transform.position,
                currentRotation,
                transform
                );

            Debug.LogWarning($"Spawned at pos {transform.localPosition}");

            // next object's rotation
            currentRotation = Quaternion.AngleAxis(deltaAngle, rotationAxis) * currentRotation;
        }
    }
}