using System.Collections.Generic;
using UnityEngine;

public class LootCorpos : MonoBehaviour
{
    [Header("Inventario do cadaver")]
    public List<InventoryItem> itensNoInventario = new List<InventoryItem>();
    private bool lootGerado = false;
    
    public void GerarLootInicial(LootTableScriptable tabela)
    {
        if (lootGerado || tabela == null) return;

        itensNoInventario = tabela.GerarLoot();

        lootGerado = true;

       /*  if(itensNoInventario.Count > 0)
        {
            foreach (var item in itensNoInventario)
            {
                Debug.Log($"Cadáver gerou {itensNoInventario.Count} itens: " + string.Join(", ", item.baseItemData.Label));
            }
        } */
    }
}
