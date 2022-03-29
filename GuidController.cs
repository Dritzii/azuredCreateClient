using System;
using System.Collections.Generic;
using System.Text;

namespace azuredCreateClient
{
    class GuidController
    {
        public Guid returnGuid()
        {
            Guid id = Guid.NewGuid();
            return id;
        }
    }
}
