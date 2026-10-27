using Game.Scripts.Infrastructure;

namespace Game.Scripts.Services.Randomizer.Interfaces
{
    public interface IRandomService : IService
    {
        /// <summary>
        /// Случайное целое от <paramref name="minValue"/> включительно до <paramref name="maxValue"/> не включительно.
        /// </summary>
        int Next(int minValue, int maxValue);
    }
}
