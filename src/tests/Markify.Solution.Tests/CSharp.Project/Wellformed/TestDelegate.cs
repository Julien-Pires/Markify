namespace CSharp.Project.Wellformed
{
    public delegate void TestDelegate();

    public delegate int TestDelegate2();

    public delegate void TestDelegate3(int foo);

    public delegate int TestDelegate4(int foo);

    public delegate void TestDelegate5(params int[] foo);

    public delegate int TestDelegate6(params int[] foo);
}