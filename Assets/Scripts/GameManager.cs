using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [SerializeField] private Stove stove;
    [SerializeField] private GameObject ui;
    private readonly HashSet<Burner> checkedBurners = new();


    private void Awake()
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

    private void OnBurnStateChanged(Burner b, bool isLit, bool brokenAttempt)
    {
        if (checkedBurners.Contains(b)) return;

        checkedBurners.Add(b);
        if (checkedBurners.Count == stove._burnerPack.Length)
            ShowUI();
    }

    private void ShowUI()
    {
        ui.SetActive(true);
    }
}
