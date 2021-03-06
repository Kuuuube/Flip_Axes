using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin;
using System;
using System.Numerics;

namespace Flip_Axes
{
    [PluginName("Flip Axes")]
    public class Flip_Axes : Flip_Axes_Base
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
            if (value is ITabletReport report)
            {
                report.Position = Filter(report.Position);
                value = report;
            }

            Emit?.Invoke(value);
        }

        public Vector2 Filter(Vector2 input) => FromUnit(Flip(ToUnit(input)));

        public override PipelinePosition Position => PipelinePosition.PostTransform;

        [BooleanProperty("Flip X Axis", ""), ToolTip
            ("Flip Axes:\n\n" +
            "Flip X Axis: Flips the X axis coordinates.")]
        public bool Flip_X { set; get; }

        [BooleanProperty("Flip Y Axis", ""), ToolTip
        ("Flip Axes:\n\n" +
        "Flip X Axis: Flips the Y axis coordinates.")]
        public bool Flip_Y { set; get; }
    }
}