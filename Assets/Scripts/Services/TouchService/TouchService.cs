namespace Games.Services
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using ServiceFramework;
    using CommanTickManager;
    public class TouchService : MonoBehaviour, IService, ITick
    {
        #region PUBLIC_VARS
        public static TouchService instance;
        public bool ShouldStartOnRegister { get { return shouldStartOnRegister; } }
        public bool shouldStartOnRegister;
        public string ServiceName
        {
            get
            {
                return _serviceName;
            }
        }
        public string _serviceName;
        List<IMouseDown> mouseDowns;
        List<IMouseUp> mouseUps;
        List<IMouse> mouses;
        // List<IDoubleClick>
        #endregion
        #region UNITY_CALLBACKS
        void Awake()
        {
            mouseUps = new List<IMouseUp>();
            mouseDowns = new List<IMouseDown>();
            mouses= new List<IMouse>();
            instance=this;
        }
        void OnEnable()
        {
            ServiceManager.instance.RegisterService(this);
            ProcessingUpdate.Instance.Add(this);
        }
        void OnDisable()
        {
            ServiceManager.instance.UnRegisterService(this);
            ProcessingUpdate.Instance.Remove(this);
        }
        #endregion

        #region CommanTickManager
        public void Tick()
        {
            if (Input.GetMouseButtonUp(0))
            {
                ExecuteMouseUp(Input.mousePosition);
            }
            if(Input.GetMouseButton(0))
            {
                ExecuteMouse(Input.mousePosition);
                // Debug.Log("Mousebutton");
            }
            if (Input.GetMouseButtonDown(0))
            {
                ExecuteMouseDown(Input.mousePosition);
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void ExecuteMouseDown(Vector3 position)
        {
            //Debug.Log("working ");
            for (int indexOfMouseDown = 0; indexOfMouseDown < mouseDowns.Count; indexOfMouseDown++)
            {
                mouseDowns[indexOfMouseDown].OnMouseDown(position);
            }
        }
        public void ExecuteMouseUp(Vector3 position)
        {
            for (int indexOfMouseUp = 0; indexOfMouseUp < mouseUps.Count; indexOfMouseUp++)
            {
                mouseUps[indexOfMouseUp].OnMouseUp(position);
            }
        }
        
        public void ExecuteMouse(Vector3 position)
        {
            for (int indexOfMouse = 0; indexOfMouse < mouses.Count; indexOfMouse++)
            {
                mouses[indexOfMouse].OnMouse(position);
            }
        }
        public void Add(object touchable)
        {
            var mouseDown = touchable as IMouseDown;
            if (mouseDown != null)
            {
                mouseDowns.Add(mouseDown);
            }

            var mouseUp = touchable as IMouseUp;
            if (mouseUp != null)
            {
                mouseUps.Add(mouseUp);
            }

            var mouse = touchable as IMouse;
            if (mouse != null)
            {
                mouses.Add(mouse);
            }

        }
        public void Remove(object touchable)
        {
            mouseUps.Remove(touchable as IMouseUp);
            mouseDowns.Remove(touchable as IMouseDown);
            mouses.Remove(touchable as IMouse);
        }
        #endregion

        #region SERVICE_CALLBACKS
        public void StartService()
        {
            ProcessingUpdate.Instance.Add(this);
        }
        public void StopService()
        {
            ProcessingUpdate.Instance.Remove(this);
        }
        #endregion
    }
}
