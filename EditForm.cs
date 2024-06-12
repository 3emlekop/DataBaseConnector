using System.Data;
using System.Windows.Forms;
using System;

namespace DataBaseConnector
{
    public partial class EditForm : Form
    {
        private IDbConnection connection;
        private string tableName;
        private int? id; // This will be null for adding new rows and will have a value for updating rows

        public EditForm(IDbConnection connection, string tableName, int? id = null)
        {
            InitializeThisComponent();
            this.connection = connection;
            this.tableName = tableName;
            this.id = id;

            if (id.HasValue)
            {
                LoadData(id.Value);
            }
        }

        private void LoadData(int id)
        {
            string query = $"SELECT * FROM {tableName} WHERE Id = {id}";
            IDbCommand command = connection.CreateCommand();
            command.CommandText = query;
            IDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                txtId.Text = reader["Id"].ToString();
                txtField1.Text = reader["Field1"].ToString();
                txtField2.Text = reader["Field2"].ToString();
                // Continue for all fields...
            }
            reader.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string query;
            if (id.HasValue)
            {
                query = $"UPDATE {tableName} SET Field1 = '{txtField1.Text}', Field2 = '{txtField2.Text}' WHERE Id = {id.Value}";
            }
            else
            {
                query = $"INSERT INTO {tableName} (Field1, Field2) VALUES ('{txtField1.Text}', '{txtField2.Text}')";
            }

            IDbCommand command = connection.CreateCommand();
            command.CommandText = query;

            try
            {
                command.ExecuteNonQuery();
                MessageBox.Show("Operation successful.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to execute operation. {ex.Message}");
            }
        }

        private void InitializeThisComponent()
        {
            this.txtId = new System.Windows.Forms.TextBox();
            this.txtField1 = new System.Windows.Forms.TextBox();
            this.txtField2 = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(12, 12);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(100, 20);
            this.txtId.TabIndex = 0;
            // 
            // txtField1
            // 
            this.txtField1.Location = new System.Drawing.Point(12, 38);
            this.txtField1.Name = "txtField1";
            this.txtField1.Size = new System.Drawing.Size(100, 20);
            this.txtField1.TabIndex = 1;
            // 
            // txtField2
            // 
            this.txtField2.Location = new System.Drawing.Point(12, 64);
            this.txtField2.Name = "txtField2";
            this.txtField2.Size = new System.Drawing.Size(100, 20);
            this.txtField2.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 90);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EditForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtField2);
            this.Controls.Add(this.txtField1);
            this.Controls.Add(this.txtId);
            this.Name = "EditForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.TextBox txtField1;
        private System.Windows.Forms.TextBox txtField2;
        private System.Windows.Forms.Button btnSave;
    }
}
