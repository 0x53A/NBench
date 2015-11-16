﻿// Copyright (c) Petabridge <https://petabridge.com/>. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using NBench.Metrics;
using NBench.Sdk;

namespace NBench.Reporting
{
    /// <summary>
    ///     The cumulative results for an entire <see cref="Benchmark" />
    /// </summary>
    public class BenchmarkResults
    {
        public BenchmarkResults(string typeName, BenchmarkSettings settings, IReadOnlyList<BenchmarkRunReport> runs)
        {
            Contract.Requires(!string.IsNullOrEmpty(typeName));
            BenchmarkName = typeName;
            Settings = settings;
            StatsByMetric = new Dictionary<MetricName, AggregateMetrics>();

            StatsByMetric = Aggregate(runs);
        }

        /// <summary>
        /// Usually prints out the type name of the spec being run
        /// </summary>
        public string BenchmarkName { get; private set; }

        /// <summary>
        ///     The settings for this <see cref="Benchmark" />
        /// </summary>
        public BenchmarkSettings Settings { get; private set; }

        /// <summary>
        ///     Per-metric aggregate statistics
        /// </summary>
        public IReadOnlyDictionary<MetricName, AggregateMetrics> StatsByMetric { get; private set; }

        public static IReadOnlyDictionary<MetricName, AggregateMetrics> Aggregate(IReadOnlyList<BenchmarkRunReport> runs)
        {
            var intermediate = new Dictionary<MetricName, List<MetricRunReport>>();

            foreach (var metric in runs.SelectMany(run => run.Metrics))
            {
                if (!intermediate.ContainsKey(metric.Key))
                    intermediate.Add(metric.Key, new List<MetricRunReport>());
                intermediate[metric.Key].Add(metric.Value);
            }

            return intermediate.ToDictionary(k => k.Key,
                v => new AggregateMetrics(v.Value.First().Name, v.Value.First().Unit, v.Value));
        }
    }
}

