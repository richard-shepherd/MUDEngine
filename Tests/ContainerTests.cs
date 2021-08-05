using NUnit.Framework;
using System;
using System.IO;
using WorldLib;

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
        }
    }
}