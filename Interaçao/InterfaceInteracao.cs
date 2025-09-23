using UnityEngine;

public interface InterfaceInteracao
{
    KeyCode BotaoDeInteracao { get; }
    public void Interagir();
    public void MostarBotaoInteraçao(bool mostrar);
}
