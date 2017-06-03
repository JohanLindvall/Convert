// Copyright(c) 2017 Johan Lindvall
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class Test
    {
        [TestMethod]
        public void TestToBase64()
        {
            var data = new byte[] { 0xfe, 0x33, 0x17, 0x82 };
            var converted = Convert.Convert.ToBase64(data);
            Assert.AreEqual("/jMXgg==", converted);
        }

        [TestMethod]
        public void TestToBase64Url()
        {
            var data = new byte[] { 0xfe, 0x33, 0x17, 0x82 };
            var converted = Convert.Convert.ToBase64Url(data);

            Assert.AreEqual("_jMXgg", converted);
        }

        [TestMethod]
        public void TestFromBase64()
        {
            var data = new byte[] { 0xfe, 0x33, 0x17, 0x82 };
            var input = "/jMXgg==";

            var converted = Convert.Convert.FromBase64(input);
            CollectionAssert.AreEqual(data, converted);
        }

        [TestMethod]
        public void TestFromBase64Url()
        {
            var data = new byte[] { 0xfe, 0x33, 0x17, 0x82 };
            var input = "_jMXgg";

            var converted = Convert.Convert.FromBase64Url(input);
            CollectionAssert.AreEqual(data, converted);
        }

        [TestMethod]
        public void TestToHex()
        {
            var data = new byte[] { 0xfe, 0x33, 0x17, 0x82 };
            var converted = Convert.Convert.ToHex(data);
            Assert.AreEqual("fe331782", converted);
        }


        [TestMethod]
        public void TestFromHex()
        {
            var data = new byte[] { 0xfe, 0x33, 0x17, 0x82 };
            CollectionAssert.AreEqual(data, Convert.Convert.FromHex("fe331782"));
        }
    }
}
