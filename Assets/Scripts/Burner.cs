using UnityEngine;


public class Burner : MonoBehaviour
{
    [SerializeField] GameObject jetPrefab;    // a single jetPrefab of gas prefab
    [SerializeField] [Min(1)] int jetsAmount;
    [SerializeField] float offsetAngle; // angle to shift rotation of all jets
    [HideInInspector] public bool isLit = false;
    
    GameObject[] jets;
    float deltaAngle;                   // angle from one jetPrefab to another
    Vector3 rotationAxis = Vector3.up;



    void Start()
    {
        jets = new GameObject[jetsAmount];

        SpawnGasJets();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Burner collided with {other.name}");

        if (!other.TryGetComponent(out Match match)) return;
        if (isLit == match.isLit) return;

        if (isLit) match.LightUp();
        else LightUp();
    }

    void LightUp()
    {
        Debug.Log($"Burner lighted up!");

        foreach (var jet in jets)
        {
            if (!jet) continue;
            jet.SetActive(true);
        }
    }

    void SpawnGasJets()
    {
        if (!jetPrefab)
        {
            Debug.LogWarning("Prefab not assigned!");
            return;
        }

        deltaAngle = 360 / (jetsAmount);
        Quaternion currentRotation = Quaternion.AngleAxis(offsetAngle, rotationAxis);

        for (int i = 0; i < jetsAmount; i++)
        {
            var jet = Instantiate(
                jetPrefab,
                transform.position,
                currentRotation,
                transform
                );

            jet.SetActive(false);
            jets[i] = jet;

            // next object's rotation
            currentRotation = Quaternion.AngleAxis(deltaAngle, rotationAxis) * currentRotation;
        }
    }
}