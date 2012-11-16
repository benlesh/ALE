using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace ALE.Tests
{
    [TestClass]
    public class EventLoop_Tests
    {
        [TestMethod]
        public void EventLoop_Start()
        {
            var wait = new AutoResetEvent(false);
            EventLoop.Start(() => wait.Set());
            if (wait.WaitOne(100))
            {
                Assert.AreEqual(1, EventLoop.WorkerCount);
                Assert.IsTrue(EventLoop.IsRunning);
            } else
            {
                Assert.Fail("Did not fire initial event.");
            }
        }

        [TestMethod]
        public void EventLoop_Stop()
        {
            var wait = new AutoResetEvent(false);
            EventLoop.Start();
            const int loops = 10;
            for (int i = 0; i < loops; i++)
            {
                var x = i;
                EventLoop.Pend(() =>
                                   {
                                       Thread.Sleep(100);
                                       if (x == loops - 1)
                                       {
                                           wait.Set();
                                       }
                                   });

            }
            EventLoop.Stop();
            if (wait.WaitOne(1200))
            {
                Assert.Fail("EventLoop did not stop.");
            }
        }

        [TestMethod]
        public void EventLoop_Pend()
        {
            var wait = new AutoResetEvent(false);
            EventLoop.Start();
            EventLoop.Pend(() =>
                               {
                                   wait.Set();
                               });
            if (!wait.WaitOne(100))
            {
                Assert.Fail("Event did not fire correctly.");
            }
        }

        [TestMethod]
        public void EventLoop_IsRunning_Property()
        {
            EventLoop.Start();
            Assert.IsTrue(EventLoop.IsRunning);
            EventLoop.Stop();
            Assert.IsFalse(EventLoop.IsRunning);
        }

        [TestMethod]
        public void EventLoop_IdleWorkerCount()
        {
            EventLoop.Start();
            Thread.Sleep(10);
            Assert.AreEqual(1, EventLoop.IdleWorkerCount);
            EventLoop.Pend(() => Thread.Sleep(100));
            Thread.Sleep(10);
            Assert.AreEqual(0, EventLoop.IdleWorkerCount);
            EventLoop.Stop();
        }

        //[TestMethod]
        //public void ContinuationTask_Idle_Check()
        //{
        //    var wait = new AutoResetEvent(false);
        //    var task1 = Task.Factory.StartNew(() =>
        //                                          {
        //                                              Thread.Sleep(100);
        //                                          });
        //    var task2 = task1.ContinueWith(t1 =>
        //                                       {
        //                                           Assert.IsTrue(t1.IsCompleted);
        //                                           t1.Dispose();
        //                                           wait.Set();
        //                                       });
        //    Assert.IsFalse(task2.IsCompleted);
        //    wait.WaitOne(200);
        //    Assert.IsTrue(task2.IsCompleted);
        //}
        //[TestMethod]
        //public void TaskChaining()
        //{
        //    var actions = new Queue<Action>();
        //    for (int i = 0; i < 1000; i++)
        //    {
        //        actions.Enqueue(() => { });
        //    }
        //    Task worker = Task.Factory.StartNew(() => { });
        //    Func<Action> getNext = () => actions.Count > 0 ? actions.Dequeue() : null;
        //    Action<Task> work = null;
        //    work = (inboundTask) =>
        //                           {
        //                               Assert.AreEqual(worker, inboundTask);
        //                               var next = getNext();
        //                               if (next != null)
        //                               {
        //                                   worker = inboundTask.ContinueWith(t =>
        //                                                                         {
        //                                                                             next();
        //                                                                             t.Dispose();
        //                                                                         });
        //                                   worker.ContinueWith(work);
        //                                   Assert.AreNotEqual(inboundTask, worker);
        //                                   inboundTask.Dispose();
        //                               }
        //                           };
        //    work(worker);
        //}
    }
}