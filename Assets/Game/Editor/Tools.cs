using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class Tools
    {
        [MenuItem( "Tools/Clear Prefs")]
        public static void ClearPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}
