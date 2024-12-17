using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication8.Models
{
    public class StudentDBContext
    {
        string cs = ConfigurationManager.ConnectionStrings["dbcccd"].ConnectionString;

        public List<Registration> GetStudents()
        {
            List<Registration> registrationList = new List<Registration>();
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("GET_STUDENT", con);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
           
            while (dr.Read())
            {
                Registration reg = new Registration();
                reg.RegId = Convert.ToInt32(dr.GetValue(0).ToString());
                reg.FullName = dr.GetValue(1).ToString();
                reg.BookingType = dr.GetValue(2).ToString();
                reg.Country = dr.IsDBNull(3) ? string.Empty : dr.GetString(3);
                reg.State = dr.IsDBNull(4) ? string.Empty : dr.GetString(4);
                reg.BookingDate = Convert.ToDateTime(dr.GetValue(5).ToString());
                reg.Status = Convert.ToBoolean(dr.GetValue(6).ToString());
                reg.Description = dr.GetValue(7).ToString();
                reg.Image = dr.GetValue(8).ToString();
                reg.Pdf = dr.GetValue(9).ToString();
                reg.CreatedDate = dr.IsDBNull(10) ? DateTime.MinValue : Convert.ToDateTime(dr.GetValue(10));
                reg.UpdatedDate = dr.IsDBNull(11) ? DateTime.MinValue : Convert.ToDateTime(dr.GetValue(11));



                registrationList.Add(reg);

            }
            con.Close();

            return registrationList;
        }
       
        public Registration GetStudents(int sid)
        {
            Registration reg = new Registration();
            SqlConnection con = new SqlConnection(cs);

            SqlCommand cmd = new SqlCommand("GET_STUDENT_UPDATE", con);
            cmd.Parameters.AddWithValue("@intId", sid);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                reg.RegId = Convert.ToInt32(dr.GetValue(0).ToString());
                reg.FullName = dr.GetValue(1).ToString();
                reg.BookingType = dr.GetValue(2).ToString();
                reg.Country = dr.IsDBNull(3) ? string.Empty : dr.GetString(3);
                reg.State = dr.IsDBNull(4) ? string.Empty : dr.GetString(4);
                reg.BookingDate = Convert.ToDateTime(dr.GetValue(5).ToString());
                reg.Status = Convert.ToBoolean(dr.GetValue(6).ToString());
                reg.Description = dr.GetValue(7).ToString();
                reg.Image = dr.GetValue(8).ToString();
                reg.Pdf = dr.GetValue(9).ToString();
                reg.CreatedDate = dr.IsDBNull(10) ? DateTime.MinValue : Convert.ToDateTime(dr.GetValue(10));
                reg.UpdatedDate = dr.IsDBNull(11) ? DateTime.MinValue : Convert.ToDateTime(dr.GetValue(11));



            }
            con.Close();
            return reg;
        }

        public bool AddStudent(Registration reg)
        {
            SqlConnection con = new SqlConnection(cs);

            SqlCommand cmd = new SqlCommand("INSERT_STUDENT", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@strFullName", reg.FullName);
            cmd.Parameters.AddWithValue("@strBookingType", reg.BookingType);
            cmd.Parameters.AddWithValue("@strCountry", reg.Country ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@strState", reg.State ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@dteBookingDate", reg.BookingDate);
            cmd.Parameters.AddWithValue("@bitStatus", reg.Status);
            cmd.Parameters.AddWithValue("@strDescription", reg.Description);
            
            //cmd.Parameters.AddWithValue("@strImages", reg.Image);
            //cmd.Parameters.AddWithValue("@strPdf", reg.Pdf);
            cmd.Parameters.AddWithValue("@strImages", string.IsNullOrEmpty(reg.Image) ? (object)DBNull.Value : reg.Image);
            cmd.Parameters.AddWithValue("@strPdf", string.IsNullOrEmpty(reg.Pdf) ? (object)DBNull.Value : reg.Pdf);
           




            con.Open();
            int i = cmd.ExecuteNonQuery();

            con.Close();

            if (i > 0)
            {
                return true;
            }
            else
            {

            }
            {
                return false;
            }
        }
        public bool UpdateStudent(Registration reg)
        {
            SqlConnection con = new SqlConnection(cs);
            con.Open();

            SqlCommand cmd = new SqlCommand("UPDATE_STUDENT", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@intId", reg.RegId);
            cmd.Parameters.AddWithValue("@strFullName", reg.FullName);
            cmd.Parameters.AddWithValue("@strBookingType", reg.BookingType);
            cmd.Parameters.AddWithValue("@strCountry", reg.Country ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@strState", reg.State ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@dteBookingDate", reg.BookingDate);
            cmd.Parameters.AddWithValue("@bitStatus", reg.Status);
            cmd.Parameters.AddWithValue("@strDescription", reg.Description);
           
            //cmd.Parameters.AddWithValue("@strImages", reg.Image);
            //cmd.Parameters.AddWithValue("@strPdf", reg.Pdf);
            cmd.Parameters.AddWithValue("@strPdf", string.IsNullOrEmpty(reg.Pdf) ? (object)DBNull.Value : reg.Pdf);
            cmd.Parameters.AddWithValue("@strImages", string.IsNullOrEmpty(reg.Image) ? (object)DBNull.Value : reg.Image);


            int i = cmd.ExecuteNonQuery();

            con.Close();
            if (i > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        [HttpPost]
        public bool DeleteStudent(int id)
        {
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("DELETE_STUDENT", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@intId", id);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            if (i > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Countries> GetCountry()
        {
            List<Countries> countryList = new List<Countries>();
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("SEARCH_COUNTRY", con);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Countries cntr = new Countries();
                cntr.Cid = Convert.ToInt32(dr.GetValue(0).ToString());
                cntr.CName = dr.GetValue(1).ToString();
                countryList.Add(cntr);

            }
            con.Close();
            return countryList;
        }
        public List<States> GetState()
        {
            List<States> stateList = new List<States>();
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("SEARCH_STATE", con);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                States sta = new States();
                sta.Sid = Convert.ToInt32(dr.GetValue(0).ToString());
                sta.Sname = dr.GetValue(1).ToString();
                sta.Counid = Convert.ToInt32(dr.GetValue(2).ToString());
                stateList.Add(sta);

            }
            con.Close();
            return stateList;
        }

        [HttpPost]
        public bool DELIMG(int id)
        {
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("DEL_IMG", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@intId", id);


            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();


            if (i > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public bool DELPDF(int id)
        {
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("DEL_PDF", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@intId", id);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            if (i > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool addStudentExcel(Registration reg)
        {
            try
            {
                List<Registration> registrations = new List<Registration>();

                SqlConnection conn = new SqlConnection(cs);
                conn.Open();
                SqlCommand cmd = new SqlCommand("ImportRegistrations", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strFullName", reg.FullName);
                cmd.Parameters.AddWithValue("@strBookingType", reg.BookingType);
                cmd.Parameters.AddWithValue("@strCountry", reg.Country );
                cmd.Parameters.AddWithValue("@strState", reg.State );
                cmd.Parameters.AddWithValue("@dteBookingDate", reg.BookingDate);
                cmd.Parameters.AddWithValue("@bitStatus", reg.Status);
                cmd.Parameters.AddWithValue("@strDescription", reg.Description);

                int res = cmd.ExecuteNonQuery();
                conn.Close();
                if (res > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
