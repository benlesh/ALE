namespace ALE
{
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