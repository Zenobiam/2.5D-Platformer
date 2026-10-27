using Game.Scripts.Services.Randomizer.Interfaces;
using Random = UnityEngine.Random;

namespace Game.Scripts.Services.Randomizer
{
    public class RandomService : IRandomService
    {
        public int Next(int minValue, int maxValue) =>
            Random.Range(minValue, maxValue);
    }
}
