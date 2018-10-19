using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMTANGEE.Tools.ReplicationTransfer
{
    public class ReplicationEntry
    {
        private Guid guid;
        public Guid GUID
        {
            get { return guid; }
            set { guid = value; }
        }

        private Guid destination;
        public Guid Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public ReplicationEntry(Guid guid, Guid destination, string name)
        {
            this.guid = guid;
            this.destination = destination;
            this.name = name;
        }

        public ReplicationEntry(System.Data.DataRow row)
        {
            this.guid = (Guid)row["GUID"];
            this.destination = (Guid)row["Destination"];
            this.name = row["Name"].ToString();
        }

        public static List<ReplicationEntry> GetEntrys()
        {
            List<ReplicationEntry> result = new List<ReplicationEntry>();

            var ds = AMTANGEE.DB.Select("select R.GUID, R.DESTINATION, RL.NAME from ReplicationLocations RL right join Replications R on RL.GUID = R.DESTINATION where RL.ISOWNLOCATION <> 1 order by RL.NAME");

            if (ds != null)
                foreach (System.Data.DataRow row in ds.Tables[0].Rows)
                    result.Add(new ReplicationEntry(row));

            return result;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
