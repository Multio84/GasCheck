using UnityEditor.ShaderKeywordFilter;
using UnityEngine;


public class Burner : MonoBehaviour
{
    [SerializeField] GameObject jetPrefab;      // a single jetPrefab of gas prefab
    [SerializeField] GameObject broken;
    [SerializeField] [Min(1)] int jetsAmount;
    [SerializeField] float offsetAngle;         // angle to shift rotation of all jets
    [SerializeField] bool hasGas;
    public bool HasGas 
    {
        get => hasGas;
        internal set
        {
            if (HasGas == value) return;
            hasGas = value;
            UpdateBurning();
        }
    }
    [HideInInspector] public bool isLit = false;
    //[HideInInspector]
    public bool IsBroken = false;
    
    GameObject[] jets;
    float deltaAngle;                   // angle from one jetPrefab to another
    Vector3 rotationAxis = Vector3.up;
    bool isLitMatchInside = false;


    void Start()
    {
        jets = new GameObject[jetsAmount];

        SpawnGasJets();
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"{other.name} entered {name}");
        if (!other.CompareTag("MatchTip")) return;

        Interact(other);
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log($"{other.name} quited {name}.");
        if (!other.CompareTag("MatchTip")) return;

        isLitMatchInside = false;
    }

    void Interact(Collider obj)
    {
        Match match = obj.GetComponentInParent<Match>();
        if (!match) return;
        if (match.isLit) isLitMatchInside = true;

        if (isLit == match.isLit) return;

        Debug.Log($"{name} is cheching for light up");

        if (isLit) match.LightUp();
        else UpdateBurning();
    }

    void UpdateBurning()
    {
        Debug.Log($"{name} is trying to UpdateBurning (HasGas = {hasGas}).");
        if (hasGas && !isLitMatchInside) return;

        //Debug.Log($"Burner lighted up!");

        if (IsBroken && hasGas)
        {
            broken.SetActive(true);
        }
        else
        {
            foreach (var jet in jets)
            {
                if (!jet) continue;
                jet.SetActive(hasGas);
            }

            isLit = hasGas;
        }
    }

    void SpawnGasJets()
    {
        if (IsBroken) return;

        if (!jetPrefab)
        {
            Debug.LogWarning("Prefab not assigned!");
            return;
        }

        deltaAngle = 360 / (jetsAmount - 1);
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