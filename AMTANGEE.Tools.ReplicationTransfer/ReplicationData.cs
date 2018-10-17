using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tool.ReplicationTransfer
{
    public class ReplicationData
    {
        private Guid destination;
        public Guid Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        private Guid replicationGUID;
        public Guid ReplicationGUID
        {
            get { return replicationGUID; }
            set { replicationGUID = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string settings;
        public string Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        public List<ReplicationObject> Objects = new List<ReplicationObject>();
        public List<ReplicationUser> User = new List<ReplicationUser>();

        public ReplicationData() { }

        public ReplicationData(Guid destination)
        {
            this.destination = destination;

            var ds = AMTANGEE.DB.Select("SELECT [GUID],[SETTINGS] FROM [Replications] WHERE [Destination] = '" + destination + "'");

            if(ds != null)
            {
                System.Data.DataRow row = ds.Tables[0].Rows[0];

                this.replicationGUID = (Guid)row["GUID"];
                this.settings = (string)row["SETTINGS2"];
            }

            GetData();
        }

        public ReplicationData(Guid destination, Guid replicationGUID)
        {
            this.destination = destination;
            this.replicationGUID = replicationGUID;
            this.settings = AMTANGEE.DB.SelectScalar("SELECT [SETTINGS] FROM [Replications] WHERE [Destination] = '" + destination + "'").ToString();

            GetData();
        }

        public ReplicationData(Guid destination, Guid replicationGUID, bool copyUser = true, bool copySettings = true)
        {
            this.destination = destination;
            this.replicationGUID = replicationGUID;
            if (copySettings)
                this.settings = AMTANGEE.DB.SelectScalar("SELECT [SETTINGS] FROM [Replications] WHERE [Destination] = '" + destination + "'").ToString();

            GetData(copyUser);
        }

        private void GetData(bool getUser = true)
        {
            var ds1 = AMTANGEE.DB.Select("SELECT [OBJECT],[SUBCATEGORIES] FROM ReplicationsObjects WHERE [REPLICATION] = '" + replicationGUID + "'");

            if(ds1 != null)
                foreach (System.Data.DataRow row in ds1.Tables[0].Rows)
                    Objects.Add(new ReplicationObject(row));

            if (!getUser)
                return;

            var ds2 = AMTANGEE.DB.Select("SELECT [USER],[CANLOGIN],[USERNAME],[PASSWORD] FROM ReplicationsUsers WHERE [LOCATION] = '" + destination + "'");
            if(ds2 != null)
                foreach (System.Data.DataRow row in ds2.Tables[0].Rows)
                    User.Add(new ReplicationUser(row));
        }        

        public void SaveToXML(string path)
        {
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(ReplicationData));

            using (System.IO.TextWriter tw = new System.IO.StreamWriter(path))
                xs.Serialize(tw, this);
        }

        public static ReplicationData FromXML(string path)
        {
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(ReplicationData));

            using (var sr = new System.IO.StreamReader(path))
                return (ReplicationData)xs.Deserialize(sr);
        }
    }
}
