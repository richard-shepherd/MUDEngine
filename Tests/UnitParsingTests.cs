using NUnit.Framework;
using Utility;

namespace Tests
{
    /// <summary>
    /// Tests of unit parsing, eg "10cm" -> 0.1 (meters).
    /// </summary>
    public class UnitParsingTests
    {
        /// <summary>
        /// Tests parsing lengths in kilometers.
        /// </summary>
        [Test]
        public void lengthKM()
        {
            var length = UnitsHelper.parse("2.5km");
            Assert.AreEqual(2500.0, length);
        }

        /// <summary>
        /// Tests parsing lengths in meters.
        /// </summary>
        [Test]
        public void lengthM()
        {
            var length = UnitsHelper.parse("7m");
            Assert.AreEqual(7.0, length);
        }

        /// <summary>
        /// Tests parsing lengths in centimeters.
        /// </summary>
        [Test]
        public void lengthCM()
        {
            var length = UnitsHelper.parse("14.75cm");
            Assert.AreEqual(0.1475, length);
        }

        /// <summary>
        /// Tests parsing lengths in millimeters.
        /// </summary>
        [Test]
        public void lengthMM()
        {
            var length = UnitsHelper.parse("7.5mm");
            Assert.AreEqual(0.0075, length);
        }

        /// <summary>
        /// Tests parsing weight in kilograms.
        /// </summary>
        [Test]
        public void weightKG()
        {
            var weight = UnitsHelper.parse("34kg");
            Assert.AreEqual(34.0, weight);
        }

        /// <summary>
        /// Tests parsing weight in grams.
        /// </summary>
        [Test]
        public void weightG()
        {
            var weight = UnitsHelper.parse("90g");
            Assert.AreEqual(0.090, weight);
        }

        /// <summary>
        /// Tests parsing weight in grams.
        /// </summary>
        [Test]
        public void negativeWeightG()
        {
            var weight = UnitsHelper.parse("-90g");
            Assert.AreEqual(-0.090, weight);
        }
    }
}
