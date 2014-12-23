using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SystemColorChart
{
    public class ColorsModel
    {
        private readonly IEnumerable<ColorModel> _colorModels;

        [DllImport("dwmapi.dll", EntryPoint = "#127")]
        static extern void DwmGetColorizationParameters(ref DWMCOLORIZATIONPARAMS dp);

        [StructLayout(LayoutKind.Sequential)]
        public struct DWMCOLORIZATIONPARAMS
        {
            public UInt32 ColorizationColor;
            public UInt32 ColorizationAfterglow;
            public UInt32 ColorizationColorBalance;
            public UInt32 ColorizationAfterglowBalance;
            public UInt32 ColorizationBlurBalance;
            public UInt32 ColorizationGlassReflectionIntensity;
            public UInt32 ColorizationOpaqueBlend;
        }

        public ColorsModel()
        {            
            var colorType = typeof (Color);
            var colorModels = typeof(SystemColors).GetProperties().Where(pi => pi.PropertyType == colorType)
                .Select(pi => new ColorModel("SystemColor", pi.Name, (Color)pi.GetValue(null)));            

            var colorizationParams = new DWMCOLORIZATIONPARAMS();
            DwmGetColorizationParameters(ref colorizationParams);

            _colorModels =
                colorModels.Union(GetDwmColorModels(colorizationParams))
                    .OrderBy(m => m.Classification)
                    .ThenBy(m => m.Name)
                    .ToList();
        }

        private static IEnumerable<ColorModel> GetDwmColorModels(DWMCOLORIZATIONPARAMS colorizationParams)
        {
            const string classification = "DwmColorization";
            
            var frameColor = ToColor(colorizationParams.ColorizationColor);

            yield return new ColorModel(classification, "ColorizationColor", frameColor);

            var baseColor = Color.FromRgb(217, 217, 217);
            var blendedColor = BlendColor(frameColor, baseColor, 100 - colorizationParams.ColorizationColorBalance);

            yield return new ColorModel(classification, "ColorizationColor (balanced)", blendedColor);
        }

        private static Color BlendColor(Color color1, Color color2, double percentage)
        {
            if ((percentage < 0) || (100 < percentage))
                throw new ArgumentOutOfRangeException("percentage");

            return Color.FromRgb(
                BlendColorChannel(color1.R, color2.R, percentage),
                BlendColorChannel(color1.G, color2.G, percentage),
                BlendColorChannel(color1.B, color2.B, percentage));
        }

        private static byte BlendColorChannel(double channel1, double channel2, double channel2Percentage)
        {
            var buff = channel1 + (channel2 - channel1) * channel2Percentage / 100D;
            return Math.Min((byte)Math.Round(buff), (byte)255);
        }

        private static Color ToColor(UInt32 value)
        {
            return Color.FromArgb(255,
                (byte) (value >> 16),
                (byte) (value >> 8),
                (byte) value
                );

        }

        public IEnumerable<ColorModel> ColorModels
        {
            get { return _colorModels; }            
        }
    }
}
