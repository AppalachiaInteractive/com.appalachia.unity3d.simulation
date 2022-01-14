using System;
using System.Collections.Generic;
using Appalachia.Core.Extensions;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Settings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Transmission
{
    public static class TransmissionColorCalculator
    {
        #region Static Fields and Autoproperties

        private static Dictionary<int, Color> textureAverages = new Dictionary<int, Color>();

        #endregion

        public static Color GetAverageColor(Texture2D texture, bool ignoreAlpha, bool ignoreBlack)
        {
            if (textureAverages == null)
            {
                textureAverages = new Dictionary<int, Color>();
            }

            var textureID = texture.GetInstanceID();

            if (textureAverages.ContainsKey(textureID))
            {
                return textureAverages[textureID];
            }

            var pixels = texture.GetPixels();

            var sum = Vector3.zero;
            var colors = 0f;

            for (var j = 0; j < pixels.Length; j++)
            {
                var pixel = pixels[j];

                if (ignoreAlpha && (pixel.a < .01f))
                {
                    continue;
                }

                if (ignoreBlack && (pixel.r < .01f) && (pixel.g < .01f) && (pixel.b < .01f))
                {
                    continue;
                }

                sum += new Vector3(pixel.r, pixel.g, pixel.b);
                colors += 1f;
            }

            var average = sum / colors;

            var result = new Color(average.x, average.y, average.z, 1f);

            textureAverages.Add(textureID, result);

            return result;
        }

        public static void SetAutomaticTransmission(
            Texture2D texture,
            MaterialTransmissionValues transmission,
            TransmissionSettings settings)
        {
            using (BUILD_TIME.TRN_COLOR_CALC.SetAutomaticTransmission.Auto())
            {
                if (texture == null)
                {
                    return;
                }

                if ((texture == transmission.lastAutoTransmissionTexture2D) &&
                    (Math.Abs(
                         settings.automaticTransmissionColorBrightness -
                         transmission.lastAutoTransmissionBrightness
                     ) <
                     float.Epsilon))
                {
                    transmission.automaticTransmissionColor = transmission.lastAutoTransmissionColor;
                    return;
                }

                texture.SetReadable();

                var leafAverage = GetAverageColor(texture, true, true);
                Color.RGBToHSV(leafAverage, out var h, out var s, out var v);
                var color = Color.HSVToRGB(h, s, settings.automaticTransmissionColorBrightness);

                transmission.lastAutoTransmissionTexture2D = texture;
                transmission.lastAutoTransmissionBrightness = settings.automaticTransmissionColorBrightness;
                transmission.lastAutoTransmissionColor = color;
                transmission.automaticTransmissionColor = color;
            }
        }
    }
}
