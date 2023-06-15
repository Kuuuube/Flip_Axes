using OpenTabletDriver;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;
using System;
using System.Linq;
using System.Numerics;

namespace Flip_Axes;
public abstract class Flip_Axes_Base : IPositionedPipelineElement<IDeviceReport>
{
    protected Vector2 ToUnit_Screen(Vector2 input)
    {
        if (output_mode_type == OutputModeType.absolute && absolute_output_mode != null) {
            var display = absolute_output_mode?.Output;
            var offset = (Vector2)(absolute_output_mode?.Output?.Position);
            var shiftoffX = offset.X - (display.Width / 2);
            var shiftoffY = offset.Y - (display.Height / 2);
            return new Vector2(
                (input.X - shiftoffX) / display.Width * 2 - 1,
                (input.Y - shiftoffY) / display.Height * 2 - 1
                );
        }
        try_resolve_output_mode();
        return input;
    }

    protected Vector2 FromUnit_Screen(Vector2 input)
    {
        if (output_mode_type == OutputModeType.absolute && absolute_output_mode != null) {
            var display = absolute_output_mode?.Output;
            var offset = (Vector2)(absolute_output_mode?.Output?.Position);
            var shiftoffX = offset.X - (display.Width / 2);
            var shiftoffY = offset.Y - (display.Height / 2);
            return new Vector2(
                (input.X + 1) / 2 * display.Width + shiftoffX,
                (input.Y + 1) / 2 * display.Height + shiftoffY
            );
        }
        try_resolve_output_mode();
        return input;
    }

    protected Vector2 ToUnit_Tablet(Vector2 input)
    {
        if (output_mode_type == OutputModeType.absolute && absolute_output_mode != null) {
            return new Vector2(
                input.X / absolute_output_mode.Tablet.Properties.Specifications.Digitizer.MaxX * 2 - 1,
                input.Y / absolute_output_mode.Tablet.Properties.Specifications.Digitizer.MaxY * 2 - 1
            );
        } else if (output_mode_type == OutputModeType.relative && relative_output_mode != null) {
            return new Vector2(
                input.X / relative_output_mode.Tablet.Properties.Specifications.Digitizer.MaxX * 2 - 1,
                input.Y / relative_output_mode.Tablet.Properties.Specifications.Digitizer.MaxY * 2 - 1
            );
        }
        try_resolve_output_mode();
        return default;
    }

    protected Vector2 FromUnit_Tablet(Vector2 input)
    {
        if (output_mode_type == OutputModeType.absolute && absolute_output_mode != null) {
            return new Vector2(
                (input.X + 1) / 2 * absolute_output_mode.Tablet.Properties.Specifications.Digitizer.MaxX,
                (input.Y + 1) / 2 * absolute_output_mode.Tablet.Properties.Specifications.Digitizer.MaxY
            );
        } else if (output_mode_type == OutputModeType.relative && relative_output_mode != null) {
            return new Vector2(
                (input.X + 1) / 2 * relative_output_mode.Tablet.Properties.Specifications.Digitizer.MaxX,
                (input.Y + 1) / 2 * relative_output_mode.Tablet.Properties.Specifications.Digitizer.MaxY
            );
        }
        try_resolve_output_mode();
        return default;
    }

    [Resolved]
    public IDriver driver;
    public OutputModeType output_mode_type;
    private AbsoluteOutputMode absolute_output_mode;
    private RelativeOutputMode relative_output_mode;
    private void try_resolve_output_mode()
    {
        if (driver is Driver drv)
        {
            IOutputMode output = drv.InputDevices
                .Where(dev => dev?.OutputMode?.Elements?.Contains(this) ?? false)
                .Select(dev => dev?.OutputMode).FirstOrDefault();

            if (output is AbsoluteOutputMode abs_output) {
                absolute_output_mode = abs_output;
                output_mode_type = OutputModeType.absolute;
                return;
            }
            if (output is RelativeOutputMode rel_output) {
                relative_output_mode = rel_output;
                output_mode_type = OutputModeType.relative;
                return;
            }
            output_mode_type = OutputModeType.unknown;
        }
    }

    public abstract event Action<IDeviceReport> Emit;
    public abstract void Consume(IDeviceReport value);
    public abstract PipelinePosition Position { get; }
}
public enum OutputModeType {
    absolute,
    relative,
    unknown
}