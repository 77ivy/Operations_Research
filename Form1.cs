using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using ILOG.Concert;
using ILOG.CPLEX;
namespace CPLEX_WyndorGlassProblem // 這個不能改嗎？
{
    public partial class Form1 : Form
    {
        public static INumVar[] GetDataRecords(int nj, int nt, INumVar[][] sales)
        {
            INumVar[] Labels;
            Labels = new INumVar[(nt+1)];

            int cnt = 0;
            
                for(int t = 0; t <= nt; t++)
                {
                    Labels[cnt++] = sales[nj][t];
                }
            

            return Labels;
        }

        public static INumVar[] GetDataX(int nj, int nt, INumVar[][][] x)
        {
            INumVar[] Labels;
            Labels = new INumVar[6];

            int cnt = 0;

            for (int i = 0; i < 6; i++)
            {
                Labels[cnt++] = x[nj][i][nt];
            }



            return Labels;
        }

        public static double[] GetCoefficient(int nj, int nt)
        {
            double[] Labels;
            Labels = new double[(nt+1)];
            for (int i = 0; i < (nt+1); i++) Labels[i] = 1; 
            return Labels;
        }

        public static double GetDemand(int nj, int nt, double[][] demand)
        {
            double Labels = 0;
                for(int t = 0; t <= nt; t++)
                {
                    Labels += demand[nj][t];
                }
            return Labels;
        }

        public static INumVar[] GetDataSales_all(int nj, INumVar[][] sales)
        {
            INumVar[] Labels;
            Labels = new INumVar[24];

            int cnt = 0;

            for(int t = 0; t < 24; t++)
            {
                Labels[cnt++] = sales[nj][t];
            }
            



            return Labels;
        }
        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // initialize cpelxModel via test.lp                               
            Cplex cplexModel = new Cplex();            
            cplexModel.ImportModel("test.lp");

            // solve the lp
            cplexModel.Solve();

            // output the objective value
            label1.Text = "Objective Value:" + cplexModel.ObjValue;

            // clear the cplex object from the memory
            cplexModel.End();                            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //這個地方要輸入檔案
            StreamReader Set_j = new StreamReader(@"Data\product.csv");

            // Read csv to string J
            List<string> J = new List<string>();
            string Set_line;
            while ((Set_line = Set_j.ReadLine()) != null)
            {
                J.Add(Set_line); //一次讀一行，存入List J中
            }
            Set_j.Close();

            //open csv file
            StreamReader Set_t = new StreamReader(@"Data\period.csv");

            // Read csv to string T
            List<string> T = new List<string>();
            while ((Set_line = Set_t.ReadLine()) != null)
            {
                T.Add(Set_line);
            }
            Set_t.Close();

            /*
            foreach(string a in T)
            {
                Console.WriteLine(a);
            }
            */

            StreamReader Parameter_file = new StreamReader(@"Data\demand.csv");

            //Create two dimensions array
            double[][] demand = new double[J.Count][]; //第一維的大小是產品數量
            for (int i = 0; i < J.Count; i++)
            {
                demand[i] = new double[T.Count]; // 第二維的大小是週期長度
            }
            //read csv to 2D array
            
            string Parameter_line;
            while ((Parameter_line = Parameter_file.ReadLine()) != null)
            {
                string[] temp_line = Parameter_line.Split(','); //一次讀一行，以逗號隔開數字(j,t,djt)

                //將index轉成int, 需求轉成double
                demand[(int.Parse(temp_line[0])) - 1 ][(int.Parse(temp_line[1])) - 1] = Convert.ToDouble(temp_line[2]);
            }

            //Console.WriteLine(demand[10][2]);

            StreamReader Parameter_pfile = new StreamReader(@"Data\price.csv");

            //Create two dimensions array
            double[] price = new double[J.Count]; //第一維的大小是產品數量
            
            //read csv to 2D array
            string Parameter_pline;
            while ((Parameter_pline = Parameter_pfile.ReadLine()) != null)
            {
                string[] temp_pline = Parameter_pline.Split(','); // 用逗號隔開(j, pj)

                //將index轉成int, 需求轉成double 
                price[int.Parse(temp_pline[0]) - 1] = Convert.ToDouble(temp_pline[1]);
            }

