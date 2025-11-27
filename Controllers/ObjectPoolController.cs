using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectPoolController : MonoBehaviour
{
    public static ObjectPoolController instance;
    public Transform painelObjetosInstanciados;

    [System.Serializable] // FAZ COM QUE A LISTA DE POOL SEJA VISIVEL NO INSPECTO 
    public class Pool // O TIPO DE OBJECTO A SER GERENCIADO PELO POOL
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> polls; //A LISTA DE POOLS PARA SER ADICIONADO
    public Dictionary<string, Queue<GameObject>> poolDictionary; // QUAL A CHAVE QUE ELE USARA PARA PROCURAR QUE VAI SER A TAG, E QUAL A FILA QUE SERA USADA

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        SettingsManager.ZombieDensity globalDensity = SettingsManager.instance.currentZombieDensity;

        foreach (Pool pool in polls)
        {
            DensidadeZombie(pool, globalDensity);
            
            Queue<GameObject> objectpool = new Queue<GameObject>(); // CRIA UMA FILA

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab); // INSTANCIA A QUANTIDADE DEFINIDA (SIZE)
                obj.transform.SetParent(painelObjetosInstanciados);
                obj.SetActive(false);
                objectpool.Enqueue(obj); // COLOCA NA FILA OBJETOS EM ORDEM ESPECIFICA DE FILA DO PRIMEIRO AO ULTIMO
            }

            poolDictionary.Add(pool.tag, objectpool); // ADICIONA AO DICIONARIO
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Tag nao encontrada" + tag);
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Tag nao encontrada" + tag);
            return;
        }

        // 1. Desativa o objeto
        objectToReturn.SetActive(false);

        // 2. Coloca ele de volta na fila
        poolDictionary[tag].Enqueue(objectToReturn);
    }

    #region ZombiePool

    public GameObject SpawnZumbiFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Tag nao encontrada" + tag);
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // 1. TELEPORTA o zumbi (que ainda está INATIVO)
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // 2. AGORA "acorda" o zumbi.
        objectToSpawn.SetActive(true);

        // 3. (BÔNUS DE SEGURANÇA)
        // Avisa manualmente ao NavMeshAgent "Ei, você acordou AQUI"
        NavMeshAgent agent = objectToSpawn.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.Warp(position);
        }

        return objectToSpawn;
    }

    private void DensidadeZombie(Pool pool, SettingsManager.ZombieDensity globalDensity)
    {
        if (pool.tag == "Zumbi")
        {
            switch (globalDensity)
            {
                case SettingsManager.ZombieDensity.Baixa:
                    pool.size = ZombieSpawnManager.Instance.quantidadeZumbiBaixa + 20;
                    break;
                case SettingsManager.ZombieDensity.Media:
                    pool.size = ZombieSpawnManager.Instance.quantidadeZumbiMedia + 20;
                    break;
                case SettingsManager.ZombieDensity.Alta:
                    pool.size = ZombieSpawnManager.Instance.quantidadeZumbiAlta + 20;
                    break;
            }
        }
    }
    #endregion
}
