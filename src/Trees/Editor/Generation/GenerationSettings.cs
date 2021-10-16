/*using System;
using Appalachia.Simulation.Trees.Collections;
using Appalachia.Simulation.Trees.Individuals.Types;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation
{

    

    [Serializable]
    public class GenerationSettings
    {

        private GenerationState _state = GenerationState.Normal;



        public bool graph
        {
            get =>
                _state == GenerationState.Disabled ? false :
                _state == GenerationState.ForceFull ? true :
                _state == GenerationState.Full ? true : _graph;
            set => _graph = value;
        }

        public bool mesh
        {
            get =>
                _state == GenerationState.Disabled ? false :
                _state == GenerationState.ForceFull ? true :
                _state == GenerationState.Full ? true : _mesh;
            set => _mesh = value;
        }

        public bool materialRegeneration
        {
            get =>
                _state == GenerationState.Disabled ? false :
                _state == GenerationState.ForceFull ? true :
                _state == GenerationState.Full ? true :
                _state == GenerationState.Fast ? false : _materialRegeneration;
            set => _materialRegeneration = value;
        }

        public bool materialProperties
        {
            get =>
                _state == GenerationState.Disabled ? false :
                _state == GenerationState.ForceFull ? true :
                _state == GenerationState.Full ? true :
                (_materialProperties || _materialRegeneration || _mesh);
            set => _materialProperties = value;
        }

        public bool ambientOcclusion
        {
            get =>
                _state == GenerationState.Disabled ? false :
                _state == GenerationState.ForceFull ? true :
                _state == GenerationState.Full ? true : _ambientOcclusion;
            set => _ambientOcclusion = value;
        }

        public bool levelsOfDetail
        {
            get =>
                _state == GenerationState.Disabled ? false :
                _state == GenerationState.ForceFull ? true :
                _state == GenerationState.Full ? true :
                _state == GenerationState.Normal ? _mesh : // intentional
                _levelsOfDetail;
            set => _levelsOfDetail = value;
        }

        public bool variants
        {
            get =>
                _state == GenerationState.Disabled ? false :
                _state == GenerationState.ForceFull ? true :
                _state == GenerationState.Full ? true : _variants;
            set => _variants = value;
        }

        public bool collaring
        {
            get =>
                _state == GenerationState.Disabled ? false :
                _state == GenerationState.ForceFull ? true :
                _state == GenerationState.Full ? true :
                _state == GenerationState.Fast ? false : _collaring;
            set => _collaring = value;
        }

        public bool wind
        {
            get =>
                _state == GenerationState.Disabled ? false :
                _state == GenerationState.ForceFull ? true :
                _state == GenerationState.Full ? true : _wind;
            set => _wind = value;
        }

        public bool saveAssets =>
            _state == GenerationState.Disabled ? false :
            _state == GenerationState.ForceFull ? true :
            _state == GenerationState.Fast ? false : true;


        private bool any
        {
            get =>
                collaring || graph || mesh || variants || wind || ambientOcclusion ||
                levelsOfDetail || materialProperties || materialRegeneration;
        }

        public bool requiresRebuild
        {
            get => any;
        }

        public bool disabled => _state == GenerationState.Disabled;

        public bool lowQuality => _state == GenerationState.Fast;

        public void ForceFull()
        {
            _state = GenerationState.ForceFull;
        }

        public void Full()
        {
            if (_state == GenerationState.ForceFull)
            {
                return;
            }

            _state = GenerationState.Full;
        }



        public void Explicit()
        {
            if (_state == GenerationState.ForceFull)
            {
                return;
            }

            if (_state == GenerationState.Full)
            {
                return;
            }

            _state = GenerationState.Explicit;
        }


        public void Fast(GenerationState followWith)
        {
            if (_state == GenerationState.ForceFull)
            {
                return;
            }

            if (_state == GenerationState.Full)
            {
                return;
            }

            if (_state == GenerationState.Explicit)
            {
                return;
            }

            _pendingBuild = followWith != GenerationState.Disabled &&
                followWith != GenerationState.None;

            _pendingBuildState = followWith;
            _state = GenerationState.Fast;
        }


        public void Disabled()
        {
            _state = GenerationState.Disabled;
        }

        public void Enabled()
        {
            _state = GenerationState.Normal;
        }

        public void Reset()
        {
            if (mesh)
            {
                _mesh = false;
            }

            if (materialRegeneration)
            {
                _materialRegeneration = false;
            }

            if (materialProperties)
            {
                _materialProperties = false;
            }

            if (ambientOcclusion)
            {
                _ambientOcclusion = false;
            }

            if (levelsOfDetail)
            {
                _levelsOfDetail = false;
            }

            if (variants)
            {
                _variants = false;
            }

            if (collaring)
            {
                _collaring = false;
            }

            if (wind)
            {
                _wind = false;
            }

            if (_state != GenerationState.Disabled)
            {
                _state = GenerationState.Normal;
            }
        }

        public void DequeuePendingBuild()
        {
            if (_state != GenerationState.Disabled)
            {
                if (!_pendingBuild)
                {
                    _state = GenerationState.Normal;
                    _pendingBuildState = GenerationState.None;
                }
                else
                {
                    _pendingBuild = false;

                    switch (_pendingBuildState)
                    {
                        case GenerationState.ForceFull:
                        case GenerationState.Normal:
                        case GenerationState.Full:
                        case GenerationState.Explicit:
                            _state = _pendingBuildState;
                            break;
                        case GenerationState.Fast:
                        case GenerationState.Disabled:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    _pendingBuildState = GenerationState.None;
                }
            }
        }


        public GenerationState GetState()
        {
            return _state;
        }

        public void SetState(GenerationState state)
        {
            _state = state;
        }


    }
}*/