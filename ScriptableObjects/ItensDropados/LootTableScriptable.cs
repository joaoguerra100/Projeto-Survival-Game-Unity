using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootTable", menuName = "LootTable/NewLootTable")]
public class LootTableScriptable : ScriptableObject
{
    [System.Serializable]
    public class DropItem
    {
        public InventoryItem item;
        [Range(0, 100)] public float chanceDrop;
        [Range(0, 100)] public int quantidadeMax;
    }

    public List<DropItem> itensPossiveis;

    public List<InventoryItem> GerarLoot()
    {
        List<InventoryItem> itensSorteados = new List<InventoryItem>();

        foreach (var item in itensPossiveis)
        {
            if (Random.Range(0f, 100f) <= item.chanceDrop)
            {
                int qtd = Random.Range(1, item.quantidadeMax + 1);
                for (int i = 0; i < qtd; i++)
                {
                    itensSorteados.Add(item.item);
                }
            }
        }
        return itensSorteados;
    }
    public string nomeContainer;
    public InventoryItem[] possiveisItens;
}
