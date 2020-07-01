using System.Collections.Generic;
using UnityEngine;

namespace DUI //Dimensional User Interface
{
    public class DUI : MonoBehaviour
    {
        /// <summary>
        /// handles the global DUI variables
        /// </summary>

        public static float cameraHeight; //height of the screen in Unity units
        public static float cameraWidth; //width of the screen in Unity Units 

        public static Vector3 inputPos; //position of the mouse or touch in Unity Unitys
        public static Vector3 inputPosPrev; //position of the mouse or touch in Unity Units last frame

        private DUIButton buttonOver;
        private void Awake()
        {
            Camera cam = Camera.main;

            //Orthographic setup
            if (cam.orthographic)
            {
                cameraHeight = cam.orthographicSize;
            }
            //Perspective setup
            else
            {
                //make sure vertical fov is 60
                cam.fieldOfView = 60;
                cameraHeight = (transform.position.z - cam.transform.position.z) / Mathf.Sqrt(3);
            }

            //calculate width based on height
            cameraWidth = cameraHeight * Screen.width / Screen.height;

            //recursively call set position on any child objects
            List<DUIAnchor> duias = new List<DUIAnchor>();
            for (int i = 0; i < transform.childCount; i++)
            {
                DUIAnchor a = transform.GetChild(i).GetComponent<DUIAnchor>();
                if (a != null)
                    duias.Add(a);
            }
            foreach (DUIAnchor duia in duias)
            {
                duia.SetPosition(new Bounds(Vector2.zero, new Vector2(DUI.cameraWidth, DUI.cameraHeight) * 2));
            }
        }

        public static bool Contains(Vector2 pos)
        {
            return Mathf.Abs(pos.x) < cameraWidth && Mathf.Abs(pos.y) < cameraHeight;
        }

        private void Update()
        {
            //set the previous input position
            inputPosPrev = inputPos;

            //only need to calculate input position once
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            inputPos.z = 0;
            
            RaycastHit hit;
            if (Physics.Raycast(inputPos + Vector3.back * 10, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                if (Input.GetMouseButtonDown(0) && (buttonOver = hit.transform.GetComponent<DUIButton>()) != null)
                {
                    buttonOver.OnClick?.Invoke();
                }
            }

            if (Input.GetMouseButton(0) && buttonOver != null && buttonOver.OnDrag != null)
            {
                buttonOver?.OnDrag(inputPosPrev - inputPos);
            }
            if (Input.GetMouseButtonUp(0))
            {
                buttonOver = null;
            }

#elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0) //make sure there are touches
            {
                inputPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                inputPos.z = 0;

                TouchPhase phase = Input.GetTouch(0).phase;

                RaycastHit hit;
                if (Physics.Raycast(inputPos + Vector3.back * 10, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began && (buttonOver = hit.transform.GetComponent<DUIButton>()) != null)
                    {
                        buttonOver.OnClick?.Invoke();
                    }
                }

                if ((phase == TouchPhase.Moved || phase == TouchPhase.Stationary) && buttonOver != null && buttonOver.OnDrag != null)
                {
                    buttonOver?.OnDrag(inputPosPrev - inputPos);
                }

            }
            else
            {
                buttonOver = null;
            }
#endif  
        }

    }
}
