using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class ScreenSwitcher : MonoBehaviour
    {
        [SerializeField] private Button back;
        [SerializeField] private DataStore dataStore;
        [SerializeField] private GameObject loading;
        [SerializeField] private SelectionScreenBase[] screens;

        private int CurrentIndex
        {
            get => index;
            set
            {
                index = value;
                back.gameObject.SetActive(index != 0 && enabled);
            }
        }
        private int index;

        private int lastIndex;

        private CancellationTokenSource tokenSource;

        public void Start()
        {
            ShowScreenAndWaitForSelection();
        }

        private void OnEnable()
        {
            back.onClick.AddListener(Back);
        }

        private void OnDisable()
        {
            back.onClick.RemoveListener(Back);
        }

        private async void ShowScreenAndWaitForSelection()
        {
            screens[lastIndex].gameObject.SetActive(false);

            if (CurrentIndex >= screens.Length)
            {
                return;
            }

            tokenSource = new CancellationTokenSource();

            var currentScreen = screens[CurrentIndex];
            currentScreen.DataStore = dataStore;
            currentScreen.IsSelected = false;
            currentScreen.Loading = loading;
            currentScreen.gameObject.SetActive(true);

            await currentScreen.WaitForSelection(tokenSource.Token);
            lastIndex = CurrentIndex;

            if (!tokenSource.IsCancellationRequested)
            {
                CurrentIndex++;
                ShowScreenAndWaitForSelection();
            }
        }

        private async void Back()
        {
            tokenSource.Cancel();
            await Task.Yield();

            CurrentIndex--;
            ShowScreenAndWaitForSelection();
        }
    }
}
