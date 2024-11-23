using UnityEngine;
using MH;
using UnityEngine.Events;
using System.Collections.Generic;

namespace MH
{
    [System.Serializable]
    public class AnimationEvent
    {
        public string Key;
        public UnityEvent Event;
    }

    public class AnimationEventManager : MonoBehaviour
    {
        #region -------------------- Fields -------------------

        public List<AnimationEvent> AnimationEvents;

        #endregion

        #region -------------------- Properties -------------------
        #endregion


        #region -------------------- Public Methods -------------------

        public void OnEvent(string key)
        {
            foreach (var animationEvent in AnimationEvents) 
            {
                if(animationEvent.Key == key)
                {
                    animationEvent.Event?.Invoke();  
                }
            }
        }

        #endregion

        #region -------------------- Private Methods -------------------
        #endregion

    }

}
