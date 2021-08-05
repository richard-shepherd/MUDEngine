using NUnit.Framework;
using System.IO;
using WorldLib;
using static WorldLib.ObjectBase;

namespace Tests
{
    public class ContainerTests
    {
        [Test]
        public void Test1()
        {
            var builtInObjectsRoot = Path.GetFullPath("../WorldLib/BuiltInObjects");
            var smallBagPath = Path.Combine(builtInObjectsRoot, "Containers/SmallBag.json");
            var smallBag = ObjectUtils.loadObject(smallBagPath);
            Assert.AreEqual(ObjectTypeEnum.CONTAINER, smallBag.ObjectType);
        }
    }
}