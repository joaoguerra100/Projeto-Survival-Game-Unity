using UnityEngine;

public class SensorMusica : MonoBehaviour
{

    public static bool sensorMusicaBatalha;

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Inimigo"))
        {
            sensorMusicaBatalha = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Inimigo"))
        {
            sensorMusicaBatalha = false;
        }
    }

}
