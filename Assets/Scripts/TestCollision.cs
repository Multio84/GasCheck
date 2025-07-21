using UnityEngine;

public class TestCollision : MonoBehaviour
{
    public string logAuthor;

    void OnTriggerEnter(Collider c)
    {
        Debug.Log($"{logAuthor}: Lighter touched by " + c.name);
    }
}
