using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.StaticData.Windows
{
    /// <summary>
    /// Окна игры как данные (урок 13): новый экран — строка в списке, а не код.
    /// Ассет — Resources/StaticData/UI/WindowStaticData (грузит StaticDataService.Load).
    /// </summary>
    [CreateAssetMenu(fileName = "WindowStaticData", menuName = "StaticData/Window static data")]
    public class WindowStaticData : ScriptableObject
    {
        public List<WindowConfig> Configs;
    }
}
