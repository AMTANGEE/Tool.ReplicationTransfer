using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMTANGEE.Tools.ReplicationTransfer
{
    public class ReplicationUser
    {
        private Guid usergroup;
        public Guid UserGroup
        {
            get { return usergroup; }
            set { usergroup = value; }
        }

        private string displayname;
        public string Displayname
        {
            get { return displayname; }
            set { displayname = value; }
        }

        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private bool allowLogIn;
        public bool AllowLogIn
        {
            get { return allowLogIn; }
            set { allowLogIn = value; }
        }

        public ReplicationUser() { }

        public ReplicationUser(Guid usergroup, string username, string password)
        {
            this.usergroup = usergroup;
            displayname = AMTANGEE.DB.SelectScalar("select [DISPLAYNAME] from [USERS] where [GUID] = '" + usergroup + "'").ToString();
            this.username = username;
            this.password = password;
        }

        public ReplicationUser(Guid usergroup, string displayname, string username, string password)
        {
            this.usergroup = usergroup;
            this.displayname = displayname;
            this.username = username;
            this.password = password;
        }

        public ReplicationUser(System.Data.DataRow row)
        {
            usergroup = (Guid)row["USER"];
            username = row["USERNAME"].ToString();
            password = row["PASSWORD"].ToString();
            allowLogIn = (bool)row["CANLOGIN"];
        }
    }
}
