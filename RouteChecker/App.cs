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

        static bool CheckLineByLine = false; // Used to determine whether to check only line by line or to check every source against every destination for every port.
                
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
                SourceInstances.Clear();
                DestInstances.Clear();
                

                Submit_BTN.Enabled = false;

                


                //foreach (string item in Source_TB.Lines)
                //{
                    backgroundWorker1.RunWorkerAsync();
                //}


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


        void DisplayResults()
        {
            int i = 0;
            foreach (string line in Source_TB.Lines)
            { 
                
                // If this is an instance, not just an IP
                if (SourceInstances[i].IsInstance)
                {
                                        
                    foreach (GroupIdentifier sgi in SourceInstances[i].SecurityGroups)
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
                                    Outbound_DataGrid.Rows[rowNumber].Cells["Port_Out"].Value = Port_TB.Lines[i];
                                    Outbound_DataGrid.Rows[rowNumber].Cells["Source_Out"].Value = SourceInstances[i].Name;
                                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceIP_Out"].Value = SourceInstances[i].IpAddress;
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
                                    Outbound_DataGrid.Rows[rowNumber].Cells["Dest_Out"].Value = DestInstances[i].Name;
                                    Outbound_DataGrid.Rows[rowNumber].Cells["DestIP_Out"].Value = DestInstances[i].IpAddress;   
                                }
                                 
                                
                                foreach (UserIdGroupPair pair in perm.UserIdGroupPairs)
                                {                                        
                                    DataGridViewRow row = new DataGridViewRow();
                                    int rowNumber = Outbound_DataGrid.Rows.Add(row);
                                    Outbound_DataGrid.Rows[rowNumber].Cells["Port_Out"].Value = Port_TB.Lines[i];
                                    Outbound_DataGrid.Rows[rowNumber].Cells["Source_Out"].Value = SourceInstances[i].Name;
                                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceIP_Out"].Value = SourceInstances[i].IpAddress;
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
                                    Outbound_DataGrid.Rows[rowNumber].Cells["Dest_Out"].Value = DestInstances[i].Name;
                                    Outbound_DataGrid.Rows[rowNumber].Cells["DestIP_Out"].Value = DestInstances[i].IpAddress;                                      
                                }                             
                             }
                        }
                    }

                }
                else // If not an instance, just an IP
                {
                    DataGridViewRow row = new DataGridViewRow();
                    int rowNumber = Outbound_DataGrid.Rows.Add(row);
                    Outbound_DataGrid.Rows[rowNumber].Cells["Port_Out"].Value = Port_TB.Lines[i];
                    Outbound_DataGrid.Rows[rowNumber].Cells["Source_Out"].Value = SourceInstances[i].Name;
                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceIP_Out"].Value = SourceInstances[i].IpAddress;
                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceSG_Out"].Value = "n/a";
                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceProtocol_Out"].Value = "n/a";
                    Outbound_DataGrid.Rows[rowNumber].Cells["SourcePortRange_Out"].Value = "n/a";
                    Outbound_DataGrid.Rows[rowNumber].Cells["SourceCidr_Out"].Value = "n/a"; ;
                    Outbound_DataGrid.Rows[rowNumber].Cells["Dest_Out"].Value = DestInstances[i].Name;
                    Outbound_DataGrid.Rows[rowNumber].Cells["DestIP_Out"].Value = DestInstances[i].IpAddress;
                    
                }
                
                i++;

            }

            i = 0;
            foreach (string line in Dest_TB.Lines)
            {
                // If this is an instance, not just an IP
                if (DestInstances[i].IsInstance)
                {

                    foreach (GroupIdentifier sgi in DestInstances[i].SecurityGroups)
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
                                    Inbound_DataGrid.Rows[rowNumber].Cells["Port_In"].Value = Port_TB.Lines[i];
                                    Inbound_DataGrid.Rows[rowNumber].Cells["Source_In"].Value = SourceInstances[i].Name;
                                    Inbound_DataGrid.Rows[rowNumber].Cells["SourceIP_In"].Value = SourceInstances[i].IpAddress;
                                    Inbound_DataGrid.Rows[rowNumber].Cells["Dest_In"].Value = DestInstances[i].Name;
                                    Inbound_DataGrid.Rows[rowNumber].Cells["DestIP_In"].Value = DestInstances[i].IpAddress;
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
                                    Inbound_DataGrid.Rows[rowNumber].Cells["Port_In"].Value = Port_TB.Lines[i];
                                    Inbound_DataGrid.Rows[rowNumber].Cells["Source_In"].Value = SourceInstances[i].Name;
                                    Inbound_DataGrid.Rows[rowNumber].Cells["SourceIP_In"].Value = SourceInstances[i].IpAddress;
                                    Inbound_DataGrid.Rows[rowNumber].Cells["Dest_In"].Value = DestInstances[i].Name;
                                    Inbound_DataGrid.Rows[rowNumber].Cells["DestIP_In"].Value = DestInstances[i].IpAddress;
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

                    Inbound_DataGrid.Rows[rowNumber].Cells["Port_In"].Value = Port_TB.Lines[i];
                    Inbound_DataGrid.Rows[rowNumber].Cells["Source_In"].Value = SourceInstances[i].Name;
                    Inbound_DataGrid.Rows[rowNumber].Cells["SourceIP_In"].Value = SourceInstances[i].IpAddress;
                    Inbound_DataGrid.Rows[rowNumber].Cells["Dest_In"].Value = DestInstances[i].Name;
                    Inbound_DataGrid.Rows[rowNumber].Cells["DestIP_In"].Value = DestInstances[i].IpAddress;
                    Inbound_DataGrid.Rows[rowNumber].Cells["DestSG_In"].Value = "n/a";
                    Inbound_DataGrid.Rows[rowNumber].Cells["DestProtocol_In"].Value = "n/a";
                    Inbound_DataGrid.Rows[rowNumber].Cells["DestPortRange_In"].Value = "n/a";
                    Inbound_DataGrid.Rows[rowNumber].Cells["DestCidr_In"].Value = "n/a";

                }
            }

        }


        void AnalyzeOutbound()
        {
            for (int i = 0; i < Outbound_DataGrid.Rows.Count; i++)
            {
                if (Outbound_DataGrid.Rows[i].Cells[0] != null)
                {
                    string DestIP = Outbound_DataGrid.Rows[i].Cells["DestIP_Out"].Value.ToString();
                    string SGCidr = Outbound_DataGrid.Rows[i].Cells["SourceCidr_Out"].Value.ToString();
                    string DestInstance = Outbound_DataGrid.Rows[i].Cells["Dest_Out"].Value.ToString();

                    if (SGCidr.StartsWith("sg-"))
                    {
                        var a = DestInstances.Find(x => x != null && x.Name == DestInstance);
                        var b = a.SecurityGroups.Find(x => x != null && x.GroupId == SGCidr);
                    }
                    else if (IsInCidrRange(Outbound_DataGrid.Rows[i].Cells["DestIP_Out"].Value.ToString(), Outbound_DataGrid.Rows[i].Cells["SourceCidr_Out"].Value.ToString()))
                    {

                    }
                }
            }

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
                PopulateInstanceLists(item.Trim(), ref SourceInstances);
            }

            foreach (string item in Dest_TB.Lines)
            {
                PopulateInstanceLists(item.Trim(), ref DestInstances);
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
                DisplayResults();
            }

            ProgressBar1.Style = ProgressBarStyle.Continuous;
            ProgressBar1.Value = 0;
            Submit_BTN.Enabled = true;            
            Status_LB.Text = "Finished";
        }

        private void App_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AnalyzeOutbound();
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
    public class Instance
    {
        public bool IsInstance { get; set; }
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public string InstanceId { get; set; }

        public List<GroupIdentifier> SecurityGroups { get; set; }

        public Instance()
        {
            SecurityGroups = new List<GroupIdentifier>();
        }

    }
    
}