            // initialize cpelxModel via test.lp                               
            Cplex cplexModel = new Cplex();
            /*
            IEnumerator matrixEnum = cplexModel.GetLPMatrixEnumerator();
            matrixEnum.MoveNext();
            ILPMatrix lp = (ILPMatrix)matrixEnum.Current;
            */

            //Create three dimensions decision variables  
            INumVar[][][] x = new INumVar[3412][][];
            for (int j = 0; j < 3412; j++)
            {
                x[j] = new INumVar[6][];
            }
            for(int j = 0; j < 3412; j++)
            {
                for(int i = 0; i < 6; i++)
                {
                    x[j][i] = new INumVar[24];
                }
            }
            // 設置變數 xjiy
            for (int j = 0; j < 3412; j++)
            {
                for (int i = 0; i < 6; i++)
                {
                    for (int t = 0; t < 24; t++)
                    {
                        switch (i)
                        {
                            case 0:
                                if (j >= 994 && j <= 2130) x[j][i][t] = cplexModel.NumVar(0, double.MaxValue, NumVarType.Float, "x_" + j + "_" + i + "_" + t);
                                else x[j][i][t] = cplexModel.NumVar(0, 0, NumVarType.Float, "x_" + j + "_" + i + "_" + t);
                                break;

                            case 1:
                                if (j >= 2131 && j <= 3027) x[j][i][t] = cplexModel.NumVar(0, double.MaxValue, NumVarType.Float, "x_" + j + "_" + i + "_" + t);
                                else x[j][i][t] = cplexModel.NumVar(0, 0, NumVarType.Float, "x_" + j + "_" + i + "_" + t);
                                break;

                            case 2:
                                if (j >= 0 && j <= 993) x[j][i][t] = cplexModel.NumVar(0, double.MaxValue, NumVarType.Float, "x_" + j + "_" + i + "_" + t);
                                else if (j >= 3395 && j <= 3411) x[j][i][t] = cplexModel.NumVar(0, double.MaxValue, NumVarType.Float, "x_" + j + "_" + i + "_" + t);
                                else x[j][i][t] = cplexModel.NumVar(0, 0, NumVarType.Float, "x_" + j + "_" + i + "_" + t);
                                break;
                            case 3:
                                if (j >= 0 && j <= 3411) x[j][i][t] = cplexModel.NumVar(0, double.MaxValue, NumVarType.Float, "x_" + j + "_" + i + "_" + t);
                                else x[j][i][t] = cplexModel.NumVar(0, 0, NumVarType.Float, "x_" + j + "_" + i + "_" + t);
                                break;
                            case 4:
                                if (j >= 0 && j <= 2130) x[j][i][t] = cplexModel.NumVar(0, double.MaxValue, NumVarType.Float, "x_" + j + "_" + i + "_" + t);
                                else x[j][i][t] = cplexModel.NumVar(0, 0, NumVarType.Float, "x_" + j + "_" + i + "_" + t);
                                break;
                            case 5:
                                if (j >= 2131 && j <= 3394) x[j][i][t] = cplexModel.NumVar(0, double.MaxValue, NumVarType.Float, "x_" + j + "_" + i + "_" + t);
                                else x[j][i][t] = cplexModel.NumVar(0, 0, NumVarType.Float, "x_" + j + "_" + i + "_" + t);
                                break;

                        }
                    }
                }
            }
            // 設置sales
            INumVar[][] sales = new INumVar[3412][];
            for (int j = 0; j < 3412; j++)
            {
                sales[j] = new INumVar[24]; // 第二維的大小是週期長度
            }
            for (int j = 0; j < 3412; j++)
            {
                for (int t = 0; t < 24; t++)
                {
                    sales[j][t] = cplexModel.NumVar(0, double.MaxValue, NumVarType.Float,"s_" +j+"_"+t);
                }
            }
            //設置inventory
            INumVar[][] inventory = new INumVar[3412][];
            for (int i = 0; i < 3412; i++)
            {
                inventory[i] = new INumVar[24]; // 第二維的大小是週期長度
            }
            for (int j = 0; j < 3412; j++)
            {
                for (int t = 0; t < 24; t++)
                {
                    inventory[j][t] = cplexModel.NumVar(0, double.MaxValue, NumVarType.Float,"v_" + j + "_" + t);
                }
            }
            // 設置 backorder
            INumVar[][] backorder = new INumVar[3412][];
            for (int i = 0; i < 3412; i++)
            {
                backorder[i] = new INumVar[24]; // 第二維的大小是週期長度
            }

