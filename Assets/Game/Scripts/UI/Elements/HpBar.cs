using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Elements
{
    public class HpBar : MonoBehaviour
    {
        public Image ImageCurrent;

        public void SetValue(float current, float max) => 
            ImageCurrent.fillAmount = current / max;
    }
}