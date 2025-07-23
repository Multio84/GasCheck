using UnityEngine;


public class Match : MonoBehaviour
{
    [SerializeField] float strikeDistance = 0.025f;   // 2–3 см
    [SerializeField] GameObject fire;
    public bool isLit = false;

    bool touching = false;
    Transform lighter;      // коробка
    Vector3 startLocalPos;  // где был кончик при первом касании
    Vector3 surfaceNormal;  // нормаль площадки (для отбрасывания «углубления»)


    void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Entered trigger {other.name}");

        if (isLit || !other.CompareTag("Lighter")) return;

        Debug.Log("Entered Lighter");

        touching = true;
        lighter = other.transform;
        startLocalPos = lighter.InverseTransformPoint(transform.position);

        /*  У триггера нет ContactPoint, поэтому нормаль задаём вручную.
         *  Пусть нормаль «снаружи» – это локальная +Z  light-era.
         *  При другом расположении выберите другую ось или
         *  храните в самом light-ере скрипт с нужным Vector3.              */
        surfaceNormal = lighter.forward;
    }

    void OnTriggerStay(Collider other)
    {
        if (!touching || other.transform != lighter) return;

        // текущая позиция головки в локальных координатах пятачка
        Vector3 curLocal = lighter.InverseTransformPoint(transform.position);
        Vector3 delta = curLocal - startLocalPos;

        // отбрасываем «вдавливание» вдоль нормали
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
        Debug.Log("Match is lighted!");
    }
}


//{
//    [Header("Setup")]
//    [SerializeField] Collider tipCollider;            // коллайдер кончика спички
//    [SerializeField] float strikeDistance = 0.025f;   // 2.5 см – сколько нужно прочесать

//    [Header("Runtime")]
//    public bool IsLighted { get; private set; }

//    // --- внутреннее состояние ---
//    bool touching;      // кончик сейчас трётся о коробок?
//    float traveled;     // накопленное расстояние
//    Transform lighter;    // тот самый Lighter, по которому чиркаем
//    Vector3 lastLocalPos;   // позиция кончика в локальной СК коробка
//    Vector3 surfaceNormalWorld; // нормаль площадки, фиксируется при первом контакте


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

//    /* -------------- ЛОГИКА -------------- */

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

//        // Переводим приращение в мир, чтобы вычеркнуть компонент вдоль нормали
//        Vector3 deltaWorld = lighter.TransformVector(deltaLocal);
//        Vector3 tangential = Vector3.ProjectOnPlane(deltaWorld, surfaceNormalWorld);
//        traveled += tangential.magnitude;

//        if (traveled >= strikeDistance)
//            LightUp();
//    }

//    void ResetStrike()
//    {
//        touching = false;
//        traveled = 0f;
//        lighter = null;
//    }

//    void LightUp()
//    {
//        IsLighted = true;
//        touching = false;

//        Debug.Log("Match is lighted!");
//    }
//}
