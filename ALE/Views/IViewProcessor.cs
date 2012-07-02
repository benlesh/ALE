using System;
namespace ALE.Views
{
    public interface IViewProcessor
    {
        void Render(string view, object model, Action<Exception, string> callback);
    }
}
