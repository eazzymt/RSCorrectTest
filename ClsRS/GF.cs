
namespace ClsRS
{
    /// <summary>
    /// ガロア拡大体演算クラス
    /// </summary>
    class GF
    {
        public short exp, val;
        public enum InKind
        {
            inVal,
            inExp
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="inData"></param>
        /// <param name="kind"></param>
        public GF(short inData, InKind kind)
        {
            if (kind == InKind.inExp)
            {
                exp = (short)(inData % (RS.GF_MOD - 1));
                val = RS.exp2val[exp];
            }
            else
            {
                val = inData;
                exp = RS.val2exp[val];
            }
        }

        public GF(GF gf)
        {
            if (gf == null)
            {
                exp = -1;
                val = 0;
            }
            else
            {
                exp = gf.exp;
                val = gf.val;
            }
        }

        /// <summary>
        /// 足し算
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        public static GF operator + (GF val1, GF val2)
        {
            if (val1 == null || val1.val == 0) return new GF(val2);
            if (val2 == null || val2.val == 0) return new GF(val1);

            return new GF((short)(val1.val ^ val2.val), InKind.inVal);
        }

        /// <summary>
        /// 引き算（GF(2^m)上では足し算と等価）
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        public static GF operator -(GF val1, GF val2)
        {
            return val1 + val2;
        }

        /// <summary>
        /// 掛け算
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        public static GF operator *(GF val1, GF val2)
        {
            GF result;
            if (val1 == null || val2 == null || val1.val == 0 || val2.val == 0)
            {
                result = new GF(0, InKind.inVal);
            }
            else
            {
                result = new GF((short)((val1.exp + val2.exp) % (RS.GF_MOD - 1)), InKind.inExp);
            }

            return result;
        }

        /// <summary>
        /// 割り算(０除算のチェック不要なら、掛け算と兼用できるが・・・)
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        public static GF operator /(GF val1, GF val2)
        {
            if (val2 == null || val2.val == 0)
            {
                throw new System.DivideByZeroException();
            }
            else if (val1 == null || val1.val == 0)
            {
                return new GF(0, InKind.inVal);
            }
            else
            {
                return new GF((short)((val1.exp - val2.exp + RS.GF_MOD - 1) % (RS.GF_MOD - 1)), InKind.inExp);
            }
        }
    }
}
