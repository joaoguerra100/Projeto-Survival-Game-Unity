using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraLook : MonoBehaviour
{
    [Header("Scripts")]
    public static CameraLook instance;

    public float yOffset, yOffsetAgaichado; // altura que vai ficar a camera
    public float moveDelay;
    public float sensitivity; //sensibilidade
    public float rotationLimit; //limite da rotaçao
    public float rotationDelay; // atraso que o corpo vai ter em relaçao a cabeça

    private float mouseX, mouseY;
    private float rotX, rotY;
    [SerializeField]private Transform playerTransform;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (InventoryView.instance.VisiblePanel == true || PauseController.instance.visiblePanel == true || GameOverController.instance.travarGameOver == true)
        {
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        rotX -= mouseY * sensitivity * Time.deltaTime;
        rotY += mouseX * sensitivity * Time.deltaTime;
        rotX = Mathf.Clamp(rotX, -rotationLimit, rotationLimit);

        transform.rotation = Quaternion.Euler(rotX, rotY, 0);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, Quaternion.Euler(0, rotY, 0), rotationDelay * Time.deltaTime);
    } 

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, playerTransform.position + playerTransform.up * (Player.instance.agaichado? yOffsetAgaichado : yOffset), moveDelay * Time.deltaTime);
    }
}
