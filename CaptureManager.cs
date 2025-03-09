using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tesseract;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace PatheonParser
{
    public partial class CaptureManager
        : IDisposable
    {
        [GeneratedRegex(@"[{\[]\d+[:;]\d+[:;]\d+[\]}]", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex TimeStampRegex();

        [GeneratedRegex(@"([\w\s]+) dealt (\d+) ([\w\s]+) damage to ([\w\s]+) with ([\w\s]+). \((\d+) mitigated\)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex AttackRegex();

        [GeneratedRegex(@"([\w\s]+) was healed for (\d+) by ([\w\s]+).", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex HealRegex();

        [GeneratedRegex(@"([\w\s]+)'s ([\w\s]+) healed ([\w\s]+) for (\d+).", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex Heal2Regex();

        public string TessDataPath { get; set; }
        public TesseractEngine TesseractEngine { get; set; }
        public Tesseract.Rect CombatWindowRect { get; set; }
        public string LastHash { get; set; }
        public Queue<string> RecentLines { get; set; }
        public HashSet<string> SeenLines { get; set; }
        public DateTime LastAction { get; set; } = DateTime.MinValue;
        public int CurrentEncounterId { get; set; } = 0;

        public CaptureManager()
        {
            TessDataPath = Path.Combine(Environment.CurrentDirectory, "tessdata");
            TesseractEngine = new(TessDataPath, "eng", EngineMode.Default);
            LastHash = string.Empty;
            RecentLines = new Queue<string>();
            SeenLines = new HashSet<string>();

            CreateTables();
        }

        public int GetCurrentEncounterId()
        {
            var delta = DateTime.Now - LastAction;
            if (delta.TotalSeconds > Properties.Settings.Default.EncounterTimeout)
            {
                CurrentEncounterId = AddEncounter(string.Empty);
            }
            LastAction = DateTime.Now;
            return CurrentEncounterId;
        }

        public void CheckForChanges()
        {
            using var bitmap = Utilities.ScreenCapture();
            //bitmap.Save("c:\\temp\\raw.png");

            if (CombatWindowRect.Height == 0)
            {
                var newHash = Utilities.ComputeImageHash(bitmap);
                if (newHash == LastHash)
                {
                    return;
                }

                LastHash = newHash;

                using var originalImage = PixConverter.ToPix(bitmap);
                using var invertedImage = originalImage.Invert();

                CombatWindowRect = FindCombatWindowRect(invertedImage);

                LastHash = string.Empty;
            }
            else
            {
                var cropArea = new Rectangle(CombatWindowRect.X1, CombatWindowRect.Y1, CombatWindowRect.Width, CombatWindowRect.Height);
                cropArea = Rectangle.Intersect(cropArea, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                //bitmap.Save("c:\\temp\\bitmap.png");
                using var croppedBitmap = bitmap.Clone(cropArea, bitmap.PixelFormat);
                //croppedBitmap.Save("c:\\temp\\croppedBitmap.png");

                var newHash = Utilities.ComputeImageHash(croppedBitmap);
                if (newHash == LastHash)
                {
                    return;
                }

                LastHash = newHash;

                using var originalImage = PixConverter.ToPix(croppedBitmap);
                using var invertedImage = originalImage.Invert();

                CheckImageForChanges(invertedImage);
            }
        }

        public void CheckImageForChanges(Pix image)
        {
            using var scaledImage = image.Scale(2.0f, 2.0f);
            using var page = TesseractEngine.Process(scaledImage);
            var text = page.GetText();

            //Remove line wrapping
            text = text.Replace('\r', ' ');
            text = text.Replace('\n', ' ');
            text = text.Replace("[", "\n[");

            //Handle common OCR problems.
            text = text.Replace("mitigatad", "mitigated");
            text = text.Replace("Aftack", "Attack");
            text = text.Replace("spirt's", "spirit's");
            text = text.Replace("(Immuna)", "(immune)");
            text = text.Replace("(Immune)", "(immune)");
            text = text.Replace("vangseful", "vengeful");
            text = text.Replace("vengsful", "vengeful");
            text = text.Replace("Tallsman", "Talisman");
            text = text.Replace("Lifa Leach", "Life Leach");

            //Remove extra spaces.
            text = text.Replace("  ", " ");

            var lines = text.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (lines is not null)
            {
                foreach (var line in lines)
                {
                    if (line.StartsWith("[") && !SeenLines.Contains(line))
                    {
                        if (RecentLines.Count >= 20)
                        {
                            string oldestLine = RecentLines.Dequeue();
                            SeenLines.Remove(oldestLine);
                        }

                        var currentEncounterId = GetCurrentEncounterId();

                        var matches = AttackRegex().Matches(line);
                        if (matches is not null && matches.Count > 0)
                        {
                            foreach (Match match in matches)
                            {
                                //"(\w+) dealt (\d+) (\w+) damage to (\w+) with (\w+). \((\d+) mitigated\)"
                                var sourceName = match.Groups.Count > 1 ? match.Groups[1].Value : string.Empty;
                                var damageAmount = match.Groups.Count > 2 ? int.Parse(match.Groups[2].Value.Trim()) : 0;
                                var damageType = match.Groups.Count > 3 ? match.Groups[3].Value : string.Empty;
                                var targetName = match.Groups.Count > 4 ? match.Groups[4].Value : string.Empty;
                                var skillName = match.Groups.Count > 5 ? match.Groups[5].Value : string.Empty;
                                var mitigatedAmount = match.Groups.Count > 6 ? int.Parse(match.Groups[6].Value.Trim()) : 0;
                                AddAttack(currentEncounterId, sourceName, targetName, damageType, skillName, damageAmount, mitigatedAmount);
                            }
                        }
                        else
                        {
                            matches = HealRegex().Matches(line);
                            if (matches is not null && matches.Count > 0)
                            {
                                foreach (Match match in matches)
                                {
                                    //"(\w+) was healed for (\d+) by (\w+)."
                                    var targetName = match.Groups.Count > 1 ? match.Groups[1].Value : string.Empty;
                                    var amount = match.Groups.Count > 2 ? int.Parse(match.Groups[2].Value.Trim()) : 0;
                                    var skillName = match.Groups.Count > 3 ? match.Groups[3].Value : string.Empty;
                                    AddHeal(currentEncounterId, string.Empty, targetName, skillName, amount);
                                }
                            }
                            else
                            {
                                matches = Heal2Regex().Matches(line);
                                if (matches is not null && matches.Count > 0)
                                {
                                    foreach (Match match in matches)
                                    {
                                        //"(\w+)'s (\w+) healed (\w+) for (\d+)."
                                        var sourceName = match.Groups.Count > 1 ? match.Groups[1].Value : string.Empty;
                                        var skillName = match.Groups.Count > 2 ? match.Groups[2].Value : string.Empty;
                                        var targetName = match.Groups.Count > 3 ? match.Groups[3].Value : string.Empty;
                                        var amount = match.Groups.Count > 4 ? int.Parse(match.Groups[4].Value.Trim()) : 0;
                                        AddHeal(currentEncounterId, sourceName, targetName, skillName, amount);
                                    }
                                }
                            }
                        }
                        File.AppendAllText("log.txt", string.Concat(line, "\n"));
                        RecentLines.Enqueue(line);
                        SeenLines.Add(line);
                        if (MainForm.Instance is not null)
                        {
                            MainForm.Instance.Invoke((MethodInvoker)(() => MainForm.Instance.AddLog(line)));
                        }
                    }
                }
            }
        }

        public void CreateTables()
        {
            using var connection = new SQLiteConnection("Data Source=PatheonParser.db");
            connection.Open();

            using var encounterCommand = new SQLiteCommand("CREATE TABLE IF NOT EXISTS encounters (id INTEGER PRIMARY KEY,notes TEXT)", connection);
            encounterCommand.ExecuteNonQuery();

            using var attackCommand = new SQLiteCommand("CREATE TABLE IF NOT EXISTS attacks (id INTEGER PRIMARY KEY,encounter_id INTEGER,source_name TEXT,target_name TEXT,damage_type TEXT,skill_name TEXT,damage_amount INTEGER,mitigated_amount INTEGER)", connection);
            attackCommand.ExecuteNonQuery();

            using var healCommand = new SQLiteCommand("CREATE TABLE IF NOT EXISTS heals (id INTEGER PRIMARY KEY,encounter_id INTEGER,source_name TEXT,target_name TEXT,skill_name TEXT,amount INTEGER)", connection);
            healCommand.ExecuteNonQuery();
        }

        public int AddEncounter(string notes)
        {
            using var connection = new SQLiteConnection("Data Source=PatheonParser.db");
            connection.Open();
            using var command = new SQLiteCommand("INSERT INTO encounters (notes) VALUES (@Notes) RETURNING id;", connection);
            command.Parameters.AddWithValue("@Notes", notes);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetInt32(0);
            }

            return 0;
        }

        public int AddAttack(int encounterId, string sourceName, string targetName, string damageType, string skillName, int damageAmount, int mitigatedAmount)
        {
            using var connection = new SQLiteConnection("Data Source=PatheonParser.db");
            connection.Open();
            using var command = new SQLiteCommand("INSERT INTO attacks (encounter_id, source_name,target_name,damage_type,skill_name,damage_amount,mitigated_amount) VALUES (@EncounterId,@SourceName,@TargetName,@DamageType,@SkillName,@DamageAmount,@MitigatedAmount) RETURNING id;", connection);
            command.Parameters.AddWithValue("@EncounterId", encounterId);
            command.Parameters.AddWithValue("@SourceName", sourceName.Trim());
            command.Parameters.AddWithValue("@TargetName", targetName.Trim());
            command.Parameters.AddWithValue("@DamageType", damageType.Trim());
            command.Parameters.AddWithValue("@SkillName", skillName.Trim());
            command.Parameters.AddWithValue("@DamageAmount", damageAmount);
            command.Parameters.AddWithValue("@MitigatedAmount", mitigatedAmount);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetInt32(0);
            }

            return 0;
        }

        public int AddHeal(int encounterId, string sourceName, string targetName, string skillName, int amount)
        {
            using var connection = new SQLiteConnection("Data Source=PatheonParser.db");
            connection.Open();
            using var command = new SQLiteCommand("INSERT INTO heals (encounter_id,source_name,target_name,skill_name,amount) VALUES (@EncounterId,@SourceName,@TargetName,@SkillName,@Amount) RETURNING id;", connection);
            command.Parameters.AddWithValue("@EncounterId", encounterId);
            command.Parameters.AddWithValue("@SourceName", sourceName.Trim());
            command.Parameters.AddWithValue("@TargetName", targetName.Trim());
            command.Parameters.AddWithValue("@SkillName", skillName.Trim());
            command.Parameters.AddWithValue("@Amount", amount);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetInt32(0);
            }

            return 0;
        }

        public Tesseract.Rect FindCombatWindowRect(Pix image)
        {
            var minX = image.Width / 2;
            var minY = image.Height / 2;

            using var page = TesseractEngine.Process(image, PageSegMode.SparseText);

            var imageRect = new Tesseract.Rect(0, 0, image.Width, image.Height);
            var currentRect = new Tesseract.Rect(image.Width, image.Height, 0, 0);
            var lastText = string.Empty;

            using var it = page.GetIterator();
            while (it.Next(PageIteratorLevel.TextLine))
            {
                if (it.TryGetBoundingBox(PageIteratorLevel.TextLine, out var rect))
                {
                    if (rect.X1 > minX && rect.Y1 > minY && rect.X1 < currentRect.X1 && rect.Y1 < currentRect.Y1)
                    {
                        lastText = it.GetText(PageIteratorLevel.TextLine);
                        if (TimeStampRegex().IsMatch(lastText))
                        {
                            currentRect = new Tesseract.Rect(rect.X1 - 32, rect.Y1 - 32, (imageRect.X2 - rect.X1) + 32, (imageRect.Y2 - rect.Y1) + 32);
                        }
                    }
                }
            }

            return currentRect;
        }

        public void Dispose()
        {
            if (TesseractEngine is not null)
            {
                TesseractEngine.Dispose();
            }
        }
    }
}
