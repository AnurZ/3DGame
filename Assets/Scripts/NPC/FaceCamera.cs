using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    
        void Update()
        {
            if (Camera.main != null)
                transform.LookAt(Camera.main.transform);
        }
    

}
