using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading;

namespace ALE.Tests
{
    [TestClass]
    public class Do_Tests
    {
        [TestMethod]
        public void Do_Timeout()
        {
            var sw = Stopwatch.StartNew();
            const int ms = 100;
            var i = 0;
            var ellapsed = 0L;
            var wait = new AutoResetEvent(false);
            Do.Timeout(() =>
                           {
                               ellapsed = sw.ElapsedMilliseconds;
                               i++;
                               wait.Set();
                           }, ms);
            if (wait.WaitOne(ms * 3))
            {
                Assert.AreEqual(1, i);
                Assert.IsTrue(ellapsed > ms * .9);
            } else
            {
                Assert.Fail("callback not executed.");
            }

            sw.Restart();
            var killswitch = Do.Timeout(() => { wait.Set(); }, ms * 2);
            killswitch.Kill();
            ellapsed = sw.ElapsedMilliseconds;
            Assert.IsTrue(ellapsed < ms * 2);
            if (wait.WaitOne(ms * 4))
            {
                Assert.Fail("Unable to stop timeout.");
            }
        }

        [TestMethod]
        public void Do_Interval()
        {
            var sw = Stopwatch.StartNew();
            var i = 0;
            var elapsed = 0L;
            var wait = new AutoResetEvent(false);
            const int intervalTime = 100;
            var killswitch = Do.Interval(() =>
                            {
                                i++;
                                if (i == 3)
                                {
                                    elapsed = sw.ElapsedMilliseconds;
                                    wait.Set();
                                }
                            }, intervalTime);
            if (wait.WaitOne(intervalTime * 5))
            {
                killswitch.Kill();
                var testMS = intervalTime * .9m;
                Assert.IsTrue(elapsed > testMS, "Elapsed time is greater than " + testMS + " ms. (" + elapsed + ")");
                Assert.AreEqual(3, i, "Too many executions.");
            } else
            {
                Assert.Fail("interval not executed properly. Timeout occurred.");
            }
            Thread.Sleep(intervalTime * 2);
            Assert.AreEqual(3, i, "interval not dead.");
        }
    }
}
