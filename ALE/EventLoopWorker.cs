using System.Threading;
using System.Threading.Tasks;

namespace ALE
{
    public class EventLoopWorker
    {
        private Task _worker;

        public readonly ManualResetEvent StopHandle = new ManualResetEvent(false);

        public bool Idle { get; private set; }

        public void Start()
        {
            StopHandle.Reset();
            _worker.ContinueWith(Work);
        }

        public void Stop()
        {
            StopHandle.Set();
        }

        public void Wait()
        {
            _worker.Wait();
        }

        public EventLoopWorker()
        {
            _worker = Task.Factory.StartNew(() => { });
        }

        private void Work(Task incomingTask)
        {
            if (StopHandle.WaitOne(0))
            {
                return;
            }
            var evt = EventLoop.NextEvent();
            if (evt != null)
            {
                Idle = false;
                _worker = incomingTask.ContinueWith(w2 => evt());
                _worker.ContinueWith(Work);
            } else
            {
                Idle = true;
            }
            incomingTask.Dispose();
        }
    }
}