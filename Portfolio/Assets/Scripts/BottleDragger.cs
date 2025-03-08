using UnityEngine;

public class BottleDragger : MonoBehaviour
{
    private Vector3 offset;
    private float zCoord;
    private Rigidbody rb;
    private Vector3 initialPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Asegurar configuración
    }

    void OnMouseDown()
    {
        zCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        rb.MovePosition(GetMouseWorldPos() + offset);
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseUp()
    {
        // Opcional: Añadir fuerza al soltar
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
        StartCoroutine(ResetBottle());
    }

    System.Collections.IEnumerator ResetBottle()
    {
        yield return new WaitForSeconds(2f);
        rb.isKinematic = true;
        transform.position = initialPosition; // Asegúrate de tener esta variable
    }
}