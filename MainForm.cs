using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using Npgsql;

namespace DataBaseConnector
{
    public partial class MainForm : Form
    {
        private string connectionString;
        private IDbConnection connection;
        private SettingsManager settingsManager;

        public MainForm()
        {
            InitializeComponent();
            settingsManager = new SettingsManager("config.txt");
            LoadSettings();
        }

        private void SaveSettings()
        {
            string dbPath = txtDbPath.Text;
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            bool isPostgreSQL = rbPostgreSQL.Checked;
            string database = txtDbName.Text;
            string table = txtTableName.Text;
            settingsManager.SaveSettings(dbPath, username, password, isPostgreSQL, database, table);
        }

        private void LoadSettings()
        {
            if (settingsManager.ExistsSettingsFile() == false)
            {
                return;
            }

            try
            {
                var settings = settingsManager.LoadSettings();
                txtDbPath.Text = settings.dbPath;
                txtUsername.Text = settings.username;
                txtPassword.Text = settings.password;
                if (settings.isPostgreSQL)
                {
                    rbPostgreSQL.Checked = true;
                }
                else
                {
                    rbAccess.Checked = true;
                }
                txtDbName.Text = settings.database; 
                txtTableName.Text = settings.table;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load settings. {ex.Message}");
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            SaveSettings();
            string dbPath = txtDbPath.Text;
            string user = txtUsername.Text;
            string password = txtPassword.Text;

            if (rbPostgreSQL.Checked)
            {
                connectionString = $"Host={dbPath};Username={user};Password={password};Database=" + txtDbName.Text;
                connection = new NpgsqlConnection(connectionString);
            }
            else if (rbAccess.Checked)
            {
                connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};User Id={user};Password={password};";
                connection = new OleDbConnection(connectionString);
            }

            try
            {
                connection.Open();
                MessageBox.Show("Connected to the database successfully.");
                ExecuteCommand("SELECT * FROM " + txtTableName.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to the database. {ex.Message}");
            }
        }

        private void ExecuteCommand(string commandText)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = commandText;

            DataTable dataTable = new DataTable();

            try
            {
                if (rbPostgreSQL.Checked)
                {
                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter((NpgsqlCommand)command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
                else
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter((OleDbCommand)command))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load data. {ex.Message}");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string[] fieldNames = new string[dataGridView1.Columns.Count];
            for (int i = 0; i < fieldNames.Length && i < 6; i++)
            {
                fieldNames[i] = dataGridView1.Columns[i].Name;
            }

            AddUpdateForm addForm = new AddUpdateForm(fieldNames);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                string[] fieldValues = addForm.FieldValues;

                List<string> nonEmptyFieldNames = new List<string>();
                List<string> nonEmptyFieldValues = new List<string>();
                for (int i = 0; i < fieldNames.Length && i < 6; i++)
                {
                    if (!string.IsNullOrEmpty(fieldValues[i]))
                    {
                        nonEmptyFieldNames.Add(fieldNames[i]);
                        nonEmptyFieldValues.Add($"'{fieldValues[i].Replace("'", "''")}'"); // Escape single quotes
                    }
                }

                string commandText = $"INSERT INTO {txtTableName.Text} ({string.Join(",", nonEmptyFieldNames)}) VALUES ({string.Join(",", nonEmptyFieldValues)})";

                ExecuteNonQuery(commandText);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int rowIndex = dataGridView1.SelectedRows[0].Index;
                string id = dataGridView1.Rows[rowIndex].Cells[0].Value.ToString();
                string commandText = $"DELETE FROM {txtTableName.Text} WHERE ID = {id}";

                ExecuteNonQuery(commandText);
            }
            else
            {
                MessageBox.Show("No row selected.");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int rowIndex = dataGridView1.SelectedRows[0].Index;
                string id = dataGridView1.Rows[rowIndex].Cells[0].Value.ToString();

                string[] fieldNames = new string[dataGridView1.Columns.Count];
                string[] fieldValues = new string[dataGridView1.Columns.Count];
                for (int i = 0; i < fieldNames.Length && i < 6; i++)
                {
                    fieldNames[i] = dataGridView1.Columns[i].Name;
                    fieldValues[i] = dataGridView1.Rows[rowIndex].Cells[i].Value.ToString();
                }

                AddUpdateForm updateForm = new AddUpdateForm(fieldNames, fieldValues);
                if (updateForm.ShowDialog() == DialogResult.OK)
                {
                    string[] newFieldValues = updateForm.FieldValues;
                    string setClause = "";
                    for (int i = 0; i < fieldNames.Length && i < 6; i++)
                    {
                        if (!string.IsNullOrEmpty(newFieldValues[i]))
                        {
                            setClause += $"{fieldNames[i]} = '{newFieldValues[i]}', ";
                        }
                    }
                    setClause = setClause.TrimEnd(',', ' ');
                    string commandText = $"UPDATE {txtTableName.Text} SET {setClause} WHERE ID = {id}";

                    ExecuteNonQuery(commandText);
                }
            }
            else
            {
                MessageBox.Show("No row selected.");
            }
        }

        private void ExecuteNonQuery(string commandText)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = commandText;

            try
            {
                command.ExecuteNonQuery();
                MessageBox.Show("Operation successful.");
                ExecuteCommand($"SELECT * FROM {txtTableName.Text}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to execute operation. {ex.Message}");
            }
        }
    }
}
