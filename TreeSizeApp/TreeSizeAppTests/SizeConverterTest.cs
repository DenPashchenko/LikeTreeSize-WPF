using TreeSizeApp.Services;

namespace TreeSizeAppTests
{
    [TestClass]
    public class SizeConverterTest
    {
        [TestMethod]
        [DataRow(10, "10 bytes")]
        [DataRow(1_023, "1023 bytes")]
        [DataRow(1_024, "1,00 KB")]
        [DataRow(2_000, "1,95 KB")]
        [DataRow(1_047_552, "1023,00 KB")]
        [DataRow(1_048_576, "1,00 MB")]
        [DataRow(1_072_693_248, "1023,00 MB")]
        [DataRow(1_073_741_824, "1,00 GB")]
        [DataRow(2_000_000_000, "1,86 GB")]
        public void Convert_DifferentNumbers_SuccesfullConversion(long size, string expected)
        {
            SizeConverter converter = new();
            string expectedResult = expected;

            string actualResult = converter.Convert(size);

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
