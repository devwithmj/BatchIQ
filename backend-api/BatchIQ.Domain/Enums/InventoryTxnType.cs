// InventoryTxnType enum
namespace BatchIQ.Domain.Enums
{
    public enum InventoryTxnType
    {
        Move,        // Location ➜ Location
        Produce,     // BOM-based manufacturing (mixed nuts, roasting)
        Package,     // Same product → smaller packs (bulk ➜ retail bag)
        Consume,     // Ingredient draw-down without output (spoilage, etc.)
        Adjust
    }

}
