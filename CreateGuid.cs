using System;
using System.Collections.Generic;
using System.Text;

namespace azuredCreateClient
{
    class CreateGuid
    {
        public Guid returnGuid()
        {
            Guid id = Guid.NewGuid();
            return id;
        }
    }
}
