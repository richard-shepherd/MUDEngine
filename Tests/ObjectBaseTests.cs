using NUnit.Framework;
using System.IO;
using WorldLib;
using static WorldLib.ObjectBase;

namespace Tests
{
    /// <summary>
    /// Tests for the ObjectBase class.
    /// </summary>
    public class ObjectBaseTests
    {
        /// <summary>
        /// Sets up the tests.
        /// </summary>
        [SetUp]
        public void setup()
        {
            m_objectFactory = new ObjectFactory();
            m_objectFactory.addRootFolder("../WorldLib/BuiltInObjects");
        }
        private ObjectFactory m_objectFactory;

        /// <summary>
        /// Creates a small-bag and checks its properties.
        /// </summary>
        [Test]
        public void loadSmallBag()
        {
            // We create a small-bag...
            var smallBag = m_objectFactory.createObjectAs<Container>("small-bag");

            // We check base-object properties...
            Assert.AreEqual("small-bag", smallBag.ID);
            Assert.AreEqual(ObjectTypeEnum.CONTAINER, smallBag.ObjectType);
            Assert.AreEqual("small bag", smallBag.Name);
            Assert.IsTrue(smallBag.Aliases.Contains("bag"));
            Assert.AreEqual(0.6, smallBag.Dimensions.HeightM);
            Assert.AreEqual(0.4, smallBag.Dimensions.WidthM);
            Assert.AreEqual(0.3, smallBag.Dimensions.DepthM);
            Assert.AreEqual(1.0, smallBag.WeightKG);
        }
    }
}
