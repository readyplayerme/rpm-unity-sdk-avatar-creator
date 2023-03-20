using UnityEngine;
using UnityEngine.EventSystems;

namespace ReadyPlayerMe
{
    public class RotateAvatar : MonoBehaviour
    {
        [SerializeField] private float speed = 50;

        private float lastPosX;
        private bool rotate;
        private const int MOUSE_BUTTON_INDEX = 0;
        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject() && !rotate)
            {
               return;
            }
            
            if (Input.GetMouseButtonDown(MOUSE_BUTTON_INDEX))
            {
                lastPosX = Input.mousePosition.x;
                rotate = true;
            }

            if (Input.GetMouseButtonUp(MOUSE_BUTTON_INDEX))
            {
                rotate = false;
            }
            
            if (Input.GetMouseButton(MOUSE_BUTTON_INDEX))
            {
                transform.Rotate(Vector3.up, (lastPosX - Input.mousePosition.x) * (Time.deltaTime * speed));
                lastPosX = Input.mousePosition.x;
            }
        }
        
    }
}
