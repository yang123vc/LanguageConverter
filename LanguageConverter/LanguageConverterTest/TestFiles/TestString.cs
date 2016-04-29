namespace LanguageConverterTest.TestFiles
{
    public class Program
    {
        public void TestString(string[] args)
        {
            string str = "testString";
            bool isNullOrEmpty = string.IsNullOrEmpty(str);
            var length = str.Length;
            var c = str[1];
            var lower = str.ToLower();
            var b = str.Contains("te");
        }
    }
}
