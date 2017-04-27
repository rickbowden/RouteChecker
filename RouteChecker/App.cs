using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Amazon;
using Amazon.Runtime;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.EC2.Util;
using System.Collections.Specialized;
using System.Net;

namespace RouteChecker
{
    public partial class App : Form
    {

        static AWSCredentials creds;
        static Dictionary<string, string> MfaDevices = new Dictionary<string, string>();
        public static string mfacode = string.Empty;
        static DateTime mfaExpires;
        static string UpdateUrl = Properties.Settings.Default.UpdateUrl;
        static double ThisVersion = Convert.ToDouble(Properties.Settings.Default.Version);
        static string UserName = Environment.UserName;
        static string AppName = "RouteChecker";

        //static bool CheckLineByLine = false; // Used to determine whether to check only line by line or to check every source against every destination for every port.
             

        List<Instance> SourceInstances = new List<Instance>();
        List<Instance> DestInstances = new List<Instance>();
        List<SecurityGroup> SecGroups;
                
        
        public App()
        {
            InitializeComponent();

            try
            {   
                
                if (Properties.Settings.Default.UpgradeRequired)
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.UpgradeRequired = false;
                    Properties.Settings.Default.Save();
                }

                PopulateMfaDevicesDic();
                PopulateRegionDropDown();
                PopulateProfileDropDown();
                PopulateRoleDropDown();
                

                //Set default region
                AWSConfigs.AWSRegion = Properties.Settings.Default.AWSDefaultRegion;

                CheckForUpdates();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }

        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form options = new Options();
                options.ShowDialog();
                //Reload role dropdown
                PopulateRoleDropDown();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void AWSRegion_CBB_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void AssumeRole_CBB_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Submit_BTN_Click(object sender, EventArgs e)
        {
            if (Profile_CBB.Text == "select a profile") { return; }
            try
            {
                //Set region
                AWSConfigs.AWSRegion = AWSRegion_CBB.Text;

                //Set Creds
                SetCreds();

                //Reset Grid                 
                Inbound_DataGrid.Rows.Clear();
                Outbound_DataGrid.Rows.Clear();

                //Clear Lists
                //SourceInstances.Clear();
                //DestInstances.Clear();
                
                Submit_BTN.Enabled = false;

                // Run In Background
                backgroundWorker1.RunWorkerAsync();
                
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
                Submit_BTN.Enabled = true;
            }
        }

        void DisplayError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }

        void PopulateMfaDevicesDic()
        {
            if (MfaDevices != null)
            {
                MfaDevices.Clear();
            }
            foreach (string item in Properties.Settings.Default.MFADevices)
            {
                string[] items = item.Split('|');
                if (items.Length == 2)
                {
                    MfaDevices.Add(items[0], items[1]);
                }
            }
        }

        void PopulateRegionDropDown()
        {
            AWSRegion_CBB.Items.Clear();
            foreach (string s in Properties.Settings.Default.AWSRegion)
            {
                AWSRegion_CBB.Items.Add(s);
            }
            if (Properties.Settings.Default.AWSDefaultRegion != null)
            {
                AWSRegion_CBB.Text = Properties.Settings.Default.AWSDefaultRegion;
            }
        }

        void PopulateProfileDropDown()
        {
            Profile_CBB.Items.Clear();
            foreach (var item in Amazon.Util.ProfileManager.ListProfiles())
            {
                Profile_CBB.Items.Add(item.Name);
            }
        }

        void PopulateRoleDropDown()
        {
            AssumeRole_CBB.Items.Clear();
            AssumeRole_CBB.Sorted = true;
            foreach (string role in Properties.Settings.Default.Roles)
            {
                string[] a = role.Split('|');
                if (a.Length > 1)
                {
                    ComboboxItem item = new ComboboxItem();
                    item.Name = a[0];
                    item.Role = a[1];
                    AssumeRole_CBB.Items.Add(item);
                }
            }
            AssumeRole_CBB.Sorted = false;
            ComboboxItem item0 = new ComboboxItem();
            item0.Name = "none";
            item0.Role = "none";
            AssumeRole_CBB.Items.Insert(0, item0);
        }

        private void SetCreds()
        {
            if (creds == null || DateTime.Now >= mfaExpires || AWSConfigs.AWSProfileName != Profile_CBB.Text)
            {
                AWSConfigs.AWSProfileName = Profile_CBB.Text;
                creds = new StoredProfileAWSCredentials(Profile_CBB.Text);


                if (AssumeRole_CBB.Text != "none")
                {
                    //Get selected role to assume
                    ComboboxItem o = (ComboboxItem)AssumeRole_CBB.SelectedItem;
                    //Create AssumeRole request
                    Amazon.SecurityToken.Model.AssumeRoleRequest assumeRequest = new Amazon.SecurityToken.Model.AssumeRoleRequest();
                    assumeRequest.RoleArn = o.Role;
                    assumeRequest.RoleSessionName = UserName + "@" + AppName;

                    //Get MFA Device
                    Amazon.IdentityManagement.AmazonIdentityManagementServiceClient imc = new Amazon.IdentityManagement.AmazonIdentityManagementServiceClient(creds);
                    Amazon.IdentityManagement.Model.ListMFADevicesRequest mfaRequest = new Amazon.IdentityManagement.Model.ListMFADevicesRequest();
                    Amazon.IdentityManagement.Model.ListMFADevicesResponse mfaResponse = imc.ListMFADevices(mfaRequest);
                    if (mfaResponse != null)
                    {
                        if (mfaResponse.MFADevices.Count > 0)
                        {
                            assumeRequest.SerialNumber = mfaResponse.MFADevices[0].SerialNumber;
                        }
                    }

                    //If MFA Device was not obtained
                    if (assumeRequest.SerialNumber == string.Empty)
                    {
                        //Get mfa associated with selected profile
                        if (MfaDevices.ContainsKey(Profile_CBB.Text))
                        {
                            assumeRequest.SerialNumber = MfaDevices[Profile_CBB.Text];
                        }
                        else
                        {
                            assumeRequest.SerialNumber = MfaDevices["default"];
                        }
                    }


                    //Get MFA code
                    mfa m = new mfa();
                    m.ShowDialog();
                    assumeRequest.TokenCode = mfacode.Trim(); //MFA code

                    Amazon.SecurityToken.AmazonSecurityTokenServiceClient secClient = new Amazon.SecurityToken.AmazonSecurityTokenServiceClient();
                    Amazon.SecurityToken.Model.AssumeRoleResponse assumeResponse = secClient.AssumeRole(assumeRequest);

                    mfaExpires = assumeResponse.Credentials.Expiration;

                    creds = assumeResponse.Credentials;
                }
            }
        }



