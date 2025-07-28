using System;
using UnityEngine;


public class Burner : MonoBehaviour
{
    [SerializeField] private GameObject jetPrefab;      // a single jetPrefab of gas prefab
    [SerializeField] [Min(1)] private int jetsAmount;
    [SerializeField] private float offsetAngle;         // angle to shift rotation of all jets
    [SerializeField] private bool hasGas;
    [HideInInspector]
    public bool HasGas 
    {
        get => hasGas;
        internal set
        {
            if (HasGas == value) return;
            hasGas = value;
            GasStateChanged?.Invoke(hasGas);
            UpdateBurning();
        }
    }
    public bool isLit = true;
    public bool IsBroken = false;

    public event Action<bool /*hasGas*/> GasStateChanged;
    public event Action<Burner, bool /*isLit*/, bool /*isBroken*/> BurnStateChanged;

    private GameObject[] jets;
    private float deltaAngle;   // angle from one jetPrefab to another
    private Vector3 rotationAxis = Vector3.up;
    private bool isLitMatchInside = false;


    private void Start()
    {
        jets = new GameObject[jetsAmount];

        SpawnGasJets();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"{other.name} entered {name}");
        if (!other.CompareTag("MatchTip")) return;
        Interact(other);
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log($"{other.name} quited {name}.");
        if (!other.CompareTag("MatchTip")) return;
        isLitMatchInside = false;
    }

    private void Interact(Collider obj)
    {
        Match match = obj.GetComponentInParent<Match>();
        if (!match) return;

        if (match.isLit)
        {
            isLitMatchInside = true;
            if (!isLit) UpdateBurning();
            return;
        }

        if (isLit) match.LightUp();
    }

    private void UpdateBurning()
    {
        // broken ignition
        if (hasGas && isLitMatchInside && IsBroken)
        {
            BurnStateChanged?.Invoke(this, false, true/*isBroken*/);
            return;
        }

        bool newLitState = hasGas && isLitMatchInside && !IsBroken;
        if (newLitState == isLit) return;

        isLit = newLitState;
        foreach (var jet in jets) 
            if (jet) jet.SetActive(isLit);

        BurnStateChanged?.Invoke(this, isLit, false);
    }

    private void SpawnGasJets()
    {
        if (IsBroken) return;

        if (!jetPrefab)
        {
            Debug.LogError("Prefab not assigned!");
            return;
        }

        deltaAngle = 360f / jetsAmount;
        Quaternion currentRotation = Quaternion.AngleAxis(offsetAngle, rotationAxis);

        for (int i = 0; i < jetsAmount; i++)
        {
            var jet = Instantiate(
                jetPrefab,
                transform.position,
                currentRotation,
                transform
                );

            jet.SetActive(isLit);
            jets[i] = jet;

            // next object's rotation
            currentRotation = Quaternion.AngleAxis(deltaAngle, rotationAxis) * currentRotation;
        }
    }
}