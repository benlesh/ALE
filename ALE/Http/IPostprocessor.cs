using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALE.Http
{
    public interface IPostprocessor
    {
        void Execute(IRequest req, IResponse res);
    }
}
