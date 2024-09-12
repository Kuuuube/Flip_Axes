using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Output;
using System;
using System.Numerics;

namespace Flip_Axes
{
    [PluginName("Flip Tablet Axes")]
    public class Flip_Tablet_Axes : Flip_Axes_Base
    {

        public Vector2 Flip(Vector2 input)
        {
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

        public Vector2 Filter(Vector2 input) => FromUnit_Tablet(Flip(ToUnit_Tablet(input)));

        public override PipelinePosition Position => PipelinePosition.PreTransform;

        [BooleanProperty("Flip X Axis", ""), ToolTip
            ("Flip Tablet Axes:\n\n" +
            "Flip X Axis: Flips the tablet's X axis coordinates.")]
        public bool Flip_X { set; get; }

        [BooleanProperty("Flip Y Axis", ""), ToolTip
        ("Flip Tablet Axes:\n\n" +
        "Flip X Axis: Flips the tablet's Y axis coordinates.")]
        public bool Flip_Y { set; get; }
    }
}