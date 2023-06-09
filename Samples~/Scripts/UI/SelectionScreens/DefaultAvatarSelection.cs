using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class DefaultAvatarSelection : State
    {
        private const string LOADING_MESSAGE = "Fetching default avatars";

        private readonly Dictionary<string, GameObject> avatarRenderMap = new Dictionary<string, GameObject>();

        [SerializeField] private Transform parent;
        [SerializeField] private GameObject buttonPrefab;

        public override StateType StateType => StateType.DefaultAvatarSelection;
        public override StateType NextState => StateType.Editor;

        private AvatarAPIRequests avatarAPIRequests;
        private CancellationTokenSource ctxSource;

        public override async void ActivateState()
        {
            LoadingManager.EnableLoading(LOADING_MESSAGE);
            if (!AuthManager.IsSignedIn)
            {
                await AuthManager.LoginAsAnonymous();
            }

            avatarAPIRequests = new AvatarAPIRequests();
            var templateAvatars = await avatarAPIRequests.GetTemplates(AvatarCreatorData.AvatarProperties.Gender);

            ctxSource = new CancellationTokenSource();

            var downloadRenderTasks = new List<Task>();

            foreach (var template in templateAvatars)
            {
                if (!avatarRenderMap.ContainsKey(template.Key))
                {
                    downloadRenderTasks.Add(CreateAvatarRender(template.Key, template.Value));
                }
                else
                {
                    avatarRenderMap[template.Key].SetActive(true);
                }
            }

            while (!downloadRenderTasks.All(x => x.IsCompleted) && !ctxSource.IsCancellationRequested)
            {
                await Task.Yield();
            }
            LoadingManager.DisableLoading();
        }

        public override void DeactivateState()
        {
            ctxSource?.Cancel();
            foreach (Transform child in parent)
            {
                child.gameObject.SetActive(false);
            }
        }

        private async Task CreateAvatarRender(string id, string url)
        {
            Texture renderImage;
            try
            {
                renderImage = await avatarAPIRequests.GetTemplateAvatarImage(url);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            var button = Instantiate(buttonPrefab, parent);
            var rawImage = button.GetComponentInChildren<RawImage>();
            button.GetComponent<Button>().onClick.AddListener(() => OnAvatarSelected(id));
            rawImage.texture = renderImage;
            rawImage.SizeToParent();
            avatarRenderMap.Add(id, button);
        }

        private void OnAvatarSelected(string avatarId)
        {
            AvatarCreatorData.AvatarProperties.Id = avatarId;
            AvatarCreatorData.AvatarProperties.Base64Image = string.Empty;
            AvatarCreatorData.IsExistingAvatar = false;
            
            StateMachine.SetState(StateType.Editor);
        }
    }
}
