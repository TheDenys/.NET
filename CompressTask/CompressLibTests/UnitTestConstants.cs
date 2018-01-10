namespace CompressLibTests
{
    class UnitTestConstants
    {
        internal const string File1Mb = "file_1mb.bin";
        internal const string File10Mb = "file_10mb.bin";
        internal static long Size1Mb { get; } = 1024 * 1024;
        internal static long Size10Mb { get; } = 10 * Size1Mb;
    }
}