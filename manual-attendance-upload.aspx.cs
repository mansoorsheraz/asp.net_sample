using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

public partial class manual_attendance_upload : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        conn myconnobj = new conn();
        myconnobj.conOpen();
        btnprint.PostBackUrl = "";
        Button1.PostBackUrl = "";


        if (myconnobj.checkSession() == 0)
        {
            Response.Redirect("login.aspx");
            Response.End();
        }
        else
        {
            lblwelcome.Text = Session["userName"].ToString();
        }

        lbldept.Text = "";

        if (!IsPostBack)
        {
            
            
            SqlDataReader deptreader;

            deptreader = myconnobj.getUserDepts(Session["userName"].ToString());
            departments.DataSource = deptreader;
            departments.DataTextField = "DeptName";
            departments.DataValueField = "DeptID";
            departments.DataBind();
            myconnobj.conClose();
            employees.Items.Insert(0, new ListItem("Select Employee", "0"));
            departments.Items.Insert(0, new ListItem("Select Department", "0"));
            lbldept.Text = "";
            txtdatefrom.Text = "";
            txtremarks.Text = "";




        }
    }


    protected void Dept_Changed(object sender, EventArgs e)
    {
        employees.Enabled = false;
        employees.Items.Clear();
        employees.Items.Insert(0, new ListItem("Select Employee", "0"));
        int deptId;
        deptId = Convert.ToInt32(departments.SelectedValue.ToString());
        
        if (deptId > 0)
        {

            conn myconn = new conn();
            myconn.conOpen();
            SqlDataReader empreader;
            empreader = myconn.getEmployees(deptId);
            employees.DataTextField = "FullName";
            employees.DataValueField = "ID";
            employees.DataSource = empreader;
            employees.DataBind();
            employees.Enabled = true;
            myconn.conClose();
        }
         
    }


    protected void Button1_Click(object sender, EventArgs e)
    {
        try { 
        conn myconn = new conn();
        myconn.conOpen();
        int empid;
        string remarks, date, ip;
        empid = Convert.ToInt32(employees.SelectedValue.ToString());
        remarks = txtremarks.Text.ToString();
        date = txtdatefrom.Text.ToString();
        ip = Request.ServerVariables["REMOTE_ADDR"];
        myconn.markAttendance(empid, date, remarks, ip, Convert.ToInt32(Session["userID"].ToString()));
        myconn.conClose();
        lbldept.Text = "Attendance Marked Successfully";

        txtdatefrom.Text = "";
        txtremarks.Text = "";
        }
        catch (Exception ex)
        {
            lbldept.Text = ex.Message.ToString();

        }


    }
}