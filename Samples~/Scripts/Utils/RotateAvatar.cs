using UnityEngine;
using UnityEngine.EventSystems;

namespace ReadyPlayerMe
{
    public class RotateAvatar : MonoBehaviour
    {
        [SerializeField] private float speed = 50;

        private float lastPosX;
        private bool rotate;

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject() && !rotate)
            {
               return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                lastPosX = Input.mousePosition.x;
                rotate = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                rotate = false;
            }
            
            if (Input.GetMouseButton(0))
            {
                transform.Rotate(Vector3.up, (lastPosX - Input.mousePosition.x) * (Time.deltaTime * speed));
                lastPosX = Input.mousePosition.x;
            }
        }
        
    }
}
