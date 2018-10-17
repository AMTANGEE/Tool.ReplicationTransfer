using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tool.ReplicationTransfer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            var con = new AMTANGEE.Custom.ConnectionForm(true);

            if (con.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                System.Diagnostics.Process.GetCurrentProcess().Kill();

            try
            {
                AMTANGEE.DB.Open(con.ConnectionString);
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show(
                    "Fehler beim Verbinden!\r\nSiehe Log für mehr Informationen.",
                    "SQL Fehler",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }


            AMTANGEE.DB.Open();

            IcCopyFrom.SetReplications(ReplicationEntry.GetEntrys());
            IcTransferTo.SetReplications(ReplicationEntry.GetEntrys());
        }

        public void Transfer()
        {
            ReplicationData quelle;
            if (IcCopyFrom.Destination == InputControl.DestinationTypes.Replication)
            {
                ReplicationEntry re = IcCopyFrom.SelectedReplication;
                quelle = new ReplicationData(re.Destination, re.GUID, ChkBTransferUser.IsChecked ?? false, ChkBTransferSettings.IsChecked ?? false);
            }
            else
                quelle = ReplicationData.FromXML(IcCopyFrom.SelectedPath);

            if (IcTransferTo.Destination == InputControl.DestinationTypes.File)
            {
                quelle.SaveToXML(IcTransferTo.SelectedPath);
                MessageBox.Show("Freigaben in Datei gespeichert!");
            }
            else
            {
                ReplicationEntry ziel = IcTransferTo.SelectedReplication;

                //Objekte übertragen
                foreach (ReplicationObject cur in quelle.Objects)
                {
                    string command = "if not exists (select [Object] from ReplicationsObjects where [replication] = '" + ziel.GUID + "' and [object] = '" + cur.Object + "') begin " +
                        "insert into ReplicationsObjects values ('" + ziel.GUID + "','" + cur.Object + "',1," + Convert.ToInt32(cur.Subcategories) + ") end else begin " +
                        "update ReplicationsObjects set Subcategories = " + Convert.ToInt32(cur.Subcategories) + " where [replication] = '" + ziel.GUID + "' and [object] = '" + cur.Object + "' end";
                    AMTANGEE.DB.Exec(command);
                }

                //Nutzer übertragen, falls gewünscht
                if (ChkBTransferUser.IsChecked ?? false)
                {
                    foreach (ReplicationUser cur in quelle.User)
                    {
                        string command = "if not exists (select [user] from ReplicationsUsers  where [location] = '" + ziel.Destination + "' and [user] = '" + cur.UserGroup + "') begin " +
                               "insert into ReplicationsUsers values ('" + Guid.NewGuid() + "','" + ziel.Destination + "','" + cur.UserGroup + "',1,'" + cur.Username + "','" + cur.Password + "','','') end else begin " +
                               "update ReplicationsUsers set Username = '" + cur.Username + "', Password = '" + cur.Password + "' where [location] = '" + ziel.Destination + "' and [user] = '" + cur.UserGroup + "' end";
                        AMTANGEE.DB.Exec(command);
                    }
                }

                //Einstellungen übertragen, falls gewünscht
                if (ChkBTransferSettings.IsChecked ?? false)
                    AMTANGEE.DB.Update("UPDATE [REPLICATIONS] SET SETTINGS = '" + quelle.Settings + "' where [GUID] = '" + quelle.ReplicationGUID + "'");

                MessageBox.Show("Freigaben übertragen! Bitte führen Sie für die betroffene Replikation eine Wartung durch.");
            }
        }

        private void BtnTransfer_Click(object sender, RoutedEventArgs e)
        {
            if ((IcCopyFrom.Destination == InputControl.DestinationTypes.Replication && IcCopyFrom.SelectedReplication == null) ||
                (IcCopyFrom.Destination == InputControl.DestinationTypes.File && IcCopyFrom.SelectedPath.Length < 5) ||
                (IcTransferTo.Destination == InputControl.DestinationTypes.Replication && IcTransferTo.SelectedReplication == null) ||
                (IcTransferTo.Destination == InputControl.DestinationTypes.File && IcTransferTo.SelectedPath.Length < 5))
                return;

            Transfer();
        }
    }
}
