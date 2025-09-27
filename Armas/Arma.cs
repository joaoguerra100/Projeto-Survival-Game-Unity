using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arma : MonoBehaviour
{
    #region VARIAVEIS

    [Header("Referencias")]
    public FireWeaponInstance equipedWeapon;
    public WeaponItemScriptable weaponData; 
    private Animator anim;

    [Header("ModoDeDisparo")]
    [SerializeField]private GameObject semi_auto;
    [SerializeField]private GameObject full_auto;

    [Header("Prefabs")]
    //public GameObject bulletPrefab;
    public GameObject capsulaPrefab;

    [Header("Locais de Spawn")]
    [SerializeField]private Transform shotSpawnSemMirar;
    [SerializeField]private Transform shotSpawnMirando;
    [SerializeField]private Transform capsuleEjector;
    [SerializeField]private Transform muzzleEfectPos;

    [Header("Configuraçao Dos Disparos")]
    //[HideInInspector] public float forcadotiro = 700f;
    [SerializeField] private float tempoDisparoS_Mirar;
    [HideInInspector] public float nextFireTime = 0;
    [HideInInspector] public bool ligarGzimos;
    private Coroutine coroutineRecarga;

    [Header("SomFx")]
    [SerializeField] private AudioClip tiroFx;
    public AudioClip reloadFx;
    public AudioClip engatilharFx;
    [SerializeField] private LayerMask layerInimigo;
    [HideInInspector]public AudioSource audioSourceArma;

    #endregion

    #region METODOS INICIAS

    void OnEnable()
    {
        if (InventoryManagerController.instance.equipedWeapon != null)
        {
            equipedWeapon = InventoryManagerController.instance.equipedWeapon;
            Hud.instance.qtdMunicaoGo.SetActive(true);
            AtualizarModoDeTiroVisual();
        }
    }

    void OnDisable()
    {
        semi_auto.SetActive(false);
        full_auto.SetActive(false);
        Hud.instance.qtdMunicaoGo.SetActive(false);
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        audioSourceArma = GetComponent<AudioSource>();
    }

    void Update()
    {
        VerificarMunicaoParaRecarregar();
        VerificarInterrupcaoRecarga();
        Hud.instance.AtualizarContadorDeMuniçao(equipedWeapon.currentAmmo, weaponData.municaoParaRecarregar);
    }

    #endregion

    #region  Atirar

    public void Atirar()
    {
        bool mirando = Input.GetKey(InputManager.instance.aimKey);
        bool apertandoBotao = weaponData.modoDeTiro == ModoDeTiro.SEMIAUTOMATICA ? Input.GetKeyDown(InputManager.instance.fireKey) : Input.GetKey(InputManager.instance.fireKey);

        PlayerBracos.instance.anim.SetBool("Mirando", mirando);
        PlayerBracos.instance.crosshairPanel.SetActive(!mirando);

        if (apertandoBotao)
        {
            if (equipedWeapon.currentAmmo > 0 && Time.time > nextFireTime)
            {
                float fireRate = weaponData.modoDeTiro == ModoDeTiro.SEMIAUTOMATICA
                    ? weaponData.fireRateSingle
                    : weaponData.fireRateAutomatic;

                nextFireTime = Time.time + (1f / fireRate);

                // ANIMAÇÃO
                if (mirando)
                {
                    if (weaponData.modoDeTiro == ModoDeTiro.SEMIAUTOMATICA)
                        PlayerBracos.instance.anim.SetTrigger("AtirarMirando");
                    else
                        PlayerBracos.instance.anim.SetBool("AtirandoMirando", true);
                }
                else
                {
                    if (weaponData.sniper) return; // não atira sem mirar
                    if (weaponData.modoDeTiro == ModoDeTiro.SEMIAUTOMATICA)
                        PlayerBracos.instance.anim.SetTrigger("AtirarSemMirar");
                    else
                        PlayerBracos.instance.anim.SetBool("Atirando", true);
                }

                // TIRO
                if (mirando)
                    AtirarBala(shotSpawnMirando);
                else
                    StartCoroutine(IEAtirarSemMirar());
            }
            else if (equipedWeapon.currentAmmo <= 0)
            {
                TiroVazioFX();
            }
        }
        else // Soltou botão ou não está atirando
        {
            if (weaponData.modoDeTiro == ModoDeTiro.AUTOMATICA)
            {
                PlayerBracos.instance.anim.SetBool("Atirando", false);
                PlayerBracos.instance.anim.SetBool("AtirandoMirando", false);
            }
        }
    }
    #endregion

    #region ModoDeDisparo

    public void TrocarModoDeTiro()
    {
        if (weaponData.podeTrocarModoTiro == true && Input.GetKeyDown(InputManager.instance.switchFireModeKey))
        {
            weaponData.modoDeTiro = weaponData.modoDeTiro == ModoDeTiro.AUTOMATICA ? ModoDeTiro.SEMIAUTOMATICA : ModoDeTiro.AUTOMATICA;
            AudioManager.instance.TocarFx(AudioManager.instance.trocarModoDeTiro);
            AtualizarModoDeTiroVisual();
        }
    }

    void AtualizarModoDeTiroVisual()
    {
        switch (weaponData.modoDeTiro)
        {
            case ModoDeTiro.SEMIAUTOMATICA:
                semi_auto.SetActive(true);
                full_auto.SetActive(false);
                break;

            case ModoDeTiro.AUTOMATICA:
                semi_auto.SetActive(false);
                full_auto.SetActive(true);
                break;
        }
    }

    #endregion

    #region Disparo
    public void OnDrawGizmos()
    {
        if (ligarGzimos)
        {
            Camera cam = Camera.main;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 shootDirection = ray.direction;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(shotSpawnMirando.position, shotSpawnMirando.position + shootDirection * weaponData.bulletRange);
        }
    }

    /*//PARTE DO SCRIPT PARA CASO FOR FAZER ANIMAÇOES DE ARCO E BESTA
    public void AtirarBalaMirando()
    {
        anim.SetTrigger("Atirar");
        GameObject bala = Instantiate(bulletPrefab, shotSpawnMirando.position, shotSpawnMirando.rotation);
        bala.GetComponent<Rigidbody>().linearVelocity = shotSpawnMirando.transform.forward * forcadotiro;
        municao -= 1;
    }

    public void AtirarBalaSemMirar()
    {
        anim.SetTrigger("Atirar");
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 shootDirection = ray.direction;

        GameObject bala = Instantiate(bulletPrefab, shotSpawnSemMirar.position, Quaternion.LookRotation(shootDirection));
        bala.GetComponent<Rigidbody>().linearVelocity = shootDirection * forcadotiro;
        municao -= 1;

    }*/

    void AtirarBala(Transform posicao)
    {
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 shootDirection = ray.direction;

        RaycastHit hit;
        LayerMask mask = ~LayerMask.GetMask("UI");
        var collider = Physics.Raycast(posicao.position, shootDirection, out hit, weaponData.bulletRange, mask);
        GameObject bulletTracer = ObjectPoolController.instance.SpawnFromPool("TrailTracerEffect", posicao.position, posicao.rotation);
        bulletTracer.GetComponent<ProjetilTracante>().Tracante(shootDirection);

        MuzzleEffect();
        TiroFX();

        Collider[] hits = Physics.OverlapSphere(transform.position, audioSourceArma.maxDistance, layerInimigo);
        foreach (Collider col in hits)
        {
            Zumbi zumbi = col.GetComponentInParent<Zumbi>();
            if (zumbi != null)
            {
                zumbi.InvestigarArea(transform.position);
            }
        }

        anim.SetTrigger("Atirar");
        equipedWeapon.currentAmmo -= 1;
        if (collider)
        {
            switch (hit.transform.tag)
            {
                case "Concreto":
                    ObjectPoolController.instance.SpawnFromPool("Concreto", hit.point + (hit.normal * 0.01f), Quaternion.LookRotation(hit.normal));
                    ObjectPoolController.instance.SpawnFromPool("Impacto-Concreto", hit.point, Quaternion.LookRotation(hit.normal));
                    break;
                case "Grama":
                    ObjectPoolController.instance.SpawnFromPool("Grama", hit.point + (hit.normal * 0.01f), Quaternion.LookRotation(hit.normal));
                    ObjectPoolController.instance.SpawnFromPool("Impacto-Grama", hit.point, Quaternion.LookRotation(hit.normal));
                    break;
                case "Metal":
                    ObjectPoolController.instance.SpawnFromPool("Metal", hit.point + (hit.normal * 0.01f), Quaternion.LookRotation(hit.normal));
                    ObjectPoolController.instance.SpawnFromPool("Impacto-Metal", hit.point, Quaternion.LookRotation(hit.normal));
                    break;
                case "Lama":
                    ObjectPoolController.instance.SpawnFromPool("Lama", hit.point + (hit.normal * 0.01f), Quaternion.LookRotation(hit.normal));
                    ObjectPoolController.instance.SpawnFromPool("Impacto-Lama", hit.point, Quaternion.LookRotation(hit.normal));
                    break;
                case "Madeira":
                    ObjectPoolController.instance.SpawnFromPool("Madeira", hit.point + (hit.normal * 0.01f), Quaternion.LookRotation(hit.normal));
                    ObjectPoolController.instance.SpawnFromPool("Impacto-Madeira", hit.point, Quaternion.LookRotation(hit.normal));
                    break;
                case "Agua":
                    //ObjectPoolController.instance.SpawnFromPool("Impacto-Agua", hit.point, Quaternion.LookRotation(hit.normal));
                    break;
                case "Inimigo":
                    ObjectPoolController.instance.SpawnFromPool("Impacto-Inimigo", hit.point, Quaternion.LookRotation(hit.normal));
                    hit.transform.SendMessage("Danos", weaponData.danoMunicao, SendMessageOptions.DontRequireReceiver);
                    if (hit.collider.TryGetComponent(out BodyPartHitbox hitbox))
                    {
                        hitbox.zumbi.ReceberHit(hitbox.parteDoCorpo);
                    }
                    break;
            }
        }
    }

    IEnumerator IEAtirarSemMirar()
    {
        yield return new WaitForSeconds(tempoDisparoS_Mirar);
        AtirarBala(shotSpawnSemMirar);
        //TiroFX();
        //yield return new WaitForSeconds(tempoDisparoS_Mirar);
        //MuzzleEffect();
    }
    #endregion

    #region Reload

    public void CarregarArma()
    {
        if (Player.instance.bloqueioControle == false && Player.instance.trocaAnimator == false)
        {
            if (Input.GetKeyDown(InputManager.instance.reloadKey))
            {
                if (equipedWeapon.currentAmmo == weaponData.municaoMaxima || weaponData.municaoParaRecarregar == 0)
                    return;

                if (weaponData.tipoDeRecarga == TipoDeRecarga.Cartucho)
                {
                    PlayerBracos.instance.anim.SetTrigger("Carregar"); // recarga normal
                    PlayerBracos.instance.carregandoArma = true;
                    PlayerBracos.instance.anim.SetBool("Mirando", false);
                }
                else if (weaponData.tipoDeRecarga == TipoDeRecarga.BalaPorBala)
                {
                    IniciarRecargaBalaPorBala();
                    PlayerBracos.instance.carregandoArma = true;
                    PlayerBracos.instance.anim.SetBool("Mirando", false);
                }
            }
        }
    }

    void IniciarRecargaBalaPorBala()
    {
        if (coroutineRecarga == null && equipedWeapon.currentAmmo < weaponData.municaoMaxima && weaponData.municaoParaRecarregar > 0)
        {
            coroutineRecarga = StartCoroutine(RecarregarBalaPorBala());
        }
    }

    IEnumerator RecarregarBalaPorBala()
    {
        PlayerBracos.instance.anim.SetTrigger("ComecarRecargaBala");
        yield return new WaitForSeconds(weaponData.tempoCarregarInicio); // Tempo da animação de início

        while (equipedWeapon.currentAmmo < weaponData.municaoMaxima && weaponData.municaoParaRecarregar > 0)
        {
            PlayerBracos.instance.anim.ResetTrigger("ComecarRecargaBala");
            PlayerBracos.instance.anim.SetTrigger("LoopRecargaBala");
            yield return new WaitForSeconds(weaponData.tempoCarregarLoop); // Tempo da animação de colocar uma bala
        }

        PlayerBracos.instance.anim.ResetTrigger("LoopRecargaBala");
        PlayerBracos.instance.anim.SetTrigger("FinalizarRecargaBala");
        yield return new WaitForSeconds(weaponData.tempoCarregarFim); // Tempo da animação final
        PlayerBracos.instance.anim.ResetTrigger("FinalizarRecargaBala");
        coroutineRecarga = null;
        PlayerBracos.instance.carregandoArma = false;
    }

    void VerificarInterrupcaoRecarga()
    {
        if (PlayerBracos.instance.carregandoArma && Input.GetKeyDown(InputManager.instance.fireKey) && weaponData.tipoDeRecarga == TipoDeRecarga.BalaPorBala)
        {
            if (coroutineRecarga != null)
            {
                StopCoroutine(coroutineRecarga);
                coroutineRecarga = null;
            }

            PlayerBracos.instance.anim.SetTrigger("FinalizarRecargaBala");
            PlayerBracos.instance.carregandoArma = false;
        }
    }

    public void VerificarMunicao()
    {
        var difBullets = weaponData.municaoMaxima - equipedWeapon.currentAmmo;

        if (difBullets > weaponData.municaoParaRecarregar)
        {
            equipedWeapon.currentAmmo += weaponData.municaoParaRecarregar;
            InventoryManagerController.instance.UseAmmunition(weaponData.tipoDeMunicao, weaponData.municaoParaRecarregar);
        }
        else
        {
            equipedWeapon.currentAmmo += difBullets;
            InventoryManagerController.instance.UseAmmunition(weaponData.tipoDeMunicao, difBullets);
        }
        weaponData.municaoParaRecarregar = Mathf.Clamp(weaponData.municaoParaRecarregar -= difBullets, 0, weaponData.municaoParaRecarregar);
    }

    void VerificarMunicaoParaRecarregar()
    {
        int quantidadeTotal = 0;
        foreach (var bag in InventoryManagerController.instance.TodasAsBags())
        {
            InventoryItem resultItem = bag.FindItemByTipoDeMunicao(weaponData.tipoDeMunicao);
            if (resultItem != null)
            {
                quantidadeTotal += resultItem.baseItemData.CurrentNumber;
            }
        }
        
        weaponData.municaoParaRecarregar = quantidadeTotal;
    }

    #endregion

    #region Efeitos

    void MuzzleEffect( )
    {
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        ObjectPoolController.instance.SpawnFromPool("MuzzleEfect", muzzleEfectPos.position, Quaternion.LookRotation(ray.direction));
    }

    

    #endregion

    #region FX

    void TiroFX()
    {
        audioSourceArma.clip = tiroFx;
        audioSourceArma.PlayOneShot(tiroFx);

    }
    void TiroVazioFX()
    {
        AudioManager.instance.TocarFx(AudioManager.instance.semMunicao);
    }

    #endregion

    #region Animation Event

    public void CapsulaEjeting()
    {
        if (equipedWeapon.currentAmmo > 0)
        {
            Instantiate(capsulaPrefab, capsuleEjector.position, capsuleEjector.rotation);
        }
    }
    public void AdicionarUmaBala()
    {
        if (equipedWeapon.currentAmmo < weaponData.municaoMaxima && weaponData.municaoParaRecarregar > 0)
        {
            equipedWeapon.currentAmmo++;
            weaponData.municaoParaRecarregar--;
            InventoryManagerController.instance.UseAmmunition(weaponData.tipoDeMunicao, 1);
        }
    }

    #endregion

    #region Helpers Methods

    private List<BagScriptable> GetTodasAsBagsConvertidas()
    {
        List<BagScriptable> todas = new List<BagScriptable>();

        if (InventoryManagerController.instance.mochilaBag is BagScriptable m) todas.Add(m);
        if (InventoryManagerController.instance.blusaBag is BagScriptable c) todas.Add(c);
        if (InventoryManagerController.instance.calcaBag is BagScriptable b) todas.Add(b);
        if (InventoryManagerController.instance.vesteBag is BagScriptable h) todas.Add(h);

        return todas;
    }

    #endregion

}
