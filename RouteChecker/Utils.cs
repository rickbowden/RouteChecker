using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteChecker
{
    class Utils
    {
        public static string GetEC2PropFromString(string input, Amazon.EC2.Model.Instance i)
        {
            string result = "";

            input = input.Replace(" ", string.Empty);

            string[] inputArr = input.Split('=');
            if (inputArr.Length > 1)
            {
                if (inputArr[0].ToLower() == "tag")
                {
                    result = GetEC2Tag(i.Tags, inputArr[1]);
                }
            }
            else
            {

                switch (input.ToLower())
                {
                    case "architecture":
                        result = i.Architecture;
                        break;
                    case "blockdevicemappings":
                        result = "";
                        break;
                    case "ebsoptimized":
                        result = i.EbsOptimized.ToString();
                        break;
                    case "enasupport":
                        result = i.EnaSupport.ToString();
                        break;
                    case "hypervisor":
                        result = i.Hypervisor;
                        break;
                    case "iaminstanceprofile":
                        result = i.IamInstanceProfile.Id;
                        break;                    
                    case "instanceid":
                        result = i.InstanceId;
                        break;
                    case "instancetype":
                        result = i.InstanceType;
                        break;
                    case "kernelid":
                        result = i.KernelId;
                        break;
                    case "keyname":
                        result = i.KeyName;
                        break;
                    case "launchtime":
                        result = i.LaunchTime.ToString("yyyy-MM-dd HH:mm:ss");
                        break;
                    case "monitoring":
                        result = i.Monitoring.State.Value;
                        break;
                    case "networkinterfaces":
                        result = "";
                        break;
                    case "placement":
                        result = "";
                        break;
                    case "platform":
                        result = i.Platform.Value;
                        break;
                    case "privatednsname":
                        result = i.PrivateDnsName;
                        break;
                    case "privateipaddress":
                        result = i.PrivateIpAddress;
                        break;
                    case "publicdnsname":
                        result = i.PublicDnsName;
                        break; 
                    case "publicipaddress":
                        result = i.PublicIpAddress;
                        break;
                    case "rootdevicename":
                        result = i.RootDeviceName;
                        break;
                    case "rootdevicetype":
                        result = i.RootDeviceType.Value;
                        break;
                    case "securitygroups":
                        result = GetEC2SecurityGroups(i.SecurityGroups);
                        break;
                    case "subnetid":
                        result = i.SubnetId;
                        break;
                    case "state":
                        result = i.State.Name;
                        break;
                    case "imageid":
                        result = i.ImageId;
                        break;
                    case "vpcid":
                        result = i.VpcId;
                        break;
                    case "name":
                        result = GetEC2Tag(i.Tags, "name");
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public static string GetEC2Tag(List<Amazon.EC2.Model.Tag> tags, string search)
        {
            string result = "";
            if (tags != null)
            {
                var a = tags.Find(n => n != null && n.Key.ToLower() == search.ToLower());
                if (a != null)
                {
                    result = a.Value;
                }
            }
            return result;
        }

        public static string GetEC2SecurityGroups(List<Amazon.EC2.Model.GroupIdentifier> groups)
        {
            string result = "";
            if (groups != null)
            {
                foreach (var sg in groups)
                {
                    result = result + "," + sg.GroupId;
                }
            }
            if (result.Length > 0)
            {
                return result.Substring(1, result.Length - 1);
            }
            else
            {
                return result;
            }
        }

    }
}
