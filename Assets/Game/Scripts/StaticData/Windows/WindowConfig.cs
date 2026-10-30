using System;
using Game.Scripts.UI.Services.Windows;
using Game.Scripts.UI.Windows;

namespace Game.Scripts.StaticData.Windows
{
    /// <summary>
    /// Какой префаб у окна. Template — тип WindowBase: в поле встаёт только префаб с наследником WindowBase на корне.
    /// </summary>
    [Serializable]
    public class WindowConfig
    {
        public WindowId WindowId;
        public WindowBase Template;
    }
}
