namespace Orc.Skia.Vulkan
{
    using SharpVk.Khronos;
    using SkiaSharp;
    using Device = SharpVk.Device;
    using Instance = SharpVk.Instance;
    using PhysicalDevice = SharpVk.PhysicalDevice;
    using Queue = SharpVk.Queue;

    public class VkContext
    {
        public virtual Instance Instance { get; protected set; }

        public virtual PhysicalDevice PhysicalDevice { get; protected set; }

        public virtual Surface Surface { get; protected set; }

        public virtual Device Device { get; protected set; }

        public virtual Queue GraphicsQueue { get; protected set; }

        public virtual Queue PresentQueue { get; protected set; }

        public virtual uint GraphicsFamily { get; protected set; }

        public virtual uint PresentFamily { get; protected set; }

        public virtual GRVkGetProcedureAddressDelegate GetProc { get; protected set; }

        public virtual GRSharpVkGetProcedureAddressDelegate SharpVkGetProc { get; protected set; }

#pragma warning disable IDISP009 // Add IDisposable interface.
#pragma warning disable IDISP007 // Don't dispose injected.
        public virtual void Dispose() => Instance?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
#pragma warning restore IDISP009 // Add IDisposable interface.
    }
}
