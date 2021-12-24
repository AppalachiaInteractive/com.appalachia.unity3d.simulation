using System;
using Appalachia.Core.Objects.Root;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Utilities
{
    public class ProgressDisplay : AppalachiaSimpleBase, IDisposable
    {
        public ProgressDisplay(
            float step = .33f,
            bool canCancel = true,
            bool cancelOnError = true,
            bool logOnError = true,
            bool failOnError = false)
        {
            _step = step;
            _canCancel = canCancel;
            _cancelOnError = cancelOnError;
            _logOnError = logOnError;
            _failOnError = failOnError;
        }

        public ProgressDisplay(
            int actions,
            bool canCancel = true,
            bool cancelOnError = true,
            bool logOnError = true,
            bool failOnError = false) : this(.5f, canCancel, cancelOnError, logOnError, failOnError)
        {
            _incrementsTotal = actions;
        }

        #region Fields and Autoproperties

        private int _incrementsConsumed;
        private readonly float _step;
        private readonly int _incrementsTotal;
        private readonly bool _canCancel;
        private readonly bool _cancelOnError;
        private readonly bool _logOnError;
        private readonly bool _failOnError;

        public bool cancelled { get; private set; }

        public float progress { get; private set; }

        #endregion

        public void Cancel()
        {
            if (!_canCancel)
            {
                throw new NotSupportedException("Need to set cancellable flag.");
            }

            cancelled = true;
        }

        public void Do(Action action, string title, string info = null)
        {
            if (cancelled)
            {
                return;
            }

            try
            {
                _incrementsConsumed += 1;

                if (_incrementsTotal == 0)
                {
                    progress += (1 - progress) * _step;
                }
                else
                {
                    progress = _incrementsConsumed / (float)_incrementsTotal;
                }

                progress = Mathf.Clamp01(progress);

                if (_canCancel)
                {
                    cancelled = cancelled |
                                EditorUtility.DisplayCancelableProgressBar(title, info ?? title, progress);
                }
                else
                {
                    EditorUtility.DisplayProgressBar(title, info ?? title, progress);
                }

                action();
            }
            catch (Exception ex)
            {
                if (_logOnError)
                {
                    Context.Log.Error(ex);
                }

                if (_cancelOnError)
                {
                    cancelled = true;
                }

                if (_failOnError)
                {
                    throw;
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            EditorUtility.ClearProgressBar();
        }

        #endregion
    }
}
