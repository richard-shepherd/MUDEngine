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
        /// Loads a small-bag and checks its properties.
        /// </summary>
        [Test]
        public void loadSmallBag()
        {
            // We load the small-bag...
            var builtInObjectsRoot = Path.GetFullPath("../WorldLib/BuiltInObjects");
            var smallBagPath = Path.Combine(builtInObjectsRoot, "Containers/small-bag.json");
            var smallBag = ObjectUtils.loadObject(smallBagPath);

            // We check base-object properties...
            Assert.AreEqual("small-bag", smallBag.ObjectID);
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
