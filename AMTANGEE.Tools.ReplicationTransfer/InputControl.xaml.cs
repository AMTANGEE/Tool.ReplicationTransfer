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
    /// Interaction logic for InputControl.xaml
    /// </summary>
    public partial class InputControl : UserControl
    {
        public enum DestinationTypes
        {
            Replication,
            File
        }

        private List<ReplicationEntry> _entries;
        private bool isTransferTo = false;
        public bool IsTransferTo
        {
            get => isTransferTo;
            set => isTransferTo = value;
        }

        public ReplicationEntry SelectedReplication => CbReplicationEntry.SelectedIndex >= 0 ?
            (ReplicationEntry) CbReplicationEntry.SelectedItem : null;

        public string SelectedPath => TbXmlPath.Text;

        public DestinationTypes Destination { get; set; } = DestinationTypes.Replication;

        public InputControl()
        {
            InitializeComponent();
        }

        public void SetReplications(List<ReplicationEntry> entries)
        {
            CbReplicationEntry.ItemsSource = _entries = entries;

            CbReplicationEntry.SelectedIndex = -1;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if(TbXmlPath != null)
                TbXmlPath.IsEnabled = BtnSearch.IsEnabled = !(CbReplicationEntry.IsEnabled = RbReplication.IsChecked ?? false);
            Destination = RbReplication.IsChecked ?? false ? DestinationTypes.Replication : DestinationTypes.File;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!isTransferTo)
            {
                var ofd = new Microsoft.Win32.OpenFileDialog();

                ofd.Multiselect = false;
                ofd.Filter = "XML-Datei|*.xml;*.XML|Alle Dateien|*.*";

                if (ofd.ShowDialog() ?? false)
                    TbXmlPath.Text = ofd.FileName;
            }
            else
            {
                var sfd = new Microsoft.Win32.SaveFileDialog();

                sfd.AddExtension = true;
                sfd.DefaultExt = "xml";
                sfd.Filter = "XML-Datei|*.xml;*.XML|Alle Dateien|*.*";

                if (sfd.ShowDialog() ?? false)
                    TbXmlPath.Text = sfd.FileName;
            }
        }
    }
}
