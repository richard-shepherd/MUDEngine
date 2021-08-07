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
        /// Tests adding items to a small-bag, checking how many items the bag can hold.
        /// </summary>
        [Test]
        public void addItems_Capacity()
        {
            // We create a small-bag...
            var smallBag = m_objectFactory.createObjectAs<Container>("small-bag");

            // We apples and add them to the bag, checking that each addition succeeds...
            var apple = m_objectFactory.createObjectAs<Food>("apple");
            var result = smallBag.add(apple);
            Assert.AreEqual(ActionResult.StatusEnum.SUCCEEDED, result.Status);
            Assert.AreEqual(1, smallBag.ItemCount);

            apple = m_objectFactory.createObjectAs<Food>("apple");
            result = smallBag.add(apple);
            Assert.AreEqual(ActionResult.StatusEnum.SUCCEEDED, result.Status);
            Assert.AreEqual(2, smallBag.ItemCount);

            apple = m_objectFactory.createObjectAs<Food>("apple");
            result = smallBag.add(apple);
            Assert.AreEqual(ActionResult.StatusEnum.SUCCEEDED, result.Status);
            Assert.AreEqual(3, smallBag.ItemCount);

            apple = m_objectFactory.createObjectAs<Food>("apple");
            result = smallBag.add(apple);
            Assert.AreEqual(ActionResult.StatusEnum.SUCCEEDED, result.Status);
            Assert.AreEqual(4, smallBag.ItemCount);

            apple = m_objectFactory.createObjectAs<Food>("apple");
            result = smallBag.add(apple);
            Assert.AreEqual(ActionResult.StatusEnum.SUCCEEDED, result.Status);
            Assert.AreEqual(5, smallBag.ItemCount);

            // We add another apple. This addition should fail as the small-bag can 
            // only hold five items...
            apple = m_objectFactory.createObjectAs<Food>("apple");
            result = smallBag.add(apple);
            Assert.AreEqual(ActionResult.StatusEnum.FAILED, result.Status);
            Assert.IsTrue(result.Description.Contains("full"));
            Assert.AreEqual(5, smallBag.ItemCount);
        }

        /// <summary>
        /// Tests adding items to a small-bag, checking the weight the bag can hold.
        /// </summary>
        [Test]
        public void addItems_Weight()
        {
            // We create a small-bag...
            var smallBag = m_objectFactory.createObjectAs<Container>("small-bag");

            // We create 'heavy' apples and add them to the bag, checking that each addition succeeds...
            var apple = m_objectFactory.createObjectAs<Food>("apple");
            apple.WeightKG = 4.0;
            var result = smallBag.add(apple);
            Assert.AreEqual(ActionResult.StatusEnum.SUCCEEDED, result.Status);
            Assert.AreEqual(1, smallBag.ItemCount);

            apple = m_objectFactory.createObjectAs<Food>("apple");
            apple.WeightKG = 4.0;
            result = smallBag.add(apple);
            Assert.AreEqual(ActionResult.StatusEnum.SUCCEEDED, result.Status);
            Assert.AreEqual(2, smallBag.ItemCount);

            // We add another apple. This addition should fail as the small-bag can 
            // only hold 10kg...
            apple = m_objectFactory.createObjectAs<Food>("apple");
            apple.WeightKG = 4.0;
            result = smallBag.add(apple);
            Assert.AreEqual(ActionResult.StatusEnum.FAILED, result.Status);
            Assert.IsTrue(result.Description.Contains("too heavy"));
            Assert.AreEqual(2, smallBag.ItemCount);
        }
    }
}