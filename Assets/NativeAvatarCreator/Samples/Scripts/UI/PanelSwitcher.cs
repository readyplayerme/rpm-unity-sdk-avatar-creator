using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarCreatorExample
{
    public class PanelSwitcher : MonoBehaviour
    {
        [SerializeField] private Button back;
        [SerializeField] private DataStore dataStore;
        [SerializeField] private GameObject loading;
        [SerializeField] private SelectionPanel[] panels;

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
            ShowPanelAndWaitForSelection();
        }

        private void OnEnable()
        {
            back.onClick.AddListener(Back);
        }

        private void OnDisable()
        {
            back.onClick.RemoveListener(Back);
        }

        private async void ShowPanelAndWaitForSelection()
        {
            panels[lastIndex].gameObject.SetActive(false);

            if (CurrentIndex >= panels.Length)
            {
                return;
            }

            tokenSource = new CancellationTokenSource();

            var currentPanel = panels[CurrentIndex];
            currentPanel.DataStore = dataStore;
            currentPanel.IsSelected = false;
            currentPanel.Loading = loading;
            currentPanel.gameObject.SetActive(true);

            await currentPanel.WaitForSelection(tokenSource.Token);
            lastIndex = CurrentIndex;

            if (!tokenSource.IsCancellationRequested)
            {
                CurrentIndex++;
                ShowPanelAndWaitForSelection();
            }
        }

        private async void Back()
        {
            tokenSource.Cancel();
            await Task.Yield();

            CurrentIndex--;
            ShowPanelAndWaitForSelection();
        }
    }
}
