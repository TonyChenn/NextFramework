using XLua;

[LuaCallCSharp]
public enum Direction
{
    Left, Right, Buttom, Top,
}


public delegate void GetInfo();
public class Animal
{
    public string Name;
}

class A
{
    public int num1;
    public int num2;
}
class B
{
    public A a;
    public int num3;

    public int Add(B b)
    {
        return b.a.num1 + b.a.num2 + b.num3;
    }
}
