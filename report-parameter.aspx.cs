using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Sql;
using System.Data.SqlClient;

public partial class report_parameter : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        conn myconnobj = new conn();

        if (myconnobj.checkSession() == 0)
        {
            Response.Redirect("login.aspx");
            Response.End();

        }
        else
        {
            lblwelcome.Text = Session["userName"].ToString();
        }

        Label1.Text = Session["querytitle"].ToString();
        int queryid, arrindex = 0, flag_has_rights = 0;
        string parameters, paramhelptext, qallowedroles, querydetail;
        string[] pmtr, pmtrhelp, qar;
        if (Session["QUERYID"] != null)
        {

            queryid = Convert.ToInt32(Session["QUERYID"]);

            conn myconn = new conn();
            myconn.conOpen();
            SqlDataReader myreader;

            myreader = myconn.getQueryParameters(queryid);
            while (myreader.Read())
            {
                //Response.Write(myreader[2].ToString());
                querydetail = myreader[6].ToString();
                Label2.Text = querydetail.ToString();
                Label3.Text = myreader[7].ToString();
                parameters = myreader[2].ToString();
                qallowedroles = myreader[5].ToString();
                qar = qallowedroles.Split(',');
                foreach(string j in qar){
                    if (Session["userRoles"].ToString() == j.ToString().Trim())
                    {
                        flag_has_rights = 1;
                    }
                }
                if (flag_has_rights == 0)
                {
                    Response.Redirect("reports.aspx");
                    Response.End();
                }
                paramhelptext = myreader[4].ToString();
                Session["QUERYPARAMETERS"] = parameters;
                pmtr = parameters.Split(',');
                pmtrhelp = paramhelptext.Split(',');

		//creating table to display fields.
                paramdiv.Controls.Add(new LiteralControl("<br /><table border='0' cellpadding='0' width='500'>"));
                foreach (string i in pmtr)
                {
		   // this loop searches for the required input and create run time text boxes. input parameters come from the database as they are stored as comma separated

			
                    Label mylbl = new Label();
                    mylbl.Text = i.ToString();
                    Label helplbl = new Label();
                    helplbl.Text = pmtrhelp[arrindex].ToString();
                    TextBox mytxt = new TextBox();

                    paramdiv.Controls.Add(new LiteralControl("<tr><td style='font-family:arial;font-size:11pt;font-weight:bold;color:#333333;'>"));
                    mytxt.ID = i.ToString();


                    if (Session["userRoles"].ToString() == "Faculty")
                    {
                        if (i.ToString() == "Email")
                        {
			    // check for faculty console so that the bydefault parameter for facultyemail is set
                            mytxt.Text = Session["facultyEmail"].ToString();
                            mytxt.ReadOnly = true;
                            
                        }
                    }
                    paramdiv.Controls.Add(mylbl);
                    paramdiv.Controls.Add(new LiteralControl("</td><td>"));
                    paramdiv.Controls.Add(mytxt);
                    paramdiv.Controls.Add(new LiteralControl("</td><td width='200' style='font-family:arial;font-size:08pt;color:#007700;'>e.g. "));
                    paramdiv.Controls.Add(helplbl);
                    paramdiv.Controls.Add(new LiteralControl("</tr>"));
                    arrindex++;

                }
               
                paramdiv.Controls.Add(new LiteralControl("</table><br />"));
                Button mybutton = new Button();
                mybutton.PostBackUrl = "show-report-parameter.aspx";
                mybutton.Text = "Show Report";
              
                paramdiv.Controls.Add(mybutton);
            }
            myconn.conClose();

        }
    }
}