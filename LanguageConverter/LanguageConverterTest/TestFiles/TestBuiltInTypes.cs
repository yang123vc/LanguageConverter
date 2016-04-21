namespace LanguageConverterTest.TestFiles
{
    public class TestBuiltinTypes
    {
        public void TestVoid() { }

        public byte TestByte()
        {
            return 0;
        }

        public char TestChar()
        {
            return 'a';
        }

        public short TestShort()
        {
            int a = 1;
            var b = "a";
            char c;
            return 0;
        }

        public ushort TestUShort()
        {
            return 0;
        }

        public uint TestUInt()
        {
            return 34;
        }

        public int TestInt()
        {
            return -1;
        }

        public long TestLong()
        {
            return 0;
        }

        public ulong TestULong()
        {
            return 45;
        }

        public double TestDouble()
        {
            return 0.0;
        }

        public bool TestBool()
        {
            return true;
        }

        public object TestObject()
        {
            return null;
        }
    }
}
