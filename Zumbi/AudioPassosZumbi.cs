using System.Collections.Generic;
using UnityEngine;

public class AudioPassosZumbi : MonoBehaviour
{
    public AudioSource sourcePassos;
    private AudioClip clipes;

    public List<AudioClip> concretoPassos;
    public List<AudioClip> gramaPassos;
    public List<AudioClip> metalPassos;
    public List<AudioClip> lamaPassos;
    public List<AudioClip> madeiraPassos;
    public List<AudioClip> aguaPassos;

    public List<AudioClip> terraPassos;
    public List<AudioClip> cascalhoPassos;
    public List<AudioClip> pedraPassos;
    public List<AudioClip> areiaPassos;



    public List<AudioClip> listaAtual;

    void Start()
    {
        if (concretoPassos.Count > 0)
        {
            listaAtual = concretoPassos;
        }
    }

    public void Passos()
    {
        clipes = listaAtual[Random.Range(0, listaAtual.Count)];
        sourcePassos.PlayOneShot(clipes);
    }
    void OnCollisionEnter(Collision col)
    {
        switch (col.gameObject.tag)
        {
            case "Concreto":
                listaAtual = concretoPassos;
                break;
            case "Grama":
                listaAtual = gramaPassos;
                break;
            case "Metal":
                listaAtual = metalPassos;
                break;
            case "Lama":
                listaAtual = lamaPassos;
                break;
            case "Madeira":
                listaAtual = madeiraPassos;
                break;
            case "Agua":
                listaAtual = aguaPassos;
                break;
            case "Terra":
                listaAtual = terraPassos;
                break;
            case "Cascalho":
                listaAtual = cascalhoPassos;
                break;
            case "Pedra":
                listaAtual = pedraPassos;
                break;
            case "Areia":
                listaAtual = areiaPassos;
                break;
        }
    }
}
