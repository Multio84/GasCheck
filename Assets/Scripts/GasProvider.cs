using System;
using UnityEngine;


public sealed class GasProvider : MonoBehaviour
{
    [Serializable]
    private struct BurnerPack
    {
        public Burner burner;
        public GasValve handle;
    }

    [Header("Main Valve")]
    [SerializeField] private GasValve _mainValve;

    [Header("Burners and their handles")]
    [SerializeField] private BurnerPack[] _burners = new BurnerPack[4];


    private void Awake()
    {
        _mainValve.StateChanged += UpdateBurners;

        foreach (var pack in _burners)
            pack.handle.StateChanged += UpdateBurners;

        UpdateBurners();
    }

    private void OnDestroy()
    {
        _mainValve.StateChanged -= UpdateBurners;
        foreach (var pack in _burners)
            pack.handle.StateChanged -= UpdateBurners;
    }

    private void UpdateBurners()
    {
        bool gasInManifold = _mainValve.IsOpen;

        foreach (var pack in _burners)
            pack.burner.HasGas = gasInManifold && pack.handle.IsOpen;
    }
}
