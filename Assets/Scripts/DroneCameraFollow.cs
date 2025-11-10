using UnityEngine;

public class DroneCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 3, -8);
    public float followSpeed = 5f;
    public float rotateSpeed = 70f;
    public bool allowOrbit = true;

    void LateUpdate()
    {
        if (!target) return;

        // 🎯 Mantén distancia con suavidad
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // 👁️ Siempre mirar al dron
        transform.LookAt(target);

        // 🔄 Control orbital opcional (ratón o stick derecho)
        if (allowOrbit)
        {
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");

            if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
            {
                Quaternion camRot = Quaternion.Euler(v, h, 0);
                offset = camRot * offset;
            }
        }
    }
}
