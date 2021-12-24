using System;
using System.Diagnostics;
using Appalachia.Simulation.Trees.Hierarchy.Options;
using Appalachia.Utility.Extensions;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Rects
{
    [Serializable]
    public class UVRect : ICloneable<UVRect>
    {
        [SerializeField] private Vector2 _center = new Vector2(.5f, .5f);
        [SerializeField] private float _rotation;
        [SerializeField] private Vector2 _size = Vector2.one;


        public Vector2 center
        {
            get
            {
                return _center;
            }
            set
            {
                _center = value;
                Clamp();
            }
        }
        
        public float rotation
        {
            get
            {
                return _modifierRotation + _rotation;
            }
            set
            {
                _rotation = value;
                Clamp();
            }
        }
        
        public Vector2 size
        {
            get
            {
                return _modifierScale * _size;
            }
            set
            {
                _size = value;
                Clamp();
            }
        }

        [HorizontalGroup("C", .5f), LabelWidth(20), OnValueChanged(nameof(Clamp))]
        public bool flipX;

        [HorizontalGroup("C", .5f), LabelWidth(20), OnValueChanged(nameof(Clamp))]
        public bool flipY;
        
        private void Clamp()
        {
            /*centerX = Mathf.Clamp01(centerX);
            centerY = Mathf.Clamp01(centerY);

            if (centerX > .5f)
            {
                width = Mathf.Clamp(width, 0.01f, 1f - centerX);
            }
            else
            {
                width = Mathf.Clamp(width, 0.01f, centerX*2f);
            }

            if (centerY > .5f)
            {
                height = Mathf.Clamp(height, 0.01f, 1f - centerY);
            }
            else
            {
                height = Mathf.Clamp(height, 0.01f, centerY*2f);
            }

            var bl = corner(true, true);

            if (bl.x < 0)
            {
                width += 2 * bl.x;
            }

            if (bl.y < 0)
            {
                height += 2 * bl.y;
            }
            
            var tl = corner(true, false);
            
            if (tl.x < 0)
            {
                width += 2 * tl.x;
            }

            if (tl.y > 1)
            {
                height -= 2 * (1 - tl.y);
            }

            var tr = corner(false, false);
            
            if (tr.x > 1)
            {
                width -= 2 * (1 - tr.x);
            }

            if (tr.y > 1)
            {
                height -= 2 * (1 - tr.y);
            }
            
            var br = corner(false, true);
            
            if (br.x > 1)
            {
                width -= 2 * (1 - br.x);
            }

            if (br.y < 0)
            {
                height += 2 * br.y;
            }*/

        }
        
        public UVRect Clone()
        {
            return new UVRect()
            {
                _center = _center,
                _rotation = _rotation,
                _size = _size,
            };
        }

        private Vector2 corner(bool left, bool bottom)
        {
            var offsetX = .5f * size.x * (left ? -1f : 1f);
            var offsetY = .5f * size.y * (bottom ? -1f : 1f);

            var rads = Mathf.Deg2Rad * -rotation;
            
            return new Vector2(
                (center.x + (offsetX  * Mathf.Cos(rads))) - (offsetY * Mathf.Sin(rads)),
                center.y + (offsetX  * Mathf.Sin(rads)) + (offsetY * Mathf.Cos(rads))
                );
        }

        [SerializeField, HideInInspector]
        private Vector2[] _coordinates = new Vector2[4];

        public Vector2[] coordinates
        {
            get
            {
                for (var i = 0; i < 4; i++)
                {
                    _coordinates[i] = this[i];
                }

                if (flipX)
                {
                    float swap;
                    
                    swap = _coordinates[0].x;
                    _coordinates[0].x = _coordinates[3].x;
                    _coordinates[3].x = swap;
                    
                    swap = _coordinates[1].x;
                    _coordinates[1].x = _coordinates[2].x;
                    _coordinates[2].x = swap;
                }

                if (flipY)
                {
                    float swap;
                    
                    swap = _coordinates[0].y;
                    _coordinates[0].y = _coordinates[1].y;
                    _coordinates[1].y = swap;
                    
                    swap = _coordinates[3].y;
                    _coordinates[3].y = _coordinates[2].y;
                    _coordinates[2].y = swap;
                }
                
                return _coordinates;
            }
        }

        public Vector2 this[int i]
        {
            get
            {            
                ResetIfNecessary();
                
                if (i == 0)
                {
                    return corner(true, true);
                }
                if (i == 1)
                {
                    return corner(true, false);
                }
                if (i == 2)
                {
                    return corner(false, false);
                }
                if (i == 3)
                {
                    return corner(false, true);
                }
                
                throw new IndexOutOfRangeException();
            }
        }

        public float xMin => _center.x - (.5f * size.x);
        public float yMin => _center.y - (.5f * size.y);
        public float xMax => _center.x + (.5f * size.x);
        public float yMax => _center.y + (.5f * size.y);
        
        public Vector2 min => new Vector2(xMin, yMin);
        public Vector2 max => new Vector2(xMax, yMax);

        [DebuggerStepThrough] public override string ToString()
        {
            return ZString.Format(
                "{0}: {1}, {2}: {3}, {4}: {5}",
                nameof(center).ToTitleCase(),
                center,
                nameof(coordinates).ToTitleCase(),
                coordinates,
                nameof(size).ToTitleCase(),
                size
            );
        }

        [SerializeField] private float _modifierRotation;
        [SerializeField] private Vector2 _modifierScale = Vector2.one;

        public void SetRotationModifier(float rot)
        {
            _modifierRotation = rot;
            ResetIfNecessary();
        }
        
        public void SetScaleModifier(Vector2 scale)
        {
            _modifierScale = scale;
            ResetIfNecessary();
        }

        public float GetRotationModifier()
        {
            return _modifierRotation;
        }

        public Vector2 GetScaleModifier()
        {
            return _modifierScale;
        }
        
        public void Finish()
        {
            _rotation = rotation;
            _size = size;

            _modifierRotation = 0f;
            _modifierScale = Vector2.one;
            
            ResetIfNecessary();
        }

        public void Reset()
        {
            _center = new Vector2(.5f, .5f);
            _rotation = 0f;
            _size = Vector2.one;
        }

        private void ResetIfNecessary()
        {
            _rotation = _rotation %= 360f;
            if (_rotation < 0)
            {
                _rotation += 360;
            }

            _size = new Vector2(Mathf.Clamp01(_size.x), 
                Mathf.Clamp01(_size.y));

            _center = new Vector2(Mathf.Clamp01(_center.x), 
                Mathf.Clamp01(_center.y));

            if (_size.magnitude == 0)
            {
                Reset();
            }
        }

        public Vector2 ScaleNormalizedPointWithin(Vector2 point)
        {
            return new Vector2(xMin + (point.x * size.x), yMin + (point.y * size.y));
        }
        
    }
}
