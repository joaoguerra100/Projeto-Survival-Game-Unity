using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ZumbiAudioProfile
{
    public string nomePerfil;
    public List<AudioClip> idleSounds;
    public List<AudioClip> damageSounds;
    public List<AudioClip> deathSounds;
    public List<AudioClip> attackMaoSounds;
    //public List<AudioClip> attackMordidaSounds;
    public List<AudioClip> rageSounds;
    //public List<AudioClip> walkSounds;
    
}

public class ControladorAudioZumbi : MonoBehaviour
{
    public static ControladorAudioZumbi instance;
    [Header("Controladores")]
    public AudioSource sourceVozFx;
    private AudioClip clip;

    [SerializeField] private List<ZumbiAudioProfile> perfisDeAudio;
    private ZumbiAudioProfile perfilAtual;
    void Start()
    {

        if (perfisDeAudio.Count > 0)
        {
            perfilAtual = perfisDeAudio[Random.Range(0, perfisDeAudio.Count)];
        }
    
    }

    public void TocarIdle()
    {
        if (perfilAtual != null && perfilAtual.idleSounds.Count > 0)
        {
            clip = perfilAtual.idleSounds[Random.Range(0, perfilAtual.idleSounds.Count)];
            sourceVozFx.PlayOneShot(clip);
        }
    }
    public void TocarDamage()
    {
        if (perfilAtual != null && perfilAtual.damageSounds.Count > 0)
        {
            clip = perfilAtual.damageSounds[Random.Range(0, perfilAtual.damageSounds.Count)];
            sourceVozFx.PlayOneShot(clip);
        }
    }
    public void TocarDeath()
    {
        if (perfilAtual != null && perfilAtual.deathSounds.Count > 0)
        {
            clip = perfilAtual.deathSounds[Random.Range(0, perfilAtual.deathSounds.Count)];
            sourceVozFx.PlayOneShot(clip);
        }
    }
    
    public void TocarAttackMao()
    {
        if (perfilAtual != null && perfilAtual.attackMaoSounds.Count > 0)
        {
            clip = perfilAtual.attackMaoSounds[Random.Range(0, perfilAtual.attackMaoSounds.Count)];
            sourceVozFx.PlayOneShot(clip);
        }
    }

    /*public void TocarAttackMordida()
    {
        if (perfilAtual != null && perfilAtual.attackMordidaSounds.Count > 0)
        {
            clip = perfilAtual.attackMordidaSounds[Random.Range(0, perfilAtual.attackMordidaSounds.Count)];
            sourceVozFx.PlayOneShot(clip);
        }
    }*/

    public void TocarRage()
    {
        if (perfilAtual != null && perfilAtual.rageSounds.Count > 0)
        {
            clip = perfilAtual.rageSounds[Random.Range(0, perfilAtual.rageSounds.Count)];
            sourceVozFx.PlayOneShot(clip);
        }
    }

}
    
    

