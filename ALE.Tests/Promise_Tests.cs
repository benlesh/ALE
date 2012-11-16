using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ALE.Tests
{
    [TestClass]
    public class Promise_Tests
    {
        [TestMethod]
        public void Promise_Test1()
        {
            var list = new List<int>();
            var handle = new ManualResetEvent(false);
            EventLoop.Start(() =>
                {
                    Promise.To((deferrer) =>
                        {
                            Do.Timeout(() =>
                                {
                                    list.Add(123);
                                    deferrer.Resolve(null);
                                }, 300);

                        }).Then((data) =>
                            {
                                list.Add(456);
                                handle.Set();
                            });
                });
            if(handle.WaitOne(4000))
            {
                Assert.AreEqual(123, list[0]);
                Assert.AreEqual(456, list[1]);
            }else
            {
                Assert.Fail();
            }
            EventLoop.Stop();
        }

        [TestMethod]
        public void Future_When_Test()
        {
            int i = 0;
            Func<Promise> createPromise = () =>
                {
                    return Promise.To((deferrer) =>
                        {
                            Do.Timeout(() =>
                                {
                                    i++;
                                    deferrer.Resolve(null);
                                }, 500);
                        });
                };
            EventLoop.Start();
            var handle = new ManualResetEvent(false);
            EventLoop.Start(() =>
                {
                    Promise.When(createPromise(), createPromise(), createPromise())
                        .Then((data) =>
                            {
                                handle.Set();
                            });
                });

            if (handle.WaitOne(2000))
            {
                Assert.AreEqual(3, i);
            }else
            {
                Assert.Fail();
            }
            EventLoop.Stop();
        }

        [TestMethod]
        public void PromiseError_Test()
        {
            var handle = new ManualResetEvent(false);
            Promise.To((deferrer) =>
                {
                    Do.Timeout(() => deferrer.Reject("Testing rejection."), 400);
                }).Error((reason) =>
                    {
                        handle.Set();
                    });
            if (!handle.WaitOne(600))
            {
                Assert.Fail();
            }
        }
    }
}
