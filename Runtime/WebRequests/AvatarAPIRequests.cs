using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    [Serializable]
    public enum PartnerAssetColor
    {
        SkinColor,
        HairColor,
        BeardColor,
        EyebrowColor
    }
    
    [Serializable]
    public struct ColorResponse
    {
        public string[] skin;
        public string[] eyebrow;
        public string[] beard;
        public string[] hair;
    }

    [Serializable]
    public struct ColorPalette
    {
        public PartnerAssetColor partnerAssetColor;
        public Color[] hexColors;

        public ColorPalette(string name, string[] colorHex)
        {
            hexColors = new Color[colorHex.Length];
            for (int i = 0; i < colorHex.Length; i++)
            {
                var trimmedHex = colorHex[i].Trim('#');
                ColorUtility.TryParseHtmlString(trimmedHex, out hexColors[i]);
            }

            switch (name)
            {
                case "skin": 
                    partnerAssetColor = PartnerAssetColor.SkinColor;
                    break;
                case "eyebrow": 
                    partnerAssetColor = PartnerAssetColor.EyebrowColor;
                    break;
                case "beard": 
                    partnerAssetColor = PartnerAssetColor.BeardColor;
                    break;
                case "hair":
                    partnerAssetColor = PartnerAssetColor.HairColor;
                    break;
                default:
                    break;
            }
            partnerAssetColor = PartnerAssetColor.SkinColor;
        }
        
    }
    
    public class AvatarAPIRequests
    {
        private const string PREVIEW_PARAMETER = "preview=true";
        private const string RESPONSE_TYPE_PARAMETER = "responseType=glb";
        private const string COLOURS_PARAMETERS = "colors?type=skin,beard,hair,eyebrow";
        private readonly Dictionary<string, string> header;
        private readonly CancellationToken cancellationToken;
        
        public static string GetColorEndpoint(string avatarId)
        {
            return $"{Endpoints.AVATAR_API_V2}/{avatarId}/{COLOURS_PARAMETERS}";
        }

        public AvatarAPIRequests(string token, CancellationToken cancellationToken = default)
        {
            this.cancellationToken = cancellationToken;
            header = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Authorization", $"Bearer {token}" }
            };
        }
        
        public async Task<ColorPalette[]> GetAllAvatarColors(string avatarId)
        {
            var response = await WebRequestDispatcher.SendRequest(
                GetColorEndpoint(avatarId),
                Method.GET,
                header,
                token: cancellationToken);
            
            var metadata = JObject.Parse(response.Text);
            Debug.Log($"data =  {metadata}");
            ColorResponse colorResponse = ((JObject)metadata["data"]).ToObject<ColorResponse>();
            var colorPalettes = new ColorPalette[4];
            colorPalettes[0] = new ColorPalette("skin", colorResponse.skin);
            colorPalettes[1] = new ColorPalette("eyebrow", colorResponse.eyebrow);
            colorPalettes[2] = new ColorPalette("beard", colorResponse.beard);
            colorPalettes[3] = new ColorPalette("hair", colorResponse.hair);
            Debug.Log($"COLORS RETRIEVED {response.Text}");
            foreach (var skin in colorResponse.skin)
            {
                Debug.Log($"{skin}");
            }
            
            return colorPalettes;
        }

        public async Task<string> CreateNewAvatar(AvatarProperties avatarProperties)
        {
            var response = await WebRequestDispatcher.SendRequest(
                Endpoints.AVATAR_API_V2,
                Method.POST,
                header,
                avatarProperties.ToJson(),
                token: cancellationToken);

            var metadata = JObject.Parse(response.Text);
            var avatarId = metadata["data"]?["id"]?.ToString();
            return avatarId;
        }

        public async Task<byte[]> GetPreviewAvatar(string avatarId, string parameters = null)
        {
            var url = string.IsNullOrEmpty(parameters)
                ? $"{Endpoints.AVATAR_API_V2}/{avatarId}.glb?{PREVIEW_PARAMETER}"
                : $"{Endpoints.AVATAR_API_V2}/{avatarId}.glb{parameters}&{PREVIEW_PARAMETER}";

            var response = await WebRequestDispatcher.SendRequest(
                url,
                Method.GET,
                header,
                token: cancellationToken);
            return response.Data;
        }

        public async Task<byte[]> UpdateAvatar(string avatarId, AvatarProperties avatarProperties, string parameters = null)
        {
            var url = string.IsNullOrEmpty(parameters)
                ? $"{Endpoints.AVATAR_API_V2}/{avatarId}?{RESPONSE_TYPE_PARAMETER}"
                : $"{Endpoints.AVATAR_API_V2}/{avatarId}{parameters}&{RESPONSE_TYPE_PARAMETER}";

            var response = await WebRequestDispatcher.SendRequest(
                url,
                Method.PATCH,
                header,
                avatarProperties.ToJson(true),
                token: cancellationToken);

            return response.Data;
        }

        public async Task<string> SaveAvatar(string avatarId)
        {
            var response = await WebRequestDispatcher.SendRequest(
                $"{Endpoints.AVATAR_API_V2}/{avatarId}",
                Method.PUT,
                header,
                token: cancellationToken);

            return response.Text;
        }

        public async Task DeleteAvatar(string avatarId)
        {
            await WebRequestDispatcher.SendRequest(
                $"{Endpoints.AVATAR_API_V1}/{avatarId}",
                Method.DELETE,
                header,
                token: cancellationToken);
        }
    }
}
