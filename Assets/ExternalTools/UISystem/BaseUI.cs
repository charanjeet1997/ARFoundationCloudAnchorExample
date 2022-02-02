

using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    
    public class BaseUI : MonoBehaviour
    {
        [HideInInspector]
        public GameObject content;
        protected Canvas canvas;
        protected UIAnimator uiAnimator;
        protected UIAnimation uiAnimation;
        [HideInInspector]
        public CanvasGroup canvasGroup;
        public bool isActive { get; private set; }
        public int maxVisibleLayer=2;

        public virtual void Awake()
        {
            content = transform.GetChild(0).gameObject;
          
            canvas = GetComponent<Canvas>();
            canvasGroup = content.GetComponent<CanvasGroup>();
            uiAnimator = GetComponent<UIAnimator>();
            uiAnimation = GetComponent<UIAnimation>();
        }
        public virtual void Disable()
        {
            canvas.enabled = false;//screws with the joysticks because apparently they scale (what?)// content.SetActive(false);
            isActive = false;
        }
        public virtual void Enable()
        {
            canvas.enabled = true;//screws with the joysticks because apparently they scale (what?)// content.SetActive(true);
            isActive=true;
        } 
        public virtual void Show()
        {
         
            if (isActive)
                return;
            
            canvasGroup.interactable = true;
            if (uiAnimator)
            {
                uiAnimator.StopHide();
                uiAnimator.StartShow();
                isActive = true;
            }
            else
            {
                Enable();
                isActive = true;
            }
            Redraw();
        }
        public virtual void Hide()
        {
            canvasGroup.interactable = false;
            if (uiAnimator)
            {
                uiAnimator.StopShow();
                uiAnimator.StartHide();
                isActive = false;
            }
            else
            {
                Disable();
                isActive = false;
            }
        }
        public virtual void Redraw()
        {
        }
        public int GetSortingOrder()
        {
            return canvas.sortingOrder;
        }
        public void SetSortingOrder(int sortingOrder)
        {
            canvas.sortingOrder=sortingOrder;
        }
        public void ToggleCanvas(bool status)
        {
            canvas.enabled=status;
        }
    }
}