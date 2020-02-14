using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

using FTD2XX_NET;


namespace CSChipID
{
    class EEPROM
    {
        StringBuilder content = new StringBuilder();
        Dictionary<string, FTDI.FT_DEVICE_INFO_NODE> keyProMap= new Dictionary<string, FTDI.FT_DEVICE_INFO_NODE>();
        public void readRom()
        {
            keyProMap.Clear();
            content.Clear();
            UInt32 ftdiDeviceCount = 0;
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;

            // Create new instance of the FTDI device class
            FTDI myFtdiDevice = new FTDI();

            // Determine the number of FTDI devices connected to the machine
            ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            // Check status
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                content.Append("Number of FTDI devices: " + ftdiDeviceCount.ToString() + "\n");
            }
            else
            {
                content.Append("Failed to get number of devices (error " + ftStatus.ToString() + ")" + "\n");
                return ;
            }

            // If no devices available, return
            if (ftdiDeviceCount == 0)
            {
                // Wait for a key press
                content.Append("Failed to get number of devices (error " + ftStatus.ToString() + ")" + "\n");
                return;
            }

            // Allocate storage for device info list
            FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];

            // Populate our device list
            ftStatus = myFtdiDevice.GetDeviceList(ftdiDeviceList);
            int numDevices = 0, ChipID=0;

