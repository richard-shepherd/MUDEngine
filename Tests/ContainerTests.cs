using NUnit.Framework;
using System.IO;
using WorldLib;

namespace Tests
{
    /// <summary>
    /// Tests loading Containers and checking properties and
    /// operations on them.
    /// </summary>
    public class ContainerTests
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

            // We check container properties...
            Assert.AreEqual(5, smallBag.Capacity.Items);
            Assert.AreEqual(10.0, smallBag.Capacity.WeightKG);
        }

        /// <summary>
        /// Tests adding items to a small-bag.
        /// </summary>
        [Test]
        public void addItems()
        {
            // We create a small-bag...
            var smallBag = m_objectFactory.createObjectAs<Container>("small-bag");

            // We load two apples...

            // We add two apples

        }
    }
}