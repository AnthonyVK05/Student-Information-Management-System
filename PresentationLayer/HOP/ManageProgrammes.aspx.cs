using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Student_Information_Management_System.HOP
{
    public partial class ManageProgrammes : System.Web.UI.Page
    {
        string cs = ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProgrammes();
            }
        }

        private void LoadProgrammes()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"SELECT programme_id, programme_name, code, duration_years, department
                             FROM PROGRAMME
                             ORDER BY programme_id DESC";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvProgrammes.DataSource = dt;
                gvProgrammes.DataBind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"INSERT INTO PROGRAMME(programme_name, code, duration_years, department)
                             VALUES(@name, @code, @duration, @department)";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@name", txtProgrammeName.Text.Trim());
                cmd.Parameters.AddWithValue("@code", txtCode.Text.Trim());
                cmd.Parameters.AddWithValue("@duration", txtDuration.Text.Trim());
                cmd.Parameters.AddWithValue("@department", txtDepartment.Text.Trim());

                con.Open();
                cmd.ExecuteNonQuery();
            }

            Clear();
            LoadProgrammes();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfProgrammeID.Value))
                return;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"UPDATE PROGRAMME
                             SET programme_name = @name,
                                 code = @code,
                                 duration_years = @duration,
                                 department = @department
                             WHERE programme_id = @id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@name", txtProgrammeName.Text.Trim());
                cmd.Parameters.AddWithValue("@code", txtCode.Text.Trim());
                cmd.Parameters.AddWithValue("@duration", txtDuration.Text.Trim());
                cmd.Parameters.AddWithValue("@department", txtDepartment.Text.Trim());
                cmd.Parameters.AddWithValue("@id", hfProgrammeID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            Clear();
            LoadProgrammes();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfProgrammeID.Value))
                return;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = "DELETE FROM PROGRAMME WHERE programme_id = @id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@id", hfProgrammeID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            Clear();
            LoadProgrammes();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        protected void gvProgrammes_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvProgrammes.SelectedRow;

            hfProgrammeID.Value = gvProgrammes.DataKeys[row.RowIndex].Value.ToString();

            txtProgrammeName.Text = row.Cells[2].Text;
            txtCode.Text = row.Cells[3].Text;
            txtDuration.Text = row.Cells[4].Text;
            txtDepartment.Text = row.Cells[5].Text;
        }

        private void Clear()
        {
            hfProgrammeID.Value = "";
            txtProgrammeName.Text = "";
            txtCode.Text = "";
            txtDuration.Text = "";
            txtDepartment.Text = "";
        }
    }
}