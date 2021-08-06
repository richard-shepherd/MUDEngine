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
        /// Loads a small-bag and checks its properties.
        /// </summary>
        [Test]
        public void loadSmallBag()
        {
            // We load the small-bag...
            var builtInObjectsRoot = Path.GetFullPath("../WorldLib/BuiltInObjects");
            var smallBagPath = Path.Combine(builtInObjectsRoot, "Containers/small-bag.json");
            var smallBag = ObjectUtils.loadObjectAs<Container>(smallBagPath);

            // We check container properties...
            Assert.AreEqual(5, smallBag.Capacity.Items);
            Assert.AreEqual(10.0, smallBag.Capacity.WeightKG);
        }
    }
}