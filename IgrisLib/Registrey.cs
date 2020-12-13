using IgrisLib.MessageBox;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace IgrisLib
{
    public class Registrey
    {
        public static string registryName = @"FrenchModdingTeam\CCAPI";

        public static void CreateRegistry(string fileName, string value)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey(registryName);
            key = key.OpenSubKey(registryName, true);
            if (Registry.GetValue($@"HKEY_CURRENT_USER\SOFTWARE\{registryName}\Consoles", fileName, null) == null)
            {
                key.CreateSubKey(fileName);
                key.SetValue(fileName, value);
            }
        }

        public static string ReadRegistry()
        {
            RegistryKey Key = Registry
                .CurrentUser
                .OpenSubKey($@"SOFTWARE\{registryName}\InstallFolder");
            if (Key != null)
            {
                string Path = Key.GetValue("path") as string;
                if (!string.IsNullOrEmpty(Path))
                {
                    string DllUrl = Path + @"\CCAPI.dll";
                    if (File.Exists(DllUrl))
                    {
                        return DllUrl;
                    }
                    else
                    {
                        IgrisMessageBox.Show("You need to install CCAPI 2.60/2.70/2.80/+ to use this library.", "CCAPI.dll not found", MessageBoxButton.OK, MessageBoxImage.Error);
                        Environment.Exit(0);
                        return null;
                    }
                }
                else
                {
                    IgrisMessageBox.Show("Invalid CCAPI folder, please re-install it.", "CCAPI not installed", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                    return null;
                }
            }
            else
            {
                IgrisMessageBox.Show("You need to install CCAPI 2.60/2.70/2.80/+ to use this library.", "CCAPI not installed", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
                return null;
            }
        }

        public static string ReadRegistry(string fileName)
        {
            Registrey reg = new Registrey()
            {
                BaseRegistryKey = Registry.CurrentUser,
                SubKey = "SOFTWARE\\" + registryName
            };
            string cleaning = reg.Read(fileName).Replace("{00000000-0000-0000-0000-000000000000}", "").Replace("%UserProfile%", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            return cleaning.Contains("|") ? cleaning.Remove(cleaning.IndexOf("|")) : cleaning;
        }

        public static List<CCAPI.ConsoleInfo> GetConsoles()
        {
            List<CCAPI.ConsoleInfo> consoles = new List<CCAPI.ConsoleInfo>();
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key = key.OpenSubKey(registryName, true);
            key = key.OpenSubKey("Consoles", true);
            foreach (string subKeyName in key.GetValueNames())
            {
                // Read Value from Registry Sub Key
                string softwareName = (string)key.GetValue(subKeyName);

                if (!string.IsNullOrEmpty(softwareName))
                {
                    consoles.Add(new CCAPI.ConsoleInfo() { name = subKeyName, ip = softwareName });
                }
            }
            return consoles;
        }

        public static void WriteRegistry(string fileName, string data)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key = key.OpenSubKey(registryName, true);
            key.SetValue(fileName, data);
        }

        public static void DeleteRegistry(string fileName)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key = key.OpenSubKey(registryName, true);
            key.DeleteValue(fileName);
        }

        private bool showError = false;
        public bool ShowError
        {
            get { return showError; }
            set { showError = value; }
        }

        private string subKey; // = "SOFTWARE\\" + Application.ProductName.ToUpper();
        public string SubKey
        {
            get { return subKey; }
            set { subKey = value; }
        }

        private RegistryKey baseRegistryKey = Registry.LocalMachine;
        public RegistryKey BaseRegistryKey
        {
            get { return baseRegistryKey; }
            set { baseRegistryKey = value; }
        }
        public string Read(string KeyName)
        {
            // Opening the registry key
            RegistryKey rk = baseRegistryKey;
            // Open a subKey as read-only
            RegistryKey sk1 = rk.OpenSubKey(subKey);
            // If the RegistrySubKey doesn't exist -> (null)
            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its mue
                    // or null is returned.
                    return (string)sk1.GetValue(KeyName.ToUpper());
                }
                catch (Exception e)
                {
                    // AAAAAAAAAAARGH, an error!
                    ShowErrorMessage(e, "Reading registry " + KeyName.ToUpper());
                    return null;
                }
            }
        }
        public bool Write(string KeyName, object mue)
        {
            try
            {
                // Setting
                RegistryKey rk = baseRegistryKey;
                // I have to use CreateSubKey 
                // (create or open it if already exits), 
                // 'cause OpenSubKey open a subKey as read-only
                RegistryKey sk1 = rk.CreateSubKey(subKey);
                // Save the mue
                sk1.SetValue(KeyName.ToUpper(), mue);

                return true;
            }
            catch (Exception e)
            {
                // AAAAAAAAAAARGH, an error!
                ShowErrorMessage(e, "Writing registry " + KeyName.ToUpper());
                return false;
            }
        }
        public bool DeleteKey(string KeyName)
        {
            try
            {
                // Setting
                RegistryKey rk = baseRegistryKey;
                RegistryKey sk1 = rk.CreateSubKey(subKey);
                // If the RegistrySubKey doesn't exists -> (true)
                if (sk1 == null)
                    return true;
                else
                    sk1.DeleteValue(KeyName);

                return true;
            }
            catch (Exception e)
            {
                // AAAAAAAAAAARGH, an error!
                ShowErrorMessage(e, "Deleting SubKey " + subKey);
                return false;
            }
        }
        public bool DeleteSubKeyTree()
        {
            try
            {
                // Setting
                RegistryKey rk = baseRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(subKey);
                // If the RegistryKey exists, I delete it
                if (sk1 != null)
                    rk.DeleteSubKeyTree(subKey);

                return true;
            }
            catch (Exception e)
            {
                // AAAAAAAAAAARGH, an error!
                ShowErrorMessage(e, "Deleting SubKey " + subKey);
                return false;
            }
        }
        public int SubKeyCount()
        {
            try
            {
                // Setting
                RegistryKey rk = baseRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(subKey);
                // If the RegistryKey exists...
                if (sk1 != null)
                    return sk1.SubKeyCount;
                else
                    return 0;
            }
            catch (Exception e)
            {
                // AAAAAAAAAAARGH, an error!
                ShowErrorMessage(e, "Retriving subkeys of " + subKey);
                return 0;
            }
        }
        public int ValueCount()
        {
            try
            {
                // Setting
                RegistryKey rk = baseRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(subKey);
                // If the RegistryKey exists...
                if (sk1 != null)
                    return sk1.ValueCount;
                else
                    return 0;
            }
            catch (Exception e)
            {
                // AAAAAAAAAAARGH, an error!
                ShowErrorMessage(e, "Retriving keys of " + subKey);
                return 0;
            }
        }
        private void ShowErrorMessage(Exception e, string Title)
        {
            if (showError == true)
                IgrisMessageBox.Show(e.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
