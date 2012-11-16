using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALE
{
    public class Future
    {
        public static Promise Promise(Action<Deferrer> act)
        {
            var promise = new Promise();
            var deferrer = new Deferrer(promise);
            act(deferrer);
            return promise;
        }

        public static Promise When(params Promise[] promises)
        {
            var i = promises.Length;
            return Promise((deferrer) =>
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
    }

    public class Promise
    {
        protected readonly List<Action<object>> ThenCallbacks = new List<Action<object>>();
        protected readonly List<Action<string>> ErrorCallbacks = new List<Action<string>>();

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

    public class Deferrer
    {
        public readonly Promise Promise;

        public Deferrer(Promise promise)
        {
            Promise = promise;
        }

        public void Resolve(object data)
        {
            Promise.PendThen(data);
        }

        public void Reject(string reason)
        {
            Promise.PendError(reason);
        }
    }
}