            for (int j = 0; j < 3412; j++)
            {
                for (int t = 0; t < 24; t++)
                {
                    backorder[j][t] = cplexModel.NumVar(0, double.MaxValue, NumVarType.Float,"b_" + j + "_" + t);
                }
            }

            // x限制式壓一維
            INumVar[][] x1_temp = new INumVar[24][]; // 1137
            INumVar[][] x2_temp = new INumVar[24][]; // 897
            INumVar[][] x3_temp = new INumVar[24][];  // 994
            INumVar[][] x4_temp = new INumVar[24][];   // 17
            INumVar[][] x5_temp = new INumVar[24][]; // 3412
            INumVar[][] x6_temp = new INumVar[24][]; // 2131
            INumVar[][] x7_temp = new INumVar[24][]; // 1264

            for(int t = 0; t < 24; t++)
            {
                x1_temp[t] = new INumVar[1137]; // 1137
                x2_temp[t] = new INumVar[897];  // 897
                x3_temp[t] = new INumVar[994];  // 994
                x4_temp[t] = new INumVar[17];   // 17
                x5_temp[t] = new INumVar[3412]; // 3412
                x6_temp[t] = new INumVar[2131]; // 2131
                x7_temp[t] = new INumVar[1264]; // 1264
            }
            //b為第幾個月的第幾個限制式
            int b = 0;
            for(int t = 0; t < 24; t++)
            {
                for (int j = 994; j < 2131; j++)
                {
                    x1_temp[t][b++] = x[j][0][t];
                    //Console.WriteLine(t + " " + b + " " + j);
                }
                b = 0;
            }

            b = 0;
            for (int t = 0; t < 24; t++)
            {
                for (int j = 2131; j < 3028; j++)
                {
                    x2_temp[t][b++] = x[j][1][t];
                }
                b = 0;
            }
            b = 0;
            for (int t = 0; t < 24; t++)
            {
                for (int j = 0; j < 994; j++)
                {
                    x3_temp[t][b++] = x[j][2][t];
                }
                b = 0;
            }
            b = 0;
            for (int t = 0; t < 24; t++)
            {
                for (int j = 3395; j < 3412; j++)
                {
                    x4_temp[t][b++] = x[j][2][t];
                }
                b = 0;
            }
            b = 0;
            for (int t = 0; t < 24; t++)
            {
                for (int j = 0; j < 3412; j++)
                {
                    x5_temp[t][b++] = x[j][3][t];
                }
                b = 0;
            }
            b = 0;
            for (int t = 0; t < 24; t++)
            {
                for (int j = 0; j < 2131; j++)
                {
                    x6_temp[t][b++] = x[j][4][t];
                    //Console.WriteLine(t + " " + b + " " + j);
                }
                b = 0;
            }
            b = 0;
            for (int t = 0; t < 24; t++)
            {
                for (int j = 2131; j < 3395; j++)
                {
                    x7_temp[t][b++] = x[j][5][t];
                }
                b = 0;
            }

            IRange[] range_capicity = new IRange[6*24];

            
            double[] coefficient = new double[1137];
            for (int i = 0; i < 1137; i++) coefficient[i] = 1;
            b = 0;
            for(int t = 0; t < 24; t++)
            {
                range_capicity[b++] = cplexModel.AddLe(cplexModel.ScalProd(coefficient, x1_temp[t]), 45000, "Factory1_Capa_Limit");
            }
            double[] coefficient_2 = new double[897];
            for (int i = 0; i < 897; i++) coefficient_2[i] = 1;
            for(int t = 0; t < 24; t++)
            {
                range_capicity[b++] = cplexModel.AddLe(cplexModel.ScalProd(coefficient_2, x2_temp[t]), 30000, "Factory2_Capa_Limit");
            }
            /*
            INumVar[] combined_3_4 = x3_temp.Concat(x4_temp).ToArray();
            double[] coefficient_3_4 = new double[24264];
            for (int i = 0; i < 24264; i++) coefficient[i] = 1;
            range_capicity[2] = cplexModel.AddLe(cplexModel.ScalProd(coefficient_3_4, combined_3_4), 40000, "Factory1_Capa_Limit");
            */

