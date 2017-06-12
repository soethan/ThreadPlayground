using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadingLab.Models;

namespace ThreadingLab
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Pass Parameter to Task

            Console.WriteLine("======= Pass Parameter to Task =======");

            //no return parameter
            var t1 = new Task(new Action<object>(PrintMessage), new Person { Id = 1, Name = "Steven" });
            t1.Start();

            var t2 = new Task(
                delegate(object obj)
                {
                    PrintMessage(obj);
                }, 
                new Person { Id = 2, Name = "David" }
            );

            t2.Start();

            var t3 = new Task(
                (obj) => {
                    PrintMessage(obj);
                }, 
                new Person{ Id = 3, Name = "Bob" }
            );

            t3.Start();

            //with return parameter 'int'
            var t4 = new Task<int>(() => {
                int sum = 0;
                for (int i = 0; i < 100; i++)
                {
                    sum += i;
                }
                return sum;
            });

            t4.Start();

            //Main thread will wait at t4.Result until t4 finishes execution and its return statement is executed.
            Console.WriteLine("t4.Result = {0};", t4.Result);
            Console.WriteLine("t4 Ends...");

            var t5 = new Task<int>((objParam) =>
            {
                int max = (int)objParam;
                int sum = 0;
                for (int i = 0; i < max; i++)
                {
                    sum += i;
                }
                return sum;
            }, 200);

            t5.Start();

            //Main thread will wait at t4.Result until t4 finishes execution and its return statement is executed.
            Console.WriteLine("t5.Result = {0};", t5.Result);
            Console.WriteLine("t5 Ends...");

            Task.WaitAll(t1, t2, t3);
            Console.WriteLine("==================================");

            #endregion

            #region Task Result

            Console.WriteLine("======= Task Result =======");
            //1. With new Task().Start

            var task1 = new Task<int>(() => {
                int sum = 0;
                for(int i = 0; i < 100; i++)
                {
                    sum += i;
                }
                return sum;
            });
            task1.Start();

            //Main thread will wait at task1.Result until task1 finishes execution and its return statement is executed.
            Console.WriteLine("task1.Result = {0};", task1.Result);
            Console.WriteLine("task1 Ends...");

            var task2 = new Task<int>(
                (obj) => {
                    int max = (int)obj;
                    int sum = 0;
                    for (int i = 0; i < max; i++)
                    {
                        sum += i;
                    }
                    return sum;
                },
                200
            );

            task2.Start();
            Console.WriteLine("task2.Result = {0};", task2.Result);
            Console.WriteLine("task2 Ends...");

            //2. With Task.Factory.StartNew

            var tfTask1 = Task.Factory.StartNew<int>(() => {
                int sum = 0;
                for (int i = 0; i < 100; i++)
                {
                    sum += i;
                }
                Console.WriteLine("Inside Task Body...Task Unique int value=" + Task.CurrentId);
                return sum;
            });

            //Main thread will wait at tfTask1.Result until tfTask1 finishes execution and its return statement is executed.
            Console.WriteLine("Task.Factory task1.Result = {0};", tfTask1.Result);
            Console.WriteLine("Task.Factory task1 Ends...");

            Console.WriteLine("Outside Task Body...Task Unique int value=" + Task.CurrentId);

            var tfTask2 = Task.Factory.StartNew<int>((objParam) =>
            {
                int max = (int)objParam;
                int sum = 0;
                for (int i = 0; i < max; i++)
                {
                    sum += i;
                }
                Console.WriteLine("Inside Task Body...Task Unique int value=" + Task.CurrentId);
                return sum;
            }, 200);

            Console.WriteLine("Task.Factory task2.Result = {0};", tfTask2.Result);
            Console.WriteLine("Task.Factory task2 Ends...");

            Console.WriteLine("==================================");

            #endregion

            #region Exception Handling
            /*
            Console.WriteLine("======= Exception Handling =======");
            // create the cancellation token source and the token
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            // create a task that waits on the cancellation token
            Task eTask1 = new Task(() => {
                // wait forever or until the token is cancelled
                token.WaitHandle.WaitOne(-1);
                // throw an exception to acknowledge the cancellation
                throw new OperationCanceledException(token);
                
            }, token);

            // create a task that throws an exception
            Task eTask2 = new Task(() => {
                throw new NullReferenceException();
            });

            eTask1.Start(); 
            eTask2.Start();
            // cancel the token
            tokenSource.Cancel();
            // wait on the tasks and catch any exceptions
            try
            {
                Task.WaitAll(eTask1, eTask2);
            }
            catch (AggregateException ex)
            {
                // iterate through the inner exceptions using
                // the handle method
                ex.Handle(
                    (inner) =>
                    {
                        //It seems resolved by unchecking Tools | Options | Debugging | General "Enable Just My Code".
                        if (inner is OperationCanceledException)
                        {
                            Console.WriteLine("OperationCanceledException handled...");
                            return true;
                        }
                        else
                        {
                            // this is an exception we don't know how
                            // to handle, so return false
                            //return false;
                            Console.WriteLine("Other Exceptions handled...");
                            return true;
                        }
                    }
                );
            }
             
            // write out the details of the task exception
            Console.WriteLine("Task 1 completed: {0}", eTask1.IsCompleted);//true
            Console.WriteLine("Task 1 faulted: {0}", eTask1.IsFaulted);//false
            Console.WriteLine("Task 1 cancelled: {0}", eTask1.IsCanceled);//true
            Console.WriteLine(eTask1.Exception);
            // write out the details of the task exception
            Console.WriteLine("Task 2 completed: {0}", eTask2.IsCompleted);//true
            Console.WriteLine("Task 2 faulted: {0}", eTask2.IsFaulted);//true
            Console.WriteLine("Task 2 cancelled: {0}", eTask2.IsCanceled);//false
            Console.WriteLine(eTask2.Exception);

            Console.WriteLine("==================================");
            */
            #endregion

            #region Task Continuation

            Console.WriteLine("======= Task Continuation =======");

            var firstTask = new Task<BankAccount>(() => {
                var account = new BankAccount();
                for (int i = 0; i < 1000; i++)
                {
                    account.Balance++;
                }
                return account;
            });

            var continuedTask = firstTask.ContinueWith<int>((Task<BankAccount> antecedent) => {
                Console.WriteLine("Interim Balance: {0}", antecedent.Result.Balance);
                return antecedent.Result.Balance * 2;
            });

            firstTask.Start();
            Console.WriteLine("Final balance: {0}", continuedTask.Result);

            Console.WriteLine("==================================");

            #endregion

            #region One to Many Task Continuation

            Console.WriteLine("======= One to Many Task Continuation =======");

            var rootTask = new Task<BankAccount>(() => {
                BankAccount account = new BankAccount();
                for (int i = 0; i < 1000; i++)
                {
                    account.Balance++;
                }
                return account;
            });

            // create the second-generation task, which will double the antecdent balance
            var conTask1 = rootTask.ContinueWith<int>((Task<BankAccount> antecedent) => {
                Console.WriteLine("Interim Balance 1: {0}", antecedent.Result.Balance);
                return antecedent.Result.Balance * 2;
            });
            // create a third-generation task, which will print out the result
            Task conTask2 = conTask1.ContinueWith((Task<int> antecedent) => {
                Console.WriteLine("Final Balance 1: {0}", antecedent.Result);
            });

            // create a second and third-generation task in one step
            var resultTask2 = rootTask
                .ContinueWith<int>((Task<BankAccount> antecedent) => {
                    Console.WriteLine("Interim Balance 2: {0}", antecedent.Result.Balance);
                    return antecedent.Result.Balance / 2;
                })
                .ContinueWith((Task<int> antecedent) => {
                    Console.WriteLine("Final Balance 2: {0}", antecedent.Result);
                });

            rootTask.Start();

            Task.WaitAll(conTask2, resultTask2);

            Console.WriteLine("==================================================");

            #endregion

            #region Many to One Continuation

            Console.WriteLine("======= Many to One Task Continuation =======");

            var account2 = new BankAccount();
            var tasks = new Task<int>[10];

            for (int i = 0; i < 10; i++)
            {
                tasks[i] = new Task<int>(
                    (objParam) => {
                        int isolatedBalance = (int)objParam;
                        for (int j = 0; j < 1000; j++)
                        {
                            isolatedBalance++;
                        }
                        return isolatedBalance;
                    },
                    account2.Balance
                );
            }

            // set up a multitask continuation
            Task continuation = Task.Factory.ContinueWhenAll<int>(tasks, antecedents => {
                // run through and sum the individual balances
                foreach (Task<int> t in antecedents)
                {
                    account2.Balance += t.Result;
                }
            });
            // start the antecedent tasks
            foreach (Task t in tasks)
            {
                t.Start();
            }

            // wait for the contination task to complete
            continuation.Wait();
            // write out the counter value
            Console.WriteLine("Expected value {0}, Balance: {1}", 10000, account2.Balance);

            Console.WriteLine("==================================================");

            #endregion

            #region Producer/Consumer Pattern

            Console.WriteLine("======= Producer/Consumer Pattern =======");

            //System.Collections.Concurrent.BlockingCollection combines the collection and the synchronization primitive into one class, 
            //which makes it ideal for implementing the producer/consumer pattern.
            var blockingCollection = new BlockingCollection<Deposit>();
            // create and start the producers, which will generate deposits and place them into the collection
            Task[] producers = new Task[3];
            for (int i = 0; i < 3; i++)
            {
                producers[i] = Task.Factory.StartNew(() => {
                    // create a series of deposits
                    for (int j = 0; j < 5; j++)
                    {
                        var deposit = new Deposit { Amount = 100 };
                        blockingCollection.Add(deposit);
                        Console.WriteLine("+ Producer adds deposit");
                    }
                });
            }

            // create a many to one continuation that will signal the end of production to the consumer
            Task.Factory.ContinueWhenAll(producers, antecedents => {
                // signal that production has ended
                Console.WriteLine("Signalling production end");
                blockingCollection.CompleteAdding();
            });

            Console.WriteLine("Before consumer starts");

            var accountPC = new BankAccount();
            // create the consumer, which will update the balance based on the deposits
            /*
            Task consumer = Task.Factory.StartNew(() => {
                Console.WriteLine("consumer waiting...");
                while (!blockingCollection.IsCompleted)
                {
                    Deposit deposit;
                    // try to take the next item
                    if (blockingCollection.TryTake(out deposit))
                    {
                        // update the balance with the transfer amount
                        accountPC.Balance += deposit.Amount;
                        Console.WriteLine("- Consumer processes deposit");
                    }
                }
                // print out the final balance
                Console.WriteLine("Final Balance: {0}", accountPC.Balance);
            });
            */
            Task<int> consumer1 = Task.Factory.StartNew<int>((objParam) => {
                var balance = (int)objParam;
                return ProcessProducerOutput(blockingCollection, balance, 1);
            }, accountPC.Balance);

            Task<int> consumer2 = Task.Factory.StartNew<int>((objParam) => {
                var balance = (int)objParam;
                return ProcessProducerOutput(blockingCollection, balance, 2);
            }, accountPC.Balance);

            // wait for the consumers to finish
            Task.WaitAll(consumer1, consumer2);
            Console.WriteLine("Final Balance: {0}", consumer1.Result + consumer2.Result);

            Console.WriteLine("==================================================");

            #endregion

            int[] a1 = new int[] { 1, 2, 3 };
            int[] a2 = new int[] { 3, 2, 1 };
            int[] a3 = new int[] { 1, 2, 3 };
            Console.WriteLine(a1.SequenceEqual(a2));//false
            Console.WriteLine(a1.SequenceEqual(a3));//true

            Console.Read();
        }

        private static int ProcessProducerOutput(BlockingCollection<Deposit> blockingCollection, int balance, int consumerId)
        {
            Console.WriteLine("consumer waiting...");
            //Return true if CompleteAdding() has been called and there are no items in the collection
            while (!blockingCollection.IsCompleted)
            {
                Deposit deposit;
                // try to take the next item
                if (blockingCollection.TryTake(out deposit))
                {
                    // update the balance with the transfer amount
                    balance += deposit.Amount;
                    Console.WriteLine("- Consumer{0} processes deposit", consumerId);
                }
            }

            Console.WriteLine("Interim Balance: {0}", balance);
            return balance;
        }

        private static void PrintMessage(object obj)
        {
            var person = obj as Person;
            Console.WriteLine(string.Format("Id={0};Name={1};", person.Id.ToString(), person.Name));
        }
    }
}
