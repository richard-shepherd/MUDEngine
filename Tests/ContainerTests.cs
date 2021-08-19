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
            m_worldManager = new WorldManager("");
            m_worldManager.ObjectFactory.addRootFolder("../WorldLib/BuiltInObjects");
            m_objectFactory = m_worldManager.ObjectFactory;
        }
        private WorldManager m_worldManager;
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
            Assert.AreEqual("The small bag is full.", result.Message);
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
            Assert.AreEqual("The apple is too heavy to be added to the small bag.", result.Message);
            Assert.AreEqual(2, smallBag.ItemCount);
        }

        /// <summary>
        /// Tests adding items to a small-bag, checking that the objects fit into the bag.
        /// </summary>
        [Test]
        public void addItems_Size()
        {
            // We create a small-bag...
            var smallBag = m_objectFactory.createObjectAs<Container>("small-bag");

            // We create a too-large apple and add it to the bag...
            var apple = m_objectFactory.createObjectAs<Food>("apple");
            apple.Dimensions.WidthM = 2.0;
            var result = smallBag.add(apple);
            Assert.AreEqual(ActionResult.StatusEnum.FAILED, result.Status);
            Assert.AreEqual("The apple is too large to add to the small bag.", result.Message);
            Assert.AreEqual(0, smallBag.ItemCount);
        }

        /// <summary>
        /// Tests that we can put a small-bag in a backpack.
        /// </summary>
        [Test]
        public void smallBagInBackpack()
        {
            // We create a small-bag and a backpack...
            var smallBag = m_objectFactory.createObjectAs<Container>("small-bag");
            var backpack = m_objectFactory.createObjectAs<Container>("backpack");

            // We check that the small-bag can be put into the backpack...
            var result = backpack.add(smallBag);
            Assert.AreEqual(ActionResult.StatusEnum.SUCCEEDED, result.Status);
            Assert.AreEqual(1, backpack.ItemCount);
        }

        /// <summary>
        /// Tests that we cannot put a backpack in a small-bag.
        /// </summary>
        [Test]
        public void backpackInSmallBag()
        {
            // We create a small-bag and a backpack...
            var smallBag = m_objectFactory.createObjectAs<Container>("small-bag");
            var backpack = m_objectFactory.createObjectAs<Container>("backpack");

            // We check that the backpack cannot be put into the small-bag...
            var result = smallBag.add(backpack);
            Assert.AreEqual(ActionResult.StatusEnum.FAILED, result.Status);
            Assert.AreEqual(0, smallBag.ItemCount);
        }

        /// <summary>
        /// Tests that we cannot put a small-bag in a small-bag,
        /// ie, that only items smaller than the container can be added to it.
        /// </summary>
        [Test]
        public void smallBagInSmallBag()
        {
            // We create two small-bags...
            var smallBag1 = m_objectFactory.createObjectAs<Container>("small-bag");
            var smallBag2 = m_objectFactory.createObjectAs<Container>("small-bag");

            // We check that the small-bag cannot be put into the small-bag...
            var result = smallBag1.add(smallBag2);
            Assert.AreEqual(ActionResult.StatusEnum.FAILED, result.Status);
            Assert.AreEqual(0, smallBag1.ItemCount);
        }

        /// <summary>
        /// Tests that the total weight of a container is taken into account when adding
        /// it to another container.
        /// </summary>
        [Test]
        public void totalWeight_EmptyBags()
        {
            // We confirm that five empty small bags can be added to a backpack...
            var backpack = m_objectFactory.createObjectAs<Container>("backpack");
            for (var i = 1; i <= 5; ++i)
            {
                var smallBag = m_objectFactory.createObjectAs<Container>("small-bag");
                var result = backpack.add(smallBag);
                Assert.AreEqual(ActionResult.StatusEnum.SUCCEEDED, result.Status);
                Assert.AreEqual(i, backpack.ItemCount);
            }
        }

        /// <summary>
        /// Tests that the total weight of a container is taken into account when adding
        /// it to another container.
        /// </summary>
        [Test]
        public void totalWeight_FullBags()
        {
            // We confirm that four full small bags can be added to a backpack...
            var backpack = m_objectFactory.createObjectAs<Container>("backpack");
            for (var i = 1; i <= 4; ++i)
            {
                var smallBag = m_objectFactory.createObjectAs<Container>("small-bag");
                var apple = m_objectFactory.createObjectAs<Food>("apple");
                apple.WeightKG = 10.0;
                var result = smallBag.add(apple);
                Assert.AreEqual(ActionResult.StatusEnum.SUCCEEDED, result.Status);
                Assert.AreEqual(1, smallBag.ItemCount);
                Assert.AreEqual(11, smallBag.TotalWeightKG);

                result = backpack.add(smallBag);
                Assert.AreEqual(ActionResult.StatusEnum.SUCCEEDED, result.Status);
                Assert.AreEqual(i, backpack.ItemCount);
                Assert.AreEqual(5 + i * 11, backpack.TotalWeightKG);
            }

            // We add another full bag to the backpack. This should now be too heavy...
            {
                var smallBag = m_objectFactory.createObjectAs<Container>("small-bag");
                var apple = m_objectFactory.createObjectAs<Food>("apple");
                apple.WeightKG = 10.0;
                var result = smallBag.add(apple);
                Assert.AreEqual(ActionResult.StatusEnum.SUCCEEDED, result.Status);
                Assert.AreEqual(1, smallBag.ItemCount);
                Assert.AreEqual(11, smallBag.TotalWeightKG);

                result = backpack.add(smallBag);
                Assert.AreEqual(ActionResult.StatusEnum.FAILED, result.Status);
                Assert.AreEqual("The small bag is too heavy to be added to the backpack.", result.Message);
                Assert.AreEqual(4, backpack.ItemCount);
            }
        }
    }
}