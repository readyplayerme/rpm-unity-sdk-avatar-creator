using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class DefaultAvatarSelection : State
    {
        private const string TAG = nameof(DefaultAvatarSelection);
        private const string LOADING_MESSAGE = "Fetching default avatars";

        [SerializeField] private Transform parent;
        [SerializeField] private GameObject buttonPrefab;

        public override StateType StateType => StateType.DefaultAvatarSelection;
        public override StateType NextState => StateType.Editor;

        private Dictionary<TemplateData, GameObject> avatarRenderMap;
        private AvatarAPIRequests avatarAPIRequests;
        private CancellationTokenSource ctxSource;

        private void Awake()
        {
            avatarRenderMap = new Dictionary<TemplateData, GameObject>();
        }

        public override async void ActivateState()
        {
            if (avatarRenderMap.Count == 0)
            {
                LoadingManager.EnableLoading(LOADING_MESSAGE);

                if (!AuthManager.IsSignedIn)
                {
                    await AuthManager.LoginAsAnonymous();
                }

                await FetchTemplates();
               
                LoadingManager.DisableLoading();
            }

            foreach (var template in avatarRenderMap)
            {
                avatarRenderMap[template.Key].SetActive(template.Key.Gender == AvatarCreatorData.AvatarProperties.Gender);
            }
        }

        public override void DeactivateState()
        {
            ctxSource?.Cancel();
            foreach (Transform child in parent)
            {
                child.gameObject.SetActive(false);
            }
        }

        private async Task FetchTemplates()
        {
            var startTime = Time.time;
            var downloadRenderTasks = new List<Task>();
            ctxSource = new CancellationTokenSource();

            avatarAPIRequests = new AvatarAPIRequests();
            var templateAvatars = await avatarAPIRequests.GetTemplates();
            SDKLogger.Log(TAG, $"Fetch all avatar templates in {Time.time - startTime:F2}s ");
            foreach (var template in templateAvatars)
            {
                if (!avatarRenderMap.ContainsKey(template))
                {
                    downloadRenderTasks.Add(CreateAvatarRender(template));
                }
            }

            while (!downloadRenderTasks.All(x => x.IsCompleted) && !ctxSource.IsCancellationRequested)
            {
                await Task.Yield();
            }
        }

        private async Task CreateAvatarRender(TemplateData templateData)
        {
            Texture renderImage;
            try
            {
                renderImage = await avatarAPIRequests.GetTemplateAvatarImage(templateData.ImageUrl);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            var button = Instantiate(buttonPrefab, parent);
            var rawImage = button.GetComponentInChildren<RawImage>();
            button.GetComponent<Button>().onClick.AddListener(() => OnAvatarSelected(templateData.Id));
            rawImage.texture = renderImage;
            rawImage.SizeToParent();
            avatarRenderMap.Add(templateData, button);
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