            double[] coefficient_3 = new double[994];
            double[] coefficient_4 = new double[17];
            for (int i = 0; i < 994; i++) coefficient_3[i] = 1;
            for (int i = 0; i < 17; i++) coefficient_4[i] = 1;
            for(int t = 0; t < 24; t++)
            {
                range_capicity[b++] = cplexModel.AddLe(cplexModel.Sum(cplexModel.ScalProd(coefficient_3, x3_temp[t]),
                                                                cplexModel.ScalProd(coefficient_4, x4_temp[t])
                                                                         ), 40000, "Factory3_Capa_Limit");
            }

            double[] coefficient_5 = new double[3412];
            for (int i = 0; i < 3412; i++) coefficient_5[i] = 1;
            for(int t = 0; t < 24; t++)
            {
                range_capicity[b++] = cplexModel.AddLe(cplexModel.ScalProd(coefficient_5, x5_temp[t]), 18000, "Factory4_Capa_Limit");
            }

            double[] coefficient_6 = new double[2131];
            for (int i = 0; i < 2131; i++) coefficient_6[i] = 1;
            for(int t = 0; t < 24; t++)
            {
                range_capicity[b++] = cplexModel.AddLe(cplexModel.ScalProd(coefficient_6, x6_temp[t]), 20000, "Factory5_Capa_Limit");
            }

            double[] coefficient_7 = new double[1264];
            for (int i = 0; i < 1264; i++) coefficient_7[i] = 1;
            for(int t = 0; t < 24; t++)
            {
                range_capicity[b++] = cplexModel.AddLe(cplexModel.ScalProd(coefficient_7, x7_temp[t]), 17000, "Factory6_Capa_Limit");
            }

            

            //定義其他變數
            INumVar[] x_all_temp = new INumVar[491328];
            INumVar[] inventory_temp = new INumVar[81888];
            //INumVar[] inventory_last_temp = new INumVar[78476];
            INumVar[] backordor_temp = new INumVar[81888];
            //INumVar[] backorder_last_temp = new INumVar[78476];
            INumVar[] sales_temp = new INumVar[81888];
            /*
            int count = 0;
            for(int j = 0; j < 3412; j++)
            {
                for(int i = 0; i < 6; i++)
                {
                    for(int t = 0; t < 24; t++)
                    {
                        x_all_temp[count++] = x[j][i][t];
                    }
                }
            }

            

            count = 0;
            for (int j = 0; j < 3412; j++)
            {
                for (int t = 1; t < 24; t++)
                {
                    inventory_last_temp[count++] = inventory[j][t];
                }
            }

            count = 0;
            for (int j = 0; j < 3412; j++)
            {
                for (int t = 1; t < 24; t++)
                {
                    backorder_last_temp[count++] = backorder[j][t];
                }
            }

            count = 0;
            for (int j = 0; j < 3412; j++)
            {
                for (int t = 0; t < 24; t++)
                {
                    sales_temp[count++] = sales[j][t];
                }
            }
            */
            int count = 0;
            for (int j = 0; j < 3412; j++)
            {
                for (int i = 0; i < 6; i++)
                {
                    for (int t = 0; t < 24; t++)
                    {
                        x_all_temp[count++] = x[j][i][t];
                    }
                }
            }
            count = 0;
            for (int j = 0; j < 3412; j++)
            {
                for (int t = 0; t < 24; t++)
                {
                    backordor_temp[count++] = backorder[j][t];
                }
            }
            count = 0;
            for (int j = 0; j < 3412; j++)
            {
                for (int t = 0; t < 24; t++)
                {
                    inventory_temp[count++] = inventory[j][t];
                }
            }
            count = 0;
            for (int j = 0; j < 3412; j++)
            {
                for (int t = 0; t < 24; t++)
                {
                    sales_temp[count++] = sales[j][t];
                }
            }
            double[] coefficient_inventory_temp = new double[81888];
            for (int i = 0; i < 81888; i++) coefficient_inventory_temp[i] = -1;

            
            double[] coefficient_x_all_temp = new double[491328];
            for (int i = 0; i < 491328; i++) coefficient_x_all_temp[i] = 1;
            double[] coefficient_inventory_last_temp = new double[78476];
            for (int i = 0; i < 78476; i++) coefficient_inventory_last_temp[i] = 1;
            double[] coefficient_backordor_temp = new double[81888];
            for (int i = 0; i < 81888; i++) coefficient_backordor_temp[i] = 1;
            double[] coefficient_backorder_last_temp = new double[78476];
            for (int i = 0; i < 78476; i++) coefficient_backorder_last_temp[i] = -1;
            double[] coefficient_sales_temp = new double[81888];
            for (int i = 0; i < 81888; i++) coefficient_sales_temp[i] = -1;
            

