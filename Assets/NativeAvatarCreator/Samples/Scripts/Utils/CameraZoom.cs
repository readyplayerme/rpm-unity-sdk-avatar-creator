using System.Threading;
using System.Threading.Tasks;
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
        private bool lerpInProgress;

        private BodyType bodyType;
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
        
        public void MoveToNear()
        {
            if (lerpInProgress)
            {
                ctx.Cancel();
            }
            ctx = new CancellationTokenSource();
            Lerp(nearTransform.position, defaultDuration, ctx.Token);
        }

        public void MoveToFar()
        {
            if (lerpInProgress)
            {
                ctx.Cancel();
            }
            ctx = new CancellationTokenSource();
            Lerp(farTransform.position, defaultDuration, ctx.Token);
        }

        public void MoveToHalfBody()
        {
            cameraTransform.position = halfBodyTransform.transform.position;
        }

        private async void Lerp(Vector3 targetPosition, float duration, CancellationToken token)
        {
            var time = 0f;
            var startPosition = cameraTransform.position;
            lerpInProgress = true;
            while (time < duration && !token.IsCancellationRequested)
            {
                cameraTransform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
                time += Time.deltaTime;
                await Task.Yield();
            }
            lerpInProgress = false;

            if (!token.IsCancellationRequested)
            {
                cameraTransform.position = targetPosition;
            }
        }
    }
}
