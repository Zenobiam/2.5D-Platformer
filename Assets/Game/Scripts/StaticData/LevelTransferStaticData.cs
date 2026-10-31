using System;
using UnityEngine;

namespace Game.Scripts.StaticData
{
    /// <summary>
    /// Переход с уровня (урок 15): куда ведёт (имя сцены) и где стоит триггер.
    /// Position собирает Collect по тегу LevelTransferInitialPoint, TransferTo вписывается в Inspector
    /// (первый раз — Tools → Setup → Second Level); фабрика передаёт его триггеру.
    /// </summary>
    [Serializable]
    public class LevelTransferStaticData
    {
        public string TransferTo;
        public Vector3 Position;

        public LevelTransferStaticData(string transferTo, Vector3 position)
        {
            TransferTo = transferTo;
            Position = position;
        }
    }
}
