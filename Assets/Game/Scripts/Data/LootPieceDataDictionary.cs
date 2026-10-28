using System;

namespace Game.Scripts.Data
{
    /// <summary>
    /// Неподобранный лут сцены по UniqueId.Id.
    /// </summary>
    [Serializable]
    public class LootPieceDataDictionary : SerializableDictionary<string, LootPieceData>
    {
    }
}
