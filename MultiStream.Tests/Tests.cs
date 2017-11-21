using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MultiStream.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void Empty()
        {
            var s = new Lib.MultiStream();

            Assert.AreEqual(0, s.Length);
            Assert.AreEqual(0, s.Position);
        }

        [TestMethod]
        public void EmptySeek()
        {
            var s = new Lib.MultiStream();

            Assert.AreEqual(0, s.Seek(100, SeekOrigin.Begin));
            Assert.AreEqual(0, s.Seek(100, SeekOrigin.Current));
            Assert.AreEqual(0, s.Seek(100, SeekOrigin.End));
        }

        [TestMethod]
        public void EmptyRead()
        {
            var s = new Lib.MultiStream();

            var bytes = new byte[100];

            var bytesRead = s.Read(bytes, 0, 100);

            Assert.AreEqual(0, bytesRead);
        }

        [TestMethod]
        public void SingleStreamSeek()
        {
            var ms = GetStream("hello world");

            var s = new Lib.MultiStream(ms);

            var position = s.Seek(2, SeekOrigin.Begin);

            Assert.AreEqual(2, position);

            var bytes = new byte[100];

            var bytesRead = s.Read(bytes, 0, 100);

            Assert.AreEqual(9, bytesRead);
            Assert.AreEqual(11, s.Position);
        }

        [TestMethod]
        public void SingleStreamRead()
        {
            var ms = GetStream("hello world");

            var s = new Lib.MultiStream(ms);

            Assert.AreEqual(11, s.Length);

            var bytes = new byte[100];

            var bytesRead = s.Read(bytes, 0, 100);

            Assert.AreEqual(11, bytesRead);
            Assert.AreEqual(11, s.Position);
        }

        [TestMethod]
        public void SingleStreamReadTwice()
        {
            var ms = GetStream("hello world");

            var s = new Lib.MultiStream(ms);

            Assert.AreEqual(11, s.Length);

            var bytes = new byte[100];

            var bytesRead = s.Read(bytes, 0, 4);

            Assert.AreEqual(4, bytesRead);
            Assert.AreEqual(4, s.Position);

            bytesRead = s.Read(bytes, 0, 3);

            Assert.AreEqual(3, bytesRead);
            Assert.AreEqual(7, s.Position);
        }

        [TestMethod]
        public void SingleStreamConsumeFromStreamReader()
        {
            var ms = GetStream("hello world");

            var s = new Lib.MultiStream(ms);

            Assert.AreEqual(11, s.Length);

            var reader = new StreamReader(s);

            var output = reader.ReadToEnd();

            Assert.AreEqual("hello world", output);
        }

        [TestMethod]
        public void MultiStreamRead()
        {
            var ms1 = GetStream("hello world");

            var ms2 = GetStream("goodbye world");

            var s = new Lib.MultiStream(ms1, ms2);

            Assert.AreEqual(24, s.Length);

            var bytes = new byte[100];

            var bytesRead = s.Read(bytes, 0, 4);

            Assert.AreEqual(4, bytesRead);
            Assert.AreEqual(4, s.Position);

            bytesRead = s.Read(bytes, 0, 14);

            Assert.AreEqual(14, bytesRead);
            Assert.AreEqual(18, s.Position);
        }

        [TestMethod]
        public void MultiStreamReadTwice()
        {
            var ms1 = GetStream("hello world");

            var ms2 = GetStream("goodbye world");

            var s = new Lib.MultiStream(ms1, ms2);

            Assert.AreEqual(24, s.Length);

            var bytes = new byte[100];

            var bytesRead = s.Read(bytes, 0, 10);

            Assert.AreEqual(10, bytesRead);
            Assert.AreEqual(10, s.Position);

            bytesRead = s.Read(bytes, 0, 20);

            Assert.AreEqual(14, bytesRead);
            Assert.AreEqual(24, s.Position);
        }

        [TestMethod]
        public void MultiStreamConsumeFromStreamReader()
        {
            var ms1 = GetStream("hello world");

            var ms2 = GetStream("goodbye world");

            var s = new Lib.MultiStream(ms1, ms2);

            var reader = new StreamReader(s);

            var output = reader.ReadToEnd();

            Assert.AreEqual("hello worldgoodbye world", output);
        }

        private static MemoryStream GetStream(string msg)
        {
            var ms = new MemoryStream();

            var writer = new StreamWriter(ms, Encoding.ASCII);

            writer.Write(msg);

            writer.Flush();

            ms.Position = 0;

            return ms;
        }
    }
}
