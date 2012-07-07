using System;
using System.Threading.Tasks;
using System.Threading;

namespace ALE
{
    public class Do
    {
        public static void Async(Action operation, Action<Exception> callback = null)
        {
            EventLoop.Pend(() =>
                                    {
                                        try
                                        {
                                            operation();
                                            if (callback != null)
                                            {
                                                EventLoop.Pend(() => callback(null));
                                            }
                                        } catch (Exception ex)
                                        {
                                            if (callback != null)
                                            {
                                                EventLoop.Pend(() => callback(ex));
                                            }
                                        }
                                    });
        }

        public static void Async<TReturn>(Func<TReturn> operation, Action<Exception, TReturn> callback)
        {
            EventLoop.Pend(() =>
                               {
                                   try
                                   {
                                       var result = operation();
                                       callback(null, result);
                                   } catch (Exception ex)
                                   {
                                       EventLoop.Pend(() => callback(ex, default(TReturn)));
                                       throw;
                                   }
                               });
        }

        public static IAsyncKillswitch Timeout(Action operation, int milliseconds)
        {
            var wait = new AutoResetEvent(false);
           var regWait  =ThreadPool.RegisterWaitForSingleObject(wait,
                (st, to) => operation(),
                null,
                TimeSpan.FromMilliseconds(milliseconds),
                true);
            return new TimeoutKillswitch(regWait, wait);
        }

        public static IAsyncKillswitch Interval(Action operation, int milliseconds)
        {
            var timer = new Timer(st => operation(), null, milliseconds, milliseconds);
            return new IntervalKillswitch(timer);
        }
    }

    internal class IntervalKillswitch : IAsyncKillswitch, IDisposable
    {
        public readonly Timer Timer;

        public IntervalKillswitch(Timer timer)
        {
            Timer = timer;
        }

        public void Kill()
        {
            Dispose();
        }

        ~IntervalKillswitch()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (Timer != null)
            {
                Timer.Dispose();
            }
        }
    }
    internal class TimeoutKillswitch : IAsyncKillswitch
    {
        public readonly RegisteredWaitHandle RegisteredWaitHandle;
        public readonly WaitHandle WaitHandle;

        public TimeoutKillswitch(RegisteredWaitHandle regWait, WaitHandle wait)
        {
            RegisteredWaitHandle = regWait;
            WaitHandle = wait;
        }
        public void Kill()
        {
            RegisteredWaitHandle.Unregister(WaitHandle);
        }
    }
    public interface IAsyncKillswitch
    {
        void Kill();
    }
}