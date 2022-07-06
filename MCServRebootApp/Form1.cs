using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.IO.Compression;

namespace MCServRebootApp
{
    public partial class Form1 : Form
    {

        string path = "E:/Jeux/serv_14.01.22/start.bat";
        string process_name = "Java";

        string backup_source_folder = "E:/Jeux/serv_14.01.22";
        string backup_dest_path = "F:/Jeux/Servs_MC/Backups";

        bool isRunning = false;

        public Form1()
        {
            InitializeComponent();
            InitializeTimer();

            if (!File.Exists("E:/Jeux/serv_14.01.22/RebootLogs.txt")) // If file does not exists
                File.Create("E:/Jeux/serv_14.01.22/RebootLogs.txt").Close(); // Create file
        }


        private void InitializeTimer()
        { 
            // Set to 1 second  
            myTimer.Interval = 1000;
            myTimer.Tick += new EventHandler(myTimer_Tick);

            // init timer state  
            myTimer.Enabled = false;
        }

        private void myTimer_Tick(object sender, EventArgs e)
        {
            //checking process state
            if (Process.GetProcessesByName(process_name).Length > 0)
                isRunning = true;
            else
                isRunning = false;


            //rebooting and filling logs
            if (isRunning == false)
            {
                Process.Start(path);
                using (StreamWriter sw = File.AppendText("E:/Jeux/serv_14.01.22/RebootLogs.txt"))
                {
                    sw.WriteLine("Reboot successful on : " + DateTime.UtcNow.ToString("D") + "\t" + DateTime.Now.ToString("hh:mm:ss")); // Write text to .txt file
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Stop")
            {
                button1.Text = "Start";
                button1.BackColor = Color.Chartreuse;
                myTimer.Enabled = false;
                checkBox1.Checked = false;
            }
            else
            {
                button1.Text = "Stop";
                button1.BackColor = Color.OrangeRed;
                myTimer.Enabled = true;
                checkBox1.Checked = true;
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName(process_name).Length > 0)
            {
                foreach (var process in Process.GetProcessesByName(process_name))
                {
                    process.Kill();
                }

                Process.Start(path);

                using (StreamWriter sw = File.AppendText("E:/Jeux/serv_14.01.22/RebootLogs.txt"))
                {
                    sw.WriteLine("//FORCED// Reboot successful on : " + DateTime.UtcNow.ToString("D") + "\t" + DateTime.Now.ToString("hh:mm:ss")); // Write text to .txt file
                }

                MessageBox.Show("//FORCED// Reboot successful on : " + DateTime.UtcNow.ToString("D") + "\t" + DateTime.Now.ToString("hh:mm:ss"));
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            //stop du relaunch auto pour pas qu'il relance le serv pendant la backup
            if (checkBox1.Checked == true)
                button1_Click(sender, e);


            //kill des instances du serv avant la backup
            if (Process.GetProcessesByName(process_name).Length > 0)
            {
                foreach (var process in Process.GetProcessesByName(process_name))
                {
                    process.Kill();
                }
            }

            Thread.Sleep(5000);

            string filename = null;
            string destfile = null;
            string full_backup_dest_path = backup_dest_path + "/save_" + DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss");

            //creation d'un nouveau dossier
            Directory.CreateDirectory(full_backup_dest_path);

            //copie du contenu du dossier du serv et collage dans le dossier de la backup
            if (System.IO.Directory.Exists(backup_source_folder))
            {
                string[] files = System.IO.Directory.GetFiles(backup_source_folder);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    filename = System.IO.Path.GetFileName(s);
                    destfile = System.IO.Path.Combine(full_backup_dest_path, filename);
                    System.IO.File.Copy(s, destfile, true);
                }
            }

            //zip
            ZipFile.CreateFromDirectory(full_backup_dest_path, full_backup_dest_path + ".zip");

            //suppression du dossier pas zippé
            if (Directory.Exists(full_backup_dest_path))
            {
                string[] files = System.IO.Directory.GetFiles(full_backup_dest_path);
                foreach (string s in files)
                {
                    File.Delete(s);
                }
                Directory.Delete(full_backup_dest_path);
            }

            //relance et reactive l'auto relaunch
            Process.Start(path);
            button1_Click(sender, e);

            //msg l'user
            using (StreamWriter sw = File.AppendText("E:/Jeux/serv_14.01.22/RebootLogs.txt"))
            {
                sw.WriteLine("SERVER BACKUP then Reboot successful on : " + DateTime.UtcNow.ToString("D") + "\t" + DateTime.Now.ToString("hh:mm:ss")); // Write text to .txt file
            }
            MessageBox.Show("SERVER BACKUP then Reboot successful on : " + DateTime.UtcNow.ToString("D") + "\t" + DateTime.Now.ToString("hh:mm:ss"));
        }



        private void button3_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}