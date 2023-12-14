namespace Orc.Skia.Example;

using System;
using System.Collections.Generic;
using System.Linq;
using Catel.Data;

public class PerformanceTestResult : ModelBase
{
    private readonly List<int> _registeredFrames = new();

    public int? Slowest
    {
        get
        {
            if (_registeredFrames.Count == 0)
            {
                return null;
            }

            return _registeredFrames.MaxBy(x => x);
        }
    }

    public int? Fastest
    {
        get
        {
            if (_registeredFrames.Count == 0)
            {
                return null;
            }

            return _registeredFrames.MinBy(x => x);
        }
    }

    public int TotalRenders
    {
        get
        {
            return _registeredFrames.Count;
        }
    }

    public TimeSpan TotalDuration
    {
        get
        {
            return TimeSpan.FromMilliseconds(_registeredFrames.Sum());
        }
    }

    public TimeSpan AverageDuration
    {
        get
        {
            if (_registeredFrames.Count == 0)
            {
                return TimeSpan.Zero;
            }

            var total = _registeredFrames.Sum();
            var average = total / _registeredFrames.Count;

            return TimeSpan.FromMilliseconds(average);
        }
    }

    public double FramesPerSecond
    {
        get
        {
            var averageDuration = AverageDuration.TotalMilliseconds;
            if (averageDuration < 1d)
            {
                return 0d;
            }

            return 1000d / averageDuration;
        }
    }

    public void RegisterFrame(int duration)
    {
        _registeredFrames.Add(duration);
    }
}
