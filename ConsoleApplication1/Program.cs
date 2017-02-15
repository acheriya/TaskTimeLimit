using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var activity = new Activity();
            activity.Setup().Wait();

            Console.ReadLine();
        }

       
    }


    public class Activity
    {
        private CancellationTokenSource tokenSource;

        public async Task Setup()
        {

            int timeout = 2000;  //Time out in milliseconds

            tokenSource = new CancellationTokenSource();
            var task = Task.Run(() => Run(), tokenSource.Token);   //Execute a long running process

            //Check the task is delaying 
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                // task completed within the timeout
                Console.WriteLine("Task Completed Successfully");
            }
            else
            {
                // timeout  
                //Cancel the task             
                tokenSource.Cancel();

                Console.WriteLine("Time Out. Aborting Task");

                task.Wait(); //Waiting for the task to throw OperationCanceledException
            }

        }

        public void Run()
        {
            int action = 1;

            try
            {
                //Long running process
                while (true)
                {
                    if (tokenSource.Token.IsCancellationRequested)
                        tokenSource.Token.ThrowIfCancellationRequested();  //Stop the ling running process if the cancellation requested

                    Console.WriteLine("Running action " + action++);

                    Thread.Sleep(200);
                }

            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Task Aborted");
            }

            
        }
    }


}
