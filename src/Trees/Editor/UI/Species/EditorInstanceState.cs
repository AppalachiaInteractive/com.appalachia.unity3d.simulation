using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Appalachia.Simulation.Trees.UI.Species
{
    [Serializable]
    public class EditorInstanceState
    {
        public bool last;
        public int instanceID;
        public int delayCount;
        public bool windowChangeActive;
        public float activeWindowWidth;
        public const int delayThreshold = 1;
        
        public bool HandleStateChange(Object obj)
        {
            var objID = obj.GetInstanceID();
            var width = EditorGUIUtility.currentViewWidth;

            if (Event.current.type != EventType.Layout)
            {
                return last;
            }
        
            if (instanceID == objID)
            {
                activeWindowWidth = width;
                
                if (delayCount < delayThreshold)
                {
                    delayCount += 1;
                
                    last = false;
                    return last;
                }
            }
            else
            {
                if ((instanceID == -1) || (instanceID == 0))
                {
                    instanceID = objID;
                    activeWindowWidth = width;
                    windowChangeActive = true;
                
                    last = false;
                    return last;
                }
            
                if ((width > activeWindowWidth) && windowChangeActive)
                {
                    instanceID = objID;
                    activeWindowWidth = width;
                
                    last = false;
                    return last;
                }
                
                last = false;
                return last;
            }
            
            windowChangeActive = false;

            last = true;
            return last;
        }
    }
}
