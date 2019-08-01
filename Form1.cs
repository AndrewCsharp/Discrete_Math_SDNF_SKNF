using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ODM_LW_6
{   
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();           
        }

        ///////////////////////////////////////////////////////////////////////////////////
        #region global variables

        StringBuilder[,] values = new StringBuilder[16, 5];       
        StringBuilder sSDNF = new StringBuilder();
        StringBuilder sSKNF = new StringBuilder();
        StringBuilder sZero = new StringBuilder("0");
        StringBuilder sOne = new StringBuilder("1");

        #endregion
        ///////////////////////////////////////////////////////////////////////////////////
        #region logical funcs

        /* 00 F0=0 */
        private decimal F0Const0(decimal x1, decimal x2)
        {
            decimal c = x1 + x2;
            return 0;
        }

        /* 01 конъюнкция 	x1 and x2*/
        private decimal F1Conjunction(decimal x1, decimal x2)
        {
            return (Convert.ToBoolean(x1) && Convert.ToBoolean(x2)) ? 1 : 0;
        }

        /* 02 	запрет по x2:	x1 and not x2 */
        private decimal F2BanByX2(decimal x1, decimal x2)
        {
            return (Convert.ToBoolean(x1) && !Convert.ToBoolean(x2)) ? 1 : 0;
        }

        /* 03 F3=x1*/
        private decimal F3X1(decimal x1, decimal x2 = 0)
        {
            return x1;
        }

        /* 04 запрет по x1 */
        private decimal F4BanByX1(decimal x1, decimal x2)
        {
            return (!Convert.ToBoolean(x1) && Convert.ToBoolean(x2)) ? 1 : 0;
        }

        /* 05  F5=x2 */
        private decimal F5X2(decimal x1, decimal x2)
        {
            return x2;
        }

        /* 06 	XOR он же сложение по модулю 2 	(x1 and not x2) or (not x1 and x2) */
        private decimal F6SumMod2(decimal x1, decimal x2)
        {
            return ((Convert.ToBoolean(x1) && !Convert.ToBoolean(x2)) ||
                    (!Convert.ToBoolean(x1) && Convert.ToBoolean(x2))) ? 1 : 0;
        }

        /* 07 	дизъюнкция 	x1 or x2 */
        private decimal F7Disjunction(decimal x1, decimal x2)
        {
            return (Convert.ToBoolean(x1) || Convert.ToBoolean(x2)) ? 1 : 0;
        }

        /* 08 	функция Пирса 	not x1 and not x2 */
        private decimal F8Peirce(decimal x1, decimal x2)
        {
            return (!Convert.ToBoolean(x1) && !Convert.ToBoolean(x2)) ? 1 : 0;
        }

        /* 09 эквивалентность equivalence	(not x1 and not x2) or (x1 and x2) */
        private decimal F9Equivalence(decimal x1, decimal x2)
        {
            return ((!Convert.ToBoolean(x1) && !Convert.ToBoolean(x2)) ||
                     (Convert.ToBoolean(x1) && Convert.ToBoolean(x2))) ? 1 : 0;
        }

        /* 10 F10=not x2*/
        private decimal F10NotX2(decimal x1, decimal x2)
        {
            return (Convert.ToBoolean(x2) == true) ? 0 : 1;
        }

        /* 11 импликация по х2(обратная):  x1 or not x2 */
        private decimal F11ImplicationInvert(decimal x1, decimal x2)
        {
            return (Convert.ToBoolean(x1) || !Convert.ToBoolean(x2)) ? 1 : 0;
        }

        /* 12  F12=not x1*/
        private decimal F12NotX1(decimal x1, decimal x2 = 0)
        {
            return (Convert.ToBoolean(x1) == true) ? 0 : 1;
        }

        /* 13 	импликация (прямая) по х1:  not x1 or x2 */
        private decimal F13Implication(decimal x1, decimal x2)
        {
            return (!Convert.ToBoolean(x1) || Convert.ToBoolean(x2)) ? 1 : 0;
        }

        /* 14 	штрих Шеффера 	not x1 or not x2 */
        private decimal F14Sheffer(decimal x1, decimal x2)
        {
            return (!Convert.ToBoolean(x1) || !Convert.ToBoolean(x2)) ? 1 : 0;
        }

        /* 15 F15=1 */
        private decimal F15Const1(decimal x1, decimal x2)
        {
            decimal c = x1 + x2;
            c = 1;
            return c;
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////////////////
        #region funcs
        private string CalcRes(string s)
        {
            Mathos.Parser.MathParser parser = new Mathos.Parser.MathParser();
            parser.OperatorList = new List<string>() { "˄", "˅", "⊕", "→", "←", "↔", "↓", "|", "-" };

            //  operators parse
            parser.OperatorAction.Add("˄", delegate(decimal x1, decimal x2)
            {
                return F1Conjunction(x1, x2);
            });

            parser.OperatorAction.Add("˅", delegate(decimal x1, decimal x2)
            {
                return F7Disjunction(x1, x2);
            });

            parser.OperatorAction.Add("⊕", delegate(decimal x1, decimal x2)
            {
                return F6SumMod2(x1, x2);
            });

            parser.OperatorAction.Add("→", delegate(decimal x1, decimal x2)
            {
                return F13Implication(x1, x2);
            });

            parser.OperatorAction.Add("←", delegate(decimal x1, decimal x2)
            {
                return F11ImplicationInvert(x1, x2);   
            });

            parser.OperatorAction.Add("↔", delegate(decimal x1, decimal x2)
            {
                return F9Equivalence(x1, x2);
            });

            parser.OperatorAction.Add("↓", delegate(decimal x1, decimal x2)
            {
                return  F8Peirce(x1, x2);
            });

            parser.OperatorAction.Add("|", delegate(decimal x1, decimal x2)
            {
                return F14Sheffer(x1, x2);
            });

            // function parse (NotX)
            parser.LocalFunctions.Add("!", delegate(decimal[] input)
            {
                return F12NotX1(input[0]);
            });
         
            return Convert.ToString(parser.Parse(s));
        }

        private void GetResult(string sFunc)
        {
            //0000-------------------------------------------
            StringBuilder sF0000 = new StringBuilder(sFunc);
            sF0000.Replace("x1", "0");
            sF0000.Replace("x2", "0");
            sF0000.Replace("x3", "0");
            sF0000.Replace("x4", "0");       
            values[0, 1] = new StringBuilder("0");
            values[0, 2] = new StringBuilder("0");
            values[0, 3] = new StringBuilder("0");
            values[0, 4] = new StringBuilder("0");
            values[0, 0] = new StringBuilder(CalcRes(Convert.ToString(sF0000)));

            //0001-------------------------------------------
            StringBuilder sF0001 = new StringBuilder(sFunc);
            sF0001.Replace("x1", "0");
            sF0001.Replace("x2", "0");
            sF0001.Replace("x3", "0");
            sF0001.Replace("x4", "1");           
            values[1, 1] = new StringBuilder("0");
            values[1, 2] = new StringBuilder("0");
            values[1, 3] = new StringBuilder("0");
            values[1, 4] = new StringBuilder("1");
            values[1, 0] = new StringBuilder(CalcRes(Convert.ToString(sF0001)));

            //0010-------------------------------------------
            StringBuilder sF0010 = new StringBuilder(sFunc);
            sF0010.Replace("x1", "0");
            sF0010.Replace("x2", "0");
            sF0010.Replace("x3", "1");
            sF0010.Replace("x4", "0");
            values[2, 1] = new StringBuilder("0");
            values[2, 2] = new StringBuilder("0");
            values[2, 3] = new StringBuilder("1");
            values[2, 4] = new StringBuilder("0");
            values[2, 0] = new StringBuilder(CalcRes(Convert.ToString(sF0010)));

            //0011-------------------------------------------
            StringBuilder sF0011 = new StringBuilder(sFunc);
            sF0011.Replace("x1", "0");
            sF0011.Replace("x2", "0");
            sF0011.Replace("x3", "1");
            sF0011.Replace("x4", "1");
            values[3, 1] = new StringBuilder("0");
            values[3, 2] = new StringBuilder("0");
            values[3, 3] = new StringBuilder("1");
            values[3, 4] = new StringBuilder("1");
            values[3, 0] = new StringBuilder(CalcRes(Convert.ToString(sF0011)));

            //0100-------------------------------------------
            StringBuilder sF0100 = new StringBuilder(sFunc);
            sF0100.Replace("x1", "0");
            sF0100.Replace("x2", "1");
            sF0100.Replace("x3", "0");
            sF0100.Replace("x4", "0");
            values[4, 1] = new StringBuilder("0");
            values[4, 2] = new StringBuilder("1");
            values[4, 3] = new StringBuilder("0");
            values[4, 4] = new StringBuilder("0");
            values[4, 0] = new StringBuilder(CalcRes(Convert.ToString(sF0100)));

            //0101-------------------------------------------
            StringBuilder sF0101 = new StringBuilder(sFunc);
            sF0101.Replace("x1", "0");
            sF0101.Replace("x2", "1");
            sF0101.Replace("x3", "0");
            sF0101.Replace("x4", "1");
            values[5, 1] = new StringBuilder("0");
            values[5, 2] = new StringBuilder("1");
            values[5, 3] = new StringBuilder("0");
            values[5, 4] = new StringBuilder("1");
            values[5, 0] = new StringBuilder(CalcRes(Convert.ToString(sF0101)));

            //0110-------------------------------------------
            StringBuilder sF0110 = new StringBuilder(sFunc);
            sF0110.Replace("x1", "0");
            sF0110.Replace("x2", "1");
            sF0110.Replace("x3", "1");
            sF0110.Replace("x4", "0");
            values[6, 1] = new StringBuilder("0");
            values[6, 2] = new StringBuilder("1");
            values[6, 3] = new StringBuilder("1");
            values[6, 4] = new StringBuilder("0");
            values[6, 0] = new StringBuilder(CalcRes(Convert.ToString(sF0110)));

            //0111-------------------------------------------
            StringBuilder sF0111 = new StringBuilder(sFunc);
            sF0111.Replace("x1", "0");
            sF0111.Replace("x2", "1");
            sF0111.Replace("x3", "1");
            sF0111.Replace("x4", "1");
            values[7, 1] = new StringBuilder("0");
            values[7, 2] = new StringBuilder("1");
            values[7, 3] = new StringBuilder("1");
            values[7, 4] = new StringBuilder("1");
            values[7, 0] = new StringBuilder(CalcRes(Convert.ToString(sF0111)));

            //1000-------------------------------------------
            StringBuilder sF1000 = new StringBuilder(sFunc);
            sF1000.Replace("x1", "1");
            sF1000.Replace("x2", "0");
            sF1000.Replace("x3", "0");
            sF1000.Replace("x4", "0");
            values[8, 1] = new StringBuilder("1");
            values[8, 2] = new StringBuilder("0");
            values[8, 3] = new StringBuilder("0");
            values[8, 4] = new StringBuilder("0");
            values[8, 0] = new StringBuilder(CalcRes(Convert.ToString(sF1000)));

            //1001-------------------------------------------
            StringBuilder sF1001 = new StringBuilder(sFunc);
            sF1001.Replace("x1", "1");
            sF1001.Replace("x2", "0");
            sF1001.Replace("x3", "0");
            sF1001.Replace("x4", "1");
            values[9, 1] = new StringBuilder("1");
            values[9, 2] = new StringBuilder("0");
            values[9, 3] = new StringBuilder("0");
            values[9, 4] = new StringBuilder("1");
            values[9, 0] = new StringBuilder(CalcRes(Convert.ToString(sF1001)));

            //1010-------------------------------------------
            StringBuilder sF1010 = new StringBuilder(sFunc);
            sF1010.Replace("x1", "1");
            sF1010.Replace("x2", "0");
            sF1010.Replace("x3", "1");
            sF1010.Replace("x4", "0");
            values[10, 1] = new StringBuilder("1");
            values[10, 2] = new StringBuilder("0");
            values[10, 3] = new StringBuilder("1");
            values[10, 4] = new StringBuilder("0");
            values[10, 0] = new StringBuilder(CalcRes(Convert.ToString(sF1010)));

            //1011-------------------------------------------
            StringBuilder sF1011 = new StringBuilder(sFunc);
            sF1011.Replace("x1", "1");
            sF1011.Replace("x2", "0");
            sF1011.Replace("x3", "1");
            sF1011.Replace("x4", "1");
            values[11, 1] = new StringBuilder("1");
            values[11, 2] = new StringBuilder("0");
            values[11, 3] = new StringBuilder("1");
            values[11, 4] = new StringBuilder("1");
            values[11, 0] = new StringBuilder(CalcRes(Convert.ToString(sF1011)));

            //1100-------------------------------------------
            StringBuilder sF1100 = new StringBuilder(sFunc);
            sF1100.Replace("x1", "1");
            sF1100.Replace("x2", "1");
            sF1100.Replace("x3", "0");
            sF1100.Replace("x4", "0");
            values[12, 1] = new StringBuilder("1");
            values[12, 2] = new StringBuilder("1");
            values[12, 3] = new StringBuilder("0");
            values[12, 4] = new StringBuilder("0");
            values[12, 0] = new StringBuilder(CalcRes(Convert.ToString(sF1100)));

            //1101-------------------------------------------
            StringBuilder sF1101 = new StringBuilder(sFunc);
            sF1101.Replace("x1", "1");
            sF1101.Replace("x2", "1");
            sF1101.Replace("x3", "0");
            sF1101.Replace("x4", "1");
            values[13, 1] = new StringBuilder("1");
            values[13, 2] = new StringBuilder("1");
            values[13, 3] = new StringBuilder("0");
            values[13, 4] = new StringBuilder("1");
            values[13, 0] = new StringBuilder(CalcRes(Convert.ToString(sF1101)));

            //1110-------------------------------------------
            StringBuilder sF1110 = new StringBuilder(sFunc);
            sF1110.Replace("x1", "1");
            sF1110.Replace("x2", "1");
            sF1110.Replace("x3", "1");
            sF1110.Replace("x4", "0");
            values[14, 1] = new StringBuilder("1");
            values[14, 2] = new StringBuilder("1");
            values[14, 3] = new StringBuilder("1");
            values[14, 4] = new StringBuilder("0");
            values[14, 0] = new StringBuilder(CalcRes(Convert.ToString(sF1110)));

            //1111-------------------------------------------
            StringBuilder sF1111 = new StringBuilder(sFunc);
            sF1111.Replace("x1", "1");
            sF1111.Replace("x2", "1");
            sF1111.Replace("x3", "1");
            sF1111.Replace("x4", "1");
            values[15, 1] = new StringBuilder("1");
            values[15, 2] = new StringBuilder("1");
            values[15, 3] = new StringBuilder("1");
            values[15, 4] = new StringBuilder("1");
            values[15, 0] = new StringBuilder(CalcRes(Convert.ToString(sF1111)));
        }

        private void CalcSDNF()
        {
            for (int i = 0; i < 16; i++)
            {
                if (values[i, 0].Equals(sOne))
                {
                    if (values[i, 1].Equals(sZero))
                    {
                        sSDNF.Append("( !x1˄");
                    }
                    else
                    {
                        sSDNF.Append("( x1˄");
                    }

                    if (values[i, 2].Equals(sZero))
                    {
                        sSDNF.Append("!x2˄");
                    }
                    else
                    {
                        sSDNF.Append("x2˄");
                    }

                    if (values[i, 3].Equals(sZero))
                    {
                        sSDNF.Append("!x3˄");
                    }
                    else
                    {
                        sSDNF.Append("x3˄");
                    }

                    if (values[i, 4].Equals(sZero))
                    {
                        sSDNF.Append("!x4 ) ˅\r\n");
                    }
                    else
                    {
                        sSDNF.Append("x4 ) ˅\r\n");
                    }
                }
            }
            int last = sSDNF.Length-3;
            if (last > 3)
            {
                sSDNF.Remove(last, 3);
            }           
        }

        private void CalcSKNF()
        {
            for (int i = 0; i < 16; i++)
            {
                if (values[i, 0].Equals(sZero))
                {
                    if (values[i, 1].Equals(sOne))
                    {
                        sSKNF.Append("( !x1˅");
                    }
                    else
                    {
                        sSKNF.Append("( x1˅");
                    }

                    if (values[i, 2].Equals(sOne))
                    {
                        sSKNF.Append("!x2˅");
                    }
                    else
                    {
                        sSKNF.Append("x2˅");
                    }

                    if (values[i, 3].Equals(sOne))
                    {
                        sSKNF.Append("!x3˅");
                    }
                    else
                    {
                        sSKNF.Append("x3˅");
                    }

                    if (values[i, 4].Equals(sOne))
                    {
                        sSKNF.Append("!x4 ) ˄\r\n");
                    }
                    else
                    {
                        sSKNF.Append("x4 ) ˄\r\n");
                    }
                }
            }
            int last = sSKNF.Length - 3;
            if(last > 3)
            {
                sSKNF.Remove(last, 3);
            }                       
        }

        private void ShowResult(){
             lbRes1.Text = Convert.ToString(values[0, 0]);
             lbRes2.Text = Convert.ToString(values[1, 0]);
             lbRes3.Text = Convert.ToString(values[2, 0]);
             lbRes4.Text = Convert.ToString(values[3, 0]);
             lbRes5.Text = Convert.ToString(values[4, 0]);
             lbRes6.Text = Convert.ToString(values[5, 0]);
             lbRes7.Text = Convert.ToString(values[6, 0]);
             lbRes8.Text = Convert.ToString(values[7, 0]);
             lbRes9.Text = Convert.ToString(values[8, 0]);
            lbRes10.Text = Convert.ToString(values[9, 0]);
            lbRes11.Text = Convert.ToString(values[10, 0]);
            lbRes12.Text = Convert.ToString(values[11, 0]);
            lbRes13.Text = Convert.ToString(values[12, 0]);
            lbRes14.Text = Convert.ToString(values[13, 0]);
            lbRes15.Text = Convert.ToString(values[14, 0]);
            lbRes16.Text = Convert.ToString(values[15, 0]);
        }

        private void ShowSDNF()
        {
            richTextBox1.Text = Convert.ToString(sSDNF);
        }

        private void ShowSKNF()
        {
            richTextBox2.Text = Convert.ToString(sSKNF);
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////////////////
        #region buttons;

        private void blocKeyboard(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = '\0';
        }

        private void btnX1_Click(object sender, EventArgs e)
        {
            textBox1.Text += "x1";
        }

        private void btnX2_Click(object sender, EventArgs e)
        {
            textBox1.Text += "x2";
        }

        private void btnX3_Click(object sender, EventArgs e)
        {
            textBox1.Text += "x3";
        }

        private void btnX4_Click(object sender, EventArgs e)
        {
            textBox1.Text += "x4";
        }

        private void btnBrecketOpen_Click(object sender, EventArgs e)
        {
            textBox1.Text += "(";
        }

        private void btnBrecketClose_Click(object sender, EventArgs e)
        {
            textBox1.Text += ")";
        }

        private void btnAND_Click(object sender, EventArgs e)
        {
            textBox1.Text += "˄";
        }

        private void btnOR_Click(object sender, EventArgs e)
        {
            textBox1.Text += "˅";
        }

        private void btnXOR_Click(object sender, EventArgs e)
        {
            textBox1.Text += "⊕";
        }

        private void btnImpl_Click(object sender, EventArgs e)
        {
            textBox1.Text += "→";
        }

        private void btnImplInvers_Click(object sender, EventArgs e)
        {
            textBox1.Text += "←";
        }

        private void btnEqvl_Click(object sender, EventArgs e)
        {
            textBox1.Text += "↔";
        }

        private void btnSheffer_Click(object sender, EventArgs e)
        {
            textBox1.Text += "|";
        }

        private void btnPeirce_Click(object sender, EventArgs e)
        {
            textBox1.Text += "↓";
        }

        private void btnUnarNOT_Click(object sender, EventArgs e)
        {
            textBox1.Text += "!";
        }

        private void btnPoint_Click(object sender, EventArgs e)
        {
            textBox1.Text += ",";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            sSDNF.Clear();
            sSKNF.Clear();

        }

        private void btnResult_Click(object sender, EventArgs e)
        {
            string sFunc = textBox1.Text;
            GetResult(sFunc);
            CalcSDNF();
            CalcSKNF();

            ShowResult();           
            ShowSDNF();           
            ShowSKNF();
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////////////////
        #region ficha
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "((x1˄!(x2)↓!(x3)˄x4)→(x2˄x3|(!(x2)˅!(x4))))⊕(!(x1)˅!(x4))";
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////////////////
    }   
}
