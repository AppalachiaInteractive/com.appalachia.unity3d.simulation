using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Build.Requests;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Build.Cost
{
    [Serializable]
    public class BuildProgressTracker
    {
        [SerializeField] private float _buildCost;
        
        [SerializeField] private float _buildCompleted;

        private Dictionary<BuildCategory, string> _friendlyStrings;
        private Dictionary<BuildCategory, float> _categorySums;
        private Dictionary<BuildCategory, int> _categoryCounts;
        private Dictionary<BuildCategory, int> _categoryConsumed;
        [SerializeField] private float _startTime;


        [SerializeField] private BuildResult _buildResult;
        
        public BuildResult buildResult => _buildResult;
        
        public float buildCompleteTime;
        
        public float timeSinceBuildComplete => buildResult == BuildResult.InProgress ? 0f : Time.realtimeSinceStartup - buildCompleteTime;

        public float buildProgress => _buildCost == 0 ? 0 : _buildCompleted / _buildCost;
        
        [SerializeField] private string _buildMessage;

        public string buildMessage => _buildMessage;

        public bool buildActive => buildResult == BuildResult.InProgress;
        
        public void InitializeBuildBatch()
        {
            using (BUILD_TIME.BUILD_PRG_TRCK.InitializeBuildBatch.Auto())
            {
                _startTime = Time.realtimeSinceStartup;
                _buildResult = BuildResult.InProgress;
            }
        }

        private readonly BuildCategory[] _bcEnums = Enum.GetValues(typeof(BuildCategory)).Cast<BuildCategory>().ToArray();
        public void InitializeBuild(BuildRequestLevel level, IEnumerable<BuildCost> costs)
        {
            using (BUILD_TIME.BUILD_PRG_TRCK.InitializeBuild.Auto())
            {
                _buildCost = 0;
                _buildCompleted = 0;
                _buildResult = BuildResult.InProgress;

                if (_categorySums == null)
                {
                    _categorySums = new Dictionary<BuildCategory, float>();
                }

                if (_categoryCounts == null)
                {
                    _categoryCounts = new Dictionary<BuildCategory, int>();
                }

                if (_categoryConsumed == null)
                {
                    _categoryConsumed = new Dictionary<BuildCategory, int>();
                }

                _categorySums = new Dictionary<BuildCategory, float>();
                _categoryCounts = new Dictionary<BuildCategory, int>();
                _categoryConsumed = new Dictionary<BuildCategory, int>();

                foreach (var value in _bcEnums)
                {
                    _categorySums.Add(value, 0);
                    _categoryCounts.Add(value, 0);
                    _categoryConsumed.Add(value, 0);
                }

                var sum = 0f;

                foreach (var cost in costs)
                {
                    if (cost.category == BuildCategory.MaterialGeneration)
                    {
                        _categorySums[cost.category] += cost.cost;
                        _categoryCounts[cost.category] += 3;

                        sum += cost.cost;

                        var texCost = BuildCost.GetBuildCost(BuildCategory.SavingTextures);
                        _categorySums[BuildCategory.SavingTextures] += texCost;
                        _categoryCounts[BuildCategory.SavingTextures] += 1;

                        sum += texCost;
                    }
                    else if (cost.category == BuildCategory.AmbientOcclusion)
                    {
                        _categorySums[cost.category] += cost.cost;
                        _categoryCounts[cost.category] += 2;
                        sum += cost.cost;
                    }
                    else if (cost.category == BuildCategory.Mesh)
                    {
                        _categorySums[cost.category] += cost.cost;
                        _categoryCounts[cost.category] += 1;
                        sum += cost.cost;
                    }
                    else
                    {
                        _categorySums[cost.category] += cost.cost;
                        _categoryCounts[cost.category] += 1;
                        sum += cost.cost;
                    }
                }

                _buildCost = sum;
            }
        }

        public void MultiplyBuildCount(BuildCategory category, int number)
        {
            using (BUILD_TIME.BUILD_PRG_TRCK.MultiplyBuildCount.Auto())
            {
                if (!_categoryCounts.ContainsKey(category))
                {
                    _categoryCounts.Add(category, 0);
                }

                _categoryCounts[category] *= number;
            }
        }

        public void AddBuildCost(BuildCategory category, int number)
        {
            using (BUILD_TIME.BUILD_PRG_TRCK.AddBuildCost.Auto())
            {
                var unitCost = BuildCost.GetBuildCost(category);

                _categorySums[category] += unitCost * number;
                _categoryCounts[category] += number;

                _buildCost += unitCost * number;
            }
        }

        public void Update(BuildCategory category)
        {
            using (BUILD_TIME.BUILD_PRG_TRCK.Update.Auto())
            {
                _buildResult = BuildResult.InProgress;
                
                if (_friendlyStrings == null)
                {
                    _friendlyStrings = new Dictionary<BuildCategory, string>();
                }

                var categorySum = _categorySums[category];
                var categoryCounts = Mathf.Max(1f, _categoryCounts[category]);

                var increment = categorySum / categoryCounts;

                _buildCompleted += increment;

                _categoryConsumed[category] = Mathf.Min(_categoryConsumed[category] + 1, _categoryCounts[category]);

                SetBuildMessage(category);
            }

        }
        
        public void CompleteBuild()
        {
            using (BUILD_TIME.BUILD_PRG_TRCK.CompleteBuild.Auto())
            {
                _buildCompleted = 0;
                _buildCost = 0;
            }
        }

        public void CompleteBuildBatch(bool successful)
        {
            using (BUILD_TIME.BUILD_PRG_TRCK.CompleteBuildBatch.Auto())
            {
                CompleteBuild();

                _buildResult = successful ? BuildResult.Success : BuildResult.Error;
                buildCompleteTime = Time.realtimeSinceStartup;
                
                SetBuildMessage();
            }
        }

        private void SetBuildMessage(BuildCategory category = BuildCategory.Distribution)
        {
            switch (buildResult)
            {
                case BuildResult.Success:
                    _buildMessage = $"SUCCESS:  {Time.realtimeSinceStartup - _startTime:##.000} seconds";
                    break;
                case BuildResult.Error:
                    _buildMessage = $"FAILURE:  {Time.realtimeSinceStartup - _startTime:##.000} seconds";
                    break;
                case BuildResult.InProgress:
                    string friendly;

                    if (_friendlyStrings.ContainsKey(category))
                    {
                        friendly = _friendlyStrings[category];
                    }
                    else
                    {
                        friendly = new string(
                            category.ToString().SelectMany(ac => char.IsUpper(ac) ? new[] {' ', ac} : new[] {ac}).ToArray()
                        );
                    }

                    _buildMessage = $"IN PROGRESS:  {friendly}: {_categoryConsumed[category]} of {_categoryCounts[category]} | {Time.realtimeSinceStartup - _startTime:##.000} s";
                    
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Validate()
        {
            var duration = Time.realtimeSinceStartup - _startTime;

            if ((duration > 10) && (_buildCost == 0))
            {
                _buildResult = BuildResult.Normal;
                _buildCost = 0;
                _buildCompleted = 0;
            }

            else if ((duration > 20) && (_buildCompleted == 0))
            {
                _buildResult = BuildResult.Normal;
                _buildCost = 0;
                _buildCompleted = 0;
            }
        }
    }
}