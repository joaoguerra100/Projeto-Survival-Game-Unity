using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    public float moveSpeed = 5f;        // Velocidade normal
    public float boostMultiplier = 3f;  // Multiplicador de velocidade com Shift
    public float lookSpeed = 2f;        // Velocidade de rotação

    float rotationX = 0f;
    float rotationY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Calcula velocidade atual (normal ou com boost)
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= boostMultiplier;
        }

        // Movimento
        float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime; // A/D
        float moveZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;   // W/S

        transform.Translate(moveX, 0, moveZ);

        // Rotação com o mouse
        rotationX += Input.GetAxis("Mouse X") * lookSpeed;
        rotationY -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotationY, rotationX, 0);
    }
}
