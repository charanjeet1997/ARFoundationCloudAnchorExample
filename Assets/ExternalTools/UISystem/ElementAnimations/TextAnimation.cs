using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace UISystem
{
    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class TextAnimation : Animatable
    {
        public string[] texts;
        TMPro.TextMeshProUGUI text;
        int i=0;
        private int stringIndex;
        public override void Awake()
        {
            base.Awake();
            text=GetComponent<TMPro.TextMeshProUGUI>();
        }
        public override void OnAnimationStarted()
        {
            base.OnAnimationStarted();
        }
        public override void OnAnimationEnded()
        {
            base.OnAnimationEnded();
        }
        public override void OnAnimationRunning(float percentage)
        {
            base.OnAnimationRunning(percentage);
            text.text=texts[(i + 1) % texts.Length];
            i= (int)((texts.Length - 1)*percentage);
            
        }
    }
}
