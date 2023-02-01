using System;
using System.Threading;
using NativeAvatarCreator;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;

namespace AvatarCreatorExample
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform nearTransform;
        [SerializeField] private Transform halfBodyTransform;
        [SerializeField] private Transform farTransform;
        [SerializeField] private float defaultDuration = 0.25f;

        private CancellationTokenSource ctx;
        private BodyType bodyType;

        private void OnDestroy()
        {
            ctx?.Cancel();
        }

        public void DefaultZoom(BodyType type)
        {
            bodyType = type;
            if (bodyType == BodyType.HalfBody)
            {
                MoveToHalfBody();
            }
            else
            {
                MoveToFar();
            }
        }

        public void SwitchZoomByAssetType(AssetType assetType)
        {
            if (bodyType != BodyType.HalfBody)
            {
                if (assetType == AssetType.Outfit)
                {
                    MoveToFar();
                }
                else
                {
                    MoveToNear();
                }
            }
        }

        private void MoveToNear()
        {
            ctx?.Cancel();
            ctx = new CancellationTokenSource();
            _ = cameraTransform.LerpPosition(nearTransform.position, defaultDuration, ctx.Token);
        }

        private void MoveToFar()
        {
            ctx?.Cancel();
            ctx = new CancellationTokenSource();
            _ = cameraTransform.LerpPosition(farTransform.position, defaultDuration, ctx.Token);
        }

        private void MoveToHalfBody()
        {
            cameraTransform.position = halfBodyTransform.transform.position;
        }
    }
}
