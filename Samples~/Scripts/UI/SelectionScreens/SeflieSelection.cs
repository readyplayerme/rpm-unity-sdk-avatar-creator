using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class SeflieSelection : State
    {
        [SerializeField] private Button photoButton;
        [SerializeField] private Button fileButton;
        [SerializeField] private Button continueButton;

        public override StateType StateType => StateType.SelfieSelection;

        private void OnEnable()
        {
            photoButton.onClick.AddListener(OnPhotoButton);
            fileButton.onClick.AddListener(OnFileButton);
            continueButton.onClick.AddListener(OnContinueButton);
        }
        
        private void OnDisable()
        {
            photoButton.onClick.RemoveListener(OnPhotoButton);
            fileButton.onClick.RemoveListener(OnFileButton);
            continueButton.onClick.RemoveListener(OnContinueButton);
        }
        
        private void OnPhotoButton()
        {
            StateMachine.SetState(StateType.CameraPhoto);
        }
        
        private void OnFileButton()
        {
            // Will not be implemented
        }

        private void OnContinueButton()
        {
            StateMachine.SetState(StateType.Editor);
        }
    }
}
