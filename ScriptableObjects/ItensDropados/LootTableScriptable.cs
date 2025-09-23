using UnityEngine;

[CreateAssetMenu(fileName = "LootTable", menuName = "LootTable/NewLootTable")]
public class LootTableScriptable : ScriptableObject
{
    public string nomeContainer;
    public InventoryItem[] possiveisItens;
}
