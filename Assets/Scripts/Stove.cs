using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct BurnerPack
{
    public Burner burner;
    public GasValve handle;
}

[RequireComponent(typeof(StoveSoundManager))]
public class Stove : MonoBehaviour
{
    StoveSoundManager sound;

    [Header("Burners and their handles")]
    public BurnerPack[] _burnerPack = new BurnerPack[4];
    [SerializeField] private bool hasBrokenBurners = true;

    const int BURNERS_COUNT = 4;
    private List<bool> burnerStates;
    private int burnersWithGas = 0;
    private int burningBurners = 0;
    private enum LoopState { None, Gas, Burning }
    private LoopState currentLoop = LoopState.None;


    void Start()
    {
        sound = GetComponent<StoveSoundManager>();
        if (!sound) Debug.LogError("Stove: StoveSoundManager link is not set.");
        
        if (hasBrokenBurners) SetBrokenBurners();

        foreach (var pack in _burnerPack)
        {
            if (!pack.burner)
            {
                Debug.LogError("Stove: Burner link is not set.");
                continue;
            }

            Burner b = pack.burner;
            b.GasStateChanged += OnGasStateChanged;
            b.BurnStateChanged += OnBurnStateChanged;

            // init counters from start
            if (b.HasGas) burnersWithGas++;
            if (b.isLit) burningBurners++;
        }

        EvaluateLoop();
    }

    void OnGasStateChanged(bool hasGas)
    {
        burnersWithGas += hasGas ? 1 : -1;
        if (burnersWithGas < 0) burnersWithGas = 0;
        EvaluateLoop();
    }

    void OnBurnStateChanged(Burner b, bool isLit, bool brokenAttempt)
    {
        if (brokenAttempt)
        {
            sound.PlayBrokenIgnition();
            return;
        }

        // play ignition
        bool firstLitBefore = (burningBurners == 0);
        if (isLit)
        {
            burningBurners++;
            sound.PlayBurning(firstLitBefore);   // if first - plays burning loop
        }
        else if (burningBurners > 0) burningBurners--;

        EvaluateLoop();
    }

    /* ------------------- Главная функция выбора лупа ------------------- */
    void EvaluateLoop()
    {
        // at least one burning
        if (burningBurners > 0)
        {
            if (currentLoop != LoopState.Burning)
            {
                // insurance for a case, if burning loop isn't playing
                // if so, it will start anyway
                sound.PlayBurning(false);
                currentLoop = LoopState.Burning;
            }
            return;
        }

        // no burning, but at least 1 has gas
        if (burnersWithGas > 0)
        {
            if (currentLoop != LoopState.Gas)
            {
                sound.PlayGas();
                currentLoop = LoopState.Gas;
            }
            return;
        }

        // no burning and no gas
        if (currentLoop != LoopState.None)
        {
            sound.StopLoop();
            currentLoop = LoopState.None;
        }
    }

    void SetBrokenBurners()
    {
        int count = BURNERS_COUNT;

        // at least one burner should be in order, and one broken:
        burnerStates = new List<bool> { false, false, true };
        for (int i = burnerStates.Count; i < count; i++)
        {
            burnerStates.Add(UnityEngine.Random.value > 0.5);
        }

        // mix randomly burner states
        for (int i = 0; i < count; i++)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (burnerStates[i], burnerStates[j]) = (burnerStates[j], burnerStates[i]);
        }

        // set broken states
        for (int i = 0; i < count; i++)
        {
            if (!_burnerPack[i].burner || !_burnerPack[i].handle)
            {
                Debug.Log($"Stove: burner #{i} links are not set.");
                continue;
            }
            _burnerPack[i].burner.IsBroken = burnerStates[i];
        }
    }
}
