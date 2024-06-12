using System;
using System.Windows.Forms;

namespace DataBaseConnector
{
    public partial class AddUpdateForm : Form
    {
        public string[] FieldValues { get; private set; }
        private TextBox[] textBoxes;
        private Label[] labels;

        public AddUpdateForm(string[] fieldNames, string[] existingValues = null)
        {
            InitializeComponent();
            textBoxes = new TextBox[6];
            labels = new Label[6];
            FieldValues = new string[6];

            for (int i = 0; i < fieldNames.Length && i < 6; i++)
            {
                labels[i] = new Label
                {
                    Text = fieldNames[i],
                    Location = new System.Drawing.Point(20, 30 + (i * 30)),
                    ForeColor = System.Drawing.Color.White
                };
                this.Controls.Add(labels[i]);

                textBoxes[i] = new TextBox
                {
                    Location = new System.Drawing.Point(120, 30 + (i * 30)),
                    Size = new System.Drawing.Size(200, 23),
                    BackColor = System.Drawing.Color.FromArgb(34, 0, 34),
                    ForeColor = System.Drawing.SystemColors.Control
                };

                if (existingValues != null && i < existingValues.Length)
                {
                    textBoxes[i].Text = existingValues[i];
                }

                this.Controls.Add(textBoxes[i]);
            }

            Button btnSave = new Button
            {
                Text = "Save",
                Location = new System.Drawing.Point(120, 220),
                Size = new System.Drawing.Size(100, 30),
                BackColor = System.Drawing.Color.FromArgb(64, 0, 64),
                ForeColor = System.Drawing.SystemColors.Control,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < textBoxes.Length; i++)
            {
                FieldValues[i] = textBoxes[i]?.Text;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
