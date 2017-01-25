using Awpbs;
using Awpbs.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseSeeder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Awpbs.Web.SeederSports seeder = new Awpbs.Web.SeederSports(new ApplicationDbContext());
            seeder.SeedSportsAndResultTypes();
        }

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    //Awpbs.Web.SeederPros seeder = new Awpbs.Web.SeederPros(new ApplicationDbContext());
        //    //seeder.Seed();
        //}

        private void button3_Click(object sender, EventArgs e)
        {
            Awpbs.Web.SeederQuotes seeder = new Awpbs.Web.SeederQuotes(new ApplicationDbContext());
            seeder.Seed();
        }

        private void buttonMetros_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            var file = dlg.OpenFile();

            Awpbs.Web.SeederMetros seeder = new SeederMetros(new ApplicationDbContext());
            List<Metro> metros = null;
            try
            {
                metros = seeder.LoadMetrosFromFile(file);
            }
            catch (Exception exc)
            {
                MessageBox.Show(this, "Failed to load venues from the file. Message: " + exc.Message, "Byb", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            file.Close();
            if (metros == null)
                return;

            if (MessageBox.Show(this, metros.Count.ToString() + " metros loaded. Import into the database?", "Byb", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                return;

            int countNew, countModified, countError;
            seeder.PopulateMetrosIntoTheDatabase(metros, out countNew, out countModified, out countError);

            MessageBox.Show(this, "Completed. New:" + countNew + ", Modified:" + countModified + ". Error:" + countError, "Byb", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (MessageBox.Show(this, "Would you like to save metros along with IDs into a new .txt file?", "Byb", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                return;

            DateTime now = DateTime.Now;

            SaveFileDialog dlgSave = new SaveFileDialog();
            dlgSave.CheckFileExists = false;
            dlgSave.FileName = "metros " + now.Year + "-" + now.Month + "-" + now.Day + "--" + now.Hour + "-" + now.Minute + "-" + now.Second + ".txt";
            if (dlgSave.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            string fileName = dlgSave.FileName;
            if (File.Exists(fileName))
                File.Delete(fileName);
            file = File.Create(fileName);
            seeder.SaveMetrosToFile(metros, file);
            file.Close();
        }

        private async void buttonVenues_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            var file = dlg.OpenFile();

            Awpbs.Web.SeederVenues seeder = new SeederVenues(new ApplicationDbContext());
            List<Venue> venuesFromFile = null;
            try
            {
                venuesFromFile = seeder.LoadVenuesFromFile(file);
            }
            catch (Exception exc)
            {
                MessageBox.Show(this, "Failed to load venues from the file. Message: " + exc.Message, "Byb", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            file.Close();
            if (venuesFromFile == null)
                return;

            //MessageBox.Show(this, venues.Count.ToString() + " venues loaded.", "Byb");

            if (MessageBox.Show(this, venuesFromFile.Count.ToString() + " venues loaded from the file. Proceed with verifying with Google API?", "Byb", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.OK)
                return;

            var results = await seeder.VerifyVenuesWithGoogle(venuesFromFile);
            var venuesVerifiedWithGoogle = results.VenuesVerifiedWithGoogle;
            int countErrors = results.CountErrors;

            MessageBox.Show(this, "Of " + venuesFromFile.Count + " venues " + venuesVerifiedWithGoogle.Count + " have been verified by google. " + countErrors + " errors encountered on the way.");

            if (MessageBox.Show(this, "Import " + venuesVerifiedWithGoogle.Count + " verified venues into the database?", "Byb", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                return;

            int countNew, countModified, countError;
            seeder.PopulateVenuesIntoTheDatabase(venuesVerifiedWithGoogle, out countNew, out countModified, out countError);

            MessageBox.Show(this, "Completed. New:" + countNew + ", Modified:" + countModified + ". Error:" + countError, "Byb", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (MessageBox.Show(this, "Would you like to save uploaded venues along with IDs into a new .txt file?", "Byb", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                return;

            DateTime now = DateTime.Now;

            SaveFileDialog dlgSave = new SaveFileDialog();
            dlgSave.CheckFileExists = false;
            dlgSave.FileName = "venues " + now.Year + "-" + now.Month + "-" + now.Day + "--" + now.Hour + "-" + now.Minute + "-" + now.Second + ".txt";
            if (dlgSave.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            string fileName = dlgSave.FileName;
            if (File.Exists(fileName))
                File.Delete(fileName);
            file = File.Create(fileName);
            seeder.SaveVenuesToFile(venuesVerifiedWithGoogle, file);
            file.Close();
        }

        private void buttonApplyMetrosToVenues_Click(object sender, EventArgs e)
        {
            ApplyMetrosToVenues logic = new ApplyMetrosToVenues(new ApplicationDbContext());

            int countVenuesUpdated, countVenuesSkipped;
            logic.ApplyMetrosToVenuesWithoutMetros(out countVenuesUpdated, out countVenuesSkipped);

            MessageBox.Show(this, "Updated " + countVenuesUpdated + ", did not update " + countVenuesSkipped, "Byb", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonSeedVerifications_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Use is in debug mode only! Verify the code!!! Continue?", "Byb", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                return;

            var db = new ApplicationDbContext();
            var verifyingAthlete = db.Athletes.Where(i => i.AthleteID == 21).Single();
            var venuesToVerify = db.Venues.Where(i => i.Country == "GBR").ToList();

            int countSkipped = 0;
            int countAddedVerifications = 0;

            foreach (var venue in venuesToVerify)
            {
                if (venue.VenueEdits.Count() > 0)
                {
                    countSkipped++;
                    continue;
                }

                VenueEditTypeEnum type = VenueEditTypeEnum.ConfirmOnly;

                VenueEdit edit = new VenueEdit()
                {
                    AthleteID = verifyingAthlete.AthleteID,
                    VenueID = venue.VenueID,
                    Time = DateTime.UtcNow,
                    Type = (int)type,
                    Backup = ""
                };
                db.VenueEdits.Add(edit);
                countAddedVerifications++;
            }

            db.SaveChanges();

            MessageBox.Show(this, "Done. Skipped:" + countSkipped + ", Added verification:" + countAddedVerifications, "Byb", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonTestNotifications_Click(object sender, EventArgs e)
        {
            var db = new ApplicationDbContext();

            Athlete athlete = db.Athletes.Single(i => i.AthleteID == 1033);

            Awpbs.Web.PushNotificationProcessor.InitSingleInstance();
            int count = 0;
            new PushNotificationsLogic(new ApplicationDbContext()).SendNotification(1033, PushNotificationMessage.BuildPrivateMessage(athlete, "HELLO!"));
            bool pushedOk = Awpbs.Web.PushNotificationProcessor.TheProcessor.PushAllPendingNotifications();
            Awpbs.Web.PushNotificationProcessor.Destroy();

            MessageBox.Show(this, "Prepared: " + count.ToString() + ". Pushed ok=" + pushedOk);
        }

        private void buttonTemp1_Click(object sender, EventArgs e)
        {
            //var db = new ApplicationDbContext();
            //var list = db.Metros.ToList();

            //foreach (var metro in list)
            //{
            //    string url = metro.Name;

            //    int index = url.IndexOf(", ");
            //    if (index > 0)
            //        url = url.Substring(0, index);

            //    url = url.Replace(", ", "-");
            //    url = url.Replace(" ", "-");
            //    metro.UrlName = url;
            //}
            //db.SaveChanges();
        }
    }
}

