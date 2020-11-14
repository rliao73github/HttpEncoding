using System;
using System.Collections;

namespace FormDelegate
{
    public delegate void MyTestDelegate(int i);
    public class DelegateTest
    {

        public delegate void CompareDelegate(int a, int b);

        public delegate void SomeMethodPtr();

        public static void SomeMethod()
        {
            Console.WriteLine("Method Called!");
        }

        public static void Compare(int a, int b)
        {
            Console.WriteLine("a > b is {0}", (a > b).ToString());
        }

        public static void ReceiveDelegateArgsFunc(MyTestDelegate func)
        {
            func(21);
        }

        public static void DelegateFunction(int i)
        {
            Console.WriteLine("in-take parameter is: {0}", i);
        }


        public static void CallBack(int i)
        {
            Console.WriteLine(i);
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            SomeMethodPtr obj = new SomeMethodPtr(SomeMethod);

            obj.Invoke();
            //-- obj.Invoke();

            //ReceiveDelegateArgsFunc(new MyTestDelegate(DelegateFunction));


            CompareDelegate cd = new CompareDelegate(Compare);

            //-- cd.Invoke(15, 30);

            cd.Invoke(10, 20);

            ////-- boxing a value type
            //int i = 123;
            //// The following line boxes i.
            //object o = i;

            //o = 123;
            //i = (int)o;  // unboxing

            //ArrayList myInts = new ArrayList();
            //myInts.Add(1); // boxing
            //myInts.Add(2); // boxing

            //int myInt = (int)myInts[0]; // unboxing

            //MyClass myObj = new MyClass();
            //myObj.LongRunning(CallBack);

            Console.WriteLine("End of DelegateTest!");
        }
    }

    public class MyClass
    {
        public string MyProp { get; set; }

        private int myVar;
        public int MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty MyPropertyProperty =
        //    DependencyProperty.Register("MyProperty", typeof(int), typeof(ownerclass), new PropertyMetadata(0));

        public delegate void CallBack(int i);

        public void LongRunning(CallBack obj)
        {
            int j = 0;
            for (int i = 0; i < 10000; i++)
            {
                obj(i);
            }
        }

    }
}
