using System;
using UnityEngine;


/// <summary>
/// Универсальный вентиль/рукоятка, подписываемый на вращателей 
/// с целью получить сообщение о крайнем положении рукоятки и 
/// сообщить об этом всем слушателям.
/// </summary>
public class GasValve : MonoBehaviour
{
    [SerializeField] HandleRotator handle;
    [SerializeField] private bool _isOpen;
    public bool IsOpen => _isOpen;

    public event Action StateChanged;


    private void OnEnable()
    {
        if (!handle)
        {
            Debug.LogError($"GasValve {name}: handle link is not set!");
            return;
        }

        handle.StateChanged += SetState;
    }

    private void OnDisable()
    {
        handle.StateChanged -= SetState;
    }

    public void SetState(bool value)
    {
        if (_isOpen == value) return;
        _isOpen = value;

        StateChanged?.Invoke();
    }
}
