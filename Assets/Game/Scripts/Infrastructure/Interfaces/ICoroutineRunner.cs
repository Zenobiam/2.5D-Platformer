using System.Collections;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Interfaces
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}