            FTChipID.ChipID.GetNumDevices(ref numDevices);
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                for (int i = 0; i < ftdiDeviceCount; i++)
                {
                    FTChipID.ChipID.GetDeviceChipID(i, ref ChipID);
               
                    keyProMap.Add("0x" + ChipID.ToString("X"), ftdiDeviceList[i]);
                    content.Append("Device Index: " + i.ToString() + "\n");
                    content.Append("Flags: " + String.Format("{0:x}", ftdiDeviceList[i].Flags) + "\n");
                    content.Append("Type: " + ftdiDeviceList[i].Type.ToString() + "\n");
                    content.Append("ID: " + String.Format("{0:x}", ftdiDeviceList[i].ID) + "\n");
                    content.Append("Location ID: " + String.Format("{0:x}", ftdiDeviceList[i].LocId) + "\n");
                    content.Append("Serial Number: " + ftdiDeviceList[i].SerialNumber.ToString() + "\n");
                    content.Append("Description: " + ftdiDeviceList[i].Description.ToString() + "\n");
                    content.Append("\n");
                }
            }

        }
        public EEPROM()
        {
            
        }

        internal void encode(string SerialNumber,string ChipID,string New)
        {
            content.Clear();


            FTDI.FT_DEVICE_INFO_NODE keyPro;
            content.Clear();
            if (!keyProMap.TryGetValue(ChipID, out keyPro))
            {
                content.Append("Device not found! SerialNumber:" + SerialNumber);
                return;
            }
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            // Create new instance of the FTDI device class
            FTDI myFtdiDevice = new FTDI();
            // Open device in our list by serial number
            ftStatus = myFtdiDevice.OpenBySerialNumber(keyPro.SerialNumber);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                content.Append("Failed to open device (error " + ftStatus.ToString() + ")" + "\n");
                return;
            }
            // get encode
            string encode = getMD5("SANWA" + ChipID + New);
            encode = encode.Substring(0, 8) + encode.Substring(12, 8);

            // Create our device EEPROM structure based on the type of device we have open
            if (keyPro.Type == FTDI.FT_DEVICE.FT_DEVICE_232R)
            {
                // We have an FT232R or FT245R so use FT232R EEPROM structure
                FTDI.FT232R_EEPROM_STRUCTURE myEEData = new FTDI.FT232R_EEPROM_STRUCTURE();
                // Read the device EEPROM
                // This can throw an exception if trying to read a device type that does not 
                // match the EEPROM structure being used, so should always use a 
                // try - catch block when calling
                try
                {
                    ftStatus = myFtdiDevice.ReadFT232REEPROM(myEEData);
                }
                catch (FTDI.FT_EXCEPTION e)
                {
                    content.Append("Exception thrown when calling ReadFT232REEPROM" + "\n");
                }

                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    // Wait for a key press
                    content.Append("Failed to read device EEPROM (error " + ftStatus.ToString() + ")" + "\n");
                    // Close the device
                    myFtdiDevice.Close();
                    return;
                }

                // Change our serial number to write back to device
                // By setting to an empty string, we allow the FTD2XX DLL 
                // to generate a serial number
                myEEData.SerialNumber = New;
                myEEData.Description = encode;

                // Write our modified data structure back to the device EEPROM
                // This can throw an exception if trying to write a device type that does not 
                // match the EEPROM structure being used, so should always use a 
                // try - catch block when calling
                try
                {
                    ftStatus = myFtdiDevice.WriteFT232REEPROM(myEEData);
                }
                catch (FTDI.FT_EXCEPTION)
                {
                    content.Append("Exception thrown when calling WriteFT232REEPROM" + "\n");
                }

                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    // Wait for a key press
                    content.Append("Failed to write device EEPROM (error " + ftStatus.ToString() + ")" + "\n");
                    // Close the device
                    myFtdiDevice.Close();
                    return;
                }

                // Write common EEPROM elements to our console
                content.Append("EEPROM Contents for device at index 0:" + "\n");
                content.Append("Vendor ID: " + String.Format("{0:x}", myEEData.VendorID) + "\n");
                content.Append("Product ID: " + String.Format("{0:x}", myEEData.ProductID) + "\n");
                content.Append("Manufacturer: " + myEEData.Manufacturer.ToString() + "\n");
                content.Append("Manufacturer ID: " + myEEData.ManufacturerID.ToString() + "\n");
                //content.Append("Description: " + myEEData.Description.ToString() + "\n");
                content.Append("Description: " + encode + "\n");
                content.Append("Serial Number: " + myEEData.SerialNumber.ToString() + "\n");
                content.Append("Max Power: " + myEEData.MaxPower.ToString() + "mA" + "\n");
                content.Append("Self Powered: " + myEEData.SelfPowered.ToString() + "\n");
                content.Append("Remote Wakeup Enabled: " + myEEData.RemoteWakeup.ToString() + "\n");
            }
            else if (keyPro.Type == FTDI.FT_DEVICE.FT_DEVICE_2232)
            {
                // We have an FT2232 so use FT2232 EEPROM structure
                FTDI.FT2232_EEPROM_STRUCTURE myEEData = new FTDI.FT2232_EEPROM_STRUCTURE();
                // Read the device EEPROM
                ftStatus = myFtdiDevice.ReadFT2232EEPROM(myEEData);
                // This can throw an exception if trying to read a device type that does not 
                // match the EEPROM structure being used, so should always use a 
                // try - catch block when calling
                try
                {
                    ftStatus = myFtdiDevice.ReadFT2232EEPROM(myEEData);
                }
                catch (FTDI.FT_EXCEPTION)
                {
                    content.Append("Exception thrown when calling ReadFT2232EEPROM" + "\n");
                }

                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    // Wait for a key press
                    content.Append("Failed to read device EEPROM (error " + ftStatus.ToString() + ")" + "\n");
                    // Close the device
                    myFtdiDevice.Close();
                    return;
                }

                // Write common EEPROM elements to our console
                content.Append("EEPROM Contents for device at index 0:" + "\n");
                content.Append("Vendor ID: " + String.Format("{0:x}", myEEData.VendorID) + "\n");
                content.Append("Product ID: " + String.Format("{0:x}", myEEData.ProductID) + "\n");
                content.Append("Manufacturer: " + myEEData.Manufacturer.ToString() + "\n");
                content.Append("Manufacturer ID: " + myEEData.ManufacturerID.ToString() + "\n");
                //content.Append("Description: " + myEEData.Description.ToString() + "\n");
                content.Append("Description: " + encode + "\n");
                content.Append("Serial Number: " + myEEData.SerialNumber.ToString() + "\n");
                content.Append("Max Power: " + myEEData.MaxPower.ToString() + "mA" + "\n");
                content.Append("Self Powered: " + myEEData.SelfPowered.ToString() + "\n");
                content.Append("Remote Wakeup Enabled: " + myEEData.RemoteWakeup.ToString() + "\n");

                // Change our serial number to write back to device
                // By setting to an empty string, we allow the FTD2XX DLL 
                // to generate a serial number
                //myEEData.SerialNumber = String.Empty;
                myEEData.Description = encode;

                // Write our modified data structure back to the device EEPROM
                // This can throw an exception if trying to write a device type that does not 
                // match the EEPROM structure being used, so should always use a 
                // try - catch block when calling
                try
                {
                    ftStatus = myFtdiDevice.WriteFT2232EEPROM(myEEData);
                }
                catch (FTDI.FT_EXCEPTION)
                {
                    content.Append("Exception thrown when calling WriteFT2232EEPROM" + "\n");
                }

                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    content.Append("Failed to write device EEPROM (error " + ftStatus.ToString() + ")" + "\n");
                    // Close the device
                    myFtdiDevice.Close();
                    return;
                }
            }
            else if (keyPro.Type == FTDI.FT_DEVICE.FT_DEVICE_BM)
            {
                // We have an FT232B or FT245B so use FT232B EEPROM structure
                FTDI.FT232B_EEPROM_STRUCTURE myEEData = new FTDI.FT232B_EEPROM_STRUCTURE();
                // Read the device EEPROM
                ftStatus = myFtdiDevice.ReadFT232BEEPROM(myEEData);
                // This can throw an exception if trying to read a device type that does not 
                // match the EEPROM structure being used, so should always use a 
                // try - catch block when calling
                try
                {
                    ftStatus = myFtdiDevice.ReadFT232BEEPROM(myEEData);
                }
                catch (FTDI.FT_EXCEPTION)
                {
                    content.Append("Exception thrown when calling ReadFT232BEEPROM" + "\n");
                }

                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    // Wait for a key press
                    content.Append("Failed to read device EEPROM (error " + ftStatus.ToString() + ")" + "\n");
                    // Close the device
                    myFtdiDevice.Close();
                    return;
                }

                // Write common EEPROM elements to our console
                content.Append("EEPROM Contents for device at index 0:" + "\n");
                content.Append("Vendor ID: " + String.Format("{0:x}", myEEData.VendorID) + "\n");
                content.Append("Product ID: " + String.Format("{0:x}", myEEData.ProductID) + "\n");
                content.Append("Manufacturer: " + myEEData.Manufacturer.ToString() + "\n");
                content.Append("Manufacturer ID: " + myEEData.ManufacturerID.ToString() + "\n");
                //content.Append("Description: " + myEEData.Description.ToString() + "\n");
                content.Append("Description: " + encode + "\n");
                content.Append("Serial Number: " + myEEData.SerialNumber.ToString() + "\n");
                content.Append("Max Power: " + myEEData.MaxPower.ToString() + "mA" + "\n");
                content.Append("Self Powered: " + myEEData.SelfPowered.ToString() + "\n");
                content.Append("Remote Wakeup Enabled: " + myEEData.RemoteWakeup.ToString() + "\n");
                
                // Change our serial number to write back to device
                // By setting to an empty string, we allow the FTD2XX DLL 
                // to generate a serial number
                //myEEData.SerialNumber = String.Empty;
                myEEData.Description = encode;

                // Write our modified data structure back to the device EEPROM
                // This can throw an exception if trying to write a device type that does not 
                // match the EEPROM structure being used, so should always use a 
                // try - catch block when calling
                try
                {
                    ftStatus = myFtdiDevice.WriteFT232BEEPROM(myEEData);
                }
                catch (FTDI.FT_EXCEPTION)
                {
                    content.Append("Exception thrown when calling WriteFT232BEEPROM" + "\n");
                }

                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    // Wait for a key press
                    content.Append("Failed to write device EEPROM (error " + ftStatus.ToString() + "\n");
                    // Close the device
                    myFtdiDevice.Close();
                    return;
                }
            }


            // Use cycle port to force a re-enumeration of the device.  
            // In the FTD2XX_NET class library, the cycle port method also 
            // closes the open handle so no need to call the Close method separately.
            ftStatus = myFtdiDevice.CyclePort();

            UInt32 newFtdiDeviceCount = 0;
            do
            {
                // Wait for device to be re-enumerated
                // The device will have the same location since it has not been 
                // physically unplugged, so we will keep trying to open it until it succeeds
                ftStatus = myFtdiDevice.OpenByLocation(keyPro.LocId);
                Thread.Sleep(1000);
            } while (ftStatus != FTDI.FT_STATUS.FT_OK);

            // Close the device
            myFtdiDevice.Close();

            // Re-create our device list
            ftStatus = myFtdiDevice.GetNumberOfDevices(ref newFtdiDeviceCount);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                content.Append("Failed to get number of devices (error " + ftStatus.ToString() + ")" + "\n");
                return;
            }
        }

        private string getCheckCode(string serialNumber)
        {
            DateTime now = DateTime.Now;
            string datePrefix = (now.Year + now.Month + now.Day).ToString();
            string datePostfix = now.ToString("yyyy-MM-dd");
            string encode = getMD5(datePrefix + serialNumber + datePostfix);
            return encode.Substring(0, 8) + encode.Substring(24, 8);
        }

        internal string getContent(bool isReadRom)
        {
            if(isReadRom)
                readRom();
            return content.ToString();
        }


        public string getMD5(string data)
        {
            //MD5 md5 = MD5.Create();//建立一個MD5
            //byte[] source = Encoding.ASCII.GetBytes("abcdefg");//將字串轉為Byte[]
            //byte[] crypto = md5.ComputeHash(source);//進行MD5加密
            //string result = Convert.ToBase64String(crypto);//把加密後的字串從Byte[]轉為字串
            using (var md5Hash = MD5.Create())
            {
                // Byte array representation of source string
                var sourceBytes = Encoding.UTF8.GetBytes(data);

                // Generate hash value(Byte Array) for input data
                var hashBytes = md5Hash.ComputeHash(sourceBytes);

                // Convert hash byte array to string
                var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

                // Output the MD5 hash
                //Console.WriteLine("The MD5 hash of " + source + " is: " + hash);
                return hash;//輸出結果
            }
        }
    }
}