        void CheckForUpdates()
        {
            try
            {
                backgroundWorkerUpdate.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                DisplayError("Error during update check." + Environment.NewLine + ex.Message);
            }
        }
        private void backgroundWorkerUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                rab_update.Version v = new rab_update.Version(Properties.Settings.Default.Version, Properties.Settings.Default.UpdateUrl);
                v.CheckForNewVersion();
                e.Result = v;
            }
            catch (Exception ex)
            {
                DisplayError("Error during update check." + Environment.NewLine + ex.Message);
            }

        }
        private void backgroundWorkerUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is rab_update.Version)
            {
                rab_update.Version v = e.Result as rab_update.Version;
                if (v.UpdateAvailable)
                {
                    rab_update.UpdateForm uf = new rab_update.UpdateForm(v.NewVersionString, v.CurrentVersion, v.DownloadUrl, AppName);
                    uf.ShowDialog();
                }
            }
        }


        
        static List<Filter> BuildFilter(string input)
        {
            List<Filter> filterList = new List<Filter>();

            
            Filter f = new Filter();
            IPAddress ipOut;
            if (IPAddress.TryParse(input, out ipOut))
            {
                f.Name = "private-ip-address";
                f.Values = new List<string> { input };
            }
            else if (input.StartsWith("i-"))
            {
                f.Name = "instance-id";
                f.Values = new List<string> { input };
            }
            else
            {
                f.Name = "tag:Name";
                f.Values = new List<string> { input };
            }
            filterList.Add(f);
                       

            return filterList;
        }

        DescribeInstancesResponse GetInstances(List<Filter> searchFilters)
        {
            DescribeInstancesResponse result = null;

            using (AmazonEC2Client EC2client = new AmazonEC2Client(creds))
            {
                DescribeInstancesRequest rq = new DescribeInstancesRequest();
                rq.Filters = searchFilters;
                if (searchFilters[0].Values[0] == "")
                {
                    
                }
                else
                {
                    result = EC2client.DescribeInstances(rq);
                }
            }

            return result;
        }

        void PopulateInstanceLists(string instance, ref List<Instance> instanceList)
        {
            if (instance != null && instance != "")
            {
                DescribeInstancesResponse response = GetInstances(BuildFilter(instance));
                Instance i = new Instance();
                if (response.Reservations.Count == 0)
                {
                    i.IsInstance = false;
                    i.IpAddress = instance;
                    i.Name = "External IP";
                }
                else
                {
                    i.IsInstance = true;
                    i.IpAddress = response.Reservations[0].Instances[0].PrivateIpAddress;
                    i.InstanceId = response.Reservations[0].Instances[0].InstanceId;
                    i.SecurityGroups = response.Reservations[0].Instances[0].SecurityGroups;
                    i.Name = (Utils.GetEC2PropFromString("name", response.Reservations[0].Instances[0]));
                }
                instanceList.Add(i); 
            }
        }

        DescribeSecurityGroupsResponse GetSecurityGroups()
        {
            DescribeSecurityGroupsResponse result = null;

            using (AmazonEC2Client EC2client = new AmazonEC2Client(creds))
            {
                DescribeSecurityGroupsRequest rq = new DescribeSecurityGroupsRequest();
                result = EC2client.DescribeSecurityGroups();
            }

            return result;
        }




        //void Analyse()
        //{
        //    int i = 0;
        //    foreach (string port in Port_TB.Lines)
        //    {
        //        int p = Convert.ToInt32(port);
        //        bool found = false;
                
                

        //        // If this is an instance, not just an IP
        //        if (SourceInstances[i].IsInstance)
        //        {

        //            // using port, check if security groups allow outbound.
        //            foreach (GroupIdentifier sgi in SourceInstances[i].SecurityGroups)
        //            {
        //                //if (found) { continue; }
        //                SecurityGroup sg = SecGroups.Find(x => x.GroupId == sgi.GroupId);
        //                if (sg != null)
        //                {
        //                    foreach (IpPermission perm in sg.IpPermissionsEgress)
        //                    {
                                
        //                        // Allow all/all 0.0.0.0/0 rule
        //                        if (perm.IpProtocol == "-1" && perm.FromPort == 0 && perm.ToPort == 0)
        //                        {
        //                            foreach (String range in perm.IpRanges)
        //                            {
        //                                DataGridViewRow row = new DataGridViewRow();
        //                                int rowNumber = dataGridView1.Rows.Add(row);
        //                                dataGridView1.Rows[rowNumber].Cells["Port"].Value = port;
        //                                dataGridView1.Rows[rowNumber].Cells["Source"].Value = Source_TB.Lines[i];
        //                                dataGridView1.Rows[rowNumber].Cells["Destination"].Value = Dest_TB.Lines[i];
        //                                dataGridView1.Rows[rowNumber].Cells["SGOutbound"].Value = sg.GroupId;

        //                                if (range == "0.0.0.0/0")
        //                                {
        //                                    dataGridView1.Rows[rowNumber].Cells["SGOutboundResult"].Value = "Allow All";
        //                                    dataGridView1.Rows[rowNumber].Cells["SGOutboundResult"].Style.BackColor = System.Drawing.Color.LightGreen;
        //                                }


        //                            }
        //                                //found = true;
        //                            //continue;
        //                        }// If port is in range
        //                        else if (perm.FromPort >= p && perm.ToPort <= p)
        //                        {

        //                            // Check IP ranges
        //                            foreach (String range in perm.IpRanges)
        //                            {
        //                                //if (found) { continue; }
        //                                DataGridViewRow row = new DataGridViewRow();
        //                                int rowNumber = dataGridView1.Rows.Add(row);
        //                                dataGridView1.Rows[rowNumber].Cells["Port"].Value = port;
        //                                dataGridView1.Rows[rowNumber].Cells["Source"].Value = Source_TB.Lines[i];
        //                                dataGridView1.Rows[rowNumber].Cells["Destination"].Value = Dest_TB.Lines[i];
        //                                dataGridView1.Rows[rowNumber].Cells["SGOutbound"].Value = sg.GroupId;
        //                                // If ip is in cidr range
        //                                if (IsInCidrRange(DestInstances[i].IpAddress, range))
        //                                {
        //                                    //dataGridView1.Rows[rowNumber].Cells["SGOutbound"].Value = sg.GroupId;
        //                                    dataGridView1.Rows[rowNumber].Cells["SGOutboundResult"].Value = perm.FromPort.ToString() + "-" + perm.ToPort.ToString() + " " + range;
        //                                    dataGridView1.Rows[rowNumber].Cells["SGOutboundResult"].Style.BackColor = System.Drawing.Color.LightGreen;
        //                                    found = true;
        //                                    //continue;
        //                                }
        //                                else
        //                                {
        //                                    dataGridView1.Rows[rowNumber].Cells["SGOutbound"].Value = sg.GroupId;
        //                                    dataGridView1.Rows[rowNumber].Cells["SGOutboundResult"].Value = perm.FromPort.ToString() + "-" + perm.ToPort.ToString() + " " + range;
        //                                }
        //                            }


        //                            // Check SGs
        //                            foreach (UserIdGroupPair pair in perm.UserIdGroupPairs)
        //                            {
        //                                //if (found) { continue; }
        //                                DataGridViewRow row = new DataGridViewRow();
        //                                int rowNumber = dataGridView1.Rows.Add(row);
        //                                dataGridView1.Rows[rowNumber].Cells["Port"].Value = port;
        //                                dataGridView1.Rows[rowNumber].Cells["Source"].Value = Source_TB.Lines[i];
        //                                dataGridView1.Rows[rowNumber].Cells["Destination"].Value = Dest_TB.Lines[i];
        //                                dataGridView1.Rows[rowNumber].Cells["SGOutbound"].Value = sg.GroupId;
        //                                var foundSG = DestInstances[rowNumber].SecurityGroups.Find(x => x.GroupId == pair.GroupId);
        //                                if (foundSG != null)
        //                                {
        //                                    dataGridView1.Rows[rowNumber].Cells["SGOutbound"].Value = sg.GroupId;
        //                                    dataGridView1.Rows[rowNumber].Cells["SGOutboundResult"].Value = perm.FromPort.ToString() + "-" + perm.ToPort.ToString() + " " + pair.GroupId;
        //                                    dataGridView1.Rows[rowNumber].Cells["SGOutboundResult"].Style.BackColor = System.Drawing.Color.LightGreen;
        //                                    found = true;
        //                                    //continue;
        //                                }
        //                            }

        //                        }
        //                        else
        //                        {
        //                            foreach (String range in perm.IpRanges)
        //                            {
        //                                DataGridViewRow row = new DataGridViewRow();
        //                                int rowNumber = dataGridView1.Rows.Add(row);
        //                                dataGridView1.Rows[rowNumber].Cells["Port"].Value = port;
        //                                dataGridView1.Rows[rowNumber].Cells["Source"].Value = Source_TB.Lines[i];
        //                                dataGridView1.Rows[rowNumber].Cells["Destination"].Value = Dest_TB.Lines[i];
        //                                dataGridView1.Rows[rowNumber].Cells["SGOutbound"].Value = sg.GroupId;
        //                                dataGridView1.Rows[rowNumber].Cells["SGOutbound"].Value = sg.GroupId;
        //                                dataGridView1.Rows[rowNumber].Cells["SGOutboundResult"].Value = perm.FromPort.ToString() + "-" + perm.ToPort.ToString() + " " + range;
                                        
        //                            }

        //                            foreach (UserIdGroupPair pair in perm.UserIdGroupPairs)
        //                            {
        //                                DataGridViewRow row = new DataGridViewRow();
        //                                int rowNumber = dataGridView1.Rows.Add(row);
        //                                dataGridView1.Rows[rowNumber].Cells["Port"].Value = port;
        //                                dataGridView1.Rows[rowNumber].Cells["Source"].Value = Source_TB.Lines[i];
        //                                dataGridView1.Rows[rowNumber].Cells["Destination"].Value = Dest_TB.Lines[i];
        //                                dataGridView1.Rows[rowNumber].Cells["SGOutbound"].Value = sg.GroupId;
        //                                dataGridView1.Rows[rowNumber].Cells["SGOutbound"].Value = sg.GroupId;
        //                                dataGridView1.Rows[rowNumber].Cells["SGOutboundResult"].Value = perm.FromPort.ToString() + "-" + perm.ToPort.ToString() + " " + pair.GroupId;
        //                                found = true;
                                            
                                        
        //                            }
        //                        }
                                

        //                    }

                            
        //                }
        //            }

        //        }
        //        else // If not an instance, just an IP
        //        {
        //            DataGridViewRow row = new DataGridViewRow();
        //            int rowNumber = dataGridView1.Rows.Add(row);
        //            dataGridView1.Rows[rowNumber].Cells["Port"].Value = port;
        //            dataGridView1.Rows[rowNumber].Cells["Source"].Value = Source_TB.Lines[rowNumber];
        //            dataGridView1.Rows[rowNumber].Cells["Destination"].Value = Dest_TB.Lines[rowNumber];
        //            dataGridView1.Rows[rowNumber].Cells["SGOutbound"].Value = "External IP";
        //        }


        //        i++;
        //    }

        //}


        void BuildGridDisplay()
        {
            
            int looptimes = Math.Max(Math.Max(Source_TB.Lines.Length, Dest_TB.Lines.Length), Port_TB.Lines.Length); 
           
            for (int i = 0; i < looptimes; i++)
            {                
                // Find Source Instance using IP or Name
                Instance sourceInstance;
                if (Source_TB.Lines.Length > i)
                {
                    sourceInstance = SourceInstances.Find(x => x != null && (x.IpAddress == Source_TB.Lines[i] || x.Name == Source_TB.Lines[i]));
                }
                else
                {
                    sourceInstance = SourceInstances.Find(x => x != null && (x.IpAddress == Source_TB.Lines[Source_TB.Lines.Length - 1] || x.Name == Source_TB.Lines[Source_TB.Lines.Length - 1]));
                }
                // Find Destination Instance using IP
                Instance destinationInstance;
                if (Dest_TB.Lines.Length > i)
                {
                    destinationInstance = DestInstances.Find(x => x != null && (x.IpAddress == Dest_TB.Lines[i] || x.Name == Dest_TB.Lines[i]));
                }
                else
                {
                    destinationInstance = DestInstances.Find(x => x != null && (x.IpAddress == Dest_TB.Lines[Dest_TB.Lines.Length - 1] || x.Name == Dest_TB.Lines[Dest_TB.Lines.Length - 1]));
                }

                // If this is an instance, not just an IP
                if (sourceInstance.IsInstance)
                {

                    foreach (GroupIdentifier sgi in sourceInstance.SecurityGroups)
                    {                        
                        SecurityGroup sg = SecGroups.Find(x => x.GroupId == sgi.GroupId);
                        if (sg != null)
                        {
                                                        
                            foreach (IpPermission perm in sg.IpPermissionsEgress)
                            {
                                
                                foreach (String range in perm.IpRanges)
                                {
                                    DataGridViewRow row = new DataGridViewRow();
                                    int rowNumber = Outbound_DataGrid.Rows.Add(row);
                                    if (Protocol_TB.Lines.Length > i) { Outbound_DataGrid.Rows[rowNumber].Cells["Protocol_Out"].Value = Protocol_TB.Lines[i]; } else { Outbound_DataGrid.Rows[rowNumber].Cells["Protocol_Out"].Value = Protocol_TB.Lines[Protocol_TB.Lines .Length- 1]; }
                                    if (Port_TB.Lines.Length > i) { Outbound_DataGrid.Rows[rowNumber].Cells["Port_Out"].Value = Port_TB.Lines[i]; } else { Outbound_DataGrid.Rows[rowNumber].Cells["Port_Out"].Value = Port_TB.Lines[Port_TB.Lines .Length- 1]; }
                                    Outbound_DataGrid.Rows[rowNumber].Cells["Source_Out"].Value = sourceInstance.Name;
                                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceIP_Out"].Value = sourceInstance.IpAddress;
                                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceSG_Out"].Value = sg.GroupId;
                                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceSG_Out"].ToolTipText = sg.GroupName;
                                    if (perm.IpProtocol == "-1")
                                    {
                                        Outbound_DataGrid.Rows[rowNumber].Cells["SourceProtocol_Out"].Value = "All";
                                    }
                                    else
                                    {
                                        Outbound_DataGrid.Rows[rowNumber].Cells["SourceProtocol_Out"].Value = perm.IpProtocol;
                                    }
                                    if (perm.FromPort == 0 && perm.ToPort == 0)
                                    {
                                        Outbound_DataGrid.Rows[rowNumber].Cells["SourcePortRange_Out"].Value = "All";
                                    }
                                    else
                                    {
                                        Outbound_DataGrid.Rows[rowNumber].Cells["SourcePortRange_Out"].Value = perm.FromPort + "-" + perm.ToPort;
                                    }
                                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceCidr_Out"].Value = range;
                                    Outbound_DataGrid.Rows[rowNumber].Cells["Dest_Out"].Value = destinationInstance.Name;
                                    Outbound_DataGrid.Rows[rowNumber].Cells["DestIP_Out"].Value = destinationInstance.IpAddress;   
                                }
                                 
                                
                                foreach (UserIdGroupPair pair in perm.UserIdGroupPairs)
                                {                                        
                                    DataGridViewRow row = new DataGridViewRow();
                                    int rowNumber = Outbound_DataGrid.Rows.Add(row);
                                    if (Protocol_TB.Lines.Length > i) { Outbound_DataGrid.Rows[rowNumber].Cells["Protocol_Out"].Value = Protocol_TB.Lines[i]; } else { Outbound_DataGrid.Rows[rowNumber].Cells["Protocol_Out"].Value = Protocol_TB.Lines[Protocol_TB.Lines.Length - 1]; }
                                    if (Port_TB.Lines.Length > i) { Outbound_DataGrid.Rows[rowNumber].Cells["Port_Out"].Value = Port_TB.Lines[i]; } else { Outbound_DataGrid.Rows[rowNumber].Cells["Port_Out"].Value = Port_TB.Lines[Port_TB.Lines.Length - 1]; }
                                    Outbound_DataGrid.Rows[rowNumber].Cells["Source_Out"].Value = sourceInstance.Name;
                                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceIP_Out"].Value = sourceInstance.IpAddress;
                                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceSG_Out"].Value = sg.GroupId;
                                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceSG_Out"].ToolTipText = sg.GroupName;
                                    if (perm.IpProtocol == "-1")
                                    {
                                        Outbound_DataGrid.Rows[rowNumber].Cells["SourceProtocol_Out"].Value = "All";
                                    }
                                    else
                                    {
                                        Outbound_DataGrid.Rows[rowNumber].Cells["SourceProtocol_Out"].Value = perm.IpProtocol;
                                    }
                                    if (perm.FromPort == 0 && perm.ToPort == 0)
                                    {
                                        Outbound_DataGrid.Rows[rowNumber].Cells["SourcePortRange_Out"].Value = "All";
                                    }
                                    else
                                    {
                                        Outbound_DataGrid.Rows[rowNumber].Cells["SourcePortRange_Out"].Value = perm.FromPort + "-" + perm.ToPort;
                                    }
                                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceCidr_Out"].Value = pair.GroupId; ;
                                    Outbound_DataGrid.Rows[rowNumber].Cells["Dest_Out"].Value = destinationInstance.Name;
                                    Outbound_DataGrid.Rows[rowNumber].Cells["DestIP_Out"].Value = destinationInstance.IpAddress;                                      
                                }                             
                             }
                        }
                    }

                }
                else // If not an instance, just an IP
                {
                    DataGridViewRow row = new DataGridViewRow();
                    int rowNumber = Outbound_DataGrid.Rows.Add(row);
                    if (Protocol_TB.Lines.Length > i) { Outbound_DataGrid.Rows[rowNumber].Cells["Protocol_Out"].Value = Protocol_TB.Lines[i]; } else { Outbound_DataGrid.Rows[rowNumber].Cells["Protocol_Out"].Value = Protocol_TB.Lines[Protocol_TB.Lines.Length -1]; }
                    if (Port_TB.Lines.Length > i) { Outbound_DataGrid.Rows[rowNumber].Cells["Port_Out"].Value = Port_TB.Lines[i]; } else { Outbound_DataGrid.Rows[rowNumber].Cells["Port_Out"].Value = Port_TB.Lines[Port_TB.Lines.Length - 1]; }
                    Outbound_DataGrid.Rows[rowNumber].Cells["Source_Out"].Value = sourceInstance.Name;
                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceIP_Out"].Value = sourceInstance.IpAddress;
                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceSG_Out"].Value = "n/a";
                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceProtocol_Out"].Value = "n/a";
                    Outbound_DataGrid.Rows[rowNumber].Cells["SourcePortRange_Out"].Value = "n/a";
                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceCidr_Out"].Value = "n/a"; ;
                    Outbound_DataGrid.Rows[rowNumber].Cells["Dest_Out"].Value = destinationInstance.Name;
                    Outbound_DataGrid.Rows[rowNumber].Cells["DestIP_Out"].Value = destinationInstance.IpAddress;
                    
                }
                
                

            }

            
            for (int i = 0; i < looptimes; i++)
            {
                // Find Source Instance using IP or Name
                Instance sourceInstance;
                if (Source_TB.Lines.Length > i)
                {
                    sourceInstance = SourceInstances.Find(x => x != null && (x.IpAddress == Source_TB.Lines[i] || x.Name == Source_TB.Lines[i]));
                }
                else
                {
                    sourceInstance = SourceInstances.Find(x => x != null && (x.IpAddress == Source_TB.Lines[Source_TB.Lines.Length - 1] || x.Name == Source_TB.Lines[Source_TB.Lines.Length - 1]));
                }
                // Find Destination Instance using IP                
                Instance destinationInstance;
                if (Dest_TB.Lines.Length > i)
                {
                    destinationInstance = DestInstances.Find(x => x != null && (x.IpAddress == Dest_TB.Lines[i] || x.Name == Dest_TB.Lines[i]));
                }
                else
                {
                    destinationInstance = DestInstances.Find(x => x != null && (x.IpAddress == Dest_TB.Lines[Dest_TB.Lines.Length - 1] || x.Name == Dest_TB.Lines[Dest_TB.Lines.Length - 1]));
                }

                // If this is an instance, not just an IP
                if (destinationInstance.IsInstance)
                {

                    foreach (GroupIdentifier sgi in destinationInstance.SecurityGroups)
                    {
                        SecurityGroup sg = SecGroups.Find(x => x.GroupId == sgi.GroupId);
                        if (sg != null)
                        {
                            foreach (IpPermission perm in sg.IpPermissions)
                            {
                                foreach (String range in perm.IpRanges)
                                {
                                    DataGridViewRow row = new DataGridViewRow();
                                    int rowNumber = Inbound_DataGrid.Rows.Add(row);
                                    if (Protocol_TB.Lines.Length > i) { Inbound_DataGrid.Rows[rowNumber].Cells["Protocol_In"].Value = Protocol_TB.Lines[i]; } else { Inbound_DataGrid.Rows[rowNumber].Cells["Protocol_In"].Value = Protocol_TB.Lines[Protocol_TB.Lines.Length - 1]; }
                                    if (Port_TB.Lines.Length > i) { Inbound_DataGrid.Rows[rowNumber].Cells["Port_In"].Value = Port_TB.Lines[i]; } else { Inbound_DataGrid.Rows[rowNumber].Cells["Port_In"].Value = Port_TB.Lines[Port_TB.Lines .Length- 1]; }
                                    Inbound_DataGrid.Rows[rowNumber].Cells["Source_In"].Value = sourceInstance.Name;
                                    Inbound_DataGrid.Rows[rowNumber].Cells["SourceIP_In"].Value = sourceInstance.IpAddress;
                                    Inbound_DataGrid.Rows[rowNumber].Cells["Dest_In"].Value = destinationInstance.Name;
                                    Inbound_DataGrid.Rows[rowNumber].Cells["DestIP_In"].Value = destinationInstance.IpAddress;
                                    Inbound_DataGrid.Rows[rowNumber].Cells["DestSG_In"].Value = sg.GroupId;
                                    Inbound_DataGrid.Rows[rowNumber].Cells["DestSG_In"].ToolTipText = sg.GroupName;
                                    if (perm.IpProtocol == "-1")
                                    {
                                        Inbound_DataGrid.Rows[rowNumber].Cells["DestProtocol_In"].Value = "All";
                                    }
                                    else
                                    {
                                        Inbound_DataGrid.Rows[rowNumber].Cells["DestProtocol_In"].Value = perm.IpProtocol;
                                    }
                                    if ((perm.FromPort == 0 && perm.ToPort == 0) || (perm.FromPort == -1 && perm.ToPort == -1))
                                    {
                                        Inbound_DataGrid.Rows[rowNumber].Cells["DestPortRange_In"].Value = "All";
                                    }
                                    else
                                    {
                                        Inbound_DataGrid.Rows[rowNumber].Cells["DestPortRange_In"].Value = perm.FromPort + "-" + perm.ToPort;
                                    }
                                    Inbound_DataGrid.Rows[rowNumber].Cells["DestCidr_In"].Value = range;

                                }

                                foreach (UserIdGroupPair pair in perm.UserIdGroupPairs)
                                {
                                    DataGridViewRow row = new DataGridViewRow();
                                    int rowNumber = Inbound_DataGrid.Rows.Add(row);
                                    if (Protocol_TB.Lines.Length > i) { Inbound_DataGrid.Rows[rowNumber].Cells["Protocol_In"].Value = Protocol_TB.Lines[i]; } else { Inbound_DataGrid.Rows[rowNumber].Cells["Protocol_In"].Value = Protocol_TB.Lines[Protocol_TB.Lines.Length - 1]; }
                                    if (Port_TB.Lines.Length > i) { Inbound_DataGrid.Rows[rowNumber].Cells["Port_In"].Value = Port_TB.Lines[i]; } else { Inbound_DataGrid.Rows[rowNumber].Cells["Port_In"].Value = Port_TB.Lines[Port_TB.Lines.Length - 1]; }
                                    Inbound_DataGrid.Rows[rowNumber].Cells["Source_In"].Value = sourceInstance.Name;
                                    Inbound_DataGrid.Rows[rowNumber].Cells["SourceIP_In"].Value = sourceInstance.IpAddress;
                                    Inbound_DataGrid.Rows[rowNumber].Cells["Dest_In"].Value = destinationInstance.Name;
                                    Inbound_DataGrid.Rows[rowNumber].Cells["DestIP_In"].Value = destinationInstance.IpAddress;
                                    Inbound_DataGrid.Rows[rowNumber].Cells["DestSG_In"].Value = sg.GroupId;
                                    Inbound_DataGrid.Rows[rowNumber].Cells["DestSG_In"].ToolTipText = sg.GroupName;
                                    if (perm.IpProtocol == "-1")
                                    {
                                        Inbound_DataGrid.Rows[rowNumber].Cells["DestProtocol_In"].Value = "All";
                                    }
                                    else
                                    {
                                        Inbound_DataGrid.Rows[rowNumber].Cells["DestProtocol_In"].Value = perm.IpProtocol;
                                    }
                                    if ((perm.FromPort == 0 && perm.ToPort == 0) || (perm.FromPort == -1 && perm.ToPort == -1))
                                    {
                                        Inbound_DataGrid.Rows[rowNumber].Cells["DestPortRange_In"].Value = "All";
                                    }
                                    else
                                    {
                                        Inbound_DataGrid.Rows[rowNumber].Cells["DestPortRange_In"].Value = perm.FromPort + "-" + perm.ToPort;
                                    }
                                    Inbound_DataGrid.Rows[rowNumber].Cells["DestCidr_In"].Value = pair.GroupId;
                                }
                            }
                        }

                    }

                }
                else
                {
                    DataGridViewRow row = new DataGridViewRow();
                    int rowNumber = Inbound_DataGrid.Rows.Add(row);
                    if (Protocol_TB.Lines.Length > i) { Inbound_DataGrid.Rows[rowNumber].Cells["Protocol_In"].Value = Protocol_TB.Lines[i]; } else { Inbound_DataGrid.Rows[rowNumber].Cells["Protocol_In"].Value = Protocol_TB.Lines[Protocol_TB.Lines.Length - 1]; }
                    if (Port_TB.Lines.Length > i) { Inbound_DataGrid.Rows[rowNumber].Cells["Port_In"].Value = Port_TB.Lines[i]; } else { Inbound_DataGrid.Rows[rowNumber].Cells["Port_In"].Value = Port_TB.Lines[Port_TB.Lines.Length - 1]; }
                    Inbound_DataGrid.Rows[rowNumber].Cells["Source_In"].Value = sourceInstance.Name;
                    Inbound_DataGrid.Rows[rowNumber].Cells["SourceIP_In"].Value = sourceInstance.IpAddress;
                    Inbound_DataGrid.Rows[rowNumber].Cells["Dest_In"].Value = destinationInstance.Name;
                    Inbound_DataGrid.Rows[rowNumber].Cells["DestIP_In"].Value = destinationInstance.IpAddress;
                    Inbound_DataGrid.Rows[rowNumber].Cells["DestSG_In"].Value = "n/a";
                    Inbound_DataGrid.Rows[rowNumber].Cells["DestProtocol_In"].Value = "n/a";
                    Inbound_DataGrid.Rows[rowNumber].Cells["DestPortRange_In"].Value = "n/a";
                    Inbound_DataGrid.Rows[rowNumber].Cells["DestCidr_In"].Value = "n/a";

                }
                                
            }


            //var y = System.Drawing.Color.Gainsboro;
            //var z = System.Drawing.Color.DarkGray;
            //bool yInuse = true;

            //string a = null;
            //string b = null;
            //for (int i = 0; i < Outbound_DataGrid.Rows.Count -1; i++)
            //{
                
            //    a = Outbound_DataGrid.Rows[i].Cells["SourceSG_Out"].Value.ToString();
                
            //    if (b != null)
            //    {
            //        if (a != b && yInuse)
            //        {
            //            Outbound_DataGrid.Rows[i].Cells["SourceSG_Out"].Style.BackColor = z;
            //            yInuse = false;
            //        }
            //        else if (a != b && yInuse == false)
            //        {
            //            Outbound_DataGrid.Rows[i].Cells["SourceSG_Out"].Style.BackColor = y;
            //            yInuse = true;
            //        }
            //        else if (a == b && yInuse)
            //        {
            //            Outbound_DataGrid.Rows[i].Cells["SourceSG_Out"].Style.BackColor = y;
            //            yInuse = true;
            //        }
            //        else if (a == b && yInuse == false)
            //        {
            //            Outbound_DataGrid.Rows[i].Cells["SourceSG_Out"].Style.BackColor = z;
            //            yInuse = false;
            //        }
            //    }
            //    else
            //    {
            //        Outbound_DataGrid.Rows[i].Cells["SourceSG_Out"].Style.BackColor = y;
            //    }
            //    b = Outbound_DataGrid.Rows[i].Cells["SourceSG_Out"].Value.ToString();

            //}

        }


        void AnalyzeOutbound()
        {
            for (int i = 0; i < Outbound_DataGrid.Rows.Count; i++)
            {
                bool ipMatch = false;
                bool portMatch = false;
                bool protocolMatch = false;
                string comment = "";

                // Reset Colours
                Outbound_DataGrid.Rows[i].Cells["SourcePortRange_Out"].Style.BackColor = System.Drawing.Color.White;
                Outbound_DataGrid.Rows[i].Cells["SourceProtocol_Out"].Style.BackColor = System.Drawing.Color.White;
                Outbound_DataGrid.Rows[i].Cells["SourceCidr_Out"].Style.BackColor = System.Drawing.Color.White;

                
                if (Outbound_DataGrid.Rows[i].Cells[0].Value != null)
                {
                    string Protocol = Outbound_DataGrid.Rows[i].Cells["Protocol_Out"].Value.ToString();
                    string SGProtocol = Outbound_DataGrid.Rows[i].Cells["SourceProtocol_Out"].Value.ToString();
                    string DestIP = Outbound_DataGrid.Rows[i].Cells["DestIP_Out"].Value.ToString();
                    string SGCidr = Outbound_DataGrid.Rows[i].Cells["SourceCidr_Out"].Value.ToString();
                    string DestInstance = Outbound_DataGrid.Rows[i].Cells["Dest_Out"].Value.ToString();
                    string Port = Outbound_DataGrid.Rows[i].Cells["Port_Out"].Value.ToString();
                    string SGPortRange = Outbound_DataGrid.Rows[i].Cells["SourcePortRange_Out"].Value.ToString();


                    // Check if Eternal IP (not an instance)
                    if (Outbound_DataGrid.Rows[i].Cells["Source_Out"].Value.ToString() == "External IP")
                    {
                        Outbound_DataGrid.Rows[i].Cells["Comments_Out"].Value = "External IP. No Security Group to analyze";
                        // Skip the rest of this loop
                        continue;
                    }
                    
                    // Look for IP/Group/Source Match
                    if (SGCidr.StartsWith("sg-"))
                    {
                        var a = DestInstances.Find(x => x != null && x.Name == DestInstance);
                        if (a != null)
                        {
                            var b = a.SecurityGroups.Find(x => x != null && x.GroupId == SGCidr);
                            if (b != null)
                            {
                                ipMatch = true;
                            }
                        }
                    }
                    else if (IsInCidrRange(DestIP, SGCidr))
                    {
                        ipMatch = true;
                    }
                    else if (SGCidr == "0.0.0.0/0")
                    {
                        ipMatch = true;
                    }

                    // Look for Port Match
                    if (SGPortRange == "All" || Port == "-1")
                    {
                        portMatch = true;
                    }
                    else
                    {
                        string[] a = SGPortRange.Split('-');
                        if (a.Length == 2)
                        {
                            if (Convert.ToInt32(a[0]) <= Convert.ToInt32(Port) && Convert.ToInt32(a[1]) >= Convert.ToInt32(Port))
                            {
                                portMatch = true;
                            }
                        }
                    }
                    
                    // Look for Protocol Match
                    if (SGProtocol == "All" || SGProtocol == Protocol)
                    {
                        protocolMatch = true;
                    }


                    // Colour Cells
                    if (protocolMatch)
                    {
                        Outbound_DataGrid.Rows[i].Cells["SourceProtocol_Out"].Style.BackColor = System.Drawing.Color.Yellow;
                        comment += "Protocol matches, ";
                    }
                    if (portMatch)
                    {
                        Outbound_DataGrid.Rows[i].Cells["SourcePortRange_Out"].Style.BackColor = System.Drawing.Color.Yellow;
                        comment += "Port matches, ";
                    }
                    if (ipMatch)
                    {
                        Outbound_DataGrid.Rows[i].Cells["SourceCidr_Out"].Style.BackColor = System.Drawing.Color.Yellow;
                        comment += "Source matches, ";
                    }
                    if (protocolMatch && portMatch && ipMatch)
                    {
                        Outbound_DataGrid.Rows[i].Cells["SourcePortRange_Out"].Style.BackColor = System.Drawing.Color.LightGreen;
                        Outbound_DataGrid.Rows[i].Cells["SourceProtocol_Out"].Style.BackColor = System.Drawing.Color.LightGreen;
                        Outbound_DataGrid.Rows[i].Cells["SourceCidr_Out"].Style.BackColor = System.Drawing.Color.LightGreen;
                    }

                    if (comment.Length > 0)
                    {
                        Outbound_DataGrid.Rows[i].Cells["Comments_Out"].Value = comment.Remove(comment.Length - 2);
                    }
                }
            }


            Outbound_DataGrid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }


        void AnalyzeInbound()
        {
            for (int i = 0; i < Inbound_DataGrid.Rows.Count; i++)
            {
                bool ipMatch = false;
                bool portMatch = false;
                bool protocolMatch = false;
                string comment = "";


                // Reset Colours
                Inbound_DataGrid.Rows[i].Cells["DestPortRange_In"].Style.BackColor = System.Drawing.Color.White;
                Inbound_DataGrid.Rows[i].Cells["DestProtocol_In"].Style.BackColor = System.Drawing.Color.White;
                Inbound_DataGrid.Rows[i].Cells["DestCidr_In"].Style.BackColor = System.Drawing.Color.White;


                if (Inbound_DataGrid.Rows[i].Cells[0].Value != null)
                {
                    string Protocol = Inbound_DataGrid.Rows[i].Cells["Protocol_In"].Value.ToString();
                    string SGProtocol = Inbound_DataGrid.Rows[i].Cells["DestProtocol_In"].Value.ToString();
                    string SourceIP = Inbound_DataGrid.Rows[i].Cells["SourceIP_In"].Value.ToString();
                    string SGCidr = Inbound_DataGrid.Rows[i].Cells["DestCidr_In"].Value.ToString();
                    string SourceInstance = Inbound_DataGrid.Rows[i].Cells["Source_In"].Value.ToString();
                    string Port = Inbound_DataGrid.Rows[i].Cells["Port_In"].Value.ToString();
                    string PortRange = Inbound_DataGrid.Rows[i].Cells["DestPortRange_In"].Value.ToString();


                    // Check if Eternal IP (not an instance)
                    if (Inbound_DataGrid.Rows[i].Cells["Dest_In"].Value.ToString() == "External IP")
                    {
                        Inbound_DataGrid.Rows[i].Cells["Comments_In"].Value = "External IP. No Security Group to analyze";
                        // Skip the rest of this loop
                        continue;
                    }

                                       

                    // Look for IP/Group/Source Match
                    if (SGCidr.StartsWith("sg-"))
                    {
                        var a = SourceInstances.Find(x => x != null && x.Name == SourceInstance);
                        if (a != null)
                        {
                            var b = a.SecurityGroups.Find(x => x != null && x.GroupId == SGCidr);
                            if (b != null)
                            {
                                ipMatch = true;
                            }
                        }
                    }
                    else if (IsInCidrRange(SourceIP, SGCidr))
                    {
                        ipMatch = true;
                    }
                    else if (SGCidr == "0.0.0.0/0")
                    {
                        ipMatch = true;
                    }

                    // Look for Port Match
                    if (PortRange == "All" || Port == "-1")
                    {
                        portMatch = true;
                    }
                    else
                    {
                        string[] a = PortRange.Split('-');
                        if (a.Length == 2)
                        {
                            if (Convert.ToInt32(a[0]) <= Convert.ToInt32(Port) && Convert.ToInt32(a[1]) >= Convert.ToInt32(Port))
                            {
                                portMatch = true;
                            }
                        }
                    }
                    
                    // Look for Protocol Match
                    if (SGProtocol == "All" || SGProtocol == Protocol)
                    {
                        protocolMatch = true;
                    }


                    // Colour Cells
                    if (protocolMatch)
                    {
                        Inbound_DataGrid.Rows[i].Cells["DestProtocol_In"].Style.BackColor = System.Drawing.Color.Yellow;
                        comment += "Protocol matches, ";
                    }
                    if (portMatch)
                    {
                        Inbound_DataGrid.Rows[i].Cells["DestPortRange_In"].Style.BackColor = System.Drawing.Color.Yellow;
                        comment += "Port matches, ";
                    }
                    if (ipMatch)
                    {
                        Inbound_DataGrid.Rows[i].Cells["DestCidr_In"].Style.BackColor = System.Drawing.Color.Yellow;
                        comment += "Source matches, ";
                    }
                    if (protocolMatch && portMatch && ipMatch)
                    {
                        Inbound_DataGrid.Rows[i].Cells["DestPortRange_In"].Style.BackColor = System.Drawing.Color.LightGreen;
                        Inbound_DataGrid.Rows[i].Cells["DestProtocol_In"].Style.BackColor = System.Drawing.Color.LightGreen;
                        Inbound_DataGrid.Rows[i].Cells["DestCidr_In"].Style.BackColor = System.Drawing.Color.LightGreen;
                    }

                    if (comment.Length > 0)
                    {
                        Inbound_DataGrid.Rows[i].Cells["Comments_In"].Value = comment.Remove(comment.Length - 2);
                    }
                
                }
                
            }

            Inbound_DataGrid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

        }


        
        private static bool IsInCidrRange(string ipAddress, string CidrBlock)
        {
            string[] parts = CidrBlock.Split('/');

            if (parts.Count() != 2) { return false; }

            int IP_addr = BitConverter.ToInt32(IPAddress.Parse(parts[0]).GetAddressBytes(), 0);
            int CIDR_addr = BitConverter.ToInt32(IPAddress.Parse(ipAddress).GetAddressBytes(), 0);
            int CIDR_mask = IPAddress.HostToNetworkOrder(-1 << (32 - int.Parse(parts[1])));

            return ((IP_addr & CIDR_mask) == (CIDR_addr & CIDR_mask));
        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {               
            backgroundWorker1.ReportProgress(1, 1);

            if (SecGroups == null)
            {
                DescribeSecurityGroupsResponse secgroups = GetSecurityGroups();
                SecGroups = secgroups.SecurityGroups;
            }
            else if (SecGroups.Count == 0)
            {
                DescribeSecurityGroupsResponse secgroups = GetSecurityGroups();
                SecGroups = secgroups.SecurityGroups;
            }
            
            
            foreach (string item in Source_TB.Lines)
            {
                if (SourceInstances.Find(x => x != null && (x.IpAddress == item || x.Name == item)) == null)
                {
                    PopulateInstanceLists(item.Trim(), ref SourceInstances);
                }
            }

            foreach (string item in Dest_TB.Lines)
            {
                if (DestInstances.Find(x => x != null && (x.IpAddress == item || x.Name == item)) == null)
                {
                    PopulateInstanceLists(item.Trim(), ref DestInstances);
                }
            }
                    
            
            
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch ((int)e.UserState)
            {
                case 0:
                    ProgressBar1.Style = ProgressBarStyle.Continuous;
                    break;
                case 1:
                    ProgressBar1.Style = ProgressBarStyle.Marquee;
                    Status_LB.Text = "Getting Information";
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                DisplayError(e.Error.Message);
            }
            else
            {
                BuildGridDisplay();
            }

            ProgressBar1.Style = ProgressBarStyle.Continuous;
            ProgressBar1.Value = 0;
            Submit_BTN.Enabled = true;            
            Status_LB.Text = "Finished. Security Groups and instance details cached.";
            Cache_BTN.Enabled = true;
        }

        private void App_Load(object sender, EventArgs e)
        {

        }

        

        private void Analyze_BTN_Click(object sender, EventArgs e)
        {
            AnalyzeOutbound();
            AnalyzeInbound();
        }

        private void Port_TB_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Cache_BTN_Click(object sender, EventArgs e)
        {            
            SourceInstances.Clear();
            DestInstances.Clear();
            SecGroups.Clear();
            Cache_BTN.Enabled = false;

            Status_LB.Text = "Cache cleared";
        }



        private void versionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                rab_Base.TextBox tb = new rab_Base.TextBox(600, 800, "Version Info", Properties.Resources.version);
                tb.StartPosition = FormStartPosition.CenterParent;
                tb.Show();
            }
            catch (Exception)
            {

            } 
        }


    }

    // Custom ComboBox Class
    public class ComboboxItem
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }


    // Custom classes to hold the instance object along with it's security group rules
    // If the ip address is not an aws instance then it will still have an object
    // Only allows one subnet so only uses network interface[0].
    public class Instance
    {
        public bool IsInstance { get; set; }
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public string InstanceId { get; set; }
        public string Subnet { get; set; }

        public List<GroupIdentifier> SecurityGroups { get; set; }

        public Instance()
        {
            SecurityGroups = new List<GroupIdentifier>();
        }

    }
    
}