            /*
            range[6] = cplexModel.AddEq(cplexModel.Sum(cplexModel.ScalProd(coefficient_x_all_temp, x_all_temp),
                                                       cplexModel.ScalProd(coefficient_inventory_last_temp, inventory_last_temp),
                                                       cplexModel.ScalProd(coefficient_backorder_last_temp, backorder_last_temp),
                                                       cplexModel.ScalProd(coefficient_sales_temp, sales_temp),
                                                       cplexModel.ScalProd(coefficient_inventory_temp, inventory_temp)
                                                       ), 0.0, "Factory3_Capa_Limit");

            range[7] = cplexModel.AddGe(cplexModel.Sum(cplexModel.ScalProd(coefficient_x_all_temp, x_all_temp),
                                                       cplexModel.ScalProd(coefficient_inventory_last_temp, inventory_last_temp),
                                                       cplexModel.ScalProd(coefficient_backorder_last_temp, backorder_last_temp),
                                                       cplexModel.ScalProd(coefficient_sales_temp, sales_temp)
                                                       ), 0.0, "Factory3_Capa_Limit");
            */

            double[] coefficient_x = new double[6];
            for (int i = 0; i < 6; i++) coefficient_x[i] = 1;
            IRange[] range_inventory = new IRange[3412*24];
            int k = 0;
            for(int j = 0; j < 3412; j++)
            {
                for(int t = 0; t < 24; t++)
                {
                    if(t == 0)
                    {
                        range_inventory[k++] = cplexModel.AddEq(cplexModel.Sum(cplexModel.ScalProd(coefficient_x, GetDataX(j, t, x)),
                                                                               cplexModel.Prod(-1.0,inventory[j][t]),
                                                                               cplexModel.Prod(1.0, backorder[j][t])
                                                                              ), demand[j][t], "inventory_t0");
                    }
                    else
                    {
                        range_inventory[k++] = cplexModel.AddEq(cplexModel.Sum(cplexModel.ScalProd(coefficient_x, GetDataX(j, t, x)),
                                                                               cplexModel.Prod(1.0, backorder[j][t]),
                                                                               cplexModel.Prod(1.0, inventory[j][t-1]),
                                                                               cplexModel.Prod(-1.0, backorder[j][t-1]),                                                                         
                                                                               cplexModel.Prod(-1.0, inventory[j][t])
                                                                               ), demand[j][t], "inventory");
                    }
                }
            }
      
