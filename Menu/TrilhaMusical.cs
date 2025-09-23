using UnityEngine;

public class TrilhaMusical : MonoBehaviour
{
    [Header("TRILHA MUSICAL")]
    public AudioSource sourceTrilha;
    public AudioClip[] clipMusica;
    public AudioClip[] clipBatalha;

    private bool som;
    void Update()
    {
        Cenario();
        Batalha();
    }

    void Cenario()
    {
        if (SensorMusica.sensorMusicaBatalha == false && som == false)
        {
            som = true;
            int indexClipMusica = Random.Range(0, clipMusica.Length);
            AudioClip clipesDeMusica = clipMusica[indexClipMusica];
            sourceTrilha.clip = clipesDeMusica;
            sourceTrilha.Play();
        }
    }

    void Batalha()
    {
        if (SensorMusica.sensorMusicaBatalha == true && som == true)
        {
            som = false;
            sourceTrilha.Stop();
            sourceTrilha.clip = clipBatalha[0];
            sourceTrilha.Play();
        }
    }    
}
