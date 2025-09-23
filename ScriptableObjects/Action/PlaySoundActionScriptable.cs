using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlaySoundAction", menuName = "Action/NewPlaySoundAction")]
public class PlaySoundActionScriptable : GenericActionScriptable
{
    [SerializeField]private AudioClip audioFile;

    public override IEnumerator Execute()
    {
        yield return new WaitForSeconds(DelayToStart);

        if(audioFile != null)
        {
            AudioManager.instance.TocarFx(audioFile);
        }
        else
        {
            Debug.LogWarning("Nao tem Arquivo de audio neste PlaySound Scriptable Object");
        }
    }


    
}