            IRange[] range_sales = new IRange[3412*24];
            k = 0;
            for (int j = 0; j < 3412; j++)
            {
                for (int t = 0; t < 24; t++)
                {
                    if (t == 0)
                    {
                        range_sales[k++] = cplexModel.AddGe(cplexModel.Sum(cplexModel.ScalProd(coefficient_x, GetDataX(j, t, x)),
                                                                           //cplexModel.Prod(1.0, backorder[j][t-1]),
                                                                           cplexModel.Prod(-1.0, sales[j][t])
                                                                           ), 0.0, "sale_t0");
                    }
                    else
                    {
                        range_sales[k++] = cplexModel.AddGe(cplexModel.Sum(cplexModel.ScalProd(coefficient_x, GetDataX(j, t, x)),
                                                                           cplexModel.Prod(1.0, inventory[j][t - 1]),
                                                                           //cplexModel.Prod(1.0, backorder[j][t]),
                                                                           //cplexModel.Prod(1.0, backorder[j][t - 1]), 
                                                                           cplexModel.Prod(-1.0, sales[j][t])
                                                                           ), 0.0, "sale");
                    }
                }
            }

            // 設定所有sales會小於等於demand
            IRange[] range_sales_all = new IRange[3412];
            INumVar[] sales_all = new INumVar[24];

            double[] coefficient_sales_all = new double[24];
            for (int i = 0; i < 24; i++) coefficient_sales_all[i] = 1;
            for(int j = 0; j < 3412; j++)
            {
                range_sales_all[j] = cplexModel.AddLe(cplexModel.ScalProd(coefficient_sales_all, GetDataSales_all(j, sales)), GetDemand(j, 23, demand), "Factory1_Capa_Limit");
            }
           
            
            IRange[] range_backorder = new IRange[3412 * 24];
            k = 0;
            for (int j = 0; j < 3412; j++)
            {
                    for(int t = 0; t < 24; t++)
                    {

                        range_backorder[k++] = cplexModel.AddEq(cplexModel.Sum(cplexModel.Prod(1.0, backorder[j][t]),
                                                                               cplexModel.ScalProd(GetCoefficient(j, t), GetDataRecords(j, t, sales))
                                                                              ), GetDemand(j, t, demand), "backorder");
                    }
                
            }
            /*
            double[] coefficient_sales_temp1 = new double[81888];
            for (int i = 0; i < 81888; i++) coefficient_sales_temp1[i] = 1;
            double[] coefficient_inventory_temp1 = new double[81888];
            for (int i = 0; i < 81888; i++) coefficient_inventory_temp1[i] = 1;
            IRange[] range_other = new IRange[3];
            range_other[0] = cplexModel.AddGe(cplexModel.ScalProd(coefficient_sales_temp1, sales_temp), 0.0, "Factory1_Capa_Limit");
            range_other[1] = cplexModel.AddGe(cplexModel.ScalProd(coefficient_inventory_temp1, inventory_temp), 0.0, "Factory1_Capa_Limit");
            range_other[2] = cplexModel.AddGe(cplexModel.ScalProd(coefficient_backordor_temp, backordor_temp), 0.0, "Factory1_Capa_Limit");
            */
            double[] price_temp = new double[81888];
            double[] price_temp_x = new double[491328];
            double[] price_temp_b = new double[81888];
            int cnt = 0;
            for (int j = 0; j < 3412; j++)
            {
                for (int t = 0; t < 24; t++)
                {
                    price_temp[cnt++] = price[j];
                }
            }

            cnt = 0;
            for (int j = 0; j < 3412; j++)
            {
                for (int t = 0; t < 24; t++)
                {
                    price_temp_b[cnt++] = (-2) * price[j];
                }
            }

            cnt = 0;
            for (int j = 0; j < 3412; j++)
            {
                for (int i = 0; i < 6; i++)
                {
                    for (int t = 0; t < 24; t++)
                    {
                        price_temp_x[cnt++] = (-0.5) * price[j];
                    }
                }
            }
            cplexModel.AddMaximize(cplexModel.Sum(cplexModel.ScalProd(price_temp, sales_temp),
                                                  cplexModel.ScalProd(price_temp_x, x_all_temp),
                                                  cplexModel.ScalProd(price_temp_b, backordor_temp),
                                                  cplexModel.ScalProd(coefficient_inventory_temp, inventory_temp)
                                                  ));

   
            cplexModel.ExportModel("Result.lp");
            // 這邊的 lp 是自動會儲存限制式的 LFS 嗎？

