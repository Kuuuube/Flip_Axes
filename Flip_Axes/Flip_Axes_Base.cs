using OpenTabletDriver;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;
using System;
using System.Linq;
using System.Numerics;

namespace Flip_Axes
{
    public abstract class Flip_Axes_Base : IPositionedPipelineElement<IDeviceReport>
    {
        protected Vector2 ToUnit_Screen(Vector2 input)
        {
            if (outputMode is not null)
            {
                var display = outputMode?.Output;
                var offset = (Vector2)(outputMode?.Output?.Position);
                var shiftoffX = offset.X - (display.Width / 2);
                var shiftoffY = offset.Y - (display.Height / 2);
                return new Vector2(
                    (input.X - shiftoffX) / display.Width * 2 - 1,
                    (input.Y - shiftoffY) / display.Height * 2 - 1
                    );
            }
            else
            {
                tryResolveOutputMode();
                return default;
            }
        }

        protected Vector2 FromUnit_Screen(Vector2 input)
        {
            if (outputMode is not null)
            {
                var display = outputMode?.Output;
                var offset = (Vector2)(outputMode?.Output?.Position);
                var shiftoffX = offset.X - (display.Width / 2);
                var shiftoffY = offset.Y - (display.Height / 2);
                return new Vector2(
                    (input.X + 1) / 2 * display.Width + shiftoffX,
                    (input.Y + 1) / 2 * display.Height + shiftoffY
                );
            }
            else
            {
                return default;
            }
        }

        protected Vector2 ToUnit_Tablet(Vector2 input)
        {
            if (outputMode is not null)
            {
                var tablet = outputMode?.Tablet.Properties.Specifications.Digitizer;
                return new Vector2(
                    input.X / tablet.MaxX * 2 - 1,
                    input.Y / tablet.MaxY * 2 - 1
                    );
            }
            else
            {
                tryResolveOutputMode();
                return default;
            }
        }

        protected Vector2 FromUnit_Tablet(Vector2 input)
        {
            if (outputMode is not null)
            {
                var tablet = outputMode?.Tablet.Properties.Specifications.Digitizer;
                return new Vector2(
                    (input.X + 1) / 2 * tablet.MaxX,
                    (input.Y + 1) / 2 * tablet.MaxY
                );
            }
            else
            {
                return default;
            }
        }

        [Resolved]
        public IDriver driver;
        private AbsoluteOutputMode outputMode;
        private void tryResolveOutputMode()
        {
            if (driver is Driver drv)
            {
                IOutputMode output = drv.InputDevices
                    .Where(dev => dev?.OutputMode?.Elements?.Contains(this) ?? false)
                    .Select(dev => dev?.OutputMode).FirstOrDefault();

                if (output is AbsoluteOutputMode absOutput)
                    outputMode = absOutput;
            }
        }

        public abstract event Action<IDeviceReport> Emit;
        public abstract void Consume(IDeviceReport value);
        public abstract PipelinePosition Position { get; }
    }
}