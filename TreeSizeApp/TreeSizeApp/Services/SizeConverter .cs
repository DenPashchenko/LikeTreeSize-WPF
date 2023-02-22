using System;
using TreeSizeApp.Services.Interfaces;

namespace TreeSizeApp.Services
{
    public class SizeConverter : ISizeConverter
    {
        private const double ConversionFactor = 1024;
        public string Convert(long size)
        {
            double sizeInUnits;
            string units;
            if (size < ConversionFactor)
            {
                sizeInUnits = size;
                units = "bytes";
            }
            else if (size < Math.Pow(ConversionFactor, 2))
            {
                sizeInUnits = (double)(size / ConversionFactor);
                units = "KB";
            }
            else if (size < Math.Pow(ConversionFactor, 3))
            {
                sizeInUnits = (double)(size / Math.Pow(ConversionFactor, 2));
                units = "MB";
            }
            else
            {
                sizeInUnits = (double)(size / Math.Pow(ConversionFactor, 3));
                units = "GB";
            }

            if (units == "bytes")
            {
                return $"{sizeInUnits} {units}";
            }
            return $"{sizeInUnits:F2} {units}";
        }
    }
}
