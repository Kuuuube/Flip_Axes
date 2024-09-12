using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Output;
using System;
using System.Numerics;

namespace Flip_Axes
{
    [PluginName("Flip Area Axes")]
    public class Flip_Area_Axes : Flip_Axes_Base
    {

        public Vector2 Flip(Vector2 input)
        {
            if (output_mode_type != OutputModeType.absolute) {
                return input;
            }

            if (Flip_X)
            {
                input.X = input.X * -1;
            }
            if (Flip_Y)
            {
                input.Y = input.Y * -1;
            }
            return input;
        }

        public override event Action<IDeviceReport> Emit;

        public override void Consume(IDeviceReport value)
        {
            if (value is IAbsolutePositionReport report)
            {
                report.Position = Filter(report.Position);
                value = report;
            }

            Emit?.Invoke(value);
        }

        public Vector2 Filter(Vector2 input) => FromUnit_Screen(Flip(ToUnit_Screen(input)));

        public override PipelinePosition Position => PipelinePosition.PostTransform;

        [BooleanProperty("Flip X Axis", ""), ToolTip
            ("Flip Area Axes:\n\n" +
            "Flip X Axis: Flips the tablet area's X axis coordinates.")]
        public bool Flip_X { set; get; }

        [BooleanProperty("Flip Y Axis", ""), ToolTip
        ("Flip Area Axes:\n\n" +
        "Flip X Axis: Flips the tablet area's Y axis coordinates.")]
        public bool Flip_Y { set; get; }
    }
}