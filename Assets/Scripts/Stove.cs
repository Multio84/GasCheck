using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct BurnerPack
{
    public Burner burner;
    public GasValve handle;
}

public class Stove : MonoBehaviour
{
    [Header("Burners and their handles")]
    public BurnerPack[] _burners = new BurnerPack[4];

    const int BURNERS_COUNT = 4;
    List<bool> burnerStates;


    private void Start()
    {
        SetBrokenBurners();
    }

    void SetBrokenBurners()
    {
        int count = BURNERS_COUNT;

        // at least one burner should be in order, and one broken:
        burnerStates = new List<bool> { true, false };
        for (int i = 2; i < count; i++)
        {
            burnerStates.Add(UnityEngine.Random.value > 0.5);
        }

        // mix randomly burner states
        for (int i = 0; i < count; i++)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (burnerStates[i], burnerStates[j]) = (burnerStates[j], burnerStates[i]);
        }

        for (int i = 0; i < count; i++)
        {
            if (!_burners[i].burner || !_burners[i].handle)
            {
                Debug.Log($"Stove: burner #{i} links are not set.");
                continue;
            }
            _burners[i].burner.IsBroken = burnerStates[i];
        }
    }
}
