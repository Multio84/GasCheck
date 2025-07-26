using UnityEngine;


public class Match : MonoBehaviour
{
    [SerializeField] float strikeDistance = 0.02f;   // 2Ц3 см
    [SerializeField] GameObject fire;
    public bool isLit = false;

    bool touching = false;
    Transform lighter;      // 
    Vector3 startLocalPos;  // где был кончик при первом касании
    Vector3 surfaceNormal;  // нормаль площадки (дл€ отбрасывани€ Ђуглублени€ї)


    void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Entered trigger {other.name}");

        if (isLit || !other.CompareTag("Lighter")) return;

        touching = true;
        lighter = other.transform;
        startLocalPos = lighter.InverseTransformPoint(transform.position);

        surfaceNormal = lighter.forward;
    }

    void OnTriggerStay(Collider other)
    {
        if (!touching || other.transform != lighter) return;

        // текуща€ позици€ головки в локальных координатах п€тачка
        Vector3 curLocal = lighter.InverseTransformPoint(transform.position);
        Vector3 delta = curLocal - startLocalPos;

        // отбрасываем Ђвдавливаниеї вдоль нормали
        Vector3 tangential =
            Vector3.ProjectOnPlane(lighter.TransformVector(delta), surfaceNormal);

        if (tangential.magnitude >= strikeDistance)
            LightUp();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == lighter)
            touching = false;
    }

    public void LightUp()
    {
        isLit = true;
        touching = false;
        fire.SetActive(true);
    }
}


//{
//    [Header("Setup")]
//    [SerializeField] Collider tipCollider;            // коллайдер кончика спички
//    [SerializeField] float strikeDistance = 0.025f;   // 2.5 см Ц сколько нужно прочесать

//    [Header("Runtime")]
//    public bool IsLighted { get; private set; }

//    // --- внутреннее состо€ние ---
//    bool touching;      // кончик сейчас трЄтс€ о коробок?
//    float traveled;     // накопленное рассто€ние
//    Transform lighter;    // тот самый Lighter, по которому чиркаем
//    Vector3 lastLocalPos;   // позици€ кончика в локальной —  коробка
//    Vector3 surfaceNormalWorld; // нормаль площадки, фиксируетс€ при первом контакте


//    void OnCollisionEnter(Collision col)
//    {
//        if (IsLighted || !col.collider.CompareTag("Lighter")) return;

//        BeginStrike(col.collider.transform, col.GetContact(0).normal);
//    }


//    void OnCollisionStay(Collision col)
//    {
//        if (touching && col.collider.transform == lighter)
//            ContinueStrike();
//    }

//    void OnCollisionExit(Collision col)
//    {
//        if (touching && col.collider.transform == lighter)
//            ResetStrike();
//    }

//    /* -------------- Ћќ√» ј -------------- */

//    void BeginStrike(Transform lighter, Vector3 normalWorld)
//    {
//        this.lighter = lighter;
//        surfaceNormalWorld = normalWorld.normalized;

//        lastLocalPos = this.lighter.InverseTransformPoint(tipCollider.transform.position);
//        traveled = 0f;
//        touching = true;
//    }

//    void ContinueStrike()
//    {
//        Vector3 curLocal = lighter.InverseTransformPoint(transform.position);
//        Vector3 deltaLocal = curLocal - lastLocalPos;
//        lastLocalPos = curLocal;

//        // ѕереводим приращение в мир, чтобы вычеркнуть компонент вдоль нормали
//        Vector3 deltaWorld = lighter.TransformVector(deltaLocal);
//        Vector3 tangential = Vector3.ProjectOnPlane(deltaWorld, surfaceNormalWorld);
//        traveled += tangential.magnitude;

//        if (traveled >= strikeDistance)
//            UpdateBurning();
//    }

//    void ResetStrike()
//    {
//        touching = false;
//        traveled = 0f;
//        lighter = null;
//    }

//    void UpdateBurning()
//    {
//        IsLighted = true;
//        touching = false;

//        Debug.Log("Match is lighted!");
//    }
//}
