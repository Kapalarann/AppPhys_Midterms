using UnityEngine;

public class TrampolineLine : MonoBehaviour
{
    [SerializeField] float trampolineForce = 2f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionStay(Collision collision)
    {
        if(rb.linearVelocity.y < trampolineForce/10f) return;
        if (collision.rigidbody == null)
            return;
        if (collision.contacts[0].normal.y >= 0.9f)
            return;
        collision.rigidbody.AddForce(Vector3.up * trampolineForce, ForceMode.Impulse);
        Debug.Log(collision.gameObject.name);
    }
}