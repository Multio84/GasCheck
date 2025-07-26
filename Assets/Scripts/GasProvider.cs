using UnityEngine;


public sealed class GasProvider : MonoBehaviour
{
    [Header("Main Valve")]
    [SerializeField] private GasValve _mainValve;
    [SerializeField] private Stove _stove;
    private BurnerPack[] _burners;


    private void Awake()
    {
        if (!_stove)
        {
            Debug.Log("GasProvider: Stove link is not set.");
            return;
        }
        _burners = _stove._burnerPack;

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
        {
            var hasGas = gasInManifold && pack.handle.IsOpen;
            pack.burner.HasGas = hasGas;
        }
    }
}
