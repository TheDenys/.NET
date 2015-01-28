using SevenZipLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SevenZipLib.Tests
{
    /// <summary>
    ///This is a test class for SevenZipArchiveTest and is intended
    ///to contain all SevenZipArchive Unit Tests
    ///</summary>
    [TestClass()]
    [DeploymentItem("SevenZipLib.Tests/Archives", "Archives")]
    [DeploymentItem("SevenZipLib.Tests/TestData", "TestData")]
    [DeploymentItem("7z64.dll")]
    [DeploymentItem("7z86.dll")]
    public class SevenZipArchiveTest
    {
        private const string DataProvider = "Microsoft.VisualStudio.TestTools.DataSource.CSV";
        private static string ArchiveDirectory { get; set; }
        private static string ExpectedArchiveDumpDirectory { get; set; }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        private class ReverseStringComparer : IComparer<string>
        {
            #region IComparer<string> Members

            public int Compare(string x, string y)
            {
                return -x.CompareTo(y);
            }

            #endregion
        }

        private class XNodeComparer : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                return XNode.EqualityComparer.Equals(x as XNode, y as XNode) ? 0 : 1;
            }

            #endregion
        }

        /// <summary>
        /// Initialize the test class
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // Extract all the archives needed to run tests

            ArchiveDirectory = Path.Combine(testContext.TestDeploymentDir, "Archives");
            string path = Path.Combine(testContext.TestDeploymentDir, "Archives", "Archives.rar");
            Assert.IsTrue(File.Exists(path));
            using (var archive = new SevenZipArchive(path))
            {
                archive.ExtractAll(Path.GetDirectoryName(path));
            }

            ExpectedArchiveDumpDirectory = Path.Combine(ArchiveDirectory, "Dumps");
        }

        private void DumpArchive(SevenZipArchive archive, string path)
        {
            using (var fs = File.Create(path))
            {
                XElement dump = DumpArchive(archive);
                dump.Save(fs);
            }
        }

        private XElement DumpArchive(SevenZipArchive archive)
        {
            if (archive == null) return null;

            return new XElement("Archive",
                new XElement("FileName", Path.GetFileName(archive.FileName)),
                new XElement("Format", archive.Format),
                new XElement("PackedSize", archive.PackedSize),
                new XElement("UnPackedSize", archive.UnPackedSize),
                new XElement("Password", archive.Password),
                new XElement("IsEncrypted", archive.IsEncrypted),
                new XElement("HasSubArchive", archive.HasSubArchive),
                new XElement("Volumes",
                    from v in archive.Volumes
                    select new XElement("Volume", Path.GetFileName(v))),
                new XElement("Properties",
                    from p in archive.Properties
                    select new XElement("Property",
                        new XAttribute("Name", p.PropertyName ?? p.Property.ToString()),
                        p.Value)),
                new XElement("Entries",
                    from x in archive
                    select new XElement("Entry",
                        new XElement("FileName", x.FileName),
                        new XElement("LastWriteTime", x.LastWriteTime),
                        new XElement("CreationTime", x.CreationTime),
                        new XElement("LastAccessTime", x.LastAccessTime),
                        new XElement("Size", x.Size),
                        new XElement("Crc", x.Crc),
                        new XElement("Attributes", x.Attributes),
                        new XElement("IsDirectory", x.IsDirectory),
                        new XElement("IsEncrypted", x.IsEncrypted),
                        new XElement("IsUntitled", x.IsUntitled),
                        new XElement("Comment", x.Comment))),
                new XElement("Parent", DumpArchive(archive.Parent))
                );
        }

        private Stream GetFileStream(string path)
        {
            if (Path.GetExtension(path) == ".001")
            {
                IEnumerable<string> files = Directory.GetFiles(Path.GetDirectoryName(path),
                     Path.GetFileNameWithoutExtension(path) + ".0*");
                files = files.OrderBy(x => int.Parse(Path.GetExtension(x).TrimStart('.')));

                MemoryStream ms = new MemoryStream();
                foreach (var file in files)
                {
                    using (var fs = File.OpenRead(file))
                    {
                        byte[] bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, bytes.Length);
                        ms.Write(bytes, 0, bytes.Length);
                    }
                }
                return ms;
            }
            else
            {
                return File.OpenRead(path);
            }
        }

        private string CreateTestResultsDirectory()
        {
            string path = TestContext.TestResultsDirectory;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        private string CreateTestExtractionDirectory(string testResultsDir, string archiveName)
        {
            string path = Path.Combine(testResultsDir, archiveName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                string[] dirs = Directory.GetDirectories(path);

                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                foreach (string dir in dirs)
                {
                    DeleteDirectory(dir);
                }

                try
                {
                    Directory.Delete(path, false);
                }
                catch(IOException)
                {
                    System.Threading.Thread.Sleep(500);
                    Directory.Delete(path, false);
                }
            }
        }

        private void CompareDirectoryContents(string expected, string actual)
        {
            string[] expectedDirectories = Directory.GetDirectories(expected, "*", SearchOption.AllDirectories);
            string[] actualDirectories = Directory.GetDirectories(actual, "*", SearchOption.AllDirectories);
            string[] expectedFiles = Directory.GetFiles(expected, "*", SearchOption.AllDirectories);
            string[] actualFiles = Directory.GetFiles(actual, "*", SearchOption.AllDirectories);

            CollectionAssert.AreEqual(expectedDirectories.Select(x => x.Substring(expected.Length)).ToArray(),
                actualDirectories.Select(x => x.Substring(actual.Length)).ToArray());
            CollectionAssert.AreEqual(expectedFiles.Select(x => x.Substring(expected.Length)).ToArray(),
                actualFiles.Select(x => x.Substring(actual.Length)).ToArray());

            for (int i = 0; i < expectedFiles.Length; i++)
            {
                CompareFileContents(expectedFiles[i], actualFiles[i]);
            }
        }

        private void CompareActualDirectoryContents(string expected, string actual)
        {
            string[] expectedDirectories = Directory.GetDirectories(expected, "*", SearchOption.AllDirectories);
            string[] actualDirectories = Directory.GetDirectories(actual, "*", SearchOption.AllDirectories);
            string[] expectedFiles = Directory.GetFiles(expected, "*", SearchOption.AllDirectories);
            string[] actualFiles = Directory.GetFiles(actual, "*", SearchOption.AllDirectories);

            actualDirectories.Select(x => x.Substring(actual.Length)).ToList().ForEach(x =>
                {
                    CollectionAssert.Contains(expectedDirectories.Select(y => y.Substring(expected.Length)).ToArray(), x);
                });

            actualFiles.Select(x => x.Substring(actual.Length)).ToList().ForEach(x =>
                {
                    CollectionAssert.Contains(expectedFiles.Select(y => y.Substring(expected.Length)).ToArray(), x);
                    CompareFileContents(Path.Combine(expected, x.TrimStart('\\')), Path.Combine(actual, x.TrimStart('\\')));
                });
        }

        private void CompareFileContents(string expected, string actual)
        {
            byte[] expectedBytes = File.ReadAllBytes(expected);
            byte[] actualBytes = File.ReadAllBytes(actual);

            try
            {
                CollectionAssert.AreEqual(expectedBytes, actualBytes);
            }
            catch (Exception e)
            {
                throw new AssertFailedException(string.Format("The file '{0}' does not match the expected file '{1}'.", actual, expected), e);
            }
        }

        private void CompareFileContents(string expected, Stream stream)
        {
            byte[] expectedBytes = File.ReadAllBytes(expected);
            byte[] actualBytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(actualBytes, 0, actualBytes.Length);

            try
            {
                CollectionAssert.AreEqual(expectedBytes, actualBytes);
            }
            catch (Exception e)
            {
                throw new AssertFailedException(string.Format("The specified data does not match the expected file '{1}'.", expected), e);
            }
        }

        private void VerifyAttributes(string path, ArchiveEntry entry)
        {
            if (entry.IsDirectory)
            {
                if (entry.CreationTime.HasValue) Assert.AreEqual(entry.CreationTime.Value, Directory.GetCreationTime(path));
                if (entry.LastAccessTime.HasValue) Assert.AreEqual(entry.LastAccessTime.Value, Directory.GetLastAccessTime(path));
                if (entry.LastWriteTime.HasValue) Assert.AreEqual(entry.LastWriteTime.Value, Directory.GetLastWriteTime(path));
                if (entry.Attributes.HasValue) Assert.AreEqual(entry.Attributes.Value, new DirectoryInfo(path).Attributes);
            }
            else
            {
                if (entry.CreationTime.HasValue) Assert.AreEqual(entry.CreationTime.Value, File.GetCreationTime(path));
                if (entry.LastAccessTime.HasValue) Assert.AreEqual(entry.LastAccessTime.Value, File.GetLastAccessTime(path));
                if (entry.LastWriteTime.HasValue) Assert.AreEqual(entry.LastWriteTime.Value, File.GetLastWriteTime(path));
                if (entry.Attributes.HasValue) Assert.AreEqual(entry.Attributes.Value, File.GetAttributes(path));
            }
        }

        private bool IsInvalidEntry(string fileName)
        {
            string[] parts = fileName.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                StringSplitOptions.RemoveEmptyEntries);

            char[] invalidChars = Path.GetInvalidFileNameChars();
            string reservedNamesPattern = @"^(PRN|AUX|CLOCK\$|NUL|CON|COM\d|LPT\d|\.+)$";

            for (int j = 0; j < parts.Length; j++)
            {
                if (Regex.IsMatch(parts[j], reservedNamesPattern, RegexOptions.IgnoreCase))
                {
                    return true;
                }

                StringBuilder part = new StringBuilder(parts[j]);
                for (int i = 0; i < part.Length; i++)
                {
                    if (Array.IndexOf(invalidChars, part[i]) >= 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private string AutoRenamePath(string baseDirectory, string relativePath)
        {
            string renamedPath = null;

            char[] invalidChars = Path.GetInvalidFileNameChars();
            string reservedNamesPattern = @"^(PRN|AUX|CLOCK\$|NUL|CON|COM\d|LPT\d|\.+)$";

            string[] parts = relativePath.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                StringSplitOptions.RemoveEmptyEntries);

            List<int> invalidPartIndices = new List<int>();
            StringBuilder sb = new StringBuilder();

            for (int j = 0; j < parts.Length; j++)
            {
                StringBuilder part = new StringBuilder(parts[j]);

                if (Regex.IsMatch(parts[j], reservedNamesPattern, RegexOptions.IgnoreCase))
                {
                    invalidPartIndices.Add(j);
                    if (Regex.IsMatch(parts[j], @"^\.+$"))
                    {
                        part.Clear();
                        part.Append("[" + parts[j].Length + "]");
                    }
                    else
                    {
                        part.Insert(0, SevenZipArchive.InvalidCharReplacement);
                    }
                }
                else
                {
                    for (int i = 0; i < part.Length; i++)
                    {
                        if (Array.IndexOf(invalidChars, part[i]) >= 0)
                        {
                            invalidPartIndices.Add(j);
                            part[i] = SevenZipArchive.InvalidCharReplacement;
                        }
                    }
                }

                sb.Append(part.ToString());
                if (j < parts.Length - 1)
                {
                    sb.Append(Path.DirectorySeparatorChar);
                }
            }

            renamedPath = sb.ToString();
            return Path.Combine(baseDirectory, sb.ToString());
        }

        /// <summary>
        /// A test for SevenZipArchive Constructor
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ConstructorTests.csv", "ConstructorTests#csv", DataAccessMethod.Sequential)]
        public void SevenZipArchiveConstructorTest()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string password = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string rootFormat = Convert.ToString(TestContext.DataRow["RootFormat"]);
            ArchiveFormat format = (ArchiveFormat)Enum.Parse(typeof(ArchiveFormat), rootFormat);
            string actualDumpPath = Path.Combine(CreateTestResultsDirectory(), "[s]" + archiveName + ".xml");
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, "[s]" + archiveName + ".xml");

            using (var stream = GetFileStream(archivePath))
            {
                Assert.IsTrue(stream.CanRead);

                using (SevenZipArchive archive = new SevenZipArchive(stream, format, password))
                {
                    DumpArchive(archive, actualDumpPath);
                    CollectionAssert.AreEqual(File.ReadAllLines(expectedDumpPath), File.ReadAllLines(actualDumpPath));
                }

                Assert.IsFalse(stream.CanRead);
            }

            if (password != null)
            {
                using (var stream = GetFileStream(archivePath))
                {
                    Assert.IsTrue(stream.CanRead);

                    ExceptionAssert.Throws<PasswordRequiredException>("This archive is encrypted, and requires a password to be decrypted.", delegate
                    {
                        using (new SevenZipArchive(stream, format, null)) { }
                    });

                    Assert.IsFalse(stream.CanRead);
                }

                ExceptionAssert.Throws<BadPasswordException>("Incorrect password specified to decrypt the archive.", delegate
                {
                    using (new SevenZipArchive(GetFileStream(archivePath), format, "bad_password")) { }
                });
            }

            ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: stream", delegate
            {
                using (new SevenZipArchive((Stream)null, format, password)) { }
            });

            ExceptionAssert.Throws<ArgumentException>("The specified stream should be seekable and readable.\r\nParameter name: stream", delegate
            {
                MemoryStream ms = new MemoryStream();
                ms.Close();
                using (new SevenZipArchive(ms, format, password)) { }
            });
        }

        /// <summary>
        ///A test for SevenZipArchive Constructor
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ConstructorTests.csv", "ConstructorTests#csv", DataAccessMethod.Sequential)]
        public void SevenZipArchiveConstructorTest1()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string password = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string actualDumpPath = Path.Combine(CreateTestResultsDirectory(), archiveName + ".xml");
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, archiveName + ".xml");
            IComparer<string> comparer = new ReverseStringComparer();

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, password, comparer))
            {
                DumpArchive(archive, actualDumpPath);
                XElement expected = XDocument.Load(expectedDumpPath).Root;
                XElement actual = XDocument.Load(actualDumpPath).Root;

                do
                {
                    CollectionAssert.AreEqual(
                        expected.Elements().Where(x => x.Name != "Entries" && x.Name != "Parent").ToArray(),
                        actual.Elements().Where(x => x.Name != "Entries" && x.Name != "Parent").ToArray(),
                        new XNodeComparer());

                    CollectionAssert.AreEqual(
                        expected.Element("Entries").Elements().OrderBy(x => x.Element("FileName").Value, comparer).ToArray(),
                        actual.Element("Entries").Elements().ToArray(),
                        new XNodeComparer());

                    expected = expected.Element("Parent").Element("Archive");
                    actual = actual.Element("Parent").Element("Archive");

                } while (expected != null && expected.HasElements);
            }

            if (password != null)
            {
                ExceptionAssert.Throws<PasswordRequiredException>("This archive is encrypted, and requires a password to be decrypted.", delegate
                {
                    using (new SevenZipArchive(archivePath, ArchiveFormat.Unkown, null, comparer)) { }
                });

                ExceptionAssert.Throws<BadPasswordException>("Incorrect password specified to decrypt the archive.", delegate
                {
                    using (new SevenZipArchive(archivePath, ArchiveFormat.Unkown, "bad_password", comparer)) { }
                });
            }

            ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: fileName", delegate
            {
                using (new SevenZipArchive((string)null, ArchiveFormat.Unkown, password, comparer)) { }
            });
        }


        /// <summary>
        ///A test for SevenZipArchive Constructor
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ConstructorTests.csv", "ConstructorTests#csv", DataAccessMethod.Sequential)]
        public void SevenZipArchiveConstructorTest2()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string password = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string actualDumpPath = Path.Combine(CreateTestResultsDirectory(), "[s]" + archiveName + ".xml");
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, "[s]" + archiveName + ".xml");

            using (var stream = GetFileStream(archivePath))
            {
                Assert.IsTrue(stream.CanRead);

                using (SevenZipArchive archive = new SevenZipArchive(stream, ArchiveFormat.Unkown, password))
                {
                    DumpArchive(archive, actualDumpPath);
                    CollectionAssert.AreEqual(File.ReadAllLines(expectedDumpPath), File.ReadAllLines(actualDumpPath));
                }

                Assert.IsFalse(stream.CanRead);
            }

            if (password != null)
            {
                using (var stream = GetFileStream(archivePath))
                {
                    Assert.IsTrue(stream.CanRead);

                    ExceptionAssert.Throws<PasswordRequiredException>("This archive is encrypted, and requires a password to be decrypted.", delegate
                    {
                        using (new SevenZipArchive(stream, ArchiveFormat.Unkown, (string)null)) { }
                    });

                    Assert.IsFalse(stream.CanRead);
                }

                ExceptionAssert.Throws<BadPasswordException>("Incorrect password specified to decrypt the archive.", delegate
                {
                    using (new SevenZipArchive(GetFileStream(archivePath), ArchiveFormat.Unkown, "bad_password")) { }
                });
            }

            ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: stream", delegate
            {
                using (new SevenZipArchive((Stream)null, ArchiveFormat.Unkown, password)) { }
            });

            ExceptionAssert.Throws<ArgumentException>("The specified stream should be seekable and readable.\r\nParameter name: stream", delegate
            {
                MemoryStream ms = new MemoryStream();
                ms.Close();
                using (new SevenZipArchive(ms, ArchiveFormat.Unkown, password)) { }
            });
        }

        /// <summary>
        ///A test for SevenZipArchive Constructor
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ConstructorTests.csv", "ConstructorTests#csv", DataAccessMethod.Sequential)]
        public void SevenZipArchiveConstructorTest3()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string password = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string rootFormat = Convert.ToString(TestContext.DataRow["RootFormat"]);
            ArchiveFormat format = (ArchiveFormat)Enum.Parse(typeof(ArchiveFormat), rootFormat);
            string actualDumpPath = Path.Combine(CreateTestResultsDirectory(), archiveName + ".xml");
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, archiveName + ".xml");

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, format, password))
            {
                DumpArchive(archive, actualDumpPath);
                CollectionAssert.AreEqual(File.ReadAllLines(expectedDumpPath), File.ReadAllLines(actualDumpPath));
            }

            if (password != null)
            {
                ExceptionAssert.Throws<PasswordRequiredException>("This archive is encrypted, and requires a password to be decrypted.", delegate
                {
                    using (new SevenZipArchive(archivePath, format, null)) { }
                });

                ExceptionAssert.Throws<BadPasswordException>("Incorrect password specified to decrypt the archive.", delegate
                {
                    using (new SevenZipArchive(archivePath, format, "bad_password")) { }
                });
            }

            ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: fileName", delegate
            {
                using (new SevenZipArchive((string)null, format, password)) { }
            });
        }

        /// <summary>
        ///A test for SevenZipArchive Constructor
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ConstructorTests.csv", "ConstructorTests#csv", DataAccessMethod.Sequential)]
        public void SevenZipArchiveConstructorTest4()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string password = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string rootFormat = Convert.ToString(TestContext.DataRow["RootFormat"]);
            ArchiveFormat format = (ArchiveFormat)Enum.Parse(typeof(ArchiveFormat), rootFormat);
            string actualDumpPath = Path.Combine(CreateTestResultsDirectory(), "[s]" + archiveName + ".xml");
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, "[s]" + archiveName + ".xml");
            IComparer<string> comparer = new ReverseStringComparer();

            using (var stream = GetFileStream(archivePath))
            {
                Assert.IsTrue(stream.CanRead);

                using (SevenZipArchive archive = new SevenZipArchive(stream, format, password, comparer))
                {
                    DumpArchive(archive, actualDumpPath);
                    XElement expected = XDocument.Load(expectedDumpPath).Root;
                    XElement actual = XDocument.Load(actualDumpPath).Root;

                    do
                    {
                        CollectionAssert.AreEqual(
                            expected.Elements().Where(x => x.Name != "Entries" && x.Name != "Parent").ToArray(),
                            actual.Elements().Where(x => x.Name != "Entries" && x.Name != "Parent").ToArray(),
                            new XNodeComparer());

                        CollectionAssert.AreEqual(
                            expected.Element("Entries").Elements().OrderBy(x => x.Element("FileName").Value, comparer).ToArray(),
                            actual.Element("Entries").Elements().ToArray(),
                            new XNodeComparer());

                        expected = expected.Element("Parent").Element("Archive");
                        actual = actual.Element("Parent").Element("Archive");

                    } while (expected != null && expected.HasElements);
                }

                Assert.IsFalse(stream.CanRead);
            }

            if (password != null)
            {
                using (var stream = GetFileStream(archivePath))
                {
                    Assert.IsTrue(stream.CanRead);

                    ExceptionAssert.Throws<PasswordRequiredException>("This archive is encrypted, and requires a password to be decrypted.", delegate
                    {
                        using (new SevenZipArchive(stream, format, null, comparer)) { }
                    });

                    Assert.IsFalse(stream.CanRead);
                }

                ExceptionAssert.Throws<BadPasswordException>("Incorrect password specified to decrypt the archive.", delegate
                {
                    using (new SevenZipArchive(GetFileStream(archivePath), format, "bad_password", comparer)) { }
                });
            }

            ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: stream", delegate
            {
                using (new SevenZipArchive((Stream)null, format, password, comparer)) { }
            });

            ExceptionAssert.Throws<ArgumentException>("The specified stream should be seekable and readable.\r\nParameter name: stream", delegate
            {
                MemoryStream ms = new MemoryStream();
                ms.Close();
                using (new SevenZipArchive(ms, format, password, comparer)) { }
            });
        }

        /// <summary>
        ///A test for SevenZipArchive Constructor
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ConstructorTests.csv", "ConstructorTests#csv", DataAccessMethod.Sequential)]
        public void SevenZipArchiveConstructorTest5()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string password = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string actualDumpPath = Path.Combine(CreateTestResultsDirectory(), "[s]" + archiveName + ".xml");
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, "[s]" + archiveName + ".xml");
            IComparer<string> comparer = new ReverseStringComparer();

            using (var stream = GetFileStream(archivePath))
            {
                Assert.IsTrue(stream.CanRead);

                using (SevenZipArchive archive = new SevenZipArchive(stream, ArchiveFormat.Unkown, password, comparer))
                {
                    DumpArchive(archive, actualDumpPath);
                    XElement expected = XDocument.Load(expectedDumpPath).Root;
                    XElement actual = XDocument.Load(actualDumpPath).Root;

                    do
                    {
                        CollectionAssert.AreEqual(
                            expected.Elements().Where(x => x.Name != "Entries" && x.Name != "Parent").ToArray(),
                            actual.Elements().Where(x => x.Name != "Entries" && x.Name != "Parent").ToArray(),
                            new XNodeComparer());

                        CollectionAssert.AreEqual(
                            expected.Element("Entries").Elements().OrderBy(x => x.Element("FileName").Value, comparer).ToArray(),
                            actual.Element("Entries").Elements().ToArray(),
                            new XNodeComparer());

                        expected = expected.Element("Parent").Element("Archive");
                        actual = actual.Element("Parent").Element("Archive");

                    } while (expected != null && expected.HasElements);
                }

                Assert.IsFalse(stream.CanRead);
            }

            if (password != null)
            {
                using (var stream = GetFileStream(archivePath))
                {
                    Assert.IsTrue(stream.CanRead);

                    ExceptionAssert.Throws<PasswordRequiredException>("This archive is encrypted, and requires a password to be decrypted.", delegate
                    {
                        using (new SevenZipArchive(stream, ArchiveFormat.Unkown, null, comparer)) { }
                    });

                    Assert.IsFalse(stream.CanRead);
                }

                ExceptionAssert.Throws<BadPasswordException>("Incorrect password specified to decrypt the archive.", delegate
                {
                    using (new SevenZipArchive(GetFileStream(archivePath), ArchiveFormat.Unkown, "bad_password", comparer)) { }
                });
            }

            ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: stream", delegate
            {
                using (new SevenZipArchive((Stream)null, ArchiveFormat.Unkown, password, comparer)) { }
            });

            ExceptionAssert.Throws<ArgumentException>("The specified stream should be seekable and readable.\r\nParameter name: stream", delegate
            {
                MemoryStream ms = new MemoryStream();
                ms.Close();
                using (new SevenZipArchive(ms, ArchiveFormat.Unkown, password, comparer)) { }
            });
        }

        /// <summary>
        ///A test for SevenZipArchive Constructor
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ConstructorTests.csv", "ConstructorTests#csv", DataAccessMethod.Sequential)]
        public void SevenZipArchiveConstructorTest6()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string password = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string rootFormat = Convert.ToString(TestContext.DataRow["RootFormat"]);
            ArchiveFormat format = (ArchiveFormat)Enum.Parse(typeof(ArchiveFormat), rootFormat);
            string actualDumpPath = Path.Combine(CreateTestResultsDirectory(), archiveName + ".xml");
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, archiveName + ".xml");
            IComparer<string> comparer = new ReverseStringComparer();

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, format, password, comparer))
            {
                DumpArchive(archive, actualDumpPath);
                XElement expected = XDocument.Load(expectedDumpPath).Root;
                XElement actual = XDocument.Load(actualDumpPath).Root;

                do
                {
                    CollectionAssert.AreEqual(
                        expected.Elements().Where(x => x.Name != "Entries" && x.Name != "Parent").ToArray(),
                        actual.Elements().Where(x => x.Name != "Entries" && x.Name != "Parent").ToArray(),
                        new XNodeComparer());

                    CollectionAssert.AreEqual(
                        expected.Element("Entries").Elements().OrderBy(x => x.Element("FileName").Value, comparer).ToArray(),
                        actual.Element("Entries").Elements().ToArray(),
                        new XNodeComparer());

                    expected = expected.Element("Parent").Element("Archive");
                    actual = actual.Element("Parent").Element("Archive");

                } while (expected != null && expected.HasElements);
            }

            if (password != null)
            {
                ExceptionAssert.Throws<PasswordRequiredException>("This archive is encrypted, and requires a password to be decrypted.", delegate
                {
                    using (new SevenZipArchive(archivePath, format, null, comparer)) { }
                });

                ExceptionAssert.Throws<BadPasswordException>("Incorrect password specified to decrypt the archive.", delegate
                {
                    using (new SevenZipArchive(archivePath, format, "bad_password", comparer)) { }
                });
            }

            ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: fileName", delegate
            {
                using (new SevenZipArchive((string)null, format, password, comparer)) { }
            });
        }

        /// <summary>
        ///A test for SevenZipArchive Constructor
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ConstructorTests.csv", "ConstructorTests#csv", DataAccessMethod.Sequential)]
        public void SevenZipArchiveConstructorTest7()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string password = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string actualDumpPath = Path.Combine(CreateTestResultsDirectory(), archiveName + ".xml");
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, archiveName + ".xml");

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, password))
            {
                DumpArchive(archive, actualDumpPath);
                CollectionAssert.AreEqual(File.ReadAllLines(expectedDumpPath), File.ReadAllLines(actualDumpPath));
            }

            if (password != null)
            {
                ExceptionAssert.Throws<PasswordRequiredException>("This archive is encrypted, and requires a password to be decrypted.", delegate
                {
                    using (new SevenZipArchive(archivePath, ArchiveFormat.Unkown, (string)null)) { }
                });

                ExceptionAssert.Throws<BadPasswordException>("Incorrect password specified to decrypt the archive.", delegate
                {
                    using (new SevenZipArchive(archivePath, ArchiveFormat.Unkown, "bad_password")) { }
                });
            }

            ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: fileName", delegate
            {
                using (new SevenZipArchive((string)null, ArchiveFormat.Unkown, password)) { }
            });
        }

        /// <summary>
        ///A test for SevenZipArchive Constructor
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ConstructorTests.csv", "ConstructorTests#csv", DataAccessMethod.Sequential)]
        public void SevenZipArchiveConstructorTest8()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string password = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string rootFormat = Convert.ToString(TestContext.DataRow["RootFormat"]);
            ArchiveFormat format = (ArchiveFormat)Enum.Parse(typeof(ArchiveFormat), rootFormat);
            string actualDumpPath = Path.Combine(CreateTestResultsDirectory(), archiveName + ".xml");
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, archiveName + ".xml");

            if (password == null)
            {
                using (SevenZipArchive archive = new SevenZipArchive(archivePath, format))
                {
                    DumpArchive(archive, actualDumpPath);
                    CollectionAssert.AreEqual(File.ReadAllLines(expectedDumpPath), File.ReadAllLines(actualDumpPath));
                }
            }
            else
            {
                ExceptionAssert.Throws<PasswordRequiredException>("This archive is encrypted, and requires a password to be decrypted.", delegate
                {
                    using (new SevenZipArchive(archivePath, format)) { }
                });
            }

            ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: fileName", delegate
            {
                using (new SevenZipArchive((string)null, format)) { }
            });
        }

        /// <summary>
        ///A test for SevenZipArchive Constructor
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ConstructorTests.csv", "ConstructorTests#csv", DataAccessMethod.Sequential)]
        public void SevenZipArchiveConstructorTest9()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string password = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string rootFormat = Convert.ToString(TestContext.DataRow["RootFormat"]);
            ArchiveFormat format = (ArchiveFormat)Enum.Parse(typeof(ArchiveFormat), rootFormat);
            string actualDumpPath = Path.Combine(CreateTestResultsDirectory(), "[s]" + archiveName + ".xml");
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, "[s]" + archiveName + ".xml");

            if (password == null)
            {
                using (var stream = GetFileStream(archivePath))
                {
                    Assert.IsTrue(stream.CanRead);

                    using (SevenZipArchive archive = new SevenZipArchive(stream, format))
                    {
                        DumpArchive(archive, actualDumpPath);
                        CollectionAssert.AreEqual(File.ReadAllLines(expectedDumpPath), File.ReadAllLines(actualDumpPath));
                    }

                    Assert.IsFalse(stream.CanRead);
                }
            }
            else
            {
                using (var stream = GetFileStream(archivePath))
                {
                    Assert.IsTrue(stream.CanRead);

                    ExceptionAssert.Throws<PasswordRequiredException>("This archive is encrypted, and requires a password to be decrypted.", delegate
                    {
                        using (new SevenZipArchive(stream, format)) { }
                    });

                    Assert.IsFalse(stream.CanRead);
                }
            }

            ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: stream", delegate
            {
                using (new SevenZipArchive((Stream)null, format)) { }
            });

            ExceptionAssert.Throws<ArgumentException>("The specified stream should be seekable and readable.\r\nParameter name: stream", delegate
            {
                MemoryStream ms = new MemoryStream();
                ms.Close();
                using (new SevenZipArchive(ms, format)) { }
            });
        }

        /// <summary>
        ///A test for SevenZipArchive Constructor
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ConstructorTests.csv", "ConstructorTests#csv", DataAccessMethod.Sequential)]
        public void SevenZipArchiveConstructorTest10()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string password = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string actualDumpPath = Path.Combine(CreateTestResultsDirectory(), archiveName + ".xml");
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, archiveName + ".xml");

            if (password == null)
            {
                using (SevenZipArchive archive = new SevenZipArchive(archivePath))
                {
                    DumpArchive(archive, actualDumpPath);
                    CollectionAssert.AreEqual(File.ReadAllLines(expectedDumpPath), File.ReadAllLines(actualDumpPath));
                }
            }
            else
            {
                ExceptionAssert.Throws<PasswordRequiredException>("This archive is encrypted, and requires a password to be decrypted.", delegate
                {
                    using (new SevenZipArchive(archivePath)) { }
                });
            }

            ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: fileName", delegate
            {
                using (new SevenZipArchive((string)null)) { }
            });
        }

        /// <summary>
        ///A test for SevenZipArchive Constructor
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ConstructorTests.csv", "ConstructorTests#csv", DataAccessMethod.Sequential)]
        public void SevenZipArchiveConstructorTest11()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string password = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string actualDumpPath = Path.Combine(CreateTestResultsDirectory(), "[s]" + archiveName + ".xml");
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, "[s]" + archiveName + ".xml");

            if (password == null)
            {
                using (var stream = GetFileStream(archivePath))
                {
                    Assert.IsTrue(stream.CanRead);

                    using (SevenZipArchive archive = new SevenZipArchive(stream))
                    {
                        DumpArchive(archive, actualDumpPath);
                        CollectionAssert.AreEqual(File.ReadAllLines(expectedDumpPath), File.ReadAllLines(actualDumpPath));
                    }

                    Assert.IsFalse(stream.CanRead);
                }
            }
            else
            {
                using (var stream = GetFileStream(archivePath))
                {
                    Assert.IsTrue(stream.CanRead);

                    ExceptionAssert.Throws<PasswordRequiredException>("This archive is encrypted, and requires a password to be decrypted.", delegate
                    {
                        using (new SevenZipArchive(stream)) { }
                    });

                    Assert.IsFalse(stream.CanRead);
                }
            }

            ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: stream", delegate
            {
                using (new SevenZipArchive((Stream)null)) { }
            });

            ExceptionAssert.Throws<ArgumentException>("The specified stream should be seekable and readable.\r\nParameter name: stream", delegate
            {
                MemoryStream ms = new MemoryStream();
                ms.Close();
                using (new SevenZipArchive(ms)) { }
            });
        }

        /// <summary>
        ///A test for SevenZipArchive Constructor
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ConstructorTests.csv", "ConstructorTests#csv", DataAccessMethod.Sequential)]
        public void SevenZipArchiveConstructorTest12()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string password = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string rootFormat = Convert.ToString(TestContext.DataRow["RootFormat"]);
            ArchiveFormat format = (ArchiveFormat)Enum.Parse(typeof(ArchiveFormat), rootFormat);
            string actualDumpPath = Path.Combine(CreateTestResultsDirectory(), archiveName + ".xml");
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, archiveName + ".xml");
            IComparer<string> comparer = new ReverseStringComparer();

            if (password == null)
            {
                using (SevenZipArchive archive = new SevenZipArchive(archivePath, format, null, comparer))
                {
                    DumpArchive(archive, actualDumpPath);
                    XElement expected = XDocument.Load(expectedDumpPath).Root;
                    XElement actual = XDocument.Load(actualDumpPath).Root;

                    do
                    {
                        CollectionAssert.AreEqual(
                            expected.Elements().Where(x => x.Name != "Entries" && x.Name != "Parent").ToArray(),
                            actual.Elements().Where(x => x.Name != "Entries" && x.Name != "Parent").ToArray(),
                            new XNodeComparer());

                        CollectionAssert.AreEqual(
                            expected.Element("Entries").Elements().OrderBy(x => x.Element("FileName").Value, comparer).ToArray(),
                            actual.Element("Entries").Elements().ToArray(),
                            new XNodeComparer());

                        expected = expected.Element("Parent").Element("Archive");
                        actual = actual.Element("Parent").Element("Archive");

                    } while (expected != null && expected.HasElements);
                }
            }
            else
            {
                ExceptionAssert.Throws<PasswordRequiredException>("This archive is encrypted, and requires a password to be decrypted.", delegate
                {
                    using (new SevenZipArchive(archivePath, format, null, comparer)) { }
                });
            }

            ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: fileName", delegate
            {
                using (new SevenZipArchive((string)null, format, null, comparer)) { }
            });
        }

        /// <summary>
        ///A test for SevenZipArchive Constructor
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ConstructorTests.csv", "ConstructorTests#csv", DataAccessMethod.Sequential)]
        public void SevenZipArchiveConstructorTest13()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string password = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string rootFormat = Convert.ToString(TestContext.DataRow["RootFormat"]);
            ArchiveFormat format = (ArchiveFormat)Enum.Parse(typeof(ArchiveFormat), rootFormat);
            string actualDumpPath = Path.Combine(CreateTestResultsDirectory(), "[s]" + archiveName + ".xml");
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, "[s]" + archiveName + ".xml");
            IComparer<string> comparer = new ReverseStringComparer();

            if (password == null)
            {
                using (var stream = GetFileStream(archivePath))
                {
                    Assert.IsTrue(stream.CanRead);

                    using (SevenZipArchive archive = new SevenZipArchive(stream, format, null, comparer))
                    {
                        DumpArchive(archive, actualDumpPath);
                        XElement expected = XDocument.Load(expectedDumpPath).Root;
                        XElement actual = XDocument.Load(actualDumpPath).Root;

                        do
                        {
                            CollectionAssert.AreEqual(
                                expected.Elements().Where(x => x.Name != "Entries" && x.Name != "Parent").ToArray(),
                                actual.Elements().Where(x => x.Name != "Entries" && x.Name != "Parent").ToArray(),
                                new XNodeComparer());

                            CollectionAssert.AreEqual(
                                expected.Element("Entries").Elements().OrderBy(x => x.Element("FileName").Value, comparer).ToArray(),
                                actual.Element("Entries").Elements().ToArray(),
                                new XNodeComparer());

                            expected = expected.Element("Parent").Element("Archive");
                            actual = actual.Element("Parent").Element("Archive");

                        } while (expected != null && expected.HasElements);
                    }

                    Assert.IsFalse(stream.CanRead);
                }
            }
            else
            {
                using (var stream = GetFileStream(archivePath))
                {
                    Assert.IsTrue(stream.CanRead);

                    ExceptionAssert.Throws<PasswordRequiredException>("This archive is encrypted, and requires a password to be decrypted.", delegate
                    {
                        using (new SevenZipArchive(stream, format, null, comparer)) { }
                    });

                    Assert.IsFalse(stream.CanRead);
                }
            }

            ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: stream", delegate
            {
                using (new SevenZipArchive((Stream)null, format, null, comparer)) { }
            });

            ExceptionAssert.Throws<ArgumentException>("The specified stream should be seekable and readable.\r\nParameter name: stream", delegate
            {
                MemoryStream ms = new MemoryStream();
                ms.Close();
                using (new SevenZipArchive(ms, format, null, comparer)) { }
            });
        }

        /// <summary>
        ///A test for SevenZipArchive Constructor
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ConstructorTests.csv", "ConstructorTests#csv", DataAccessMethod.Sequential)]
        public void SevenZipArchiveConstructorTest14()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string password = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string actualDumpPath = Path.Combine(CreateTestResultsDirectory(), archiveName + ".xml");
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, archiveName + ".xml");
            IComparer<string> comparer = new ReverseStringComparer();

            if (password == null)
            {
                using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, null, comparer))
                {
                    DumpArchive(archive, actualDumpPath);
                    XElement expected = XDocument.Load(expectedDumpPath).Root;
                    XElement actual = XDocument.Load(actualDumpPath).Root;

                    do
                    {
                        CollectionAssert.AreEqual(
                            expected.Elements().Where(x => x.Name != "Entries" && x.Name != "Parent").ToArray(),
                            actual.Elements().Where(x => x.Name != "Entries" && x.Name != "Parent").ToArray(),
                            new XNodeComparer());

                        CollectionAssert.AreEqual(
                            expected.Element("Entries").Elements().OrderBy(x => x.Element("FileName").Value, comparer).ToArray(),
                            actual.Element("Entries").Elements().ToArray(),
                            new XNodeComparer());

                        expected = expected.Element("Parent").Element("Archive");
                        actual = actual.Element("Parent").Element("Archive");

                    } while (expected != null && expected.HasElements);
                }
            }
            else
            {
                ExceptionAssert.Throws<PasswordRequiredException>("This archive is encrypted, and requires a password to be decrypted.", delegate
                {
                    using (new SevenZipArchive(archivePath, ArchiveFormat.Unkown, null, comparer)) { }
                });
            }

            ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: fileName", delegate
            {
                using (new SevenZipArchive((string)null, ArchiveFormat.Unkown, null, comparer)) { }
            });
        }

        /// <summary>
        ///A test for SevenZipArchive Constructor
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ConstructorTests.csv", "ConstructorTests#csv", DataAccessMethod.Sequential)]
        public void SevenZipArchiveConstructorTest15()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string password = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string actualDumpPath = Path.Combine(CreateTestResultsDirectory(), "[s]" + archiveName + ".xml");
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, "[s]" + archiveName + ".xml");
            IComparer<string> comparer = new ReverseStringComparer();

            if (password == null)
            {
                using (var stream = GetFileStream(archivePath))
                {
                    Assert.IsTrue(stream.CanRead);

                    using (SevenZipArchive archive = new SevenZipArchive(stream, ArchiveFormat.Unkown, null, comparer))
                    {
                        DumpArchive(archive, actualDumpPath);
                        XElement expected = XDocument.Load(expectedDumpPath).Root;
                        XElement actual = XDocument.Load(actualDumpPath).Root;

                        do
                        {
                            CollectionAssert.AreEqual(
                                expected.Elements().Where(x => x.Name != "Entries" && x.Name != "Parent").ToArray(),
                                actual.Elements().Where(x => x.Name != "Entries" && x.Name != "Parent").ToArray(),
                                new XNodeComparer());

                            CollectionAssert.AreEqual(
                                expected.Element("Entries").Elements().OrderBy(x => x.Element("FileName").Value, comparer).ToArray(),
                                actual.Element("Entries").Elements().ToArray(),
                                new XNodeComparer());

                            expected = expected.Element("Parent").Element("Archive");
                            actual = actual.Element("Parent").Element("Archive");

                        } while (expected != null && expected.HasElements);
                    }

                    Assert.IsFalse(stream.CanRead);
                }
            }
            else
            {
                using (var stream = GetFileStream(archivePath))
                {
                    Assert.IsTrue(stream.CanRead);

                    ExceptionAssert.Throws<PasswordRequiredException>("This archive is encrypted, and requires a password to be decrypted.", delegate
                    {
                        using (new SevenZipArchive(stream, ArchiveFormat.Unkown, null, comparer)) { }
                    });

                    Assert.IsFalse(stream.CanRead);
                }
            }

            ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: stream", delegate
            {
                using (new SevenZipArchive((Stream)null, ArchiveFormat.Unkown, null, comparer)) { }
            });

            ExceptionAssert.Throws<ArgumentException>("The specified stream should be seekable and readable.\r\nParameter name: stream", delegate
            {
                MemoryStream ms = new MemoryStream();
                ms.Close();
                using (new SevenZipArchive(ms, ArchiveFormat.Unkown, null, comparer)) { }
            });
        }

        /// <summary>
        ///A test for CheckFiles
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void CheckFilesTest()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                foreach (var group in archive.GroupBy(x => passwords.FirstOrDefault(p => x.FileName.Contains(p))))
                {
                    if (group.Key == null)
                    {
                        Assert.IsTrue(archive.CheckFiles(group.Select(x => x.FileName)));
                    }
                    else
                    {
                        Assert.IsFalse(archive.CheckFiles(group.Select(x => x.FileName)));
                        Assert.IsTrue(archive.CheckFiles(group.Where(x => x.IsDirectory).Select(x => x.FileName)));
                        Assert.IsFalse(archive.CheckFiles(group.Where(x => !x.IsDirectory).Select(x => x.FileName)));
                    }
                    Assert.IsTrue(archive.CheckFiles(new string[0]));
                }
            }
        }

        /// <summary>
        ///A test for CheckFiles
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void CheckFilesTest1()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                foreach (var group in archive.GroupBy(x => passwords.FirstOrDefault(p => x.FileName.Contains(p))))
                {
                    if (group.Key == null)
                    {
                        Assert.IsTrue(archive.CheckFiles(group.Select(x => x.FileName), string.Empty));
                        if (headerPassword != null)
                        {
                            Assert.IsTrue(archive.CheckFiles(group.Select(x => x.FileName), headerPassword));
                            Assert.IsFalse(archive.CheckFiles(group.Where(x => !x.IsDirectory).Select(x => x.FileName), "bad_password"));
                        }
                        else
                        {
                            Assert.IsTrue(archive.CheckFiles(group.Select(x => x.FileName), "password_that_will_not_be_used"));
                        }
                    }
                    else
                    {
                        Assert.IsTrue(archive.CheckFiles(group.Select(x => x.FileName), group.Key));
                        Assert.IsTrue(archive.CheckFiles(group.Where(x => x.IsDirectory).Select(x => x.FileName), "password_that_will_not_be_used"));
                        Assert.IsFalse(archive.CheckFiles(group.Where(x => !x.IsDirectory).Select(x => x.FileName), "bad_password"));
                    }
                    Assert.IsTrue(archive.CheckFiles(new string[0], "password_that_will_not_be_used"));
                }
            }
        }

        /// <summary>
        ///A test for CheckFile
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void CheckFileTest()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                foreach (var group in archive.GroupBy(x => passwords.FirstOrDefault(p => x.FileName.Contains(p))))
                {
                    foreach (var entry in group)
                    {
                        if (group.Key == null)
                        {
                            Assert.IsTrue(archive.CheckFile(entry.FileName));
                            Assert.IsTrue(entry.Check());
                        }
                        else
                        {
                            if (entry.IsDirectory)
                            {
                                Assert.IsTrue(archive.CheckFile(entry.FileName));
                                Assert.IsTrue(entry.Check());
                            }
                            else
                            {
                                Assert.IsFalse(archive.CheckFile(entry.FileName));
                                Assert.IsFalse(entry.Check());
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        ///A test for CheckFile
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void CheckFileTest1()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                foreach (var group in archive.GroupBy(x => passwords.FirstOrDefault(p => x.FileName.Contains(p))))
                {
                    foreach (var entry in group)
                    {
                        if (group.Key == null)
                        {
                            Assert.IsTrue(archive.CheckFile(entry.FileName, string.Empty));
                            Assert.IsTrue(entry.Check(string.Empty));
                            if (headerPassword != null && !entry.IsDirectory)
                            {
                                Assert.IsTrue(archive.CheckFile(entry.FileName, headerPassword));
                                Assert.IsTrue(entry.Check(headerPassword));
                                Assert.IsFalse(archive.CheckFile(entry.FileName, "bad_password"));
                                Assert.IsFalse(entry.Check("bad_password"));
                            }
                            else
                            {
                                Assert.IsTrue(archive.CheckFile(entry.FileName, "password_that_will_not_be_used"));
                                Assert.IsTrue(entry.Check("password_that_will_not_be_used"));
                            }
                        }
                        else
                        {
                            Assert.IsTrue(archive.CheckFile(entry.FileName, group.Key));
                            Assert.IsTrue(entry.Check(group.Key));

                            if (entry.IsDirectory)
                            {
                                Assert.IsTrue(archive.CheckFile(entry.FileName, "password_that_will_not_be_used"));
                                Assert.IsTrue(entry.Check("password_that_will_not_be_used"));
                            }
                            else
                            {
                                Assert.IsFalse(archive.CheckFile(entry.FileName, "bad_password"));
                                Assert.IsFalse(entry.Check("bad_password"));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///A test for CheckAll
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void CheckAllTest()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                if (passwords.Length == 0)
                {
                    Assert.IsTrue(archive.CheckAll());
                }
                else
                {
                    Assert.IsFalse(archive.CheckAll());
                }
            }
        }

        /// <summary>
        ///A test for CheckAll
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void CheckAllTest1()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                if (passwords.Length == 0)
                {
                    Assert.IsTrue(archive.CheckAll(string.Empty));
                    if (headerPassword != null)
                    {
                        Assert.IsTrue(archive.CheckAll(headerPassword));
                        Assert.IsFalse(archive.CheckAll("bad_password"));
                    }
                    else
                    {
                        Assert.IsTrue(archive.CheckAll("password_that_will_not_be_used"));
                    }
                }
                else if (passwords.Length == 1)
                {
                    Assert.IsTrue(archive.CheckAll(passwords[0]));
                }
                else
                {
                    Assert.IsFalse(archive.CheckAll(passwords[0]));
                }
            }
        }

        /// <summary>
        ///A test for CleanFileName
        ///</summary>
        [TestMethod]
        public void CleanFileNameTest()
        {
            Assert.AreEqual(@"root\dir\file.txt", SevenZipArchive.CleanFileName(@"\root\dir\file.txt"));
            Assert.AreEqual(@"root\dir\file.txt", SevenZipArchive.CleanFileName(@"/root/dir/file.txt"));
            Assert.AreEqual(@"root\dir\file.txt", SevenZipArchive.CleanFileName(@"root/dir/file.txt"));
            Assert.AreEqual(@"root\dir", SevenZipArchive.CleanFileName(@"/root/dir/"));
            Assert.AreEqual(@"root\dir", SevenZipArchive.CleanFileName(@"\root/dir/"));
            Assert.AreEqual(@"root\dir", SevenZipArchive.CleanFileName(@"\root/dir"));
            Assert.IsNull(SevenZipArchive.CleanFileName(null));
            Assert.AreEqual(string.Empty, SevenZipArchive.CleanFileName(string.Empty));
            Assert.AreEqual("dir", SevenZipArchive.CleanFileName("dir"));
        }

        /// <summary>
        ///A test for Contains
        ///</summary>
        [TestMethod]
        public void ContainsTest()
        {
            string archiveName = "Sample.cpio.Z";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                Assert.IsTrue(archive.Contains("bugs.c"));
                Assert.IsTrue(archive.Contains("README"));
                Assert.IsTrue(archive.Parent.Contains("Sample.cpio"));
                Assert.IsFalse(archive.Contains(@"one\two\three"));

                ExceptionAssert.Throws<ArgumentNullException>(delegate
                {
                    archive.Contains((string)null);
                });
            }
        }

        /// <summary>
        ///A test for Contains
        ///</summary>
        [TestMethod]
        public void ContainsTest1()
        {
            string archiveName1 = "Sample.cpio.Z";
            string archivePath1 = Path.Combine(ArchiveDirectory, archiveName1);

            string archiveName2 = "Sample.cpio.gz";
            string archivePath2 = Path.Combine(ArchiveDirectory, archiveName2);
            
            using (SevenZipArchive archive1 = new SevenZipArchive(archivePath1))
            using (SevenZipArchive archive2 = new SevenZipArchive(archivePath2))
            {
                Assert.IsTrue(archive1.Contains(archive1[0]));
                Assert.IsTrue(archive1.Contains(archive1[1]));
                Assert.IsFalse(archive1.Contains(archive2[0]));

                ExceptionAssert.Throws<ArgumentNullException>(delegate
                {
                    archive1.Contains((ArchiveEntry)null);
                });
            }
        }
        
        /// <summary>
        ///A test for CopyTo
        ///</summary>
        [TestMethod]
        public void CopyToTest()
        {
            string archiveName = "Sample.cpio.Z";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, archiveName + ".xml");

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                XElement dump = XDocument.Load(expectedDumpPath).Root;
                string[] expected = dump.Element("Entries").Elements().Select(x => x.Element("FileName").Value).ToArray();
                string[] actual = new string[archive.Count];
                archive.CopyTo(actual, 0);
                CollectionAssert.AreEqual(expected, actual);

                ExceptionAssert.Throws<ArgumentNullException>(delegate
                {
                    archive.CopyTo((string[])null, 0);
                });

                ExceptionAssert.Throws<ArgumentOutOfRangeException>(delegate
                {
                    archive.CopyTo(new string[0], -10);
                });

                ExceptionAssert.Throws<ArgumentException>(delegate
                {
                    archive.CopyTo(new string[archive.Count - 1], 0);
                });

                ExceptionAssert.Throws<ArgumentException>(delegate
                {
                    archive.CopyTo(new string[archive.Count], 5);
                });
            }
        }
        
        /// <summary>
        ///A test for CopyTo
        ///</summary>
        [TestMethod]
        public void CopyToTest1()
        {
            string archiveName = "Sample.cpio.Z";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                ArchiveEntry[] expected = archive.ToArray();
                ArchiveEntry[] actual = new ArchiveEntry[archive.Count];
                archive.CopyTo(actual, 0);
                CollectionAssert.AreEqual(expected, actual);

                ExceptionAssert.Throws<ArgumentNullException>(delegate
                {
                    archive.CopyTo((ArchiveEntry[])null, 0);
                });

                ExceptionAssert.Throws<ArgumentOutOfRangeException>(delegate
                {
                    archive.CopyTo(new ArchiveEntry[0], -10);
                });

                ExceptionAssert.Throws<ArgumentException>(delegate
                {
                    archive.CopyTo(new ArchiveEntry[archive.Count - 1], 0);
                });

                ExceptionAssert.Throws<ArgumentException>(delegate
                {
                    archive.CopyTo(new ArchiveEntry[archive.Count], 5);
                });
            }
        }
        
        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod]
        public void DisposeTest()
        {
            string archiveName = "Sample.cpio.Z";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);

            SevenZipArchive archive = new SevenZipArchive(archivePath);
            SevenZipArchive parent = archive.Parent;
            Assert.IsFalse(archive.IsClosed);
            Assert.IsFalse(parent.IsClosed);
            
            ExceptionAssert.Throws<InvalidOperationException>(
                "This archive has a sub archive, and thus cannot be disposed directly. Dispose() should be called on the sub archive.",
                delegate
                {
                    parent.Dispose();
                });

            archive.Dispose();

            Assert.IsTrue(archive.IsClosed);
            Assert.IsTrue(parent.IsClosed);

            Assert.IsNull(archive.Volumes);
            Assert.IsNull(archive.Parent);
            Assert.IsNull(archive.Properties);

            ExceptionAssert.Throws<ObjectDisposedException>(delegate { archive.CheckAll(); });
            ExceptionAssert.Throws<ObjectDisposedException>(delegate { archive.CheckFile(string.Empty); });
            ExceptionAssert.Throws<ObjectDisposedException>(delegate { archive.CheckFiles((string[])null); });
            ExceptionAssert.Throws<ObjectDisposedException>(delegate { archive.Contains("file"); });
            ExceptionAssert.Throws<ObjectDisposedException>(delegate { archive.CopyTo(new string[0], 0); });
            ExceptionAssert.Throws<ObjectDisposedException>(delegate { archive.ExtractFile("qwertyuiop", (Stream)null); });
            ExceptionAssert.Throws<ObjectDisposedException>(delegate { archive[0].ToString(); });
            ExceptionAssert.Throws<ObjectDisposedException>(delegate { archive["qwertyuiop"].ToString(); });
        }

        /// <summary>
        ///A test for ExtractFile
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void ExtractFileTest()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string expectedExtractionDir = Path.Combine(ArchiveDirectory, "Extracted", archiveName);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                foreach (var group in archive.GroupBy(x => passwords.FirstOrDefault(p => x.FileName.Contains(p))))
                {
                    foreach (var entry in group)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            if (entry.IsDirectory)
                            {
                                ExceptionAssert.Throws<ArgumentException>(
                                    string.Format("The entry '{0}' is a directory. Only file entries can be extracted to streams.\r\nParameter name: entry", entry.FileName),
                                    delegate
                                    {
                                        archive.ExtractFile(entry.FileName, stream);
                                    });
                            }
                            else
                            {
                                if (group.Key == null)
                                {
                                    archive.ExtractFile(entry.FileName, stream);
                                    Assert.IsTrue(stream.CanWrite);
                                    CompareFileContents(AutoRenamePath(expectedExtractionDir, entry.FileName), stream);
                                }
                                else
                                {
                                    ExceptionAssert.Throws<PasswordRequiredException>(
                                        string.Format("The file '{0}' is encrypted, and requires a password to be decrypted.", entry.FileName),
                                        delegate
                                        {
                                            archive.ExtractFile(entry.FileName, stream);
                                        });
                                }
                            }
                        }
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    string entry = "directory\file_that_doesnt_exist";
                    ExceptionAssert.Throws<ArgumentException>(
                        string.Format("The entry '{0}' was not found in the archive.\r\nParameter name: entry", entry),
                        delegate
                        {
                            archive.ExtractFile(entry, stream);
                        });

                    ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: entry", delegate
                    {
                        archive.ExtractFile(null, stream);
                    });

                    ExceptionAssert.Throws<ArgumentException>("The entry's file name cannot be empty.\r\nParameter name: entry", delegate
                    {
                        archive.ExtractFile(string.Empty, stream);
                    });

                    ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: stream", delegate
                    {
                        archive.ExtractFile(archive.First(x => !x.IsDirectory).FileName, (Stream)null);
                    });

                    stream.Close();
                    ExceptionAssert.Throws<ArgumentException>("The specified stream should be writable.\r\nParameter name: stream",  delegate
                    {
                        archive.ExtractFile(archive.First(x => !x.IsDirectory).FileName, stream);
                    });
                }
            }
        }

        /// <summary>
        ///A test for ExtractFile
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void ExtractFileTest1()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string expectedExtractionDir = Path.Combine(ArchiveDirectory, "Extracted", archiveName);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                foreach (var group in archive.GroupBy(x => passwords.FirstOrDefault(p => x.FileName.Contains(p))))
                {
                    foreach (var entry in group)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            if (entry.IsDirectory)
                            {
                                ExceptionAssert.Throws<ArgumentException>(
                                    string.Format("The entry '{0}' is a directory. Only file entries can be extracted to streams.\r\nParameter name: entry", entry.FileName),
                                    delegate
                                    {
                                        archive.ExtractFile(entry.FileName, stream, ExtractOptions.NoAbortOnFailure);
                                    });
                            }
                            else
                            {
                                if (group.Key == null)
                                {
                                    archive.ExtractFile(entry.FileName, stream, ExtractOptions.NoAbortOnFailure);
                                    Assert.IsTrue(stream.CanWrite);
                                    CompareFileContents(AutoRenamePath(expectedExtractionDir, entry.FileName), stream);
                                }
                                else
                                {
                                    bool failed = false;
                                    EventHandler<FileExtractionFailedEventArgs> handler = (s, e) => { failed = true; };
                                    archive.FileExtractionFailed += handler;
                                    archive.ExtractFile(entry.FileName, stream, ExtractOptions.NoAbortOnFailure);
                                    Assert.IsTrue(stream.CanWrite);
                                    Assert.IsTrue(failed);
                                    archive.FileExtractionFailed -= handler;
                                }
                            }
                        }
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    string entry = "directory\file_that_doesnt_exist";
                    ExceptionAssert.Throws<ArgumentException>(
                        string.Format("The entry '{0}' was not found in the archive.\r\nParameter name: entry", entry),
                        delegate
                        {
                            archive.ExtractFile(entry, stream, ExtractOptions.NoAbortOnFailure);
                        });

                    ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: entry", delegate
                    {
                        archive.ExtractFile(null, stream, ExtractOptions.NoAbortOnFailure);
                    });

                    ExceptionAssert.Throws<ArgumentException>("The entry's file name cannot be empty.\r\nParameter name: entry", delegate
                    {
                        archive.ExtractFile(string.Empty, stream, ExtractOptions.NoAbortOnFailure);
                    });

                    ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: stream", delegate
                    {
                        archive.ExtractFile(archive.First(x => !x.IsDirectory).FileName, (Stream)null, ExtractOptions.NoAbortOnFailure);
                    });

                    ExceptionAssert.Throws<ArgumentException>(
                        string.Format("The flags {0} and {1} cannot both be set.\r\nParameter name: options",
                        ExtractOptions.OverwriteExistingFiles, ExtractOptions.SkipExistingFiles),
                        delegate
                        {
                            archive.ExtractFile(archive.First(x => !x.IsDirectory).FileName, stream, ExtractOptions.OverwriteExistingFiles | ExtractOptions.SkipExistingFiles);
                        });

                    stream.Close();
                    ExceptionAssert.Throws<ArgumentException>("The specified stream should be writable.\r\nParameter name: stream", delegate
                    {
                        archive.ExtractFile(archive.First(x => !x.IsDirectory).FileName, stream, ExtractOptions.NoAbortOnFailure);
                    });
                }
            }
        }

        /// <summary>
        ///A test for ExtractFile
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void ExtractFileTest2()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string expectedExtractionDir = Path.Combine(ArchiveDirectory, "Extracted", archiveName);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                foreach (var group in archive.GroupBy(x => passwords.FirstOrDefault(p => x.FileName.Contains(p))))
                {
                    foreach (var entry in group)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            if (entry.IsDirectory)
                            {
                                ExceptionAssert.Throws<ArgumentException>(
                                    string.Format("The entry '{0}' is a directory. Only file entries can be extracted to streams.\r\nParameter name: entry", entry.FileName),
                                    delegate
                                    {
                                        archive.ExtractFile(entry.FileName, stream, ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                                    });
                            }
                            else
                            {
                                if (group.Key == null)
                                {
                                    if (headerPassword != null)
                                    {
                                        ExceptionAssert.Throws<BadPasswordException>(
                                            string.Format("Incorrect password specified to decrypt the file '{0}'.", entry.FileName),
                                            delegate
                                            {
                                                archive.ExtractFile(entry.FileName, stream, ExtractOptions.None, "bad_password");
                                            });
                                        Assert.IsTrue(stream.CanWrite);

                                        bool failed = false;
                                        EventHandler<FileExtractionFailedEventArgs> handler = (s, e) => { failed = true; };
                                        archive.FileExtractionFailed += handler;
                                        archive.ExtractFile(entry.FileName, stream, ExtractOptions.NoAbortOnFailure, "bad_password");
                                        Assert.IsTrue(stream.CanWrite);
                                        Assert.IsTrue(failed);
                                        archive.FileExtractionFailed -= handler;

                                        archive.ExtractFile(entry.FileName, stream, ExtractOptions.None, headerPassword);
                                        Assert.IsTrue(stream.CanWrite);
                                        CompareFileContents(AutoRenamePath(expectedExtractionDir, entry.FileName), stream);
                                    }
                                    else
                                    {
                                        archive.ExtractFile(entry.FileName, stream, ExtractOptions.None, "password_that_will_not_be_used");
                                        Assert.IsTrue(stream.CanWrite);
                                        CompareFileContents(AutoRenamePath(expectedExtractionDir, entry.FileName), stream);
                                    }
                                }
                                else
                                {
                                    archive.ExtractFile(entry.FileName, stream, ExtractOptions.None, group.Key);
                                    CompareFileContents(AutoRenamePath(expectedExtractionDir, entry.FileName), stream);
                                    Assert.IsTrue(stream.CanWrite);
                                }
                            }
                        }
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    string entry = "directory\file_that_doesnt_exist";
                    ExceptionAssert.Throws<ArgumentException>(
                        string.Format("The entry '{0}' was not found in the archive.\r\nParameter name: entry", entry),
                        delegate
                        {
                            archive.ExtractFile(entry, stream, ExtractOptions.None, "password_that_will_not_be_used");
                        });

                    ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: entry", delegate
                    {
                        archive.ExtractFile(null, stream, ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                    });

                    ExceptionAssert.Throws<ArgumentException>("The entry's file name cannot be empty.\r\nParameter name: entry", delegate
                    {
                        archive.ExtractFile(string.Empty, stream, ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                    });

                    ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: stream", delegate
                    {
                        archive.ExtractFile(archive.First(x => !x.IsDirectory).FileName, (Stream)null, ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                    });

                    ExceptionAssert.Throws<ArgumentException>(
                        string.Format("The flags {0} and {1} cannot both be set.\r\nParameter name: options",
                        ExtractOptions.OverwriteExistingFiles, ExtractOptions.SkipExistingFiles),
                        delegate
                        {
                            archive.ExtractFile(archive.First(x => !x.IsDirectory).FileName, stream, ExtractOptions.OverwriteExistingFiles | ExtractOptions.SkipExistingFiles, "password_that_will_not_be_used");
                        });

                    stream.Close();
                    ExceptionAssert.Throws<ArgumentException>("The specified stream should be writable.\r\nParameter name: stream", delegate
                    {
                        archive.ExtractFile(archive.First(x => !x.IsDirectory).FileName, stream, ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                    });
                }
            }
        }

        /// <summary>
        ///A test for ExtractFile
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void ExtractFileTest3()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string expectedExtractionDir = Path.Combine(ArchiveDirectory, "Extracted", archiveName);
            string actualExtractionDir = Path.Combine(CreateTestExtractionDirectory(CreateTestResultsDirectory(), archiveName));

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                foreach (var group in archive.GroupBy(x => passwords.FirstOrDefault(p => x.FileName.Contains(p))))
                {
                    foreach (var entry in group)
                    {
                        if (entry.IsDirectory)
                        {
                            if (entry.IsUntitled)
                            {
                                Assert.Fail("Not tested");
                            }
                            else if (IsInvalidEntry(entry.FileName))
                            {
                                ExceptionAssert.Throws<SevenZipException>(
                                    string.Format("The file '{0}' cannot be extracted because the path contains invalid characters or components.", entry.FileName),
                                    delegate
                                    {
                                        archive.ExtractFile(entry.FileName, actualExtractionDir);
                                    });
                            }
                            else
                            {
                                archive.ExtractFile(entry.FileName, actualExtractionDir);
                                Assert.IsTrue(Directory.Exists(Path.Combine(actualExtractionDir, entry.FileName)));
                                VerifyAttributes(Path.Combine(actualExtractionDir, entry.FileName), entry);
                            }
                        }
                        else
                        {
                            if (group.Key == null)
                            {
                                if (entry.IsUntitled)
                                {
                                    Assert.Fail("Not tested");
                                }
                                else if (IsInvalidEntry(entry.FileName))
                                {
                                    ExceptionAssert.Throws<SevenZipException>(
                                        string.Format("The file '{0}' cannot be extracted because the path contains invalid characters or components.", entry.FileName),
                                        delegate
                                        {
                                            archive.ExtractFile(entry.FileName, actualExtractionDir);
                                        });
                                }
                                else
                                {
                                    archive.ExtractFile(entry.FileName, actualExtractionDir);
                                    CompareFileContents(Path.Combine(expectedExtractionDir, entry.FileName), Path.Combine(actualExtractionDir, entry.FileName));
                                    VerifyAttributes(Path.Combine(actualExtractionDir, entry.FileName), entry);
                                }
                            }
                            else
                            {
                                if (entry.IsUntitled || IsInvalidEntry(entry.FileName))
                                {
                                    Assert.Fail("Not tested");
                                }
                                else
                                {
                                    ExceptionAssert.Throws<PasswordRequiredException>(
                                        string.Format("The file '{0}' is encrypted, and requires a password to be decrypted.", entry.FileName),
                                        delegate
                                        {
                                            archive.ExtractFile(entry.FileName, actualExtractionDir);
                                        });
                                }
                            }
                        }
                    }
                }

                ExceptionAssert.Throws<ArgumentException>(
                    string.Format("The entry '{0}' was not found in the archive.\r\nParameter name: entry", @"directory\file_that_doesnt_exist"),
                    delegate
                    {
                        archive.ExtractFile(@"directory\file_that_doesnt_exist", actualExtractionDir);
                    });

                ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: entry", delegate
                {
                    archive.ExtractFile(null, actualExtractionDir);
                });

                ExceptionAssert.Throws<ArgumentException>("The entry's file name cannot be empty.\r\nParameter name: entry", delegate
                {
                    archive.ExtractFile(string.Empty, actualExtractionDir);
                });

                ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: directory", delegate
                {
                    archive.ExtractFile(archive.First(x => !x.IsDirectory).FileName, (string)null);
                });

                ExceptionAssert.Throws<DirectoryNotFoundException>(string.Format("The directory '{0}' does not exist.", @"K:\BadDirectory\"), delegate
                {
                    archive.ExtractFile(archive.First(x => !x.IsDirectory).FileName, @"K:\BadDirectory\");
                });
            }

            DeleteDirectory(actualExtractionDir);
        }

        /// <summary>
        ///A test for ExtractFile
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void ExtractFileTest4()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string expectedExtractionDir = Path.Combine(ArchiveDirectory, "Extracted", archiveName);
            string actualExtractionDir = Path.Combine(CreateTestExtractionDirectory(CreateTestResultsDirectory(), archiveName));

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                foreach (var group in archive.GroupBy(x => passwords.FirstOrDefault(p => x.FileName.Contains(p))))
                {
                    foreach (var entry in group)
                    {
                        if (entry.IsDirectory)
                        {
                            if (entry.IsUntitled)
                            {
                                Assert.Fail("Not tested");
                            }
                            else if (IsInvalidEntry(entry.FileName))
                            {
                                archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.NoAbortOnFailure);
                                Assert.IsFalse(Directory.Exists(AutoRenamePath(actualExtractionDir, entry.FileName)));

                                archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.RenameInvalidEntries);
                                Assert.IsTrue(Directory.Exists(AutoRenamePath(actualExtractionDir, entry.FileName)));
                                VerifyAttributes(AutoRenamePath(actualExtractionDir, entry.FileName), entry);
                            }
                            else
                            {
                                archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.NoAbortOnFailure);
                                Assert.IsTrue(Directory.Exists(Path.Combine(actualExtractionDir, entry.FileName)));
                                VerifyAttributes(Path.Combine(actualExtractionDir, entry.FileName), entry);
                            }
                        }
                        else
                        {
                            if (group.Key == null)
                            {
                                if (entry.IsUntitled)
                                {
                                    Assert.Fail("Not tested");                                    
                                }
                                else if (IsInvalidEntry(entry.FileName))
                                {
                                    bool failed = false;
                                    EventHandler<FileExtractionFailedEventArgs> handler = (s, e) => { failed = true; };
                                    archive.FileExtractionFailed += handler;
                                    archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.NoAbortOnFailure);
                                    Assert.IsTrue(failed);
                                    archive.FileExtractionFailed -= handler;

                                    archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.RenameInvalidEntries);
                                    CompareFileContents(AutoRenamePath(expectedExtractionDir, entry.FileName), AutoRenamePath(actualExtractionDir, entry.FileName));
                                    VerifyAttributes(AutoRenamePath(actualExtractionDir, entry.FileName), entry);

                                    File.WriteAllText(AutoRenamePath(actualExtractionDir, entry.FileName), "junk");
                                    archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.RenameInvalidEntries | ExtractOptions.SkipExistingFiles);
                                    Assert.AreEqual("junk", File.ReadAllText(AutoRenamePath(actualExtractionDir, entry.FileName)));

                                    archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.RenameInvalidEntries | ExtractOptions.OverwriteExistingFiles);
                                    CompareFileContents(AutoRenamePath(expectedExtractionDir, entry.FileName), AutoRenamePath(actualExtractionDir, entry.FileName));
                                    VerifyAttributes(AutoRenamePath(actualExtractionDir, entry.FileName), entry);

                                    ExceptionAssert.Throws<SevenZipException>(
                                        string.Format("The file '{0}' cannot be extracted to '{1}' because a file with the same name already exists.",
                                        Path.GetFileName(AutoRenamePath(actualExtractionDir, entry.FileName)), Path.GetDirectoryName(AutoRenamePath(actualExtractionDir, entry.FileName))),
                                        delegate
                                        {
                                            archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.RenameInvalidEntries);
                                        });
                                }
                                else
                                {
                                    archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.NoAbortOnFailure);
                                    CompareFileContents(Path.Combine(expectedExtractionDir, entry.FileName), Path.Combine(actualExtractionDir, entry.FileName));
                                    VerifyAttributes(Path.Combine(actualExtractionDir, entry.FileName), entry);

                                    File.WriteAllText(Path.Combine(actualExtractionDir, entry.FileName), "junk");
                                    archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.SkipExistingFiles);
                                    Assert.AreEqual("junk", File.ReadAllText(Path.Combine(actualExtractionDir, entry.FileName)));

                                    archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.OverwriteExistingFiles);
                                    CompareFileContents(Path.Combine(expectedExtractionDir, entry.FileName), Path.Combine(actualExtractionDir, entry.FileName));
                                    VerifyAttributes(Path.Combine(actualExtractionDir, entry.FileName), entry);

                                    ExceptionAssert.Throws<SevenZipException>(
                                        string.Format("The file '{0}' cannot be extracted to '{1}' because a file with the same name already exists.",
                                        Path.GetFileName(Path.Combine(actualExtractionDir, entry.FileName)), Path.GetDirectoryName(Path.Combine(actualExtractionDir, entry.FileName))),
                                        delegate
                                        {
                                            archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.RenameInvalidEntries);
                                        });
                                }
                            }
                            else
                            {
                                if (entry.IsUntitled || IsInvalidEntry(entry.FileName))
                                {
                                    Assert.Fail("Not tested");
                                }
                                else
                                {
                                    bool failed = false;
                                    EventHandler<FileExtractionFailedEventArgs> handler = (s, e) => { failed = true; };
                                    archive.FileExtractionFailed += handler;
                                    archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.NoAbortOnFailure);
                                    Assert.IsTrue(failed);
                                    archive.FileExtractionFailed -= handler;
                                }
                            }
                        }
                    }
                }

                ExceptionAssert.Throws<ArgumentException>(
                    string.Format("The entry '{0}' was not found in the archive.\r\nParameter name: entry", @"directory\file_that_doesnt_exist"),
                    delegate
                    {
                        archive.ExtractFile(@"directory\file_that_doesnt_exist", actualExtractionDir, ExtractOptions.NoAbortOnFailure);
                    });

                ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: entry", delegate
                {
                    archive.ExtractFile(null, actualExtractionDir, ExtractOptions.NoAbortOnFailure);
                });

                ExceptionAssert.Throws<ArgumentException>("The entry's file name cannot be empty.\r\nParameter name: entry", delegate
                {
                    archive.ExtractFile(string.Empty, actualExtractionDir, ExtractOptions.NoAbortOnFailure);
                });

                ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: directory", delegate
                {
                    archive.ExtractFile(archive.First(x => !x.IsDirectory).FileName, (string)null, ExtractOptions.NoAbortOnFailure);
                });

                ExceptionAssert.Throws<DirectoryNotFoundException>(string.Format("The directory '{0}' does not exist.", @"K:\BadDirectory\"), delegate
                {
                    archive.ExtractFile(archive.First(x => !x.IsDirectory).FileName, @"K:\BadDirectory\", ExtractOptions.NoAbortOnFailure);
                });
            }

            DeleteDirectory(actualExtractionDir);
        }

        /// <summary>
        ///A test for ExtractFile
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void ExtractFileTest5()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string expectedExtractionDir = Path.Combine(ArchiveDirectory, "Extracted", archiveName);
            string actualExtractionDir = Path.Combine(CreateTestExtractionDirectory(CreateTestResultsDirectory(), archiveName));

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                foreach (var group in archive.GroupBy(x => passwords.FirstOrDefault(p => x.FileName.Contains(p))))
                {
                    foreach (var entry in group)
                    {
                        if (entry.IsDirectory)
                        {
                            if (entry.IsUntitled)
                            {
                                Assert.Fail("Not tested");
                            }
                            else if (IsInvalidEntry(entry.FileName))
                            {
                                archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.RenameInvalidEntries, "password_that_will_not_be_used");
                                Assert.IsTrue(Directory.Exists(AutoRenamePath(actualExtractionDir, entry.FileName)));
                                VerifyAttributes(AutoRenamePath(actualExtractionDir, entry.FileName), entry);
                            }
                            else
                            {
                                archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.None, "password_that_will_not_be_used");
                                Assert.IsTrue(Directory.Exists(Path.Combine(actualExtractionDir, entry.FileName)));
                                VerifyAttributes(Path.Combine(actualExtractionDir, entry.FileName), entry);
                            }
                        }
                        else
                        {
                            if (group.Key == null)
                            {
                                if (entry.IsUntitled)
                                {
                                    Assert.Fail("Not tested");
                                }
                                else if (headerPassword != null)
                                {
                                    ExceptionAssert.Throws<BadPasswordException>(
                                        string.Format("Incorrect password specified to decrypt the file '{0}'.", entry.FileName),
                                        delegate
                                        {
                                            archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.None, "bad_password");
                                        });

                                    bool failed = false;
                                    EventHandler<FileExtractionFailedEventArgs> handler = (s, e) => { failed = true; };
                                    archive.FileExtractionFailed += handler;
                                    archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.NoAbortOnFailure, "bad_password");
                                    Assert.IsTrue(failed);
                                    archive.FileExtractionFailed -= handler;

                                    archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.RenameInvalidEntries, headerPassword);
                                    CompareFileContents(AutoRenamePath(expectedExtractionDir, entry.FileName), AutoRenamePath(actualExtractionDir, entry.FileName));
                                    VerifyAttributes(AutoRenamePath(actualExtractionDir, entry.FileName), entry);
                                }
                                else
                                {
                                    archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.RenameInvalidEntries, "password_that_will_not_be_used");
                                    CompareFileContents(AutoRenamePath(expectedExtractionDir, entry.FileName), AutoRenamePath(actualExtractionDir, entry.FileName));
                                    VerifyAttributes(AutoRenamePath(actualExtractionDir, entry.FileName), entry);
                                }
                            }
                            else
                            {
                                if (entry.IsUntitled)
                                {
                                    Assert.Fail("Not tested");
                                }
                                else
                                {
                                    archive.ExtractFile(entry.FileName, actualExtractionDir, ExtractOptions.RenameInvalidEntries, group.Key);
                                    CompareFileContents(AutoRenamePath(expectedExtractionDir, entry.FileName), AutoRenamePath(actualExtractionDir, entry.FileName));
                                    VerifyAttributes(AutoRenamePath(actualExtractionDir, entry.FileName), entry);
                                }
                            }
                        }
                    }
                }

                ExceptionAssert.Throws<ArgumentException>(
                    string.Format("The entry '{0}' was not found in the archive.\r\nParameter name: entry", @"directory\file_that_doesnt_exist"),
                    delegate
                    {
                        archive.ExtractFile(@"directory\file_that_doesnt_exist", actualExtractionDir, ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                    });

                ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: entry", delegate
                {
                    archive.ExtractFile(null, actualExtractionDir, ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                });

                ExceptionAssert.Throws<ArgumentException>("The entry's file name cannot be empty.\r\nParameter name: entry", delegate
                {
                    archive.ExtractFile(string.Empty, actualExtractionDir, ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                });

                ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: directory", delegate
                {
                    archive.ExtractFile(archive.First(x => !x.IsDirectory).FileName, (string)null, ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                });

                ExceptionAssert.Throws<DirectoryNotFoundException>(string.Format("The directory '{0}' does not exist.", @"K:\BadDirectory\"), delegate
                {
                    archive.ExtractFile(archive.First(x => !x.IsDirectory).FileName, @"K:\BadDirectory\", ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                });
            }

            DeleteDirectory(actualExtractionDir);
        }


        /// <summary>
        ///A test for ExtractFiles
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void ExtractFilesTest()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string expectedExtractionDir = Path.Combine(ArchiveDirectory, "Extracted", archiveName);
            string actualExtractionDir = Path.Combine(CreateTestExtractionDirectory(CreateTestResultsDirectory(), archiveName));

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                archive.ExtractFiles(new string[0], actualExtractionDir);
                Assert.AreEqual(0, Directory.GetFiles(actualExtractionDir).Length);
                Assert.AreEqual(0, Directory.GetDirectories(actualExtractionDir).Length);

                foreach (var group in archive.GroupBy(x => passwords.FirstOrDefault(p => x.FileName.Contains(p))))
                {
                    if (group.Any(x => x.IsUntitled))
                    {
                        Assert.Fail("Not tested");
                    }
                    else if (group.Any(x => IsInvalidEntry(x.FileName)))
                    {
                        if (group.Key == null)
                        {
                            ExceptionAssert.RegexThrows<SevenZipException>(
                                "The file '.*' cannot be extracted because the path contains invalid characters or components.",
                                delegate
                                {
                                    archive.ExtractFiles(group.Select(x => x.FileName), actualExtractionDir);
                                });
                        }
                        else
                        {
                            Assert.Fail("Not tested");
                        }
                    }
                    else
                    {
                        if (group.Key == null)
                        {
                            archive.ExtractFiles(group.Select(x => x.FileName), actualExtractionDir);
                            CompareActualDirectoryContents(expectedExtractionDir, actualExtractionDir);
                            group.ToList().ForEach(x => VerifyAttributes(Path.Combine(actualExtractionDir, x.FileName), x));
                        }
                        else
                        {
                            ExceptionAssert.RegexThrows<PasswordRequiredException>("The file '.*' is encrypted, and requires a password to be decrypted.", delegate
                            {
                                archive.ExtractFiles(group.Select(x => x.FileName), actualExtractionDir);
                            });
                        }
                    }
                }

                ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: entries", delegate
                {
                    archive.ExtractFiles(null, actualExtractionDir);
                });

                ExceptionAssert.Throws<ArgumentException>(
                    string.Format("The entry at index 1 is invalid. See inner exception for details.\r\nParameter name: entries"),
                    delegate
                    {
                        archive.ExtractFiles(new string[] { archive[0].FileName, @"directory\file_that_doesnt_exist" }, actualExtractionDir);
                    });

                ExceptionAssert.Throws<ArgumentException>(
                    string.Format("The entry at index 0 is invalid. See inner exception for details.\r\nParameter name: entries"),
                    delegate
                    {
                        archive.ExtractFiles(new string[] { null, archive[0].FileName }, actualExtractionDir);
                    });

                ExceptionAssert.Throws<ArgumentException>(
                    string.Format("The entry at index 0 is invalid. See inner exception for details.\r\nParameter name: entries"),
                    delegate
                    {
                        archive.ExtractFiles(new string[] { string.Empty, archive[0].FileName }, actualExtractionDir);
                    });

                ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: directory", delegate
                {
                    archive.ExtractFiles(archive.Take(1).Select(x => x.FileName), (string)null);
                });

                ExceptionAssert.Throws<DirectoryNotFoundException>(string.Format("The directory '{0}' does not exist.", @"K:\BadDirectory\"), delegate
                {
                    archive.ExtractFiles(archive.Take(1).Select(x => x.FileName), @"K:\BadDirectory\");
                });
            }

            DeleteDirectory(actualExtractionDir);
        }

        /// <summary>
        ///A test for ExtractFiles
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void ExtractFilesTest1()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string expectedExtractionDir = Path.Combine(ArchiveDirectory, "Extracted", archiveName);
            string actualExtractionDir = Path.Combine(CreateTestExtractionDirectory(CreateTestResultsDirectory(), archiveName));

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                archive.ExtractFiles(new string[0], actualExtractionDir, ExtractOptions.OverwriteExistingFiles);
                Assert.AreEqual(0, Directory.GetFiles(actualExtractionDir).Length);
                Assert.AreEqual(0, Directory.GetDirectories(actualExtractionDir).Length);

                foreach (var group in archive.GroupBy(x => passwords.FirstOrDefault(p => x.FileName.Contains(p))))
                {
                    if (group.Any(x => x.IsUntitled))
                    {
                        Assert.Fail("Not tested");
                    }
                    else
                    {
                        if (group.Key == null)
                        {
                            archive.ExtractFiles(group.Select(x => x.FileName), actualExtractionDir, ExtractOptions.RenameInvalidEntries);
                            CompareActualDirectoryContents(expectedExtractionDir, actualExtractionDir);
                            group.ToList().ForEach(x => VerifyAttributes(AutoRenamePath(actualExtractionDir, x.FileName), x));
                        }
                        else
                        {
                            ExceptionAssert.RegexThrows<PasswordRequiredException>("The file '.*' is encrypted, and requires a password to be decrypted.", delegate
                            {
                                archive.ExtractFiles(group.Select(x => x.FileName), actualExtractionDir, ExtractOptions.OverwriteExistingFiles);
                            });
                        }
                    }
                }

                ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: entries", delegate
                {
                    archive.ExtractFiles(null, actualExtractionDir, ExtractOptions.NoAbortOnFailure);
                });

                ExceptionAssert.Throws<ArgumentException>(
                    string.Format("The entry at index 1 is invalid. See inner exception for details.\r\nParameter name: entries"),
                    delegate
                    {
                        archive.ExtractFiles(new string[] { archive[0].FileName, @"directory\file_that_doesnt_exist" }, actualExtractionDir, ExtractOptions.NoAbortOnFailure);
                    });

                ExceptionAssert.Throws<ArgumentException>(
                    string.Format("The entry at index 0 is invalid. See inner exception for details.\r\nParameter name: entries"),
                    delegate
                    {
                        archive.ExtractFiles(new string[] { null, archive[0].FileName }, actualExtractionDir, ExtractOptions.NoAbortOnFailure);
                    });

                ExceptionAssert.Throws<ArgumentException>(
                    string.Format("The entry at index 0 is invalid. See inner exception for details.\r\nParameter name: entries"),
                    delegate
                    {
                        archive.ExtractFiles(new string[] { string.Empty, archive[0].FileName }, actualExtractionDir, ExtractOptions.NoAbortOnFailure);
                    });

                ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: directory", delegate
                {
                    archive.ExtractFiles(archive.Take(1).Select(x => x.FileName), (string)null, ExtractOptions.NoAbortOnFailure);
                });

                ExceptionAssert.Throws<DirectoryNotFoundException>(string.Format("The directory '{0}' does not exist.", @"K:\BadDirectory\"), delegate
                {
                    archive.ExtractFiles(archive.Take(1).Select(x => x.FileName), @"K:\BadDirectory\", ExtractOptions.NoAbortOnFailure);
                });
            }

            DeleteDirectory(actualExtractionDir);
        }

        /// <summary>
        ///A test for ExtractFiles
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void ExtractFilesTest2()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string expectedExtractionDir = Path.Combine(ArchiveDirectory, "Extracted", archiveName);
            string actualExtractionDir = Path.Combine(CreateTestExtractionDirectory(CreateTestResultsDirectory(), archiveName));

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                archive.ExtractFiles(new string[0], actualExtractionDir, ExtractOptions.OverwriteExistingFiles, "password_that_will_not_be_used");
                Assert.AreEqual(0, Directory.GetFiles(actualExtractionDir).Length);
                Assert.AreEqual(0, Directory.GetDirectories(actualExtractionDir).Length);

                foreach (var group in archive.GroupBy(x => passwords.FirstOrDefault(p => x.FileName.Contains(p))))
                {
                    if (group.Any(x => x.IsUntitled))
                    {
                        Assert.Fail("Not tested");
                    }
                    else
                    {
                        if (group.Key == null)
                        {
                            archive.ExtractFiles(group.Select(x => x.FileName), actualExtractionDir, ExtractOptions.RenameInvalidEntries, headerPassword ?? "password_that_will_not_be_used");
                            CompareActualDirectoryContents(expectedExtractionDir, actualExtractionDir);
                            group.ToList().ForEach(x => VerifyAttributes(AutoRenamePath(actualExtractionDir, x.FileName), x));
                        }
                        else
                        {
                            archive.ExtractFiles(group.Select(x => x.FileName), actualExtractionDir, ExtractOptions.RenameInvalidEntries, group.Key);
                            CompareActualDirectoryContents(expectedExtractionDir, actualExtractionDir);
                            group.ToList().ForEach(x => VerifyAttributes(AutoRenamePath(actualExtractionDir, x.FileName), x)); 
                        }
                    }
                }

                ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: entries", delegate
                {
                    archive.ExtractFiles(null, actualExtractionDir, ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                });

                ExceptionAssert.Throws<ArgumentException>(
                    string.Format("The entry at index 1 is invalid. See inner exception for details.\r\nParameter name: entries"),
                    delegate
                    {
                        archive.ExtractFiles(new string[] { archive[0].FileName, @"directory\file_that_doesnt_exist" }, actualExtractionDir, ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                    });

                ExceptionAssert.Throws<ArgumentException>(
                    string.Format("The entry at index 0 is invalid. See inner exception for details.\r\nParameter name: entries"),
                    delegate
                    {
                        archive.ExtractFiles(new string[] { null, archive[0].FileName }, actualExtractionDir, ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                    });

                ExceptionAssert.Throws<ArgumentException>(
                    string.Format("The entry at index 0 is invalid. See inner exception for details.\r\nParameter name: entries"),
                    delegate
                    {
                        archive.ExtractFiles(new string[] { string.Empty, archive[0].FileName }, actualExtractionDir, ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                    });

                ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: directory", delegate
                {
                    archive.ExtractFiles(archive.Take(1).Select(x => x.FileName), (string)null, ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                });

                ExceptionAssert.Throws<DirectoryNotFoundException>(string.Format("The directory '{0}' does not exist.", @"K:\BadDirectory\"), delegate
                {
                    archive.ExtractFiles(archive.Take(1).Select(x => x.FileName), @"K:\BadDirectory\", ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                });
            }

            DeleteDirectory(actualExtractionDir);
        }

        /// <summary>
        ///A test for ExtractAll
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void ExtractAllTest()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string expectedExtractionDir = Path.Combine(ArchiveDirectory, "Extracted", archiveName);
            string actualExtractionDir = Path.Combine(CreateTestExtractionDirectory(CreateTestResultsDirectory(), archiveName));

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                if (passwords.Length == 0)
                {
                    if (archive.Any(x => IsInvalidEntry(x.FileName)))
                    {
                        ExceptionAssert.RegexThrows<SevenZipException>("The file '.*' cannot be extracted because the path contains invalid characters or components.", delegate
                        {
                            archive.ExtractAll(actualExtractionDir);
                        });
                    }
                    else
                    {
                        archive.ExtractAll(actualExtractionDir);
                        if (archive.Format == ArchiveFormat.Xar)
                        {
                            CompareActualDirectoryContents(expectedExtractionDir, actualExtractionDir);
                        }
                        else
                        {
                            CompareDirectoryContents(expectedExtractionDir, actualExtractionDir);
                        }
                        archive.ToList().ForEach(x => VerifyAttributes(AutoRenamePath(actualExtractionDir, x.FileName), x));
                    }
                }
                else
                {
                    ExceptionAssert.RegexThrows<PasswordRequiredException>("The file '.*' is encrypted, and requires a password to be decrypted.", delegate
                    {
                        archive.ExtractAll(actualExtractionDir);
                    });
                }

                ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: directory", delegate
                {
                    archive.ExtractAll(null);
                });

                ExceptionAssert.Throws<DirectoryNotFoundException>(string.Format("The directory '{0}' does not exist.", @"K:\BadDirectory\"), delegate
                {
                    archive.ExtractAll(@"K:\BadDirectory\");
                });
            }

            DeleteDirectory(actualExtractionDir);
        }

        /// <summary>
        ///A test for ExtractAll
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void ExtractAllTest1()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string expectedExtractionDir = Path.Combine(ArchiveDirectory, "Extracted", archiveName);
            string actualExtractionDir = Path.Combine(CreateTestExtractionDirectory(CreateTestResultsDirectory(), archiveName));

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                if (passwords.Length == 0)
                {
                    archive.ExtractAll(actualExtractionDir, ExtractOptions.RenameInvalidEntries);
                    if (archive.Format == ArchiveFormat.Xar)
                    {
                        CompareActualDirectoryContents(expectedExtractionDir, actualExtractionDir);
                    }
                    else
                    {
                        CompareDirectoryContents(expectedExtractionDir, actualExtractionDir);
                    }
                    archive.ToList().ForEach(x => VerifyAttributes(AutoRenamePath(actualExtractionDir, x.FileName), x));
                }
                else
                {
                    ExceptionAssert.RegexThrows<PasswordRequiredException>("The file '.*' is encrypted, and requires a password to be decrypted.", delegate
                    {
                        archive.ExtractAll(actualExtractionDir, ExtractOptions.RenameInvalidEntries);
                    });
                }

                ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: directory", delegate
                {
                    archive.ExtractAll(null, ExtractOptions.NoAbortOnFailure);
                });

                ExceptionAssert.Throws<DirectoryNotFoundException>(string.Format("The directory '{0}' does not exist.", @"K:\BadDirectory\"), delegate
                {
                    archive.ExtractAll(@"K:\BadDirectory\", ExtractOptions.NoAbortOnFailure);
                });
            }

            DeleteDirectory(actualExtractionDir);
        }

        /// <summary>
        ///A test for ExtractAll
        ///</summary>
        [TestMethod]
        [DataSource(DataProvider, "|DataDirectory|\\TestData\\ExtractionTests.csv", "ExtractionTests#csv", DataAccessMethod.Sequential)]
        public void ExtractAllTest2()
        {
            string archiveName = Convert.ToString(TestContext.DataRow["Archive"]);
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string headerPassword = Convert.ToString(TestContext.DataRow["HeaderPassword"]) != string.Empty ? Convert.ToString(TestContext.DataRow["HeaderPassword"]) : null;
            string[] passwords = Convert.ToString(TestContext.DataRow["Passwords"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string expectedExtractionDir = Path.Combine(ArchiveDirectory, "Extracted", archiveName);
            string actualExtractionDir = Path.Combine(CreateTestExtractionDirectory(CreateTestResultsDirectory(), archiveName));

            using (SevenZipArchive archive = new SevenZipArchive(archivePath, ArchiveFormat.Unkown, headerPassword))
            {
                if (passwords.Length == 0)
                {
                    archive.ExtractAll(actualExtractionDir, ExtractOptions.RenameInvalidEntries, headerPassword ?? "password_that_will_not_be_used");
                    if (archive.Format == ArchiveFormat.Xar)
                    {
                        CompareActualDirectoryContents(expectedExtractionDir, actualExtractionDir);
                    }
                    else
                    {
                        CompareDirectoryContents(expectedExtractionDir, actualExtractionDir);
                    }
                    archive.ToList().ForEach(x => VerifyAttributes(AutoRenamePath(actualExtractionDir, x.FileName), x));
                }
                else if (passwords.Length == 1)
                {
                    archive.ExtractAll(actualExtractionDir, ExtractOptions.RenameInvalidEntries, passwords[0]);
                    CompareDirectoryContents(expectedExtractionDir, actualExtractionDir);
                    archive.ToList().ForEach(x => VerifyAttributes(AutoRenamePath(actualExtractionDir, x.FileName), x));
                }
                else
                {
                    ExceptionAssert.RegexThrows<BadPasswordException>("Incorrect password specified to decrypt the file '.*'.", delegate
                    {
                        archive.ExtractAll(actualExtractionDir, ExtractOptions.RenameInvalidEntries, passwords[0]);
                    });
                }

                ExceptionAssert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: directory", delegate
                {
                    archive.ExtractAll(null, ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                });

                ExceptionAssert.Throws<DirectoryNotFoundException>(string.Format("The directory '{0}' does not exist.", @"K:\BadDirectory\"), delegate
                {
                    archive.ExtractAll(@"K:\BadDirectory\", ExtractOptions.NoAbortOnFailure, "password_that_will_not_be_used");
                });
            }

            DeleteDirectory(actualExtractionDir);
        }

        /// <summary>
        ///A test for ExtractFile
        ///</summary>
        [TestMethod]
        public void ExtractUntitledEntryTest()
        {
            string archiveName = "Sample.tar.gz";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string actualExtractionDir = Path.Combine(CreateTestExtractionDirectory(CreateTestResultsDirectory(), archiveName));

            using (SevenZipArchive archive = new SevenZipArchive(File.OpenRead(archivePath)))
            {
                Assert.IsNotNull(archive.Parent);
                Assert.IsTrue(archive.Parent[0].IsUntitled);
                ExceptionAssert.Throws<ArgumentException>(
                    string.Format("The entry '{0}' cannot be extracted to the file system because it is untitled. " +
                    "You can try to extract it manually to a file stream.", archive.Parent[0].FileName),
                    delegate
                    {
                        archive.Parent.ExtractFile(archive.Parent[0].FileName, actualExtractionDir);
                    });

                using (MemoryStream stream = new MemoryStream())
                {
                    archive.Parent.ExtractFile(archive.Parent[0].FileName, stream);
                    Assert.IsTrue(stream.Length > 0);
                }
            }
        }

        /// <summary>
        ///A test for FileExtracted
        ///</summary>
        [TestMethod]
        public void FileExtractedEventTest()
        {
            string archiveName = "Sample.tar.gz";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string actualExtractionDir = Path.Combine(CreateTestExtractionDirectory(CreateTestResultsDirectory(), archiveName));

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                EventHandler<FileExtractedEventArgs> handler = (s, e) =>
                {
                    throw new Exception("One two three.");
                };

                archive.FileExtracted += handler;
                try
                {
                    archive.ExtractAll(actualExtractionDir);
                    Assert.Fail("Exception expected.");
                }
                catch(Exception e)
                {
                    Assert.IsInstanceOfType(e, typeof(SevenZipException));
                    Assert.AreEqual(e.Message, "An error occured while involking the SevenZipArchive.FileExtracted handler.");
                    Assert.IsInstanceOfType(e.InnerException, typeof(Exception));
                    Assert.AreEqual(e.InnerException.Message, "One two three.");
                }
                archive.FileExtracted -= handler;

                int count = 0;
                archive.FileExtracted += (s, e) =>
                {
                    count++;
                    Assert.AreEqual(1, e.EntriesProcessed);
                    Assert.AreEqual(archive.Count, e.EntriesTotal);
                    Assert.AreEqual(Path.Combine(actualExtractionDir, e.CurrentEntry.FileName), e.TargetFilePath);
                    Assert.IsFalse(e.Cancel);
                    e.Cancel = true;
                };

                archive.ExtractAll(actualExtractionDir);
                Assert.AreEqual(1, count);
            }
        }

        /// <summary>
        ///A test for FileChecked
        ///</summary>
        [TestMethod]
        public void FileCheckedEventTest()
        {
            string archiveName = "Sample.tar.gz";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                EventHandler<FileCheckedEventArgs> handler = (s, e) =>
                {
                    throw new Exception("One two three.");
                };

                archive.FileChecked += handler;
                try
                {
                    archive.CheckAll();
                    Assert.Fail("Exception expected.");
                }
                catch (Exception e)
                {
                    Assert.IsInstanceOfType(e, typeof(SevenZipException));
                    Assert.AreEqual(e.Message, "An error occured while involking the SevenZipArchive.FileChecked handler.");
                    Assert.IsInstanceOfType(e.InnerException, typeof(Exception));
                    Assert.AreEqual(e.InnerException.Message, "One two three.");
                }
                archive.FileChecked-= handler;

                int count = 0;
                archive.FileChecked += (s, e) =>
                {
                    count++;
                    Assert.AreEqual(1, e.EntriesProcessed);
                    Assert.AreEqual(archive.Count, e.EntriesTotal);
                    Assert.IsFalse(e.Cancel);
                    e.Cancel = true;
                };

                Assert.IsTrue(archive.CheckAll());
                Assert.AreEqual(1, count);
            }
        }

        /// <summary>
        ///A test for FileExtractionFailed
        ///</summary>
        [TestMethod]
        public void FileExtractionFailedEventTest()
        {
            string archiveName = "Sample.tar.gz";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string actualExtractionDir = Path.Combine(CreateTestExtractionDirectory(CreateTestResultsDirectory(), archiveName));

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                string entry = archive.First(x => !x.IsDirectory).FileName;

                archive.ExtractFile(entry, actualExtractionDir);
                Assert.IsTrue(File.Exists(Path.Combine(actualExtractionDir, entry)));

                EventHandler<FileExtractionFailedEventArgs> handler = (s, e) =>
                {
                    throw new Exception("One two three.");
                };

                archive.FileExtractionFailed += handler;
                try
                {
                    archive.ExtractFile(entry, actualExtractionDir);
                    Assert.Fail("Exception expected.");
                }
                catch (Exception e)
                {
                    Assert.IsInstanceOfType(e, typeof(SevenZipException));
                    Assert.AreEqual(e.Message, "An error occured while involking the SevenZipArchive.FileExtractionFailed handler.");
                    Assert.IsInstanceOfType(e.InnerException, typeof(Exception));
                    Assert.AreEqual(e.InnerException.Message, "One two three.");
                }
                archive.FileExtractionFailed -= handler;

                int count = 0;
                archive.FileExtractionFailed += (s, e) =>
                {
                    count++;
                    Assert.IsTrue(Regex.IsMatch(e.Exception.Message, "The file '.*' cannot be extracted to '.*' because a file with the same name already exists."));
                    Assert.AreEqual(1, e.EntriesProcessed);
                    Assert.AreEqual(1, e.EntriesTotal);
                    Assert.AreEqual(Path.Combine(actualExtractionDir, e.CurrentEntry.FileName), e.TargetFilePath);
                    Assert.IsTrue(e.AbortAndThrow);
                    e.AbortAndThrow = false;
                };

                archive.ExtractFile(entry, actualExtractionDir);
                Assert.AreEqual(1, count);
            }
        }

        /// <summary>
        ///A test for FileExists
        ///</summary>
        [TestMethod]
        public void FileExistsEventTest()
        {
            string archiveName = "Sample.tar.gz";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string actualExtractionDir = Path.Combine(CreateTestExtractionDirectory(CreateTestResultsDirectory(), archiveName));

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                string entry = archive.First(x => !x.IsDirectory).FileName;

                archive.ExtractFile(entry, actualExtractionDir);
                Assert.IsTrue(File.Exists(Path.Combine(actualExtractionDir, entry)));

                bool fileExistsHandlerCalled = false;
                archive.FileExists += (s, e) =>
                {
                    fileExistsHandlerCalled = true;
                    Assert.AreEqual(FileExistsAction.Throw, e.Action);
                    Assert.AreEqual(actualExtractionDir, e.TargetDirectory);
                    e.Action = FileExistsAction.Skip;
                };

                bool existingFileSkippedHandlerCalled = false;
                archive.ExistingFileSkipped += (s, e) =>
                {
                    existingFileSkippedHandlerCalled = true;
                    Assert.AreEqual(archive.Count, e.EntriesTotal);
                    Assert.AreEqual(Path.Combine(actualExtractionDir, entry), e.ExistingFilePath);
                };
                
                archive.ExtractAll(actualExtractionDir);
                Assert.IsTrue(existingFileSkippedHandlerCalled);
                Assert.IsTrue(fileExistsHandlerCalled);
            }
        }

        /// <summary>
        ///A test for FileCheckFailed
        ///</summary>
        [TestMethod]
        public void FileCheckFailedEventTest()
        {
            string archiveName = "Sample_crypto.zip";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                string entry = archive.First(x => !x.IsDirectory && x.IsEncrypted).FileName;

                EventHandler<FileCheckFailedEventArgs> handler = (s, e) =>
                {
                    throw new Exception("One two three.");
                };

                archive.FileCheckFailed += handler;
                try
                {
                    archive.CheckFile(entry);
                    Assert.Fail("Exception expected.");
                }
                catch (Exception e)
                {
                    Assert.IsInstanceOfType(e, typeof(SevenZipException));
                    Assert.AreEqual(e.Message, "An error occured while involking the SevenZipArchive.FileCheckFailed handler.");
                    Assert.IsInstanceOfType(e.InnerException, typeof(Exception));
                    Assert.AreEqual(e.InnerException.Message, "One two three.");
                }
                archive.FileCheckFailed -= handler;

                int count = 0;
                archive.FileCheckFailed += (s, e) =>
                {
                    count++;
                    Assert.IsTrue(Regex.IsMatch(e.Exception.Message, "The file '.*' is encrypted, and requires a password to be decrypted."));
                    Assert.AreEqual(1, e.EntriesProcessed);
                    Assert.AreEqual(1, e.EntriesTotal);
                };

                archive.CheckFile(entry);
                Assert.AreEqual(1, count);
            }
        }

        /// <summary>
        ///A test for PasswordRequested
        ///</summary>
        [TestMethod]
        public void PasswordRequestedEventTest()
        {
            string archiveName = "Sample_crypto.zip";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                string entry = archive.First(x => !x.IsDirectory && x.IsEncrypted).FileName;

                Assert.IsFalse(archive.CheckAll());

                bool handled = false;
                archive.PasswordRequested += (s, e) =>
                {
                    handled = true;
                    e.Password = "crypto";
                };

                Assert.IsTrue(archive.CheckAll());
                Assert.IsTrue(handled);
            }
        }

        /// <summary>
        ///A test for GetEnumerator
        ///</summary>
        [TestMethod]
        public void GetEnumeratorTest()
        {
            string archiveName = "Sample.rar";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, archiveName + ".xml");

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                XElement dump = XDocument.Load(expectedDumpPath).Root;
                string[] expected = dump.Element("Entries").Elements().Select(x => x.Element("FileName").Value).ToArray();
                IEnumerator<ArchiveEntry> actual = archive.GetEnumerator();

                int i = 0;
                while(actual.MoveNext())
                {
                    Assert.AreEqual(actual.Current.FileName, expected[i++]);
                }
                Assert.AreEqual(i, expected.Length);
            }
        }

        /// <summary>
        ///A test for System.Collections.IEnumerable.GetEnumerator
        ///</summary>
        [TestMethod]
        public void GetEnumeratorTest1()
        {
            string archiveName = "Sample.rar";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, archiveName + ".xml");

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                XElement dump = XDocument.Load(expectedDumpPath).Root;
                string[] expected = dump.Element("Entries").Elements().Select(x => x.Element("FileName").Value).ToArray();
                IEnumerator actual = archive.GetEnumerator();

                int i = 0;
                while (actual.MoveNext())
                {
                    Assert.AreEqual((actual.Current as ArchiveEntry).FileName, expected[i++]);
                }
                Assert.AreEqual(i, expected.Length);
            }
        }

        /// <summary>
        ///A test for System.Collections.Generic.ICollection<SevenZipLib.ArchiveEntry>.Add
        ///</summary>
        [TestMethod]
        public void AddTest()
        {
            string archiveName = "Sample.rar";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                ICollection<ArchiveEntry> target = archive;
                ExceptionAssert.Throws<NotSupportedException>(delegate { target.Add(archive[0]); });
            }
        }

        /// <summary>
        ///A test for System.Collections.Generic.ICollection<SevenZipLib.ArchiveEntry>.Clear
        ///</summary>
        [TestMethod]
        public void ClearTest()
        {
            string archiveName = "Sample.rar";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                ICollection<ArchiveEntry> target = archive;
                ExceptionAssert.Throws<NotSupportedException>(delegate { target.Clear(); });
            }
        }

        /// <summary>
        ///A test for System.Collections.Generic.ICollection<SevenZipLib.ArchiveEntry>.Remove
        ///</summary>
        [TestMethod]
        public void RemoveTest()
        {
            string archiveName = "Sample.rar";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                ICollection<ArchiveEntry> target = archive;
                ExceptionAssert.Throws<NotSupportedException>(delegate { target.Remove(archive[0]); });
            }
        }

        /// <summary>
        ///A test for IsReadOnly
        ///</summary>
        [TestMethod]
        public void IsReadOnlyTest()
        {
            string archiveName = "Sample.rar";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                Assert.IsTrue(archive.IsReadOnly);
            }
        }

        /// <summary>
        ///A test for this[string]
        ///</summary>
        [TestMethod]
        public void IndexerTest()
        {
            string archiveName = "Sample.lzh";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, archiveName + ".xml");

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                XElement dump = XDocument.Load(expectedDumpPath).Root;
                string[] expected = dump.Element("Entries").Elements().Select(x => x.Element("FileName").Value).ToArray();

                for (int i = 0; i < expected.Length; i++)
                {
                    Assert.AreEqual(expected[i], archive[expected[i]].FileName);
                }

                ExceptionAssert.Throws<ArgumentNullException>(delegate { archive[null].ToString(); });
                ExceptionAssert.Throws<KeyNotFoundException>(delegate { archive["qwertyuiop"].ToString(); });
            }
        }

        /// <summary>
        ///A test for this[int]
        ///</summary>
        [TestMethod]
        public void IndexerTest1()
        {
            string archiveName = "Sample.lzh";
            string archivePath = Path.Combine(ArchiveDirectory, archiveName);
            string expectedDumpPath = Path.Combine(ExpectedArchiveDumpDirectory, archiveName + ".xml");

            using (SevenZipArchive archive = new SevenZipArchive(archivePath))
            {
                XElement dump = XDocument.Load(expectedDumpPath).Root;
                string[] expected = dump.Element("Entries").Elements().Select(x => x.Element("FileName").Value).ToArray();

                for (int i = 0; i < expected.Length; i++)
                {
                    Assert.AreEqual(expected[i], archive[i].FileName);
                }

                ExceptionAssert.Throws<IndexOutOfRangeException>(delegate { archive[-1].ToString(); });
                ExceptionAssert.Throws<IndexOutOfRangeException>(delegate { archive[archive.Count + 1].ToString(); });
            }
        }
    }
}
