using Appalachia.Core.Objects.Root;
using UnityEngine;

namespace Appalachia.Simulation.Buoyancy
{
    public sealed class WaterSplash : AppalachiaBehaviour<WaterSplash>
    {
        #region Fields and Autoproperties

        public Transform sphere;
        public Transform splashTop;
        public Transform splashBottom;
        public ParticleSystem splashParticleSystem;

        private Vector3 lastPos;

        #endregion

        #region Event Functions

        private void Update()
        {
            if (ShouldSkipUpdate)
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }

            /*//Debug by drawing a line between the top and bottom
            Debug.DrawLine(splashBottom.position, splashTop.position, Color.blue);
    
            //What's the position of the sphere
            //Positive if above water, negative if below water
            //float bottomDistToWater = WaterController.current.DistanceToWater(splashBottom.position, CoreClock.Instance.Time);
    
            //float topDistToWater = WaterController.current.DistanceToWater(splashTop.position, CoreClock.Instance.Time);
    
            //Only add foam if one is above water and the other is below
            if (topDistToWater > 0f && bottomDistToWater < 0f)
            {
                //Cut in the same way as in http://www.gamasutra.com/view/news/237528/Water_interaction_model_for_boats_in_video_games.php
                //Figure 7:
                Vector3 H = splashTop.position;
                Vector3 M = splashBottom.position;
    
                float h_M = bottomDistToWater;
                float h_H = topDistToWater;
    
                Vector3 MH = H - M;
    
                float t_M = -h_M / (h_H - h_M);
    
                Vector3 MI_M = t_M * MH;
    
                //This is the position where the water is intersecting with the line
                Vector3 I_M = MI_M + M;
    
                //Move the sphere to this position
                sphere.position = I_M;
    
                //Add foam if the boat is moving down into the water
                if (I_M.y < lastPos.y)
                {
                    //Align the ps along the line
                    splashParticleSystem.transform.LookAt(splashTop.position);
    
                    if (!splashParticleSystem.isPlaying)
                    {
                        //Context.Log.Info("Add foam");
    
                        splashParticleSystem.Play();
                    }
                }
                else
                {
                    splashParticleSystem.Stop();
                }
    
                lastPos = I_M;
            }
            else
            {
                splashParticleSystem.Stop();
            }*/
        }

        #endregion
    }
}
