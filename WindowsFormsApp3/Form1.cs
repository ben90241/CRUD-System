using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Benson\source\repos\WindowsFormsApp3\WindowsFormsApp3\Database1.mdf;Integrated Security=True");
        public Form1()
        {
            InitializeComponent();
        }

        private void Button5_Click(object sender, EventArgs e) //reload
        {
            reload();
        }

        private void Button1_Click(object sender, EventArgs e)  //insert
        {
            
            sqlCon.Open();
            SqlCommand cmd = new SqlCommand("if not exists (select Id from Employee where Id=@Id) begin insert into Employee (Id, Name, Position, Phone, Address) values (@Id, @Name, @Position, @Phone, @Address) end", sqlCon);

            cmd.Parameters.AddWithValue("@Id", int.Parse(textBox1.Text));
            cmd.Parameters.AddWithValue("@Name", textBox2.Text);
            cmd.Parameters.AddWithValue("@Position", textBox3.Text);
            cmd.Parameters.AddWithValue("@Phone", textBox4.Text);
            cmd.Parameters.AddWithValue("@Address", textBox5.Text);

            cmd.ExecuteNonQuery();

            reload();

            sqlCon.Close();
            
        }

        private void Button4_Click(object sender, EventArgs e)  //delete
        {
            sqlCon.Open();
            SqlCommand cmd = new SqlCommand("delete Employee where Id=@Id", sqlCon);

            cmd.Parameters.AddWithValue("@Id", int.Parse(textBox6.Text));

            cmd.ExecuteNonQuery();

            reload();

            sqlCon.Close();
        }
        private void reload()
        {
            SqlCommand cmd = new SqlCommand("select * from Employee", sqlCon);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }
        private void Button3_Click(object sender, EventArgs e) //search
        {
            sqlCon.Open();
            SqlCommand cmd = new SqlCommand("select * from Employee where Name=@Name or Position=@Position or Phone=@Phone or Address=@Address", sqlCon);

            //cmd.Parameters.AddWithValue("@Id", int.Parse(textBox1.Text));
            cmd.Parameters.AddWithValue("@Name", textBox2.Text);
            cmd.Parameters.AddWithValue("@Position", textBox3.Text);
            cmd.Parameters.AddWithValue("@Phone", textBox4.Text);
            cmd.Parameters.AddWithValue("@Address", textBox5.Text);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            sqlCon.Close();
        }

        private void Button2_Click(object sender, EventArgs e) //update
        {
            sqlCon.Open();
            SqlCommand cmd = new SqlCommand("update Employee set Name=@Name, Position=@Position, Phone=@Phone, Address=@Address where Id=@Id", sqlCon);

            cmd.Parameters.AddWithValue("@Id", int.Parse(textBox1.Text));
            cmd.Parameters.AddWithValue("@Name", textBox2.Text);
            cmd.Parameters.AddWithValue("@Position", textBox3.Text);
            cmd.Parameters.AddWithValue("@Phone", textBox4.Text);
            cmd.Parameters.AddWithValue("@Address", textBox5.Text);

            cmd.ExecuteNonQuery();

            reload();

            sqlCon.Close();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            reload();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'database1DataSet.Employee' table. You can move, or remove it, as needed.
            this.employeeTableAdapter.Fill(this.database1DataSet.Employee);
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            if(checkBox1.Checked==checkBox2.Checked)
            {
                MessageBox.Show("Please select the export data format!");
                return;
            }
           
            sqlCon.Open();
            SqlCommand cmd = new SqlCommand("select * from Employee", sqlCon);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = @"C:\Users\Benson\Desktop";
            sfd.RestoreDirectory = true;

            if (checkBox1.Checked==true)
            {
                sfd.FileName = "*.xml";
                sfd.DefaultExt = "xml";
                sfd.Filter = "xml files (*.xml) | *.xml";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Stream fileStream = sfd.OpenFile();
                    StreamWriter sw = new StreamWriter(fileStream);
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    ds.WriteXml(sw, XmlWriteMode.IgnoreSchema);
                }

                sqlCon.Close();
            }
            if(checkBox2.Checked==true)
            {
                sfd.FileName = "*.json";
                sfd.DefaultExt = "json";
                sfd.Filter = "json files (*.json) | *.json";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Stream fileStream = sfd.OpenFile();
                    StreamWriter sw = new StreamWriter(fileStream);
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                    List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
                    Dictionary<string, object> childRow;

                    DataTable table = dt;

                    foreach (DataRow row in table.Rows)
                    {
                        childRow = new Dictionary<string, object>();
                        foreach (DataColumn col in table.Columns)
                        {
                            childRow.Add(col.ColumnName, row[col]);
                        }
                        parentRow.Add(childRow);
                    }

                    sw.WriteLine(jsSerializer.Serialize(parentRow));
                    sw.Close();
                    fileStream.Close();
                }

                sqlCon.Close();
            }
        }

    }
}