            cplexModel.Solve();
            StreamWriter sale = new StreamWriter(@"result_sales.csv");
            StreamWriter prod = new StreamWriter(@"result_production.csv");
            StreamWriter bg = new StreamWriter(@"result_backorder.csv");
            StreamWriter Inven = new StreamWriter(@"result_inventory.csv");
            //StreamWriter cost_x = new StreamWriter(@"result_costX.csv");
            //StreamWriter cost_bg = new StreamWriter(@"result_costBG.csv");

            for (int t = 1; t <= 24; t++)
            {
                sale.Write("T = " + t + ",");
                prod.Write("T = " + t + ",");
                bg.Write("T = " + t + ",");
                Inven.Write("T = " + t + ",");
               // cost_x.Write("T = " + t + ",");
            }
            sale.WriteLine();
            prod.WriteLine();
            bg.WriteLine();
            Inven.WriteLine();
            //cost_x.WriteLine();

            for (int j = 0; j < 3412; j++)
            {
                for (int i = 0; i < 6; i++)
                {
                    double[] x_temp = cplexModel.GetValues(x[j][i]);
                    string convert = string.Join(",", x_temp);
                    prod.Write(convert, ",");
                    prod.WriteLine();
                    /*
                    for(int t = 0; t < 24; t++)
                    {
                        cost_x.Write(x_temp[t]*price[j]*(-0.5) + ",");
                    }
                    cost_x.WriteLine();
                    */
                }
            }

            for(int j = 0; j < 3412; j++)
            {
                double[] x_temp = cplexModel.GetValues(sales[j]);
                string convert = string.Join(",", x_temp);
                sale.Write(convert, ",");
                sale.WriteLine();
            }

            for (int j = 0; j < 3412; j++)
            {
                double[] x_temp = cplexModel.GetValues(backorder[j]);
                string convert = string.Join(",", x_temp);
                bg.Write(convert, ",");
                bg.WriteLine();
                /*
                for(int t = 0; t < 24; t++)
                {
                    cost_bg.Write(x_temp[t] * price[j] * (-2) + ",");
                }
                cost_bg.WriteLine();
                */
            }

            for (int j = 0; j < 3412; j++)
            {
                double[] x_temp = cplexModel.GetValues(inventory[j]);
                string convert = string.Join(",", x_temp);
                Inven.Write(convert, ",");
                Inven.WriteLine();
            }



            /*
            double[][][] x_ans = new double[3411][][];
            for (int j = 0; j < J.Count; j++)
            {
                
                x_ans[j] = new double[6][]; // 第三維的大小是週期長度
                for (int i = 0; i < 6; i++)
                {
                    x_ans[j][i] = new double[T.Count]; // 第三維的大小是週期長度
                }

            }
            
            int index_temp = 0;
            for(int j = 0; j < 3412; j++)
            {
                for(int i = 0; i < 6; i++)
                {
                    for(int t = 0; t < 24; t++)
                    {
                        x_ans[j][i][t] = x_temp[index_temp++];
                    }
                }
            }

            double[][] sales_ans = new double[3412][];
            for (int i = 0; i < J.Count; i++)
            {
                sales_ans[i] = new double[T.Count]; // 第二維的大小是週期長度
            }

            //index_temp = 3412*6*24 + 3412*23*2 - 1;
            for (int j = 0; j < 3412; j++)
            {
                for(int t = 0; t < 24; t++)
                {
                    sales_ans[j][t] = x_temp[index_temp++];
                }
            }

            double[][] inventory_ans = new double[3412][];
            for (int i = 0; i < J.Count; i++)
            {
                inventory_ans[i] = new double[T.Count]; // 第二維的大小是週期長度
            }
            //index_temp = 3412 * 6 * 24 + 3412 * 23 * 2 + 3412 * 24 - 1;
            for (int j = 0; j < 3412; j++)
            {
                for (int t = 0; t < 24; t++)
                {
                    inventory_ans[j][t] = x_temp[index_temp++];
                }
            }

            double[][] backorder_ans = new double[3412][];
            for(int i = 0; i < J.Count; i++)
            {
                backorder_ans[i] = new double[T.Count]; // 第二維的大小是週期長度
            }
            //index_temp = 3412 * 6 * 24 + 3412 * 23 * 2 + 3412 * 24 * 2 - 1;
            for (int j = 0; j < 3412; j++)
            {
                for (int t = 0; t < 24; t++)
                {
                    backorder_ans[j][t] = x_temp[index_temp++];
                }
            }

            for (int j = 0; j < 3014; j++)
            {
                for (int i = 0; i < 6; i++)
                {
                    for (int t = 0; t < 24; t++)
                    {
                        cplexModel.Output().WriteLine("x" + j+1 + i+1 + t+1 + " = " + x_ans[j][i][t]);
                        //GetValues(lp)[index++]); // 這個地方也是用一維去印嗎？
                    }
                }
            }
           */


