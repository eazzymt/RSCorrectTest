using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClsRS
{
    public class RS
    {
        #region 変数
        private const int GFDim = 8;
        private int rsLen;
        private int parityLen;
        private GF[] genPoly;

        public const int GF_POW = 8; // GF(2^m)のmの値
        public const short GF_MOD = 1 << GF_POW; // GF(2^m)は、整数をGF_MODで割った余りの数

        // 指数⇒数値対応表
        // 【算出ロジック】
        // 数値：v=0～255
        // 指数：e=2^v (ただし、255を越えたら e=(29 Xor (v^2 And 255)、v=255のときは計算せずにe=0を設定)
        // 指数eの性質：0 <= v <= 254の範囲において、eは重複しない値(1～255のいずれか)を持つ
        //              v=255として計算するとe=1となり(つまり、周期255)、v=0の時と重複するので、e=0としておく
        public static short[] val2exp = new short[GF_MOD];
        public static short[] exp2val = new short[GF_MOD];

        #endregion

        public RS(int RSLen, int ParityLen)
        {
            this.rsLen = RSLen;
            this.parityLen = ParityLen;

            GenGF();
            MakeGenPoly();
        }

        /// <summary>
        /// GF(2^m)の一覧テーブル作成
        /// 指数→値:exp2val
        /// 値→指数:val2exp
        /// 
        /// 【算出ロジック(exp2val)】
        /// 数値：v=0～255
        /// 指数：e=2^v (ただし、255を越えたら e=(29 Xor (v^2 And 255)、v=255のときは計算せずにe=0を設定)
        /// 指数eの性質：0 ＜= v ＜= 254の範囲において、eは重複しない値(1～255のいずれか)を持つ
        /// v=255として計算するとe=1となり(つまり、周期255)、v=0の時と重複するので、e=0としておく
        /// val2expはexp2valの逆変換（exp2valに重複がないので、逆変換の作成が可能）
        /// </summary>
        private void GenGF()
        {
            short val = 1;
            short[] primPoly = { 1, 0, 1, 1, 1, 0, 0, 0, 1 }; // 原始多項式 p(x) = 1 + x^2 + x^3 + x^4 + x^8

            exp2val[GF_POW] = 0;
            // α^0～α^7 （桁あふれなし）
            for (short cnt = 0; cnt < GF_POW; cnt++)
            {
                exp2val[cnt] = val;
                val2exp[exp2val[cnt]] = cnt;
                if (primPoly[cnt] != 0) exp2val[GF_POW] ^= val;
                val *= 2;
            }
            val2exp[exp2val[GF_POW]] = GF_POW;
            val >>= 1;

            // α^9～α254 （桁あふれあり）
            for (short cnt = GF_POW + 1; cnt < GF_MOD - 1; cnt++)
            {
                if (exp2val[cnt - 1] >= val)
                {
                    exp2val[cnt] = (short)((((exp2val[cnt - 1] ^ val) * 2) ^ exp2val[GF_POW]) % GF_MOD);
                }
                else
                {
                    exp2val[cnt] = (short)((exp2val[cnt - 1] * 2) % GF_MOD);
                }

                val2exp[exp2val[cnt]] = cnt;
            }
            val2exp[0] = -1;
        }

        /// <summary>
        /// 指定パリティ長の生成多項式を作成
        /// </summary>
        /// <returns>末尾を最大次数、要素０を定数項とする</returns>
        private void MakeGenPoly()
        {
            // (x + α^n)をn = 0～parityLen-1について、順次かけていく

            // 初期データ = x + α^0 (= x + 1)
            genPoly = new GF[] { new GF(0, GF.InKind.inExp), new GF(0, GF.InKind.inExp) };

            for (short cnt = 1; cnt < parityLen; cnt++)
            {
                GF[] tmpGF = new GF[] {
                    new GF(cnt, GF.InKind.inExp),
                    new GF(0, GF.InKind.inExp) }; // 掛ける多項式 = x + α^n
                genPoly = MulPoly(tmpGF, genPoly);
            }
        }

        /// <summary>
        /// エンコード
        /// 入力データからRS法エラー訂正符号を生成する
        /// </summary>
        /// <param name="inData"></param>
        /// <returns></returns>
        public byte[] Encode(byte[] inData)
        {
            byte[] encAry = new byte[rsLen];
            GF[] eccGFAry = new GF[rsLen];

            // 入力データをGF(2^m)に変換
            for (int cnt = 0; cnt < inData.Length; cnt++)
            {
                eccGFAry[rsLen - cnt - 1] = new GF(inData[cnt], GF.InKind.inVal);
            }

            // エラー訂正符号の算出
            // エラー訂正符号の多項式＝「(データ部の多項式×X^(n-k))÷生成多項式」の余り
            eccGFAry = DividePoly(eccGFAry, genPoly);

            // エンコードデータ＝入力データ＋エラー訂正符号
            Array.Copy(inData, encAry, inData.Length);
            for (int cnt = 0; cnt < parityLen; cnt++)
            {
                encAry[rsLen - cnt - 1] = (byte)eccGFAry[cnt].val;
            }

            return encAry;
        }

        /// <summary>
        /// RS符号をエラー訂正して復号する
        /// </summary>
        /// <param name="encData"></param>
        /// <returns></returns>
        public byte[] Decode(byte[] encData)
        {
            byte[] decData = new byte[rsLen - parityLen];
            GF[] synd = new GF[parityLen];
            bool haveErr = false;
            GF[] encGFData = new GF[rsLen];

            for (int cnt = 0; cnt < rsLen; cnt++)
            {
                encGFData[rsLen - 1 - cnt] = new GF(encData[cnt], GF.InKind.inVal);
            }

            // シンドローム多項式
            haveErr = CalcSynd(encGFData, synd);
            if (!haveErr) // エラー無し
            {
                Array.Copy(encData, decData, encData.Length - parityLen);
                return decData;
            }

            // 誤り位置に関する多項式
            GF[] elp = new GF[(parityLen >> 1) + 1];
            GF[] eep = new GF[parityLen >> 1];
            CalcElpEep(elp, eep, synd);

            // 誤りロケータ
            GF[] eLoc = new GF[GetDegPoly(elp)];
            if (!CalcErrLoc(elp, eLoc)) return null;

            // 誤りデータ
            GF[] errP = new GF[rsLen];
            CalcErr(elp, eep, eLoc, errP);

            // 誤り訂正
            GF[] decGFDataWithPari = MinusPoly(encGFData, errP);
            for (int cnt = 0; cnt < decData.Length; cnt++)
            {
                decData[cnt] = (byte)(decGFDataWithPari[rsLen - 1 - cnt].val);
            }

            return decData;
        }

        /// <summary>
        /// シンドローム多項式を求める
        /// </summary>
        /// <param name="encData"></param>
        /// <returns></returns>
        private bool CalcSynd(GF[] encData, GF[] synd)
        {
            bool haveErr = false;

            for (short cntS = 0; cntS < parityLen; cntS++)
            {
                synd[cntS] = SubstAlpha(encData, cntS);
                haveErr |= synd[cntS].val != 0; // シンドロームのどれか１つに非０があれば、エラー
            }

            return haveErr;
        }

        /// <summary>
        /// 誤り位置多項式(elp)、誤り多項式(eep)を求める
        /// (ユークリッド復号法)
        /// </summary>
        /// <param name="elp"></param>
        /// <param name="eep"></param>
        /// <param name="synd"></param>
        private void CalcElpEep(GF[] elp, GF[] eep, GF[] synd)
        {
            GF[] pA, pA1, pA2, pQ; // A_i, A_(i-1), A(i-2), q_i
            GF[] pV, pV1, pV2;     // V_i, V_(i-1), V(i-2)
            GF[] pU, pU1, pU2;     // U_i, U_(i-1), U(i-2)
            int errCorNum = parityLen >> 1; // 誤り訂正可能数

            // 初期値設定
            pA2 = new GF[errCorNum * 2 + 1];
            pA2[errCorNum * 2] = new GF(0, GF.InKind.inExp); // z^(2t)
            pA1 = synd;
            pV2 = new GF[1]; // 0
            pV1 = new GF[1] { new GF(0, GF.InKind.inExp) }; // 1
            pU2 = new GF[1] { new GF(0, GF.InKind.inExp) }; // 1
            pU1 = new GF[1]; // 0

            for (int cnt = 1; cnt <= 2 * errCorNum; cnt++)
            {
                // 各パラメータを計算

                // pA2÷pA1の商をpQに格納
                pQ = QuotPoly(pA2, pA1);

                // pA = -pQ * pA1 + pA2
                pA = MinusPoly(pA2, MulPoly(pQ, pA1));

                // pV = pV2 - pV1 * pQ
                pV = MinusPoly(pV2, MulPoly(pV1, pQ));

                // pU = pU2 - pU1 * pQ
                pU = MinusPoly(pU2, MulPoly(pU1, pQ));

                if (GetDegPoly(pV) <= errCorNum && GetDegPoly(pA) <= errCorNum - 1)
                {
                    // pVを正規化する（pVの定数項を１にする）
                    // pV,pAの各項をpV(0)[＝pVの定数項]で割る
                    // 帳尻を合わせるため、pAもpV(0)で割る
                    // →それぞれ、誤り位置多項式、誤り評価多項式とする
                    for (int cnt1 = 0; cnt1 < Math.Min(pV.Length, elp.Length); cnt1++)
                    {
                        elp[cnt1] = pV[cnt1] / pV[0];
                    }
                    for (int cnt1 = 0; cnt1 < Math.Min(pA.Length, eep.Length); cnt1++)
                    {
                        eep[cnt1] = pA[cnt1] / pV[0];
                    }

                    //{
                    //    // for DEBUG（検証用コード）
                    //    GF[] tmpU = new GF[pU.Length];
                    //    // pUを正規化
                    //    for (int cnt1 = 0; cnt1 < pU.Length; cnt1++)
                    //    {
                    //        tmpU[cnt1] = pU[cnt1] / pV[0];
                    //    }
                    //    GF[] tmpA0 = new GF[errCorNum * 2 + 1];
                    //    tmpA0[errCorNum * 2] = new GF(0, GF.inKind.inExp); // z^(2t)
                    //    GF[] tmpUA = MulPoly(tmpU, tmpA0);
                    //    GF[] tmpVB = MulPoly(elp, synd);
                    //    GF[] tmpEEP = PlusPoly(tmpUA, tmpVB); // ←eepに等しいはず
                    //}

                    return;
                }

                // 次のループの準備
                pA2 = new GF[pA1.Length];
                Array.Copy(pA1, pA2, pA1.Length);
                pA1 = new GF[pA.Length];
                Array.Copy(pA, pA1, pA.Length);

                pU2 = new GF[pU1.Length];
                Array.Copy(pU1, pU2, pU1.Length);
                pU1 = new GF[pU.Length];
                Array.Copy(pU, pU1, pU.Length);

                pV2 = new GF[pV1.Length];
                Array.Copy(pV1, pV2, pV1.Length);
                pV1 = new GF[pV.Length];
                Array.Copy(pV, pV1, pV.Length);
            }
        }

        /// <summary>
        /// 各誤りロケータを求める
        /// </summary>
        /// <param name="elp"></param>
        /// <param name="eLoc"></param>
        private bool CalcErrLoc(GF[] elp, GF[] eLoc)
        {
            int cntLoc = 0;

            // elp(z)の根となるGF[2^m]の元が誤りの数だけあるはず
            // それらを総当たりで求める
            for (short cntAlpha = 0; cntAlpha < GF_MOD; cntAlpha++)
            {
                GF loc = SubstAlpha(elp, cntAlpha);
                if (loc.val == 0)
                {
                    // 誤りロケータはelp(z)の根の逆数
                    eLoc[cntLoc] = new GF(1, GF.InKind.inVal) / new GF(cntAlpha, GF.InKind.inExp);
                    cntLoc++;
                    if (cntLoc == eLoc.Length) return true;
                }
            }

            return false; // エラー訂正不能？
        }

        /// <summary>
        /// 各誤り値を算出
        /// </summary>
        /// <param name="elp"></param>
        /// <param name="eep"></param>
        /// <param name="eLoc"></param>
        /// <param name="errP"></param>
        private void CalcErr(GF[] elp, GF[] eep, GF[] eLoc, GF[] errP)
        {
            GF[] dElp = new GF[elp.Length - 1]; // elp(z)の導関数

            // 誤り位置多項式を形式的に微分
            dElp[0] = new GF(0, GF.InKind.inVal);
            for (short cnt = 1; cnt < elp.Length; cnt++)
            {
                dElp[cnt - 1] = new GF(cnt, GF.InKind.inVal) * elp[cnt];
            }

            // 各誤り値の算出
            // e[k] = elp(α^(-jk) ÷ elpP(α^(-jk)) * eLoc[jk]
            for (int cnt = 0; cnt < eLoc.Length; cnt++)
            {
                GF elpPAlpha = SubstAlpha(dElp, (short)((GF_MOD - eLoc[cnt].exp) % GF_MOD));
                GF eepAlpha = SubstAlpha(eep, (short)((GF_MOD - eLoc[cnt].exp) % GF_MOD));
                errP[eLoc[cnt].exp] = eepAlpha / elpPAlpha * eLoc[cnt];
            }
        }

        /// <summary>
        /// p1÷p2の商と余りを求める
        /// 結果は商×x^n＋余り ← 余りの次数＝p1の次数-1
        /// where n＝余りの次数＋１
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private GF[] DividePoly(GF[] p1, GF[] p2)
        {
            int p1Len = GetDegPoly(p1) + 1;
            int p2Len = GetDegPoly(p2) + 1;
            GF[] result = new GF[p1Len];

            Array.Copy(p1, p1.Length - p1Len, result, 0, p1Len); // p1の最高次数が要素０とは限らない
            for (int cnt1 = 0; cnt1 < p1Len - p2Len + 1; cnt1++)
            {
                result[p1Len - 1 - cnt1] /= p2[p2Len - 1]; // 各項の商
                for (int cnt2 = 0; cnt2 < p2Len - 1; cnt2++)
                {
                    result[p1Len - 2 - cnt1 - cnt2] += result[p1Len - 1 - cnt1] * p2[p2Len - 2 - cnt2];
                }
            }

            return result;
        }

        /// <summary>
        /// p1÷p2の商を求める
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private GF[] QuotPoly(GF[] p1, GF[] p2)
        {
            int p1Len = GetDegPoly(p1) + 1;
            int p2Len = GetDegPoly(p2) + 1;
            GF[] quot = new GF[p1Len - p2Len + 1];

            GF[] divide = DividePoly(p1, p2);

            // 余りの配列長＝p2Len-1
            // 商の配列長＝p1Len-(p2Len-1)
            // 余りの分を飛ばして、商の配列長分をコピーする
            Array.Copy(divide, p2Len - 1, quot, 0, p1Len - p2Len + 1);

            return quot;
        }

        /// <summary>
        /// p1×p2 (多項式の掛け算)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private GF[] MulPoly(GF[] p1, GF[] p2, int len = -1)
        {
            GF[] pM;
            if (len <= 0)
            {
                // 配列長＝最高次数＋１（※係数が０であることは考慮していない）
                pM = new GF[p1.Length + p2.Length - 1];
            }
            else
            {
                pM = new GF[len];
            }

            for (int cnt1 = 0; cnt1 < p1.Length; cnt1++)
            {
                for (int cnt2 = 0; cnt2 < p2.Length; cnt2++)
                {
                    pM[cnt1 + cnt2] += p2[cnt2] * p1[cnt1];
                }
            }

            return pM;
        }

        /// <summary>
        /// p1＋p2 (多項式の足し算)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private GF[] PlusPoly(GF[] p1, GF[] p2)
        {
            GF[] result = new GF[Math.Max(p1.Length, p2.Length)];

            // 配列同士の足し合わせが可能なように、長さをそろえる
            GF[] pp1 = new GF[result.Length];
            GF[] pp2 = new GF[result.Length];
            Array.Copy(p1, pp1, p1.Length);
            Array.Copy(p2, pp2, p2.Length);

            for (int cnt = 0; cnt < result.Length; cnt++)
            {
                result[cnt] = pp1[cnt] + pp2[cnt];
            }

            return result;
        }

        /// <summary>
        /// p1－p2 (多項式の引き算)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private GF[] MinusPoly(GF[] p1, GF[] p2)
        {
            return PlusPoly(p1, p2); // 足し算と引き算が等価なので、これで問題なし
        }

        /// <summary>
        /// 指定多項式の最高次数を求める
        /// </summary>
        /// <param name="pol"></param>
        /// <returns></returns>
        private int GetDegPoly(GF[] pol)
        {
            int deg = 0;

            for (int cnt = pol.Length - 1; cnt >= 0; cnt--)
            {
                if (pol[cnt] != null && pol[cnt].val != 0)
                {
                    deg = cnt;
                    break;
                }
            }

            return deg;
        }

        /// <summary>
        /// 多項式f(x) = pol に x = α^expを代入
        /// </summary>
        /// <param name="pol"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        private GF SubstAlpha(GF[] pol, short exp)
        {
            GF result = new GF(0, GF.InKind.inVal);

            for (int cnt = 0; cnt < pol.Length; cnt++)
            {
                result += pol[cnt] * new GF((short)(exp * cnt), GF.InKind.inExp);
            }

            return result;
        }
    }
}
