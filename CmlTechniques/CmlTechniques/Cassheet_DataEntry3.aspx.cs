﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using BusinessLogic;
using App_Properties;
namespace CmlTechniques
{
    public partial class Cassheet_DataEntry3 : System.Web.UI.Page
    {
        public static DataTable _dtMaster;
        //public static DataTable _dtfilter;
        public static DataTable _dtresult;
        public static bool _exp;
        protected void Page_Load(object sender, EventArgs e)
        {
            _ReadCookies();
            if (!IsPostBack)
            {
                string _prm = Request.QueryString[0].ToString();
                //ScriptManager.RegisterStartupScript(this, typeof(string), "close", "alert('" + _prm + "');", true);
                Session["project"] = _prm.Substring(0, _prm.IndexOf("_S"));
                Session["sch"] = _prm.Substring(_prm.IndexOf("_S") + 2);
                settings();
                load_cas_sys();
                Load_Master();
                Session["filter"] = "no";
                Session["zone"] = "All";
                Session["flvl"] = "All";
                Session["cat"] = "All";
                Session["fed"] = "All";
                Session["loc"] = "All";
                Session["des"] = "All";
                Load_InitialData();
                //Load_Filtering_Elements();
                Load_Flvl();
                Hide_Details();

                _exp = false;
                drdes.Visible = false;
                drfed.Visible = false;
            }
        }
        protected void load_cas_sys()
        {
            BLL_Dml _objbll = new BLL_Dml();
            _database _objdb = new _database();
            _objdb.DBName = "DB_" + (string)Session["project"];
            _clscassheet _objcls = new _clscassheet();
            _objcls.sch = Convert.ToInt32((string)Session["sch"]);
            DataTable _dt0 = _objbll.Load_cas_sys(_objcls, _objdb);
            DataTable _dt1 = new DataTable();
            _dt1.Columns.Add("_id");
            _dt1.Columns.Add("_name");
            var _List = from o in _dt0.AsEnumerable()
                        where o.Field<string>(3) == (string)Session["project"]
                        select o;

            foreach (var row in _List)
            {
                DataRow _myrow = _dt1.NewRow();
                _myrow[0] = row[0].ToString() + "_C" + row[2].ToString();
                _myrow[1] = row[1].ToString();
                _dt1.Rows.Add(_myrow);
            }
            drpackage.DataSource = _dt1;
            drpackage.DataTextField = "_name";
            drpackage.DataValueField = "_id";
            drpackage.DataBind();
            drpackage.Items.Insert(0, "--Package--");
        }
        protected void settings()
        {
            if ((string)Session["sch"] == "101")
                lblhead.Text = "6.4.10.1 - Fire Protection Plant Commissioning Activity Schedule";
            else if ((string)Session["sch"] == "97")
                lblhead.Text = "6.4.13 - Fire Fighting - Aqueous Fire Fighting Form System Commissioning Activity Schedule";
            else if ((string)Session["sch"] == "95")
                lblhead.Text = "6.4.11 - Fire Fighting - Clean Agent Extinguishing System Commissioning Activity Schedule";

        }
        void _ReadCookies()
        {
            if (Request.Browser.Cookies)
            {
                if (Request.Cookies["uid"] != null)
                {
                    Session["uid"] = Server.HtmlEncode(Request.Cookies["uid"].Value);
                }
                //if (Request.Cookies["project"] != null)
                //{
                //    Session["project"] = Server.HtmlEncode(Request.Cookies["project"].Value);
                //}
                //if (Request.Cookies["sch"] != null)
                //{
                //    Session["sch"] = Server.HtmlEncode(Request.Cookies["sch"].Value);
                //}
                if (Request.Cookies["srv"] != null)
                {
                    Session["srv"] = Server.HtmlEncode(Request.Cookies["srv"].Value);
                }
            }
        }
        void _Create_Cookies()
        {
            if (Request.Browser.Cookies)
            {
                HttpCookie _CookieSch = new HttpCookie("sch");
                _CookieSch.Value = (string)Session["sch"];
                _CookieSch.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(_CookieSch);
                HttpCookie _CookieSrv = new HttpCookie("srv");
                _CookieSrv.Value = (string)Session["srv"];
                _CookieSrv.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(_CookieSrv);

            }
            else
                return;
        }
        private void Load_Master()
        {
            _dtMaster = new DataTable();
            BLL_Dml _objbll = new BLL_Dml();
            _database _objdb = new _database();
            _objdb.DBName = "DB_" + (string)Session["project"];
            _clscassheet _objcas = new _clscassheet();
            _objcas.sch = Convert.ToInt32((string)Session["sch"]);
            _objcas.prj_code = (string)Session["project"];
            _dtMaster = _objbll.Load_casMain_Edit(_objcas, _objdb);
            _dtresult = _dtMaster;
        }
        private void Load_InitialData()
        {
            DataTable _dtable = new DataTable();
            _dtable.Columns.Add("sys_id", typeof(string));
            _dtable.Columns.Add("sys_name", typeof(string));
            var distinctRows = (from DataRow dRow in _dtresult.Rows
                                select new { col1 = dRow["Sys_Id"], col2 = dRow["Sys_name"] }).Distinct();
            foreach (var row in distinctRows)
            {
                DataRow _row = _dtable.NewRow();
                _row[0] = row.col1.ToString();
                _row[1] = row.col2.ToString();
                _dtable.Rows.Add(_row);
            }
            mymaster.DataSource = _dtable;
            mymaster.DataBind();
            Load_Filtering_Elements();

        }
        protected void mymaster_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label _sys_id = (Label)e.Row.FindControl("lbSys_Id");
                DataTable _dtdetails = new DataTable();
                _dtdetails.Columns.Add("e_b_ref", typeof(string));
                _dtdetails.Columns.Add("B_Z", typeof(string));
                _dtdetails.Columns.Add("Cat", typeof(string));
                _dtdetails.Columns.Add("F_lvl", typeof(string));
                _dtdetails.Columns.Add("Seq_No", typeof(string));
                _dtdetails.Columns.Add("Loc", typeof(string));
                _dtdetails.Columns.Add("P_P_to", typeof(string));
                _dtdetails.Columns.Add("F_from", typeof(string));
                _dtdetails.Columns.Add("Substation", typeof(string));
                _dtdetails.Columns.Add("devices1", typeof(string));
                _dtdetails.Columns.Add("C_id", typeof(int));
                _dtdetails.Columns.Add("Sys_name", typeof(string));
                _dtdetails.Columns.Add("Sys_id", typeof(int));
                _dtdetails.Columns.Add("Des", typeof(string));
                _dtdetails.Columns.Add("devices2", typeof(string));
                _dtdetails.Columns.Add("S_c_m", typeof(string));
                var _details = from _data in _dtresult.AsEnumerable()
                               where _data.Field<int>("Sys_Id") == Convert.ToInt32(_sys_id.Text)
                               select _data;
                foreach (var row in _details)
                {
                    DataRow _row = _dtdetails.NewRow();
                    _row[0] = row["e_b_ref"].ToString();
                    _row[1] = row["B_Z"].ToString();
                    _row[2] = row["Cat"].ToString();
                    _row[3] = row["F_lvl"].ToString();
                    _row[4] = SeqNo(row["Seq_No"].ToString());
                    _row[5] = row["Loc"].ToString();
                    _row[6] = row["p_p_to"].ToString();
                    _row[7] = row["F_from"].ToString();
                    _row[8] = row["Substation"].ToString();
                    _row[9] = row["devices1"].ToString();
                    _row[10] = row["C_id"].ToString();
                    _row[11] = row["Sys_name"].ToString();
                    _row[12] = row["Sys_id"].ToString();
                    _row[13] = row["Des"].ToString();
                    _row[14] = row["devices2"].ToString();
                    _row[15] = row["S_c_m"].ToString();
                    _dtdetails.Rows.Add(_row);
                }
                GridView _mydetails = (GridView)e.Row.FindControl("mydetails");
                _mydetails.DataSource = _dtdetails;
                _mydetails.DataBind();
                Button _btn = (Button)e.Row.FindControl("Button1");
                _btn.Text = "+";
                _mydetails.Visible = false;
            }
        }
        private string SeqNo(string No)
        {
            string _nNo = No;
            if (No.Length > 3)
                _nNo = No.Substring(0, 3);
            else
            {
                for (int i = 0; i < (3 - No.Length); i++)
                {
                    _nNo = "0" + _nNo;
                }
            }
            return _nNo;
        }
        private void Hide_Details()
        {
            for (int i = 0; i <= mymaster.Rows.Count - 1; i++)
            {
                GridView _mydetails = (GridView)mymaster.Rows[i].FindControl("mydetails");
                Button _btn = (Button)mymaster.Rows[i].FindControl("Button1");
                _mydetails.Visible = false;
                _btn.Text = "+";
            }
        }
        private void Open_Details(int mode)
        {
            string sys = "";
            if (mode == 1)
                sys = drpackage.SelectedItem.Value.Substring(0, drpackage.SelectedItem.Value.IndexOf("_C"));
            else
                sys = (string)Session["Sys"];
            for (int i = 0; i <= mymaster.Rows.Count - 1; i++)
            {
                Label _sys_id = (Label)mymaster.Rows[i].FindControl("lbSys_Id");
                if (_sys_id.Text == sys)
                {
                    GridView _mydetails = (GridView)mymaster.Rows[i].FindControl("mydetails");
                    Button _btn = (Button)mymaster.Rows[i].FindControl("Button1");
                    _mydetails.Visible = true;
                    _btn.Text = "-";
                }
            }
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)(sender as Control).Parent.Parent;
            int index = gvRow.RowIndex;
            //ScriptManager.RegisterStartupScript(this, typeof(string), "close", "alert('" + index.ToString() + "');", true);
            GridView _mydetails = (GridView)gvRow.FindControl("mydetails");
            Button _btn = (Button)gvRow.FindControl("Button1");
            if (_btn.Text == "+")
            {
                _mydetails.Visible = true;
                _btn.Text = "-";
            }
            else if (_btn.Text == "-")
            {
                _mydetails.Visible = false;
                _btn.Text = "+";
            }
            _exp = true;
        }
        protected void mydetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow gvRow = (GridViewRow)(sender as Control).Parent.Parent;
            GridView _mydetails = (GridView)gvRow.FindControl("mydetails");
            int _idx = Convert.ToInt32(e.CommandArgument);
            GridViewRow _srow = _mydetails.Rows[_idx];
            // _mydetails.Rows[_idx].Style.Add("background-color", "yellow");
            //int index = gvRow.RowIndex;
            TableCell _item1 = _srow.Cells[15];
            TableCell _item2 = _srow.Cells[16];
            // TableCell _item3 = _srow.Cells[2];
            //string _file = _item.Text;
            //ScriptManager.RegisterStartupScript(this, typeof(string), "close", "alert('" + _file + "');", true);
            Session["casid"] = _item1.Text;
            Session["Sys"] = _item2.Text;
            Session["idx"] = _idx.ToString();
            btnDummy_ModalPopupExtender.Show();
            arrange_edit();

        }
        protected void arrange_edit()
        {
            //int _idx = Convert.ToInt32((string)Session["idx"]);
            //GridViewRow _myrow = mymaster.SelectedRow;
            //GridView _mydetails = (GridView)_myrow.FindControl("mydetails");
            //if (_mydetails.Rows[_idx].Cells[2].Text.Trim() != "&nbsp;") upreff.Text = _mydetails.Rows[_idx].Cells[2].Text;
            //if (_mydetails.Rows[_idx].Cells[3].Text.Trim() != "&nbsp;") upzone.Text = _mydetails.Rows[_idx].Cells[3].Text;
            //if (_mydetails.Rows[_idx].Cells[4].Text.Trim() != "&nbsp;") upcate.Text = _mydetails.Rows[_idx].Cells[4].Text;
            //if (_mydetails.Rows[_idx].Cells[5].Text.Trim() != "&nbsp;") upfloor.Text = _mydetails.Rows[_idx].Cells[5].Text;
            //if (_mydetails.Rows[_idx].Cells[6].Text.Trim() != "&nbsp;") upseq.Text = _mydetails.Rows[_idx].Cells[6].Text;
            //if (_mydetails.Rows[_idx].Cells[7].Text.Trim() != "&nbsp;") uploc.Text = _mydetails.Rows[_idx].Cells[7].Text;
            //if (_mydetails.Rows[_idx].Cells[8].Text.Trim() != "&nbsp;") uplb1.Text = _mydetails.Rows[_idx].Cells[8].Text;
            //if (_mydetails.Rows[_idx].Cells[10].Text.Trim() != "&nbsp;") uplb2.Text = _mydetails.Rows[_idx].Cells[10].Text;
            //if (_mydetails.Rows[_idx].Cells[9].Text.Trim() != "&nbsp;") uplb3.Text = _mydetails.Rows[_idx].Cells[9].Text;
            //if (_mydetails.Rows[_idx].Cells[11].Text.Trim() != "&nbsp;") uplb4.Text = _mydetails.Rows[_idx].Cells[11].Text;
            //upreff.Text = upreff.Text.Replace("&amp;", "&"); upzone.Text = upzone.Text.Replace("&amp;", "&"); upfloor.Text = upfloor.Text.Replace("&amp;", "&"); uploc.Text = uploc.Text.Replace("&amp;", "&"); uplb1.Text = uplb1.Text.Replace("&amp;", "&"); uplb2.Text = uplb2.Text.Replace("&amp;", "&"); uplb3.Text = uplb3.Text.Replace("&amp;", "&"); uplb4.Text = uplb4.Text.Replace("&amp;", "&");
            Load_Master();
            var _result = from _data in _dtMaster.AsEnumerable()
                          where _data.Field<int>(0) == Convert.ToInt32((string)Session["casid"])
                          select _data;
            foreach (var row in _result)
            {
                upreff.Text = row[4].ToString();
                upzone.Text = row[5].ToString();
                upcate.Text = row[6].ToString();
                upfloor.Text = row[7].ToString();
                upseq.Text = row[48].ToString();
                uploc.Text = row[10].ToString();
                uparea.Text = row[11].ToString();
                upfed.Text = row[12].ToString();
                updesc.Text = row[9].ToString();
                upload.Text = row[13].ToString();
                upstype.Text = row[15].ToString();
                updutyp.Text = row[22].ToString();
                updutyv.Text = row[21].ToString();
            }



        }
        protected void mydetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells[15].Visible = false;
            e.Row.Cells[16].Visible = false;

            //e.Row.Cells[0].Enabled = false;
            //if (e.Row.Cells[1].Text != "") return;
            //if (e.Row.Cells[0].Text != "")
            //    e.Row.Cells[0].Text = "hai" + e.Row.Cells[0].Text;
            e.Row.Cells[1].Text = (e.Row.DataItemIndex + 1).ToString();
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                //e.Row.Cells[7].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
                //e.Row.Cells[8].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
                //e.Row.Cells[9].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
                //e.Row.Cells[10].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");

                e.Row.Cells[12].Attributes.Add("style", "word-wrap:break-word");
                e.Row.Cells[13].Attributes.Add("style", "word-wrap:break-word");
                e.Row.Cells[14].Attributes.Add("style", "word-wrap:break-word");
            }
        }
        private void Filtering(string _el1, string _el2, string _el3, string _el4, string _el5, string _el6)
        {
            if (_el1 == "") _el1 = "All";
            if (_el2 == "") _el2 = "All";
            if (_el3 == "") _el3 = "All";
            if (_el4 == "") _el4 = "All";
            if (_el5 == "") _el5 = "All";
            if (_el6 == "") _el6 = "All";
            Load_Master();
            DataTable _dtfilter = _dtMaster;
            _dtresult = new DataTable();
            _dtresult.Columns.Add("e_b_ref", typeof(string));
            _dtresult.Columns.Add("B_Z", typeof(string));
            _dtresult.Columns.Add("Cat", typeof(string));
            _dtresult.Columns.Add("F_lvl", typeof(string));
            _dtresult.Columns.Add("Seq_No", typeof(string));
            _dtresult.Columns.Add("Loc", typeof(string));
            _dtresult.Columns.Add("P_P_to", typeof(string));
            _dtresult.Columns.Add("F_from", typeof(string));
            _dtresult.Columns.Add("Substation", typeof(string));
            _dtresult.Columns.Add("devices1", typeof(string));
            _dtresult.Columns.Add("C_id", typeof(int));
            _dtresult.Columns.Add("Sys_name", typeof(string));
            _dtresult.Columns.Add("Sys_id", typeof(int));
            _dtresult.Columns.Add("Des", typeof(string));
            _dtresult.Columns.Add("devices2", typeof(string));
            _dtresult.Columns.Add("S_c_m", typeof(string));
            var _filter = from o in _dtfilter.AsEnumerable()
                          select o;
            if (_el1 == "All" && _el2 == "All" && _el3 == "All" && _el4 == "All" && _el5 == "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          select o;
            }
            else if (_el1 != "All" && _el2 == "All" && _el3 == "All" && _el4 == "All" && _el5 == "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1
                          select o;
            }
            else if (_el1 != "All" && _el2 != "All" && _el3 == "All" && _el4 == "All" && _el5 == "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("Cat") == _el2
                          select o;
            }
            else if (_el1 != "All" && _el2 != "All" && _el3 != "All" && _el4 == "All" && _el5 == "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("Cat") == _el2 && o.Field<string>("F_lvl") == _el3
                          select o;
            }
            else if (_el1 != "All" && _el2 != "All" && _el3 != "All" && _el4 != "All" && _el5 == "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("Cat") == _el2 && o.Field<string>("F_lvl") == _el3 && o.Field<string>("F_from") == _el4
                          select o;
            }
            else if (_el1 != "All" && _el2 != "All" && _el3 != "All" && _el4 != "All" && _el5 != "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("Cat") == _el2 && o.Field<string>("F_lvl") == _el3 && o.Field<string>("F_from") == _el4 && o.Field<string>("Loc") == _el5
                          select o;
            }
            else if (_el1 != "All" && _el2 != "All" && _el3 != "All" && _el4 != "All" && _el5 != "All" && _el6 != "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("Cat") == _el2 && o.Field<string>("F_lvl") == _el3 && o.Field<string>("F_from") == _el4 && o.Field<string>("Loc") == _el5 && o.Field<string>("Des") == _el6
                          select o;
            }
            else if (_el1 == "All" && _el2 != "All" && _el3 == "All" && _el4 == "All" && _el5 == "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("Cat") == _el2
                          select o;
            }
            else if (_el1 == "All" && _el2 != "All" && _el3 != "All" && _el4 == "All" && _el5 == "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("Cat") == _el2 && o.Field<string>("F_lvl") == _el3
                          select o;
            }
            else if (_el1 == "All" && _el2 != "All" && _el3 != "All" && _el4 != "All" && _el5 == "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("Cat") == _el2 && o.Field<string>("F_lvl") == _el3 && o.Field<string>("F_from") == _el4
                          select o;
            }
            else if (_el1 == "All" && _el2 != "All" && _el3 != "All" && _el4 != "All" && _el5 != "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("Cat") == _el2 && o.Field<string>("F_lvl") == _el3 && o.Field<string>("F_from") == _el4 && o.Field<string>("Loc") == _el5
                          select o;
            }
            else if (_el1 == "All" && _el2 == "All" && _el3 != "All" && _el4 == "All" && _el5 == "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("F_lvl") == _el3
                          select o;
            }
            else if (_el1 == "All" && _el2 == "All" && _el3 != "All" && _el4 != "All" && _el5 == "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("F_lvl") == _el3 && o.Field<string>("F_from") == _el4
                          select o;
            }
            else if (_el1 == "All" && _el2 == "All" && _el3 != "All" && _el4 != "All" && _el5 != "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("F_lvl") == _el3 && o.Field<string>("F_from") == _el4 && o.Field<string>("Loc") == _el5
                          select o;
            }
            else if (_el1 == "All" && _el2 == "All" && _el3 != "All" && _el4 != "All" && _el5 != "All" && _el6 != "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("F_lvl") == _el3 && o.Field<string>("F_from") == _el4 && o.Field<string>("Loc") == _el5 && o.Field<string>("Des") == _el6
                          select o;
            }
            else if (_el1 == "All" && _el2 == "All" && _el3 == "All" && _el4 != "All" && _el5 == "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("F_from") == _el4
                          select o;
            }
            else if (_el1 == "All" && _el2 == "All" && _el3 == "All" && _el4 != "All" && _el5 != "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("F_from") == _el4 && o.Field<string>("Loc") == _el5
                          select o;
            }
            else if (_el1 == "All" && _el2 == "All" && _el3 == "All" && _el4 != "All" && _el5 != "All" && _el6 != "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("F_from") == _el4 && o.Field<string>("Loc") == _el5 && o.Field<string>("Des") == _el6
                          select o;
            }
            else if (_el1 == "All" && _el2 == "All" && _el3 == "All" && _el4 == "All" && _el5 != "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("Loc") == _el5
                          select o;
            }
            else if (_el1 == "All" && _el2 == "All" && _el3 == "All" && _el4 == "All" && _el5 == "All" && _el6 != "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("Des") == _el6
                          select o;
            }
            else if (_el1 != "All" && _el2 == "All" && _el3 != "All" && _el4 == "All" && _el5 == "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("F_lvl") == _el3
                          select o;
            }
            else if (_el1 != "All" && _el2 == "All" && _el3 != "All" && _el4 != "All" && _el5 == "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("F_lvl") == _el3 && o.Field<string>("F_from") == _el4
                          select o;
            }
            else if (_el1 != "All" && _el2 == "All" && _el3 != "All" && _el4 != "All" && _el5 != "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("F_lvl") == _el3 && o.Field<string>("F_from") == _el4 && o.Field<string>("Loc") == _el5
                          select o;
            }
            else if (_el1 != "All" && _el2 == "All" && _el3 != "All" && _el4 != "All" && _el5 != "All" && _el6 != "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("F_lvl") == _el3 && o.Field<string>("F_from") == _el4 && o.Field<string>("Loc") == _el5 && o.Field<string>("Des") == _el6
                          select o;
            }
            else if (_el1 != "All" && _el2 == "All" && _el3 == "All" && _el4 != "All" && _el5 == "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("F_from") == _el4
                          select o;
            }
            else if (_el1 != "All" && _el2 == "All" && _el3 == "All" && _el4 != "All" && _el5 != "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("F_from") == _el4 && o.Field<string>("Loc") == _el5
                          select o;
            }
            else if (_el1 != "All" && _el2 == "All" && _el3 == "All" && _el4 != "All" && _el5 != "All" && _el6 != "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("F_from") == _el4 && o.Field<string>("Loc") == _el5 && o.Field<string>("Des") == _el6
                          select o;
            }
            else if (_el1 != "All" && _el2 == "All" && _el3 == "All" && _el4 == "All" && _el5 != "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("Loc") == _el5
                          select o;
            }
            else if (_el1 != "All" && _el2 == "All" && _el3 == "All" && _el4 == "All" && _el5 == "All" && _el6 != "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("Des") == _el6
                          select o;
            }
            else if (_el1 == "All" && _el2 != "All" && _el3 == "All" && _el4 != "All" && _el5 == "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("Cat") == _el2 && o.Field<string>("F_from") == _el4
                          select o;
            }
            else if (_el1 == "All" && _el2 != "All" && _el3 == "All" && _el4 != "All" && _el5 != "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("Cat") == _el2 && o.Field<string>("F_from") == _el4 && o.Field<string>("Loc") == _el5
                          select o;
            }
            else if (_el1 == "All" && _el2 != "All" && _el3 == "All" && _el4 != "All" && _el5 != "All" && _el6 != "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("Cat") == _el2 && o.Field<string>("F_from") == _el4 && o.Field<string>("Loc") == _el5 && o.Field<string>("Des") == _el6
                          select o;
            }
            else if (_el1 == "All" && _el2 != "All" && _el3 == "All" && _el4 != "All" && _el5 == "All" && _el6 != "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("Cat") == _el2 && o.Field<string>("F_from") == _el4 && o.Field<string>("Des") == _el6
                          select o;
            }
            else if (_el1 == "All" && _el2 != "All" && _el3 == "All" && _el4 == "All" && _el5 != "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("Cat") == _el2 && o.Field<string>("Loc") == _el5
                          select o;
            }
            else if (_el1 == "All" && _el2 != "All" && _el3 == "All" && _el4 == "All" && _el5 != "All" && _el6 != "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("Cat") == _el2 && o.Field<string>("Loc") == _el5 && o.Field<string>("Des") == _el6
                          select o;
            }
            else if (_el1 == "All" && _el2 != "All" && _el3 == "All" && _el4 == "All" && _el5 == "All" && _el6 != "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("Cat") == _el2 && o.Field<string>("Des") == _el6
                          select o;
            }
            else if (_el1 != "All" && _el2 != "All" && _el3 == "All" && _el4 != "All" && _el5 == "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("Cat") == _el2 && o.Field<string>("F_from") == _el4
                          select o;
            }
            else if (_el1 != "All" && _el2 != "All" && _el3 == "All" && _el4 != "All" && _el5 != "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("Cat") == _el2 && o.Field<string>("F_from") == _el4 && o.Field<string>("Loc") == _el5
                          select o;
            }
            else if (_el1 != "All" && _el2 != "All" && _el3 == "All" && _el4 != "All" && _el5 != "All" && _el6 != "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("Cat") == _el2 && o.Field<string>("F_from") == _el4 && o.Field<string>("Loc") == _el5 && o.Field<string>("Des") == _el6
                          select o;
            }
            else if (_el1 != "All" && _el2 != "All" && _el3 == "All" && _el4 == "All" && _el5 != "All" && _el6 == "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("Cat") == _el2 && o.Field<string>("Loc") == _el5
                          select o;
            }
            else if (_el1 != "All" && _el2 != "All" && _el3 == "All" && _el4 == "All" && _el5 != "All" && _el6 != "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("B_Z") == _el1 && o.Field<string>("Cat") == _el2 && o.Field<string>("Loc") == _el5 && o.Field<string>("Des") == _el6
                          select o;
            }
            else if (_el1 == "All" && _el2 == "All" && _el3 != "All" && _el4 == "All" && _el5 != "All" && _el6 != "All")
            {
                _filter = from o in _dtfilter.AsEnumerable()
                          where o.Field<string>("F_lvl") == _el3 && o.Field<string>("Loc") == _el5
                          select o;
            }
            foreach (var row in _filter)
            {
                DataRow _row = _dtresult.NewRow();
                _row[0] = row["e_b_ref"].ToString();
                _row[1] = row["B_Z"].ToString();
                _row[2] = row["Cat"].ToString();
                _row[3] = row["F_lvl"].ToString();
                _row[4] = row["Seq_No"].ToString();
                _row[5] = row["Loc"].ToString();
                _row[6] = row["p_p_to"].ToString();
                _row[7] = row["F_from"].ToString();
                _row[8] = row["Substation"].ToString();
                _row[9] = row["devices1"].ToString();
                _row[10] = row["C_id"].ToString();
                _row[11] = row["Sys_name"].ToString();
                _row[12] = row["Sys_id"].ToString();
                _row[13] = row["Des"].ToString();
                _row[14] = row["devices2"].ToString();
                _row[15] = row["S_c_m"].ToString();
                _dtresult.Rows.Add(_row);
            }
            //m.DataSource = _dtresult;
            //mygrid.DataBind();
            Load_InitialData();

        }
        protected void drbuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["zone"] = drbuilding.SelectedItem.Value;
            Filtering(drbuilding.SelectedItem.Text, drcategory.SelectedItem.Text, drflevel.SelectedItem.Text, drfed.SelectedItem.Text, drloc.SelectedItem.Text, drdes.SelectedItem.Text);
        }
        protected void drcategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["cat"] = drcategory.SelectedItem.Value;
            Filtering(drbuilding.SelectedItem.Text, drcategory.SelectedItem.Text, drflevel.SelectedItem.Text, drfed.SelectedItem.Text, drloc.SelectedItem.Text, drdes.SelectedItem.Text);
        }
        protected void drflevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["flvl"] = drflevel.SelectedItem.Value;
            Filtering(drbuilding.SelectedItem.Text, drcategory.SelectedItem.Text, drflevel.SelectedItem.Text, drfed.SelectedItem.Text, drloc.SelectedItem.Text, drdes.SelectedItem.Text);
        }
        protected void drfed_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["fed"] = drfed.SelectedItem.Value;
            Filtering(drbuilding.SelectedItem.Text, drcategory.SelectedItem.Text, drflevel.SelectedItem.Text, drfed.SelectedItem.Text, drloc.SelectedItem.Text, drdes.SelectedItem.Text);
        }
        protected void drpackage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpackage.SelectedItem.Text != "--Package--")
            {
                txtcate.Text = drpackage.SelectedItem.Value.Substring(drpackage.SelectedItem.Value.IndexOf("_C") + 2);
                Get_Position();
                Load_Flvl();
            }
        }
        private void Get_Position()
        {
            string Sys_Id = drpackage.SelectedItem.Value.Substring(0, drpackage.SelectedItem.Value.IndexOf("_C"));
            BLL_Dml _objbll = new BLL_Dml();
            _database _objdb = new _database();
            _clscassheet _objcls = new _clscassheet();
            _objdb.DBName = "DB_" + (string)Session["project"];
            _objcls.sys = Convert.ToInt32(Sys_Id);
            txtitmno.Text = _objbll.Get_Position(_objcls, _objdb).ToString();
        }
        private void Get_SeqNo()
        {
            string Sys_Id = drpackage.SelectedItem.Value.Substring(0, drpackage.SelectedItem.Value.IndexOf("_C"));
            BLL_Dml _objbll = new BLL_Dml();
            _database _objdb = new _database();
            _clscassheet _objcls = new _clscassheet();
            _objdb.DBName = "DB_" + (string)Session["project"];
            _objcls.sys = Convert.ToInt32(Sys_Id);
            _objcls.f_level = drflevel_.SelectedItem.Text;
            _objcls.b_zone = txtzone.Text;
            if (txtitmno.Text.Length <= 0)
            {
                txtitmno.Text = "0";
            }
            _objcls.Position = Convert.ToInt32(txtitmno.Text);
            txtseq.Text = _objbll.Get_Seq(_objcls, _objdb).ToString();
        }
        protected void btnadd_Click(object sender, EventArgs e)
        {
            if (drpackage.SelectedItem.Text == "--Package--")
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "close", "alert('Select Package');", true);
                return;
            }
            else if (txtengref.Text.Length <= 0)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "close", "alert('Enter Eng.Ref');", true);
                return;
            }
            else if (drflevel_.SelectedItem.Text == "- - -")
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "close", "alert('Select Floor Level');", true);
                return;
            }
            
            add_initial_data();
            Load_Master();
            Load_InitialData();
            Open_Details(1);
            Load_Flvl();
        }
        private void Load_Filtering_Elements()
        {
            drbuilding.Items.Clear();
            drcategory.Items.Clear();
            drflevel.Items.Clear();
            drfed.Items.Clear();
            drloc.Items.Clear();
            drdes.Items.Clear();
            var _bzone = (from DataRow dRow in _dtresult.Rows
                          orderby dRow["B_Z"]
                          select new { col1 = dRow["B_Z"] }).Distinct();
            foreach (var row in _bzone)
            {
                ListItem _itm = new ListItem();
                _itm.Text = row.col1.ToString();
                _itm.Value = row.col1.ToString();
                drbuilding.Items.Add(_itm);
            }
            drbuilding.DataBind();
            var _fedf = (from DataRow dRow in _dtresult.Rows
                         orderby dRow["F_from"]
                         select new { col1 = dRow["F_from"] }).Distinct();
            foreach (var row in _fedf)
            {
                ListItem _itm = new ListItem();
                _itm.Text = row.col1.ToString();
                _itm.Value = row.col1.ToString();
                drfed.Items.Add(_itm);
            }
            drfed.DataBind();
            var _cate = (from DataRow dRow in _dtresult.Rows
                         orderby dRow["Cat"]
                         select new { col1 = dRow["Cat"] }).Distinct();
            foreach (var row in _cate)
            {
                ListItem _itm = new ListItem();
                _itm.Text = row.col1.ToString();
                _itm.Value = row.col1.ToString();
                drcategory.Items.Add(_itm);
            }
            drcategory.DataBind();
            var _flvl = (from DataRow dRow in _dtresult.Rows
                         orderby dRow["F_lvl"]
                         select new { col1 = dRow["F_lvl"] }).Distinct();
            foreach (var row in _flvl)
            {
                ListItem _itm = new ListItem();
                _itm.Text = row.col1.ToString();
                _itm.Value = row.col1.ToString();
                drflevel.Items.Add(_itm);
            }
            drflevel.DataBind();
            var _loc = (from DataRow dRow in _dtresult.Rows
                        orderby dRow["Loc"]
                        select new { col1 = dRow["Loc"] }).Distinct();
            foreach (var row in _loc)
            {
                ListItem _itm = new ListItem();
                _itm.Text = row.col1.ToString();
                _itm.Value = row.col1.ToString();
                drloc.Items.Add(_itm);
            }
            drloc.DataBind();
            var _des = (from DataRow dRow in _dtresult.Rows
                        orderby dRow["Des"]
                        select new { col1 = dRow["Des"] }).Distinct();
            foreach (var row in _des)
            {
                ListItem _itm = new ListItem();
                _itm.Text = row.col1.ToString();
                _itm.Value = row.col1.ToString();
                drdes.Items.Add(_itm);
            }
            drdes.DataBind();
            drbuilding.Items.Insert(0, "All");
            drcategory.Items.Insert(0, "All");
            drflevel.Items.Insert(0, "All");
            drfed.Items.Insert(0, "All");
            drloc.Items.Insert(0, "All");
            drdes.Items.Insert(0, "All");
            drbuilding.SelectedValue = (string)Session["zone"];
            drcategory.SelectedValue = (string)Session["cat"];
            drflevel.SelectedValue = (string)Session["flvl"];
            drfed.SelectedValue = (string)Session["fed"];
            drloc.SelectedValue = (string)Session["loc"];
            drdes.SelectedValue = (string)Session["des"];
        }
        protected void btnupdate_Click(object sender, EventArgs e)
        {

            Edit_Inidata();
            Load_Master();
            Load_InitialData();
            Open_Details(0);
            btnDummy_ModalPopupExtender.Hide();
        }
        private void Edit_Inidata()
        {
            
            BLL_Dml _objbll = new BLL_Dml();
            _database _objdb = new _database();
            _objdb.DBName = "DB_" + (string)Session["project"];
            _clscassheet _objcas = new _clscassheet();
            _objcas.c_s_id = 0;
            _objcas.prj_code = (string)Session["project"];
            _objcas.uid = (string)Session["uid"];
            _objcas.sys = Convert.ToInt32((string)Session["Sys"]);
            _objcas.reff = upreff.Text;
            _objcas.b_zone = upzone.Text;
            _objcas.cate = upcate.Text;
            _objcas.f_level = upfloor.Text;
            _objcas.seq_no = Convert.ToInt32(upseq.Text);
            _objcas.desc = updesc.Text;
            _objcas.loca = uploc.Text;
            _objcas.p_power_to = uparea.Text;
            _objcas.fed_from = upfed.Text;
            _objcas.sub_st = upload.Text;
            _objcas.s_c_m = upstype.Text;
            _objcas.dev1 = updutyv.Text;
            _objcas.dev2 = updutyp.Text;
            _objcas.dev3 = "0";
            _objcas.mode = 0;
            _objcas.cas_id = Convert.ToInt32((string)Session["casid"]);
            _objbll.Cassheet_Master(_objcas, _objdb);
        }
        private void Delete_Inidata()
        {
            BLL_Dml _objbll = new BLL_Dml();
            _database _objdb = new _database();
            _objdb.DBName = "DB_" + (string)Session["project"];
            _clscassheet _objcas = new _clscassheet();
            _objcas.c_s_id = 0;
            _objcas.prj_code = (string)Session["project"];
            _objcas.uid = (string)Session["uid"];
            _objcas.sys = Convert.ToInt32((string)Session["Sys"]);
            _objcas.reff = "";
            _objcas.b_zone = "";
            _objcas.cate = "";
            _objcas.f_level = "";
            _objcas.seq_no = 0;
            _objcas.desc = "";
            _objcas.loca = "";
            _objcas.p_power_to = "";
            _objcas.fed_from = "";
            _objcas.sub_st = "";
            _objcas.s_c_m = "";
            _objcas.dev1 = "";
            _objcas.dev2 = "0";
            _objcas.dev3 = "0";
            _objcas.mode = 2;
            _objcas.cas_id = Convert.ToInt32((string)Session["casid"]);
            _objbll.Cassheet_Master(_objcas, _objdb);
        }
        protected void btndelete_Click(object sender, EventArgs e)
        {
            Delete_Inidata();
            Load_Master();
            Load_InitialData();
            Open_Details(0);
            btnDummy_ModalPopupExtender.Hide();
        }
        protected void btncancel_Click(object sender, EventArgs e)
        {
            btnDummy_ModalPopupExtender.Hide();
        }
        protected void add_initial_data()
        {

            string _sys = drpackage.SelectedItem.Value.Substring(0, drpackage.SelectedItem.Value.IndexOf("_C"));
            BLL_Dml _objbll = new BLL_Dml();
            _database _objdb = new _database();
            _objdb.DBName = "DB_" + (string)Session["project"];
            _clscassheet _objcas = new _clscassheet();
            _objcas.c_s_id = Convert.ToInt32((string)Session["sch"]);
            _objcas.prj_code = (string)Session["project"];
            _objcas.uid = (string)Session["uid"];
            _objcas.sys = Convert.ToInt32(_sys);
            _objcas.reff = txtengref.Text;
            _objcas.b_zone = txtzone.Text;
            _objcas.cate = txtcate.Text;
            _objcas.f_level = drflevel_.SelectedItem.Text;
            _objcas.seq_no = Convert.ToInt32(txtseq.Text);
            _objcas.desc = txtdesc.Text;
            _objcas.loca = txtloc.Text;
            _objcas.p_power_to = txtarea.Text;
            _objcas.fed_from = txtfed.Text;
            _objcas.sub_st = txtload.Text;
            _objcas.s_c_m = txtstype.Text;
            _objcas.dev1 = txtdutyv.Text;
            //if ((string)Session["sch"] == "10" || (string)Session["sch"] == "16" || (string)Session["sch"] == "20")
            //    _objcas.dev2 = txtdesc.Text;
            //else
            //    _objcas.dev2 = "0";
            //if ((string)Session["sch"] == "20")
            //    _objcas.dev3 = txtpprovideto.Text;
            //else
            //    _objcas.dev3 = "0";
            _objcas.dev2 = txtdutyp.Text;
            _objcas.dev3 = "";
            _objcas.mode = 1;
            _objcas.cas_id = 0;
            _objcas.Position = Convert.ToInt32(txtitmno.Text);
            _objbll.Cassheet_Master(_objcas, _objdb);
            Get_Position();
            Get_SeqNo();

        }
        static bool IsNumeric(object Expression)
        {
            // Variable to collect the Return value of the TryParse method.
            bool isNum;
            // Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
            double retNum;
            // The TryParse method converts a string in a specified style and culture-specific format to its double-precision floating point number equivalent.
            // The TryParse method does not generate an exception if the conversion fails. If the conversion passes, True is returned. If it does not, False is returned.
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
        private bool IfExistRef()
        {
            //var _result = from _data in _dtMaster.AsEnumerable()
            //              where _data.Field<string>("E_b_ref").ToUpper().Trim() == txtengref.Text.ToUpper().Trim()
            //              select _data;
            //foreach (var row in _result)
            //{
            //    ScriptManager.RegisterStartupScript(this, typeof(string), "close", "alert('Engineer Ref. Already Exist!');", true);
            //    return true;
            //}
            //return false;
            BLL_Dml _objbll = new BLL_Dml();
            _database _objdb = new _database();
            _objdb.DBName = "DB_" + (string)Session["project"];
            _clscassheet _objcls = new _clscassheet();
            _objcls.reff = txtengref.Text;
            if (_objbll.Check_EngRef(_objcls, _objdb) != 0)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "close", "alert('Engineer Ref. Already Exist!');", true);
                return true;
            }
            return false;

        }
        protected void drloc_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["loc"] = drloc.SelectedItem.Value;
            Filtering(drbuilding.SelectedItem.Text, drcategory.SelectedItem.Text, drflevel.SelectedItem.Text, drfed.SelectedItem.Text, drloc.SelectedItem.Text, drdes.SelectedItem.Text);
        }
        protected void btnnewflevel_Click(object sender, EventArgs e)
        {
            txtflvl.Text = String.Empty;
            btnDummy_ModalPopupExtender1.Show();
        }
        protected void btnsave_Click(object sender, EventArgs e)
        {
            if (txtflvl.Text.Length <= 0) return;
            BLL_Dml _objbll = new BLL_Dml();
            _database _objdb = new _database();
            _objdb.DBName = "DB_" + (string)Session["project"];
            _clscassheet _objcas = new _clscassheet();
            _objcas.f_level = txtflvl.Text;
            _objbll.Create_FLevel(_objcas, _objdb);
            Load_Flvl();
            btnDummy_ModalPopupExtender1.Hide();
        }
        private void Load_Flvl()
        {
            BLL_Dml _objbll = new BLL_Dml();
            _database _objdb = new _database();
            _objdb.DBName = "DB_" + (string)Session["project"];
            drflevel_.DataSource = _objbll.Load_Flvl(_objdb);
            drflevel_.DataTextField = "F_Level";
            drflevel_.DataValueField = "F_Level";
            drflevel_.DataBind();
            drflevel_.Items.Insert(0, "- - -");
        }
        protected void btncancel1_Click(object sender, EventArgs e)
        {
            btnDummy_ModalPopupExtender1.Hide();
        }
        protected void drflevel__SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpackage.SelectedItem.Text == "--Package--") return;
            Get_SeqNo();
        }

        protected void btnexpand_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= mymaster.Rows.Count - 1; i++)
            {
                GridView _mydetails = (GridView)mymaster.Rows[i].FindControl("mydetails");
                Button _btn = (Button)mymaster.Rows[i].FindControl("Button1");
                _mydetails.Visible = true;
                _btn.Text = "-";
            }
        }

        protected void btncollapse_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= mymaster.Rows.Count - 1; i++)
            {
                GridView _mydetails = (GridView)mymaster.Rows[i].FindControl("mydetails");
                Button _btn = (Button)mymaster.Rows[i].FindControl("Button1");
                _mydetails.Visible = false;
                _btn.Text = "+";
            }
        }

        protected void chkrow_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            GridViewRow row = (GridViewRow)checkbox.NamingContainer;
            if (checkbox.Checked == true)
                row.BackColor = System.Drawing.Color.Gainsboro;
            else
                row.BackColor = System.Drawing.Color.White;
        }

        protected void btnedit_Click(object sender, EventArgs e)
        {
            int count = 0;
            for (int i = 0; i <= mymaster.Rows.Count - 1; i++)
            {
                GridView _details = (GridView)mymaster.Rows[i].FindControl("mydetails");
                for (int j = 0; j <= _details.Rows.Count - 1; j++)
                {
                    CheckBox checkbox = (CheckBox)_details.Rows[j].FindControl("chkrow");
                    if (checkbox.Checked == true)
                    {
                        count += 1;
                        Session["casid"] = _details.Rows[j].Cells[15].Text;
                        Session["Sys"] = _details.Rows[j].Cells[16].Text;
                        Session["idx"] = j.ToString();
                    }
                }

            }
            if (count == 0) ScriptManager.RegisterStartupScript(this, typeof(string), "close", "alert('Select Row');", true);
            else if (count > 1) ScriptManager.RegisterStartupScript(this, typeof(string), "close", "alert('You can select only one Row at a time to Edit');", true);
            else if (count == 1)
            {
                //ScriptManager.RegisterStartupScript(this, typeof(string), "close", "alert('" + (string)Session["MemberId"] + "',7);", true);

                btnDummy_ModalPopupExtender.Show();
                arrange_edit();
            }
        }

        protected void btndelete1_Click(object sender, EventArgs e)
        {
            int count = 0;
            for (int i = 0; i <= mymaster.Rows.Count - 1; i++)
            {
                GridView _details = (GridView)mymaster.Rows[i].FindControl("mydetails");
                for (int j = 0; j <= _details.Rows.Count - 1; j++)
                {
                    CheckBox checkbox = (CheckBox)_details.Rows[j].FindControl("chkrow");
                    if (checkbox.Checked == true)
                    {
                        count += 1;
                        Session["casid"] = _details.Rows[j].Cells[15].Text;
                        Session["Sys"] = _details.Rows[j].Cells[16].Text;
                        Delete_Inidata();
                    }
                }

            }
            if (count == 0) ScriptManager.RegisterStartupScript(this, typeof(string), "close", "alert('Select Row');", true);
            else
            {
                string _msg = count.ToString() + " Row(s) Deleted";
                ScriptManager.RegisterStartupScript(this, typeof(string), "close", "alert('" + _msg + "');", true);
                Load_Master();
                Load_InitialData();
                Open_Details(0);
            }
        }
        protected void chkrow1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            GridViewRow row = (GridViewRow)checkbox.NamingContainer;
            GridView _details = (GridView)row.FindControl("mydetails");

            if (checkbox.Checked == true)
            {
                for (int j = 0; j <= _details.Rows.Count - 1; j++)
                {
                    CheckBox check = (CheckBox)_details.Rows[j].FindControl("chkrow");
                    check.Checked = true;
                    _details.Rows[j].BackColor = System.Drawing.Color.Gainsboro;
                }
            }
            else
            {
                for (int j = 0; j <= _details.Rows.Count - 1; j++)
                {
                    CheckBox check = (CheckBox)_details.Rows[j].FindControl("chkrow");
                    check.Checked = false;
                    _details.Rows[j].BackColor = System.Drawing.Color.White;
                }
            }
        }

        private void FnSearch()
        {
            _dtresult = new DataTable();
            _dtresult.Columns.Add("e_b_ref", typeof(string));
            _dtresult.Columns.Add("B_Z", typeof(string));
            _dtresult.Columns.Add("Cat", typeof(string));
            _dtresult.Columns.Add("F_lvl", typeof(string));
            _dtresult.Columns.Add("Seq_No", typeof(string));
            _dtresult.Columns.Add("Loc", typeof(string));
            _dtresult.Columns.Add("P_P_to", typeof(string));
            _dtresult.Columns.Add("F_from", typeof(string));
            _dtresult.Columns.Add("Substation", typeof(string));
            _dtresult.Columns.Add("devices1", typeof(string));
            _dtresult.Columns.Add("C_id", typeof(int));
            _dtresult.Columns.Add("Sys_name", typeof(string));
            _dtresult.Columns.Add("Sys_id", typeof(int));
            _dtresult.Columns.Add("Des", typeof(string));
            _dtresult.Columns.Add("devices2", typeof(string));
            _dtresult.Columns.Add("S_c_m", typeof(string));
            var _result = from _data in _dtMaster.AsEnumerable()
                          where _data.Field<string>("E_b_ref").ToUpper().IndexOf(txtsearch.Text.ToUpper()) >= 0
                          select _data;

            foreach (var row in _result)
            {
                DataRow _row = _dtresult.NewRow();
                _row[0] = row["e_b_ref"].ToString();
                _row[1] = row["B_Z"].ToString();
                _row[2] = row["Cat"].ToString();
                _row[3] = row["F_lvl"].ToString();
                _row[4] = row["Seq_No"].ToString();
                _row[5] = row["Loc"].ToString();
                _row[6] = row["p_p_to"].ToString();
                _row[7] = row["F_from"].ToString();
                _row[8] = row["Substation"].ToString();
                _row[9] = row["devices1"].ToString();
                _row[10] = row["C_id"].ToString();
                _row[11] = row["Sys_name"].ToString();
                _row[12] = row["Sys_id"].ToString();
                _row[13] = row["Des"].ToString();
                _row[14] = row["devices2"].ToString();
                _row[15] = row["S_c_m"].ToString();
                _dtresult.Rows.Add(_row);
            }
            Load_InitialData();

        }
        protected void btnsearch_Click(object sender, EventArgs e)
        {
            if (btnsearch.Text == "Search")
            {
                FnSearch();
                for (int i = 0; i <= mymaster.Rows.Count - 1; i++)
                {
                    GridView _mydetails = (GridView)mymaster.Rows[i].FindControl("mydetails");
                    Button _btn = (Button)mymaster.Rows[i].FindControl("Button1");
                    _mydetails.Visible = true;
                    _btn.Text = "-";
                }
                btnsearch.Text = "Reset";
            }
            else
            {
                txtsearch.Text = String.Empty;
                FnSearch();
                for (int i = 0; i <= mymaster.Rows.Count - 1; i++)
                {
                    GridView _mydetails = (GridView)mymaster.Rows[i].FindControl("mydetails");
                    Button _btn = (Button)mymaster.Rows[i].FindControl("Button1");
                    _mydetails.Visible = false;
                    _btn.Text = "+";
                }
                btnsearch.Text = "Search";
            }
        }

        protected void btnimport_Click(object sender, EventArgs e)
        {
            string _url = "CMS/Import.aspx?id=" + (string)Session["sch"] + "_" + (string)Session["project"];
            ScriptManager.RegisterStartupScript(this, typeof(string), "Message", "window.open('" + _url + "','','left=300,top=200,width=500,height=400,menubar=0,toolbar=0,scrollbars=1,status=0,resizable=0');", true);
        }
        protected void btnrefresh_Click(object sender, ImageClickEventArgs e)
        {
            Load_Master();
            Load_InitialData();
        }

        protected void drdes_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["des"] = drdes.SelectedItem.Value;
            Filtering(drbuilding.SelectedItem.Text, drcategory.SelectedItem.Text, drflevel.SelectedItem.Text, drfed.SelectedItem.Text, drloc.SelectedItem.Text, drdes.SelectedItem.Text);
        }
    }
}