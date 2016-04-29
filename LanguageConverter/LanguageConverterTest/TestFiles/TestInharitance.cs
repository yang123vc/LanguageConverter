namespace LanguageConverterTest.TestFiles
{
    public interface ITest
    {
        void Bar(int z);
    }

    public interface ITest2 : ITest
    {
        void Bar2(int z);
    }

    public abstract class TestBaseClass : ITest
    {
        public abstract int Tmp();
        protected TestBaseClass(int val)
        {

        }

        public virtual int Foo(int x)
        {
            return x + 1;
        }
    }

    public class TestDerivedClass : TestBaseClass, ITest2
    {
        public TestDerivedClass(int val = 45)
            : base(val)
        {

        }
        public override int Foo(int x = 50)
        {
            return base.Foo(x) * 100;
        }

        public void Bar(int z)
        {
            z++;
        }

        public void Bar2(int z)
        {
            --z;
        }
    }
}
