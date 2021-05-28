using Kingdee.BOS.Core.Bill.PlugIn;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using System;
using System.Data;
using System.Data.SqlClient;

namespace MDYHDSCH1
{


    public partial class Class1 : AbstractBillPlugIn   // 表单插件
    {

        public override void BarItemClick(BarItemClickEventArgs e)
        {



            base.BarItemClick(e);

            //int row = this.View.Model.GetEntryRowCount("FEntity");
            //object a = this.Model.GetValue("FACCOUNTID", 1);
            //staffObj1["FNumber"].ToString() 工程项目
            //staffObj2["FNumber"].ToString() 部门
            //staffObj3["FNumber"].ToString() 核算维度




            if (e.BarItemKey == "tbSave" || e.BarItemKey == "tbSplitSave")
            {
                e.Cancel = true;
                int a = 0;
                string b = "";
                string c = "";
                int row = this.View.Model.GetEntryRowCount("FEntity");
                string fbillno = "";
                if (this.View.Model.DataObject["Id"] != null)
                {
                    fbillno = this.View.Model.DataObject["Id"].ToString();
                }


                for (int i = 0; i < row; i++)
                {
                    DynamicObject staffObj3 = this.Model.GetValue("FACCOUNTID", i) as DynamicObject;//科目编码
                    string price2 = this.Model.GetValue("FCREDIT", i).ToString();//借方金额
                    if (staffObj3 != null)
                    {
                        if (staffObj3["Number"].ToString() == "1003.01" && price2.ToString() != "0")
                        {
                            DynamicObject staffObj = this.Model.GetValue("FDETAILID", i) as DynamicObject;
                            DynamicObject staffObj1 = staffObj[25] as DynamicObject;
                            DynamicObject staffObj2 = staffObj[5] as DynamicObject;
                            DynamicObject staffObj4 = staffObj[23] as DynamicObject;
                            DynamicObject ID = this.Model.GetValue("FEntrySeq", i) as DynamicObject;

                            string price = this.Model.GetValue("FCREDIT", i).ToString();
                            decimal price1 = decimal.Parse(price);
                            decimal priceO = 0;
                            for (int j = 0; j < i; j++)
                            {
                                DynamicObject staffObjj = this.Model.GetValue("FACCOUNTID", j) as DynamicObject;//科目编码
                                string pricej2 = this.Model.GetValue("FCREDIT", j).ToString();//借方金额

                                if (staffObj3["Number"].ToString() == "1003.01")
                                {
                                    DynamicObject staffObjjO = this.Model.GetValue("FDETAILID", j) as DynamicObject;
                                    DynamicObject staffObjj1 = staffObjjO[25] as DynamicObject;
                                    DynamicObject staffObjj2 = staffObjjO[5] as DynamicObject;
                                    DynamicObject staffObjj4 = staffObjjO[23] as DynamicObject;
                                    if (staffObj1["FNumber"].ToString() == staffObjj1["FNumber"].ToString()
                                        && staffObj2["Number"].ToString() == staffObjj2["Number"].ToString() &&
                                        staffObj4["FNumber"].ToString() == staffObjj4["FNumber"].ToString())
                                    {
                                        string pricej = this.Model.GetValue("FCREDIT", j).ToString();
                                        string priced = this.Model.GetValue("FDEBIT", j).ToString();
                                        if (pricej == "")
                                        {
                                            pricej = "0";
                                        }
                                        if (priced == "")
                                        {
                                            priced = "0";
                                        }
                                        priceO = priceO + decimal.Parse(pricej) - decimal.Parse(priced);
                                    }



                                }
                            }



                            string strsql = "Data Source=172.16.120.253;Initial Catalog=AIS20171226114826;uid=sa;pwd=Aa123456";//数据库链接字符串  
                            string sql = "sp_HS_JX_Client_Select_Blance_Result";//要调用的存储过程名  
                            SqlConnection conStr = new SqlConnection(strsql);//SQL数据库连接对象，以数据库链接字符串为参数  
                            SqlCommand comStr = new SqlCommand(sql, conStr);//SQL语句执行对象，第一个参数是要执行的语句，第二个是数据库连接对象  
                            comStr.CommandType = CommandType.StoredProcedure;//因为要使用的是存储过程，所以设置执行类型为存储过程  
                                                                             //依次设定存储过程的参数  
                            comStr.Parameters.Add("@ProjectNumber", SqlDbType.Text).Value = staffObj1["FNumber"].ToString();//工程项目
                            comStr.Parameters.Add("@DepartmentNumber", SqlDbType.Text).Value = staffObj2["Number"].ToString();//部门
                            comStr.Parameters.Add("@Sounce", SqlDbType.Text).Value = staffObj4["FNumber"].ToString();//部门
                            comStr.Parameters.Add("@fbillno", SqlDbType.Text).Value = fbillno;//部门
                            comStr.Parameters.Add("@Priece", SqlDbType.Decimal).Value = price1 + priceO;//贷方金额
                            conStr.Open();//打开数据库连接  
                                          //  MessageBox.Show(comStr.ExecuteNonQuery().ToString());//执行存储过程  
                            SqlDataAdapter SqlDataAdapter1 = new SqlDataAdapter(comStr);
                            DataTable DT = new DataTable();
                            SqlDataAdapter1.Fill(DT);
                            conStr.Close();//关闭连接  
                            if (DT.Rows[0][0].ToString() == "0")
                            {
                                a = a + 1;
                                if (a == 1)
                                {
                                    b = (i + 1).ToString() + "行,记账后余额为：" + DT.Rows[0][1].ToString() + " ";
                                }
                                else
                                {
                                    b = b + "," + (i + 1).ToString() + "行,记账后余额为：" + DT.Rows[0][1].ToString() + " ";
                                }


                            }

                        }
                    }

                }

                if (a != 0)
                {

                    this.View.ShowMessage("单据中第" + b.ToString() + "余额为负数,是否继续保存",
                         MessageBoxOptions.YesNoCancel,
                         new Action<MessageBoxResult>((result) =>
                         {
                             if (result == MessageBoxResult.Yes)



                             {

                                 //调用保存操作
                                 //e.Cancel = false;
                                 this.View.InvokeFormOperation(FormOperationEnum.Save);

                             }

                             else if (result == MessageBoxResult.No)
                             {

                                 e.Cancel = true;
                                 return;
                             }

                         }));


                }


            }

        }

    }

}
// massage拉出来    int判定是否加，不为0显示     string a=a+序号