            //INumVar[][][] x = cplexModel.NumVarArray(2, 0, 9999, NumVarType.Float) ;
            //deVar = x;

            //INumVar[] x = new INumVar[2];
            //x[0] = cplexModel.NumVar(0, System.Int32.MaxValue, "x1");
            //x[1] = cplexModel.NumVar(0, System.Int32.MaxValue, "x2");

            //objective Function
            //cplexModel.AddMaximize(cplexModel.Sum(cplexModel.Prod(3.0, x[0][1][2]), cplexModel.Prod(5.0, x[1][2][3])));

           
            //IRange[][] r = new IRange[1][];
            //r[0] = new IRange[4];
            //r[0][0] = cplexModel.AddLe(cplexModel.Prod(1.0, x[0][1][2]), 4.0, "Factory1_1");
            //r[0][1] = 
            //IRange[] rng = new IRange[3];
            // adding constraints (問)
            // 這邊有需要弄到三維嗎？ 不用 因為只有return 1個值
            //rng[0] = cplexModel.AddLe(cplexModel.Prod(1.0, x[0][1][2]), 4.0, "Factory1_Capa_Limit");
            //rng[1] = cplexModel.AddLe(cplexModel.Prod(2.0, x[1][1][1]), 12.0, "Factory2_Capa_Limit");
            //rng[2] = cplexModel.AddLe(cplexModel.Sum(cplexModel.Prod(3.0, x[0][1][1]), cplexModel.Prod(2.0, x[1][1][1])), 18.0, "Factory3_Capa_Limit");

            

            // solve the lp
            //cplexModel.Solve();
            //記得getvalue時，不會是三維 要用x[k], k = 1...(3014*24*6 +...)
            //double[] x_ans = cplexModel.GetValues(lp);
            // output the objective value
            label1.Text = "Objective Value:" + cplexModel.ObjValue;

            //看有幾個變數
            //int num = cplexModel.GetValues(lp).Length;
            //把每個變數都印出來(問)
            //再問一次 印出來的順序會怎麼跑
            //int index = 0;
            /*
            StreamWriter sale = new StreamWriter(@"result_sales.csv");
            StreamWriter prod= new StreamWriter(@"result_production.csv");
            StreamWriter bg = new StreamWriter(@"result_backorder.csv");
            StreamWriter Inven = new StreamWriter(@"result_inventory.csv");
            */
 

            /*
            for(int j = 0; j < 3412; j++)
            {
                for(int t = 0; t < 24; t++)
                {
                    sale.Write(sales_ans[j][t] + ",");
                }
            }

            for(int j = 0; j < 3412; j++)
            {
                for(int i = 0; i < 6; i++)
                {
                    for(int t = 0; t < 24; t++)
                    {
                        prod.Write((char)('A' + i) + "," + x[j][i][t]);
                    }
                }
            }

            for (int j = 0; j < 3412; j++)
            {
                for (int t = 0; t < 24; t++)
                {
                    bg.Write(backorder_ans[j][t] + ",");
                }
            }

            for (int j = 0; j < 3412; j++)
            {
                for (int t = 0; t < 24; t++)
                {
                    Inven.Write(inventory_ans[j][t] + ",");
                }
            }
            */
            // clear the cplex object from the memory
            prod.Close();
            Inven.Close();
            bg.Close();
            sale.Close();
            cplexModel.End();      
            
        }


    }
}
