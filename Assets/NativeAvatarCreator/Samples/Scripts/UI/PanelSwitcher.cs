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
        [SerializeField] private GameObject Loading;
        [SerializeField] private SelectionPanel[] panels;

        private int currentIndex
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
            if (currentIndex >= panels.Length)
            {
                return;
            }

            panels[lastIndex].gameObject.SetActive(false);
            tokenSource = new CancellationTokenSource();

            var currentPanel = panels[currentIndex];
            currentPanel.DataStore = dataStore;
            currentPanel.IsSelected = false;
            currentPanel.Loading = Loading;
            currentPanel.gameObject.SetActive(true);

            await currentPanel.WaitForSelection(tokenSource.Token);
            lastIndex = currentIndex;

            if (!tokenSource.IsCancellationRequested)
            {
                currentIndex++;
                ShowPanelAndWaitForSelection();
            }
        }

        private async void Back()
        {
            tokenSource.Cancel();
            await Task.Yield();

            currentIndex--;
            ShowPanelAndWaitForSelection();
        }
    }
}
