using NamespaceB;
using NamespaceC;
using NamespaceD;

namespace NamespaceA
{
    public class ClassWithManyDeps
    {
        public string MyProperty { get; set; }
        public bool MyBool { get; set; }
        public ClassC MyC { get; set; }
        public ClassB MyBe { get; set; }
        public IEnumerable<ClassB> MyBs { get; set; }
        public Dictionary<string, ClassB> MyBDict { get; set; }
        public Dictionary<int, ClassB> MyBDict2 { get; set; }
        public Dictionary<ClassC, ClassB> MyBDict3 { get; set; }
        public Dictionary<ClassC, ClassB[]> MyBDict4 { get; set; }
        public HashSet<ClassD<int>> MyDSet { get; set; }
    }
}

namespace NamespaceB
{
    public class ClassB
    {
        public string MyProperty { get; set; }
        public bool MyBool { get; set; }
    }
}

namespace NamespaceC
{
    public class ClassC
    {
        public string MyProperty { get; set; }
        public bool MyBool { get; set; }
    }
}

namespace NamespaceD
{
    public class ClassD<T>
    {
        public T MyT { get; set; }
    }
}
