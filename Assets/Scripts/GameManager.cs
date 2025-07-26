using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [SerializeField] Stove stove;
    [SerializeField] GameObject ui;
    readonly HashSet<Burner> checkedBurners = new();


    void Awake()
    {
        if (!stove)
        {
            Debug.LogError("GameManager: Stove not found.");
            return;
        }
        foreach (var pack in stove._burnerPack)
            pack.burner.BurnStateChanged += OnBurnStateChanged;

        if (!ui)
        {
            Debug.LogError("GameManager: UI not found.");
            return;
        }
        ui.SetActive(false);
    }

    void OnBurnStateChanged(Burner b, bool isLit, bool brokenAttempt)
    {
        if (checkedBurners.Contains(b)) return;

        checkedBurners.Add(b);
        if (checkedBurners.Count == stove._burnerPack.Length)
            ShowUI();
    }

    void ShowUI()
    {
        ui.SetActive(true);
    }
}
