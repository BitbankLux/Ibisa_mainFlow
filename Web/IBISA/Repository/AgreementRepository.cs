using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Web;
using IBISA.Models;
namespace IBISA.Repository
{
    public class AgreementRepository
    {
        //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["IBISA_Context"].ToString());
        //public int Addagreement(IBISAViewmodal viewmodal)//Code For Add single Question 
        //{
        //    List<IBISAViewmodal> Questionstype = new List<IBISAViewmodal>();
        //    SqlCommand cmd = new SqlCommand("insertAgreementDetails", con);
        //    int result = 0;
        //    try
        //    {

        //        //cmd.CommandType = CommandType.StoredProcedure;
        //        //cmd.CommandText = "insertAgreementDetails";
        //        //cmd.Parameters.Clear();
        //        //cmd.Parameters.AddWithValue("@AgreementNumber", viewmodal.agreement.AgreementNumber);
        //        //cmd.Parameters.AddWithValue("@Crop", viewmodal.agreement.Crop);
        //        //cmd.Parameters.AddWithValue("@AgrementStartDate", viewmodal.agreement.AgreementStartDate);
        //        //cmd.Parameters.AddWithValue("@Zone", viewmodal.agreement.Zone);
        //        //cmd.Parameters.AddWithValue("@MaxPayout", viewmodal.agreement.MaxPayout);
        //        //cmd.Parameters.AddWithValue("@PremiumAmount",viewmodal.agreement.PremiumAmount);
        //        //cmd.Parameters.AddWithValue("@AgreementCurrency",viewmodal.agreement.AgreementCurrency);
        //        //cmd.Parameters.AddWithValue("@CreatedDate", viewmodal.agreement.CreatedDate);
        //        //cmd.Parameters.AddWithValue("@ContributionDate",viewmodal.agreementcontribution.ContributionDate);
        //        //cmd.Parameters.AddWithValue("@Merit", viewmodal.agreementcontribution.Merit);
        //        //cmd.Parameters.AddWithValue("@ContributionAmount", viewmodal.agreementcontribution.ContributionAmount);
        //        //cmd.Parameters.AddWithValue("@IdemnifyDate",viewmodal.agreementindemnification.IndemnifyDate);
        //        //cmd.Parameters.AddWithValue("@IdemnifyAmount",viewmodal.agreementindemnification.IndemnifyAmount);
        //        //cmd.Parameters.AddWithValue("@DisplayName", viewmodal.user.DisplayName);
        //        //cmd.Parameters.AddWithValue("@UserName", viewmodal.user.UserName);
        //        //cmd.Parameters.AddWithValue("@Password", viewmodal.user.Password);
        //        con.Open();
        //        result = cmd.ExecuteNonQuery();
        //        con.Close();

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    if (result > 0)
        //    {
        //        return 1;
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}

        //public bool InsertData(IBISAViewmodal modal)
        //{

        //    SqlCommand cmd = new SqlCommand("InsertFollowUp", con);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    //cmd.Parameters.AddWithValue("@Audit_Id", followupmodel.Audit_Id);
        //    //cmd.Parameters.AddWithValue("@followUpDate", followupmodel.Follow_up_date);
        //    //cmd.Parameters.AddWithValue("@Follow_up_by", followupmodel.Follow_up_by);
        //    //cmd.Parameters.AddWithValue("@Finding_Id", followupmodel.Finding_Id);
        //    //cmd.Parameters.AddWithValue("@Remark", followupmodel.Remark);
        //    con.Open();
        //    int i = cmd.ExecuteNonQuery();
        //    con.Close();
        //    if (i == 1)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
    }
}