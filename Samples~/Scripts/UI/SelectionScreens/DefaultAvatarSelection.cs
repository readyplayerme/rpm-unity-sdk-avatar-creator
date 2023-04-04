using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;
using UnityEngine.UI;
using WebRequestDispatcher = ReadyPlayerMe.AvatarCreator.WebRequestDispatcher;

namespace ReadyPlayerMe
{
    public class DefaultAvatarSelection : State
    {
        private const string LOADING_MESSAGE = "Fetching default avatars";
        
        private readonly string[] maleAvatarIds =
        {
            "64229ee84a25835c6ae8a5a4",
            "6422a00651c9394af01e94ed",
            "6422a0362ec187fafe20d48e",
            "6422a08b2ec187fafe20d4ad",
            "6422a0be7fc17f5f678cd69b",
            "6422a0eb8933ad00c8629d57",
            "6422a1242ec187fafe20d4e1",
            "6422a14e4a25835c6ae8a682",
            "6422a17cc9e8aa39b5d0e6a6",
            "6422a1b34e26f24ed304729e",
            "6422a1f17fc17f5f678cd722",
            "6422a23d8933ad00c8629dfa"
        };

        private readonly string[] femaleAvatarIds =
        {
            "64229ebbc9e8aa39b5d0e598",
            "64229f0e4a25835c6ae8a5ba",
            "6422a01fc9e8aa39b5d0e620",
            "6422a053a9cf14ab7e44d413",
            "6422a0a62ec187fafe20d4b3",
            "6422a0d47fc17f5f678cd6a2",
            "6422a10a51c9394af01e9543",
            "6422a13a8933ad00c8629d8c",
            "6422a1684e26f24ed3047276",
            "6422a1962ec187fafe20d51d",
            "6422a1d87fc17f5f678cd716",
            "6422a2254a25835c6ae8a6db",
        };
        
        private readonly Dictionary<string, GameObject> avatarRenderMap = new Dictionary<string, GameObject>();

        [SerializeField] private Transform parent;
        [SerializeField] private GameObject buttonPrefab;

        public override StateType StateType => StateType.DefaultAvatarSelection;
        public override StateType NextState => StateType.Editor;
        
        private CancellationTokenSource ctxSource;

        private async void OnEnable()
        {
            LoadingManager.EnableLoading( LOADING_MESSAGE);

            ctxSource = new CancellationTokenSource();
            var avatarIds = AvatarCreatorData.AvatarProperties.Gender == OutfitGender.Feminine ? femaleAvatarIds : maleAvatarIds;
            var downloadRenderTasks = new List<Task>();

            foreach (var avatarId in avatarIds)
            {
                if (!avatarRenderMap.ContainsKey(avatarId))
                {
                    downloadRenderTasks.Add(CreateAvatarRender(avatarId));
                }
                else
                {
                    avatarRenderMap[avatarId].SetActive(true);
                }
            }

            while (!downloadRenderTasks.All(x => x.IsCompleted) && !ctxSource.IsCancellationRequested)
            {
                await Task.Yield();
            }
            LoadingManager.DisableLoading();
        }

        private void OnDisable()
        {
            ctxSource?.Cancel();
            foreach (Transform child in parent)
            {
                child.gameObject.SetActive(false);
            }
        }

        private async Task CreateAvatarRender(string avatarId)
        {
            var renderImage = await AvatarRenderHelper.GetPortrait(avatarId);
            var button = Instantiate(buttonPrefab, parent);
            var rawImage = button.GetComponentInChildren<RawImage>();
            button.GetComponent<Button>().onClick.AddListener(() => OnAvatarSelected(avatarId));
            rawImage.texture = renderImage;
        }

        private async void OnAvatarSelected(string avatarId)
        {
            var response = await WebRequestDispatcher.SendRequest($"{Endpoints.AVATAR_API_V2}/{avatarId}.json", Method.GET);
            var json = JObject.Parse(response.Text)["data"]!.ToString();
            var avatarProperties = JsonConvert.DeserializeObject<AvatarProperties>(json);
            AvatarCreatorData.AvatarProperties.Assets = avatarProperties.Assets;
            StateMachine.SetState(StateType.Editor);
        }
    }
}
