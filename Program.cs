using System;
using System.Threading;

internal static class Run
{
    private static void Main()
    {
        const int runCount = 100;

        for (var run = 0; run < runCount; run++)
        {
            var example = new InterleavedExample();
            example.Runner();
        }
    }
}

internal class InterleavedExample
{
    private static readonly object _slotLock = new object(); //Used in InterLeaving Critical Section
    private static int i;

    private static int slot = -1;
    private static int[] array = new int[2];

    public void Runner()
    {
        i = 0;
        slot = -1;
        array[0] = 0;
        array[1] = 0;

        Thread t1 = new Thread(function1);
        Thread t2 = new Thread(function1);

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();
        Console.WriteLine($"[{string.Join(", ", array)}]");
    }

    private static void function1()
    {
        // 1. Get the thread ID outside the lock since it has its own lock. Maximize concurrency!
        int tid = get_counter();
        Thread.Sleep(1);

        // 2. Lock the state whose correctness depends on slot
        lock (_slotLock)
        {
            slot++;
            array[slot] = tid;
        }
    }

    private static int get_counter()
    {
        i++;
        return i;
    }
}