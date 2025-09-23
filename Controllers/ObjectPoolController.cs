using System.Collections.Generic;
using UnityEngine;

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

        foreach (Pool pool in polls)
        {
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
        
        // PEGA UM OBJETO DA FILA, ATIVA, POSICIONA, ROTACIONA, RECOLOCA NO FINAL DA FILA PARA SER USADO FUTURAMENTE

        GameObject objectToSpawn = poolDictionary[tag].Dequeue(); // RETIRA OS OBJETOS EM ORDEM ESPECIFICA DE FILA DO PRIMEIRO AO ULTIMO

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }


}
