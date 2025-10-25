using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Windows_Forms___CRUD

    // NOTE : The table name is table-vp
{
    public partial class Form1 : Form
    {
        private MySqlConnection koneksi;
        private MySqlDataAdapter adapter;
        private MySqlCommand perintah;
        private DataSet ds = new DataSet();
        private string alamat, query;

        public Form1()
        {
            alamat = "server=localhost; database=database-vp; username=root; password=;";
            koneksi = new MySqlConnection(alamat);

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {   // Execute query to load data into DataGrid
                koneksi.Open();
                query = "SELECT * FROM `table-vp`";  
                adapter = new MySqlDataAdapter(query, koneksi);
                ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                koneksi.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
                koneksi.Close();
            }
        }

        private void DataGrid_Click(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {  // DISPLAY SELECTED ROW DATA INTO FORM FIELDS
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                ID.Text = row.Cells["ID"].Value.ToString();
                FirstName.Text = row.Cells["FirstName"].Value.ToString();
                LastName.Text = row.Cells["LastName"].Value.ToString();

                string gender = row.Cells["Gender"].Value.ToString();
                if (gender == "Laki - laki") Male.Checked = true;
                else Female.Checked = true;

                Birthdate.Value = Convert.ToDateTime(row.Cells["BirthDate"].Value);
            }
        }

        private void Button_Search(object sender, EventArgs e)
        {
            try
            {
                koneksi.Open();
                query = "SELECT * FROM `table-vp` WHERE FirstName LIKE @search OR LastName LIKE @search";
                perintah = new MySqlCommand(query, koneksi);
                perintah.Parameters.AddWithValue("@search", "%" + Search.Text + "%");

                adapter = new MySqlDataAdapter(perintah);
                ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                koneksi.Close();

                if (ds.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("No matching records found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search error: " + ex.Message);
                koneksi.Close();
            }
        }

        private void Button_Update(object sender, EventArgs e)
        {
            try
            {
                if (ID.Text != "")
                {
                    string gender = Male.Checked ? "Laki - laki" : "Perempuan";
                    string birthdate = Birthdate.Value.ToString("yyyy-MM-dd");

                    query = string.Format(
                        "UPDATE `table-vp` SET FirstName='{0}', LastName='{1}', Gender='{2}', BirthDate='{3}' WHERE ID={4}",
                        FirstName.Text, LastName.Text, gender, birthdate, ID.Text
                    );

                    koneksi.Open();
                    perintah = new MySqlCommand(query, koneksi);
                    int res = perintah.ExecuteNonQuery();
                    koneksi.Close();

                    if (res == 1)
                    {
                        MessageBox.Show("Data updated successfully!");
                        Form1_Load(null, null); // refresh table
                    }
                    else
                    {
                        MessageBox.Show("Update failed.");
                    }
                }
                else
                {
                    MessageBox.Show("Please select a record to update.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                koneksi.Close();
            }
        }

        private void ClearFields()
        {
            ID.Text = "";
            FirstName.Text = "";
            LastName.Text = "";
            Male.Checked = false;
            Female.Checked = false;
            Birthdate.Value = DateTime.Now;
        }


        private void Button_Delete(object sender, EventArgs e)
        {
            try
            {
                if (ID.Text != "")
                {
                    DialogResult confirm = MessageBox.Show(
                        "Are you sure you want to delete this record?",
                        "Confirm Delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (confirm == DialogResult.Yes)
                    {
                        query = "DELETE FROM `table-vp` WHERE ID=" + ID.Text;
                        koneksi.Open();
                        perintah = new MySqlCommand(query, koneksi);
                        int res = perintah.ExecuteNonQuery();
                        koneksi.Close();

                        if (res == 1)
                        {
                            MessageBox.Show("Data deleted successfully!");
                            Form1_Load(null, null); // refresh table
                            ClearFields(); // optional reset
                        }
                        else
                        {
                            MessageBox.Show("Delete failed.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select a record to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting: " + ex.Message);
                koneksi.Close();
            }
        }

        private void Button_Insert(object sender, EventArgs e)
        {
            try
            {
                // If all fields filled
                if (FirstName.Text != "" && LastName.Text != "" && (Male.Checked || Female.Checked)) {
                    string gender = Male.Checked ? "Laki - laki" : "Perempuan";
                    string birthdate = Birthdate.Value.ToString("yyyy-MM-dd");

                    // Query
                    query = string.Format("INSERT INTO `table-vp` (FirstName, LastName, Gender, BirthDate) VALUES ('{0}', '{1}', '{2}', '{3}')", FirstName.Text, LastName.Text, gender, birthdate);

                    // Execute query
                    koneksi.Open();
                    perintah = new MySqlCommand(query, koneksi);
                    adapter = new MySqlDataAdapter(perintah);
                    int res = perintah.ExecuteNonQuery();
                    koneksi.Close();

                    if (res == 1)
                    {
                        MessageBox.Show("Data insertion successful");
                        Form1_Load(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Data insertion failed");
                    }


         
                }
                else {MessageBox.Show("Please fill in all fields."); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
