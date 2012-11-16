using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALE
{
    public class Promise
    {
        protected readonly List<Action<object>> ThenCallbacks = new List<Action<object>>();
        protected readonly List<Action<string>> ErrorCallbacks = new List<Action<string>>();

        public static Promise When(params Promise[] promises)
        {
            var i = promises.Length;
            return new Promise((deferrer) =>
            {
                foreach (var promise in promises)
                {
                    promise.Then((data) =>
                    {
                        if (--i == 0)
                        {
                            deferrer.Resolve(data);
                        }
                    }).Error(deferrer.Reject);
                }
            });
        }
        
        public static Promise To(Action<Deferrer> act)
        {
            return new Promise(act);
        }

        private Promise(Action<Deferrer> act)
        {
            var deferrer = new Deferrer(this);
            act(deferrer);
        }

        internal void PendThen(object result)
        {
            foreach (var thenCallback in ThenCallbacks)
            {
                var callback = thenCallback;
                EventLoop.Pend(() => callback(result));
            }
        }

        internal void PendError(string reason)
        {
            foreach (var errorCallback in ErrorCallbacks)
            {
                var callback = errorCallback;
                EventLoop.Pend(() => callback(reason));
            }
        }

        public Promise Then(Action<object> callback)
        {
            ThenCallbacks.Add(callback);
            return this;
        }

        public Promise Error(Action<string> callback)
        {
            ErrorCallbacks.Add(callback);
            return this;
        }
    }
}
