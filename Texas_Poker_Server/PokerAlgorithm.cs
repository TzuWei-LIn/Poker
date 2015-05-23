using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Texas_Poker_Server
{
    class PokerAlgorithm : Server
    {
        public int[] total_card = new int[52];
        public int[] User_card = new int[2];
        public int[] Public_card = new int[5];
        private int[] flower = new int[13];
        private int[] diamond = new int[13];
        private int[] heart = new int[13];
        private int[] spades = new int[13];
        private int[] pair_of_card = new int[13];
        public int CC = 0;          //0.Player + public 1.only public
        public int score { get; set; }
        public String score_result { get; set; }

        public void Main_Process()
        {
            score = 1;
            Console.WriteLine("Come one call me");
            //Public_card[0] = 8;
            //Public_card[1] = 9;
            //Public_card[2] = 10;
            //Public_card[3] = 11;
            //Public_card[4] = 12;
            //CC = 1;
            switch (CC)
            {
                case 0:
                    foreach (int o in User_card)
                    {
                        Check(o);
                        Console.WriteLine("User card = {0}", o);
                    }
                    foreach (int o in Public_card)
                    {
                        Check(o);
                        Console.WriteLine("Public card = {0}", o);
                    }
                    break;
                case 1:
                    foreach (int o in Public_card)
                    {
                        Check(o);
                        Console.WriteLine("Only Public card = {0}", o);
                    }
                    break;
            }
            for (int i = 0; i < 13; i++)            //計算每張牌個出現幾次
            {
                int pair_count = 0;
                if (flower[i] == 1)
                    pair_count++;
                if (diamond[i] == 1)
                    pair_count++;
                if (heart[i] == 1)
                    pair_count++;
                if (spades[i] == 1)
                    pair_count++;
                pair_of_card[i] = pair_count;
            }
            // testcontrol();
            if (Detect_Player_result_Royalflush() != 1)   //判斷皇家同花順
                Console.WriteLine("Score = " + score);
            else if (Detect_Player_result_StraightFlush() != 1)     //判斷同花順
                Console.WriteLine("Score = " + score);
            else if (Detect_Player_result_Four_of_a_Kind() != 1)     //判斷鐵支
                Console.WriteLine("Score = " + score);
            else if (Detect_Player_result_FullHouse() != 1)          //判斷FullHouse
                Console.WriteLine("Score = " + score);
            else if (Detect_PLayer_result_flush() != 1)           //判斷桐花
                Console.WriteLine("Score = " + score);
            else if (Detect_PLayer_result_Straight() != 1)           //判斷順子
                Console.WriteLine("Score = " + score);
            else if (Detect_Player_result_Three_of_a_Kind() != 1)  //判斷三條
                Console.WriteLine("Score = " + score);
            else if (Detect_Player_result_Two_Pair() != 1)           //判斷TWO Pair
                Console.WriteLine("Score = " + score);
            else if (Detect_Player_result_Pair() != 1)                 //判斷Pair
                Console.WriteLine("Score = " + score);
            else 
                Console.WriteLine("Score = " + Detect_Player_result_HighCard());//判斷高排
            Console.WriteLine(score_result);
            Console.WriteLine("End");
            //Console.ReadKey();
        }
        /// <summary>
        /// 純粹比高排
        /// 先判斷是否有A
        /// 剩下從大找到小
        /// </summary>
        /// <returns></returns>
        private int Detect_Player_result_HighCard()
        {
            List<int> ll = new List<int>();
            if (CC == 0)
            {
                foreach (int o in User_card)
                    ll.Add(o % 13);
                foreach (int o in Public_card)
                    ll.Add(o % 13);
            }
            else
                foreach (int o in Public_card)
                    ll.Add(o % 13);
            ll.Sort();
            int i = 0;
            StringBuilder sb = new StringBuilder();
            if (ll.IndexOf(0) != -1)
            {
                ll.Remove(0);
                score += 14;
                sb.Append("A - ");
                i = 1;
            }
            for (int j = i; j < 5; j++)
            {
                score += ll[ll.Count - 1];
                sb.Append(Show_card_head(ll[ll.Count - 1]) + " - ");
                ll.Remove(ll[ll.Count - 1]);
            }
            score_result = sb.ToString();
            return score;
        }
        /// <summary>
        /// 紀錄每種花色的牌出現哪些
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Boolean Check(int card)
        {
                switch (card / 13)
                {
                    case 0:
                        flower[card % 13] = 1;
                        break;
                    case 1:
                        diamond[card % 13] = 1;
                        break;
                    case 2:
                        heart[card % 13] = 1;
                        break;
                    case 3:
                        spades[card % 13] = 1;
                        break;
                }//end switch
                return true;

        }
        /// <summary>
        /// 顯示牌的名稱
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public String Show_card_head(int n)
        {
            if (n % 13 == 0)
                return "A";
            else if ((n % 13) == 12)
                return "K";
            else if ((n % 13) == 11)
                return "Q";
            else if ((n % 13) == 10)
                return "J";
            else
                return ((n % 13 + 1).ToString());
        }
        /// <summary>
        /// 判斷每種花色共有幾張牌
        /// </summary>
        /// <returns></returns>
        public int Detect_which_color()
        {
            int[] c1 = Array.FindAll(flower, aaa);
            int[] c2 = Array.FindAll(diamond, aaa);
            int[] c3 = Array.FindAll(heart, aaa);
            int[] c4 = Array.FindAll(spades, aaa);
            int cc = 0;
            if (c1.Length >= 5)
                cc = 1;
            else if (c2.Length >= 5)
                cc = 2;
            else if (c3.Length >= 5)
                cc = 3;
            else if (c4.Length >= 5)
                cc = 4;
            return cc;
        }

        public bool aaa(int value)
        {
            return (value == 1) ? true : false;
        }

        /// <summary>
        /// 判斷Pair
        /// 先判斷是否只有一對Pair
        /// 剩下找最大三張
        /// 分數:50 * ? + 10*? + 5*? +1*?
        /// max = 901
        /// </summary>
        /// <returns></returns>
        private int Detect_Player_result_Pair()
        {
            int[] tmp1 = { -1, -1, -1 };
            int[] tmp2 = { -1, -1, -1, -1, -1 };
            if (Array.IndexOf(pair_of_card, 3) == -1 && Array.IndexOf(pair_of_card, 2) != -1 && Array.IndexOf(pair_of_card, 4) == -1)
            {
                for (int i = 0, j = 0, k = 0; i < pair_of_card.Length; i++)
                {
                    if (pair_of_card[i] == 1)
                        tmp2[k++] = i;
                    if (pair_of_card[i] == 2)
                        tmp1[j++] = i;
                }
                if (tmp1[0] == 0)
                {
                    score = 50 * 14;
                    score_result = "Pair of A-";
                }
                else
                {
                    score = 50 * tmp1.Max();
                    score_result = "Pair of " + Show_card_head(tmp1.Max()) + "-";
                }
                if (tmp2[0] == 0)
                {
                    score += 10*14;
                    score_result += "A-";
                    Array.Sort(tmp2);
                    score += 5*tmp2[4];
                    score += tmp2[3];
                    score_result += Show_card_head(tmp2[4]) + "-" + Show_card_head(tmp2[3]);
                }
                else
                {
                    Array.Sort(tmp2);
                    score += 10*tmp2[4] + 5*tmp2[3] + tmp2[2];
                    score_result += Show_card_head(tmp2[4]) + "-" + Show_card_head(tmp2[3]) + "-" + Show_card_head(tmp2[2]);
                }
            }
            else
                return 1;
            return score;
        }
        /// <summary>
        /// 先算出有幾個Pair
        /// 找出最大組合
        /// 分數:100 * ? + 50 * ? + ?
        /// max = 2062
        /// </summary>
        /// <returns></returns>
        private int Detect_Player_result_Two_Pair()
        {
            if (Array.IndexOf(pair_of_card, 2) == -1)
                return 1;
            int[] tmp = { -1, -1, -1, -1, -1 };
            List<int> ll = new List<int>();
            for (int i = 0, j = 0; i < pair_of_card.Length; i++)
            {
                if (pair_of_card[i] == 1)
                    tmp[j++] = i;
                if (pair_of_card[i] == 2)
                    ll.Add(i);
            }
            if (ll.Count >= 2)
            {
                if (ll.Count == 2)
                {
                    if (ll[0] == 0)
                    {
                        score = 100 * 14 + 50 * ll[1];
                        score_result = "Two Pair - A&" + Show_card_head(ll[1]) + "-";
                    }
                    else
                    {
                        score = 100 * ll[1] + 50 * ll[0];
                        score_result = "Two Pair - " + Show_card_head(ll[1]) + "&" + Show_card_head(ll[0]) + "-";
                    }
                    if (tmp[0] == 0)
                    {
                        score += 14;
                        score_result += "A";
                    }
                    else
                    {
                        score += tmp.Max();
                        score_result += Show_card_head(tmp.Max());
                    }
                }//end if ==2
                if (ll.Count == 3)
                {
                    if (ll[0] == 0)
                    {
                        score = 100 * 14 + 50 * ll.Max();
                        score_result = "Two Pair - A&" + Show_card_head(ll.Max()) + "-";
                    }
                    else
                    {
                        score = 100 * ll.Max() + 50 * ll[1];
                        score_result = "Two Pair - " + Show_card_head(ll.Max()) + "&" + Show_card_head(ll[1]) + "-";
                    }
                    if (tmp[0] == 0)
                    {
                        score += 14;
                        score_result += "A";
                    }
                    else
                    {
                        score += tmp.Max();
                        score_result += Show_card_head(tmp.Max());
                    }
                }//end if =3
            }
            else
                return 1;

            return score;
        }

        /// <summary>
        /// 先判斷是否只有三張一樣的牌
        /// 找出剩下較大的兩張牌
        /// 分數:250*? + 10*? + ?
        /// max = 3642
        /// </summary>
        /// <returns></returns>
        private int Detect_Player_result_Three_of_a_Kind()
        {
            int[] tmp_1 = { -1, -1 };
            int[] tmp_2 = { -1, -1, -1, -1 };
            if (Array.IndexOf(pair_of_card, 3) != -1 && Array.IndexOf(pair_of_card, 2) == -1)
            {
                for (int i = 0, j = 0, k = 0; i < pair_of_card.Length; i++)
                {
                    if (pair_of_card[i] == 3)
                        tmp_1[j++] = i;
                    else if (pair_of_card[i] == 1)
                        tmp_2[k++] = i;
                }
                if (tmp_1[0] == 0)
                {
                    score = 250 * 14;
                    score_result = "Three of a Kind - A -";
                }
                else
                {
                    score = 250 * tmp_1.Max();
                    score_result = "Three of a Kind - " + Show_card_head(tmp_1.Max()) + "-";
                }
                if (tmp_2[0] == 0)
                {
                    score += 10*14;
                    score += tmp_2.Max();
                    score_result += "A" + "-" + Show_card_head(tmp_2.Max());
                }
                else
                {
                    Array.Sort(tmp_2);
                    score += 10*tmp_2[3];
                    score += tmp_2[2];
                    score_result += Show_card_head(tmp_2[3]) + "-" + Show_card_head(tmp_2[2]);
                }
            }
            else
                return 1;
            return score;
        }

        /// <summary>
        /// 判斷順子
        /// 先將七張牌壓縮 ->取不重複的部分
        /// 如果不重複排少於五張不用判斷
        /// 否則:排序且三種情況
        /// 分數:300*最後一張牌
        /// max = 4200
        /// </summary>
        /// <returns></returns>
        private int Detect_PLayer_result_Straight()
        {
            HashSet<int> ll = new HashSet<int>();
            if (CC == 0)
            {
                foreach (int o in User_card)
                    ll.Add(o % 13);
                foreach (int o in Public_card)
                    ll.Add(o % 13);
            }
            else
                foreach (int o in Public_card)
                    ll.Add(o % 13);

            int[] a = new int[ll.Count];
            if (ll.Count < 5)
                return 1;
            else
            {
                ll.CopyTo(a);
                Array.Sort(a);
                if (a.Length == 5)
                {
                    if (a[0] + 4 == a[4])
                    {
                        score = 300 * a[4];
                        score_result = "順子" + Show_card_head(a[0]) + "-" + Show_card_head(a[4]);
                    }
                    else if (a[0] == 0 && a[1] + 3 == a[4])
                    {
                        score = 300 * 14;
                        score_result = "順子" + Show_card_head(a[1]) + "-A";
                    }
                }//length 5
                else if (a.Length == 6)
                {
                    if (a[0] + 4 == a[4])
                    {
                        score = 300 * a[4];
                        score_result = "順子" + Show_card_head(a[0]) + "-" + Show_card_head(a[4]);
                    }
                    else if (a[1] + 4 == a[5])
                    {
                        score = 300 * a[5];
                        score_result = "順子" + Show_card_head(a[1]) + "-" + Show_card_head(a[5]);
                    }
                    else if (a[0] == 0 && a[2] + 3 == a[5])
                    {
                        score = 300 * 14;
                        score_result = "順子" + Show_card_head(a[2]) + "-A";
                    }
                }//end 6
                else if (a.Length == 7)
                {
                    if (a[0] + 4 == a[4])
                    {
                        score = 300 * a[4];
                        score_result = "順子" + Show_card_head(a[0]) + "-" + Show_card_head(a[4]);
                    }
                    else if (a[1] + 4 == a[5])
                    {
                        score = 300 * a[5];
                        score_result = "順子" + Show_card_head(a[1]) + "-" + Show_card_head(a[5]);
                    }
                    else if (a[2] + 4 == a[6])
                    {
                        score = 300 * a[6];
                        score_result = "順子" + Show_card_head(a[2]) + "-" + Show_card_head(a[6]);
                    }
                    else if (a[0] == 0 && a[3] + 3 == a[6])
                    {
                        score = 300 * 14;
                        score_result = "順子" + Show_card_head(a[3]) + "-A";
                    }
                }//end 7
            }
            return score;
        }

        /// <summary>
        /// 同花
        /// 分數:400*?
        /// max = 5600
        /// </summary>
        /// <returns></returns>
        private int Detect_PLayer_result_flush()
        {
            int cc = Detect_which_color();
            if (cc == 0)   //同花  
                return 1;
            else
            {
                switch (cc)
                {
                    case 1:
                        if (flower[0] == 1)
                        {
                            score_result = "同花帶A";
                            score = 400 * 14;
                            break;
                        }
                        for (int i = 12; i > 0; i--)
                            if (flower[i] == 1)
                            {
                                score_result = "同花帶" + Show_card_head(i);
                                score = 400 * i;
                                break;
                            }
                        break;
                    case 2:
                        if (diamond[0] == 1)
                        {
                            score_result = "同花帶A";
                            score = 400 * 14;
                            break;
                        }
                        for (int i = 12; i > 0; i--)
                            if (diamond[i] == 1)
                            {
                                score_result = "同花帶" + Show_card_head(i);
                                score = 400 * i;
                                break;
                            }
                        break;
                    case 3:
                        if (heart[0] == 1)
                        {
                            score_result = "同花帶A";
                            score = 400 * 14;
                            break;
                        }
                        for (int i = 12; i > 0; i--)
                            if (heart[i] == 1)
                            {
                                score_result = "同花帶" + Show_card_head(i);
                                score = 400 * i;
                                break;
                            }
                        break;
                    case 4:
                        if (heart[0] == 1)
                        {
                            score_result = "同花帶A";
                            score = 400 * 14;
                            break;
                        }
                        for (int i = 12; i > 0; i--)
                            if (heart[i] == 1)
                            {
                                score_result = "同花帶" + Show_card_head(i);
                                score = 400 * i;
                                break;
                            }
                        break;
                }//end switch
            }//end else if
            return score;
        }

        /// <summary>
        /// 判斷是否有3張牌+2張牌
        /// 在找出三張牌是那些
        /// 兩張牌是哪些
        /// 分數:400*? + 100*?
        /// max = 6900
        /// </summary>
        /// <returns></returns>
        private int Detect_Player_result_FullHouse()
        {
            int[] tmp_1 = { -1, -1 };
            int[] tmp_2 = { -1, -1, -1 };
            if (Array.IndexOf(pair_of_card, 3) != -1 && Array.IndexOf(pair_of_card, 2) != -1)
            {
                for (int i = 0, j = 0, k = 0; i < pair_of_card.Length; i++)
                {
                    if (pair_of_card[i] == 3)
                        tmp_1[j++] = i;
                    if (pair_of_card[i] == 2)
                        tmp_2[k++] = i;
                }//end for (find all card if have 3 card or 2)
                if (tmp_1[0] == 0)
                {
                    score = 400 * 14;
                    score_result = "FullHouse - A -";
                }
                else
                {
                    score = 400 * tmp_1.Max();
                    score_result = "FullHouse - " + Show_card_head(tmp_1.Max()) + "-";
                }
                if (tmp_2[0] == 0)
                {
                    score += 100 * 14;
                    score_result += "A";
                }
                else
                {
                    score += 100 * tmp_2.Max();
                    score_result += Show_card_head(tmp_2.Max());
                }
            }
            else
                return 1;
            return score;
        }

        /// <summary>
        /// 判斷是否鐵支
        /// 先判斷是否有4張的排
        /// 分數:600*?+100*?
        /// max = 9700
        /// </summary>
        /// <returns></returns>
        private int Detect_Player_result_Four_of_a_Kind()
        {
            int card = Array.IndexOf(pair_of_card, 4);
            if (card != -1)
            {
                int[] tmp = { -1, -1, -1 };
                for (int i = 0, j = 0; i < pair_of_card.Length; i++)
                    if (pair_of_card[i] == 1)
                        tmp[j++] = i;
                if (card == 0)
                {
                    score = 600 * 14;
                    score_result = "鐵枝- A -";
                }
                else
                {

                    score = 600 * card;
                    score_result = "鐵枝 - " + Show_card_head(card) + "-";
                }
                if (tmp[0] == 0)
                {
                    score += 100 * 14;
                    score_result += "A";
                }
                else
                {
                    score += 100 * tmp.Max();
                    score_result += Show_card_head(tmp.Max());
                }
            }
            else
                return 1;
            return score;
        }

        /// <summary>
        /// Detect 同花順
        /// 分數:700*最後一張牌 + 100*?
        /// max = 11200
        /// </summary>
        /// <returns></returns>
        private int Detect_Player_result_StraightFlush()
        {
            int kk = Detect_which_color();
            if (kk == 0)
                return 1;
            List<int> ll = new List<int>();
            switch (kk)
            {
                case 1:
                    for (int i = 0; i < flower.Length; i++)
                        if (flower[i] == 1)
                            ll.Add(i);
                    if (ll.Count < 5)
                        return 1;
                    int[] a = new int[ll.Count];
                    ll.CopyTo(a);
                    if (a.Length == 5)
                    {
                        if (a[0] + 4 == a[4])
                        {
                            score = 700 * a[4] + 100 * a[4];
                            score_result = "同花順-" + Show_card_head(a[0]) + "-" + Show_card_head(a[4]);
                        }
                        else if (a[0] == 0 && a[1] + 3 == a[4])
                        {
                            score = 700 * 14 + 100 * 14;
                            score_result = "同花順-" + Show_card_head(a[1]) + "-A";
                        }
                    }//length 5
                    else if (a.Length == 6)
                    {
                        if (a[0] + 4 == a[4])
                        {
                            score = 700 * a[4] + 100 * a[4];
                            score_result = "同花順-" + Show_card_head(a[0]) + "-" + Show_card_head(a[4]);
                        }
                        else if (a[1] + 4 == a[5])
                        {
                            score = 700 * a[5] + 100 * a[5];
                            score_result = "同花順-" + Show_card_head(a[1]) + "-" + Show_card_head(a[5]);
                        }
                        else if (a[0] == 0 && a[2] + 3 == a[5])
                        {
                            score = 700 * 14 + 100 * 14;
                            score_result = "同花順-" + Show_card_head(a[2]) + "-A";
                        }
                    }//end 6
                    else if (a.Length == 7)
                    {
                        if (a[0] + 4 == a[4])
                        {
                            score = 700 * a[4] + 100 * a[4];
                            score_result = "同花順-" + Show_card_head(a[0]) + "-" + Show_card_head(a[4]);
                        }
                        else if (a[1] + 4 == a[5])
                        {
                            score = 700 * a[5] + 100 * a[5];
                            score_result = "同花順-" + Show_card_head(a[1]) + "-" + Show_card_head(a[5]);
                        }
                        else if (a[2] + 4 == a[6])
                        {
                            score = 700 * a[6] + 100 * a[6];
                            score_result = "同花順-" + Show_card_head(a[2]) + "-" + Show_card_head(a[6]);
                        }
                        else if (a[0] == 0 && a[3] + 3 == a[6])
                        {
                            score = 700 * 14 + 100 * 14;
                            score_result = "同花順-" + Show_card_head(a[3]) + "-A";
                        }
                    }//end 7
                    break;
                case 2:
                    for (int i = 0; i < diamond.Length; i++)
                        if (diamond[i] == 1)
                            ll.Add(i);
                    if (ll.Count < 5)
                        return 1;
                    a = new int[ll.Count];
                    ll.CopyTo(a);
                    if (a.Length == 5)
                    {
                        if (a[0] + 4 == a[4])
                        {
                            score = 700 * a[4] + 100 * a[4];
                            score_result = "同花順-" + Show_card_head(a[0]) + "-" + Show_card_head(a[4]);
                        }
                        else if (a[0] == 0 && a[1] + 3 == a[4])
                        {
                            score = 700 * 14 + 100 * 14;
                            score_result = "同花順-" + Show_card_head(a[1]) + "-A";
                        }
                    }//length 5
                    else if (a.Length == 6)
                    {
                        if (a[0] + 4 == a[4])
                        {
                            score = 700 * a[4] + 100 * a[4];
                            score_result = "同花順-" + Show_card_head(a[0]) + "-" + Show_card_head(a[4]);
                        }
                        else if (a[1] + 4 == a[5])
                        {
                            score = 700 * a[5] + 100 * a[5];
                            score_result = "同花順-" + Show_card_head(a[1]) + "-" + Show_card_head(a[5]);
                        }
                        else if (a[0] == 0 && a[2] + 3 == a[5])
                        {
                            score = 700 * 14 + 100 * 14;
                            score_result = "同花順-" + Show_card_head(a[2]) + "-A";
                        }
                    }//end 6
                    else if (a.Length == 7)
                    {
                        if (a[0] + 4 == a[4])
                        {
                            score = 700 * a[4] + 100 * a[4];
                            score_result = "同花順-" + Show_card_head(a[0]) + "-" + Show_card_head(a[4]);
                        }
                        else if (a[1] + 4 == a[5])
                        {
                            score = 700 * a[5] + 100 * a[5];
                            score_result = "同花順-" + Show_card_head(a[1]) + "-" + Show_card_head(a[5]);
                        }
                        else if (a[2] + 4 == a[6])
                        {
                            score = 700 * a[6] + 100 * a[6];
                            score_result = "同花順-" + Show_card_head(a[2]) + "-" + Show_card_head(a[6]);
                        }
                        else if (a[0] == 0 && a[3] + 3 == a[6])
                        {
                            score = 700 * 14 + 100 * 14;
                            score_result = "同花順-" + Show_card_head(a[3]) + "-A";
                        }
                    }//end 7
                    break;
                case 3:
                    for (int i = 0; i < heart.Length; i++)
                        if (heart[i] == 1)
                            ll.Add(i);
                    if (ll.Count < 5)
                        return 1;
                    a = new int[ll.Count];
                    ll.CopyTo(a);
                    if (a.Length == 5)
                    {
                        if (a[0] + 4 == a[4])
                        {
                            score = 700 * a[4] + 100 * a[4];
                            score_result = "同花順-" + Show_card_head(a[0]) + "-" + Show_card_head(a[4]);
                        }
                        else if (a[0] == 0 && a[1] + 3 == a[4])
                        {
                            score = 700 * 14 + 100 * 14;
                            score_result = "同花順-" + Show_card_head(a[1]) + "-A";
                        }
                    }//length 5
                    else if (a.Length == 6)
                    {
                        if (a[0] + 4 == a[4])
                        {
                            score = 700 * a[4] + 100 * a[4];
                            score_result = "同花順-" + Show_card_head(a[0]) + "-" + Show_card_head(a[4]);
                        }
                        else if (a[1] + 4 == a[5])
                        {
                            score = 700 * a[5] + 100 * a[5];
                            score_result = "同花順-" + Show_card_head(a[1]) + "-" + Show_card_head(a[5]);
                        }
                        else if (a[0] == 0 && a[2] + 3 == a[5])
                        {
                            score = 700 * 14 + 100 * 14;
                            score_result = "同花順-" + Show_card_head(a[2]) + "-A";
                        }
                    }//end 6
                    else if (a.Length == 7)
                    {
                        if (a[0] + 4 == a[4])
                        {
                            score = 700 * a[4] + 100 * a[4];
                            score_result = "同花順-" + Show_card_head(a[0]) + "-" + Show_card_head(a[4]);
                        }
                        else if (a[1] + 4 == a[5])
                        {
                            score = 700 * a[5] + 100 * a[5];
                            score_result = "同花順-" + Show_card_head(a[1]) + "-" + Show_card_head(a[5]);
                        }
                        else if (a[2] + 4 == a[6])
                        {
                            score = 700 * a[6] + 100 * a[6];
                            score_result = "同花順-" + Show_card_head(a[2]) + "-" + Show_card_head(a[6]);
                        }
                        else if (a[0] == 0 && a[3] + 3 == a[6])
                        {
                            score = 700 * 14 + 100 * 14;
                            score_result = "同花順-" + Show_card_head(a[3]) + "-A";
                        }
                    }//end 7
                    break;
                case 4:
                    for (int i = 0; i < spades.Length; i++)
                        if (spades[i] == 1)
                            ll.Add(i);
                    if (ll.Count < 5)
                        return 1;
                    a = new int[ll.Count];
                    ll.CopyTo(a);
                    if (a.Length == 5)
                    {
                        if (a[0] + 4 == a[4])
                        {
                            score = 700 * a[4] + 100 * a[4];
                            score_result = "同花順-" + Show_card_head(a[0]) + "-" + Show_card_head(a[4]);
                        }
                        else if (a[0] == 0 && a[1] + 3 == a[4])
                        {
                            score = 700 * 14 + 100 * 14;
                            score_result = "同花順-" + Show_card_head(a[1]) + "-A";
                        }
                    }//length 5
                    else if (a.Length == 6)
                    {
                        if (a[0] + 4 == a[4])
                        {
                            score = 700 * a[4] + 100 * a[4];
                            score_result = "同花順-" + Show_card_head(a[0]) + "-" + Show_card_head(a[4]);
                        }
                        else if (a[1] + 4 == a[5])
                        {
                            score = 700 * a[5] + 100 * a[5];
                            score_result = "同花順-" + Show_card_head(a[1]) + "-" + Show_card_head(a[5]);
                        }
                        else if (a[0] == 0 && a[2] + 3 == a[5])
                        {
                            score = 700 * 14 + 100 * 14;
                            score_result = "同花順-" + Show_card_head(a[2]) + "-A";
                        }
                    }//end 6
                    else if (a.Length == 7)
                    {
                        if (a[0] + 4 == a[4])
                        {
                            score = 700 * a[4] + 100 * a[4];
                            score_result = "同花順-" + Show_card_head(a[0]) + "-" + Show_card_head(a[4]);
                        }
                        else if (a[1] + 4 == a[5])
                        {
                            score = 700 * a[5] + 100 * a[5];
                            score_result = "同花順-" + Show_card_head(a[1]) + "-" + Show_card_head(a[5]);
                        }
                        else if (a[2] + 4 == a[6])
                        {
                            score = 700 * a[6] + 100 * a[6];
                            score_result = "同花順-" + Show_card_head(a[2]) + "-" + Show_card_head(a[6]);
                        }
                        else if (a[0] == 0 && a[3] + 3 == a[6])
                        {
                            score = 700 * 14 + 100 * 14;
                            score_result = "同花順-" + Show_card_head(a[3]) + "-A";
                        }
                    }//end 7
                    break;
            }
            return score;
        }
        /// <summary>
        /// 皇家同花順
        /// 分數:800000
        /// </summary>
        /// <returns></returns>
        private int Detect_Player_result_Royalflush()
        {
            if (flower[0] == 1 && flower[9] == 1 && flower[10] == 1 && flower[11] == 1 && flower[12] == 1 ||
               diamond[0] == 1 && diamond[9] == 1 && diamond[10] == 1 && diamond[11] == 1 && diamond[12] == 1 ||
               heart[0] == 1 && heart[9] == 1 && heart[10] == 1 && heart[11] == 1 && heart[12] == 1 ||
               spades[0] == 1 && spades[9] == 1 && spades[10] == 1 && spades[11] == 1 && spades[12] == 1)//皇家同花順
            {
                score = 800000;
                score_result = "皇家同花順";
                return score;
            }
            return score;
        }
    }
}
