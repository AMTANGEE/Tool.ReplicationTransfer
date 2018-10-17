using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMTANGEE.Tools.ReplicationTransfer
{
    public class ReplicationObject
    {
        private Guid obj;
        public Guid Object
        {
            get { return obj; }
            set { obj = value; }
        }

        private bool subcategories;
        public bool Subcategories
        {
            get { return subcategories; }
            set { subcategories = value; }
        }

        public ReplicationObject() { }

        public ReplicationObject(Guid obj, bool subcategories)
        {
            this.obj = obj;
            this.subcategories = subcategories;
        }

        public ReplicationObject(System.Data.DataRow data)
        {
            obj = (Guid)data["OBJECT"];
            subcategories = (bool)data["SUBCATEGORIES"];
        }

    }
}
