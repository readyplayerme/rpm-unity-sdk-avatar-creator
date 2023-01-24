using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AvatarCreatorExample
{
    public interface IPanel
    {
    }

    public abstract class SelectionScreen : MonoBehaviour, IPanel
    {
        public DataStore DataStore { get; set; }
        public bool IsSelected { get; set; }

        public GameObject Loading { get; set; }

        public async Task WaitForSelection(CancellationToken token)
        {
            while (!IsSelected && !token.IsCancellationRequested)
            {
                await Task.Yield();
            }
        }
    }
}
