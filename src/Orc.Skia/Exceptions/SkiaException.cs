namespace Orc.Skia;

using System;

public class SkiaException : Exception
{
    public SkiaException(string message)
        : base(message)
    {
    }
}