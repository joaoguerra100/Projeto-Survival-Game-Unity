using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewChangeCharAction", menuName = "Action/NewChangeCharAction")]
public class ChangeCharPropertiesActionScriptable : GenericActionScriptable
{
    #region Declaration
    [SerializeField][Range(0,100)]private float comida;
    [SerializeField][Range(0,100)]private float agua;
    [SerializeField][Range(0,100)]private float vida;
    [SerializeField] private bool pararSangramento;
    [SerializeField]private bool pararDor;
    [SerializeField]private bool curarPernaQuebrada;
    #endregion

    #region Methods

    public override IEnumerator Execute()
    {
        yield return new WaitForSeconds(DelayToStart);
        if(comida != 0)
        {
            GameController.instance.ChangeFome(comida);
        }
        if(agua != 0)
        {
            GameController.instance.ChangeSede(agua);
        }
         if(vida != 0)
        {
            GameController.instance.ChangeVida(vida);
        }
        if (pararSangramento)
        {
            Player.instance.anim.SetTrigger("UsarBandagem");
            PlayerBracos.instance.anim.SetTrigger("UsarBandagem");
        }
        if(pararDor)
        {
            //GameController => Funcao();
        }
        if(curarPernaQuebrada)
        {
            //GameController => Funcao();
        }
    }

    #endregion
    
}
