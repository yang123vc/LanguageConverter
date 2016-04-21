namespace LanguageConverterTest.TestFiles
{
    public abstract class TestBaseClass
    {
        protected TestBaseClass(int val)
        {

        }

        public virtual int Foo(int x)
        {
            return x + 1;
        }
    }

    public class TestDerivedClass : TestBaseClass
    {
        public TestDerivedClass(int val = 45)
            : base(val)
        {

        }
        public override int Foo(int x = 50)
        {
            return base.Foo(x) * 100;
        }
    }
}
