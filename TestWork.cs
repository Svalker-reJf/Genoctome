using System;

public class Class1
{
	static void main()
    {
        testin root = new testin();
        testin imit = root;

        imit.item = 1;

        root.report();
        imit.report();
    }

    public class testin
    {
        public int item = 0;

        public void report()
        {
            Console.WriteLine(" item = " + item.ToString());
        }
    }
}
