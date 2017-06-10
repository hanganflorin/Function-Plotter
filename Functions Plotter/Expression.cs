using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions_Plotter
{
    class Expression
    {
        //campurile
        private string sign;
        private float value;
        private Expression left;
        private Expression right;

        //constructori
        public Expression()
        {
            sign = "";
            value = 0;
            left = null;
            right = null;
        }
        public Expression(string _sign, float _value)
        {
            sign = _sign;
            value = _value;
            left = null;
            right = null;
        }
        public Expression(string _sign, float _value, Expression _left, Expression _right)
        {
            sign = _sign;
            value = _value;
            left = _left;
            right = _right;
        }
        private void RemoveSpaces(ref string s)
        {
            string aux = null;
            for ( int i = 0; i < s.Length; ++i )
                if ( s[i] != ' ' )
                    aux += s[i];
            s = aux;
        }
        private bool RemoveUselessBrackets(ref string s)
        {
            int brack = 0;
            if (s[0] != '(' || s[s.Length - 1] != ')')
                return false;
            for ( int i = 0; i < s.Length-1; ++i )
            {
                if ( s[i] == '(' )
                    brack++;
                if (s[i] == ')')
                    brack--;
                if (brack == 0)
                    return false;
            }
            s = s.Remove(0, 1);
            s = s.Remove(s.Length - 1, 1);
            return true;
        }
        private int FindSignNotInBrackets(string sign, string s)
        {
            int brack = 0;
            for ( int i = 0; i < s.Length - sign.Length+1; ++i )
            {
                if (s[i] == '(')
                    brack++;
                if (s[i] == ')')
                    brack--;
                if (brack == 0 && s.Substring(i, sign.Length) == sign)
                    return i;
            }
            return -1;
        }
        private void CutString(string s, int pos, ref string left, ref string right)
        {
            left = s.Remove(pos);
            right = s.Remove(0, pos + 1);
        }
        
        // functia FromString
        public Expression FromString(string s)
        {
            if (s == "")
                return new Expression("n", 0);
            int pos;
            string leftString = null, rightString = null;
            RemoveSpaces(ref s);
            while (RemoveUselessBrackets(ref s)) ; // eliminam toate spatiile si parantezele intutile
            pos = FindSignNotInBrackets("+", s);  // cautam pozitia fiecarui semn matematic care nu se afla intr-o paranteza
            if (pos != -1)
            {
                CutString(s, pos, ref leftString, ref rightString);
                return new Expression("+", -1, FromString(leftString), FromString(rightString)); // daca am gasit semnul returnam o noua expresie
                                                                                                  //din cele doua subexpresii 
            }
            pos = FindSignNotInBrackets("-", s);
            if (pos != -1)
            {
                CutString(s, pos, ref leftString, ref rightString);
                return new Expression("-", -1, FromString(leftString), FromString(rightString));
            }
            pos = FindSignNotInBrackets("*", s);
            if (pos != -1)
            {
                CutString(s, pos, ref leftString, ref rightString);
                return new Expression("*", -1, FromString(leftString), FromString(rightString));
            }
            pos = FindSignNotInBrackets("/", s);
            if (pos != -1)
            {
                CutString(s, pos, ref leftString, ref rightString);
                return new Expression("/", -1, FromString(leftString), FromString(rightString));
            }
            pos = FindSignNotInBrackets("^", s);
            if (pos != -1)
            {
                CutString(s, pos, ref leftString, ref rightString);
                return new Expression("^", -1, FromString(leftString), FromString(rightString));
            }
            pos = FindSignNotInBrackets("asin", s);
            if (pos != -1)
                return new Expression("asin", -1, null, FromString(s.Substring(4)));

            pos = FindSignNotInBrackets("acos", s);
            if (pos != -1)
                return new Expression("acos", -1, null, FromString(s.Substring(4)));

            pos = FindSignNotInBrackets("atan", s);
            if (pos != -1)
                return new Expression("atan", -1, null, FromString(s.Substring(4)));

            pos = FindSignNotInBrackets("sin", s);
            if (pos != -1)
                return new Expression("sin", -1, null, FromString( s.Substring(3) ));

            pos = FindSignNotInBrackets("cos", s);
            if (pos != -1)
                return new Expression("cos", -1, null, FromString(s.Substring(3)));

            pos = FindSignNotInBrackets("tan", s);
            if (pos != -1)
                return new Expression("tan", -1, null, FromString(s.Substring(3)));

            pos = FindSignNotInBrackets("ln", s);
            if (pos != -1)
                return new Expression("ln", -1, null, FromString(s.Substring(2)));

            pos = FindSignNotInBrackets("sqrt", s);
            if (pos != -1)
                return new Expression("sqrt", -1, null, FromString(s.Substring(4)));

            pos = FindSignNotInBrackets("abs", s);
            if (pos != -1)
                return new Expression("abs", -1, null, FromString(s.Substring(3)));

            pos = FindSignNotInBrackets("log", s);
            if (pos != -1)
            {
                s = s.Substring(3);
                while (RemoveUselessBrackets(ref s)) ;
                pos = FindSignNotInBrackets(",", s);
                if (pos == -1)
                    throw new ArithmeticException();
                CutString(s, pos, ref leftString, ref rightString);
                return new Expression("log", -1, FromString(leftString), FromString(rightString));
            }

            pos = FindSignNotInBrackets("x", s);
            if (pos != -1)
                return new Expression("x", 0);
            pos = FindSignNotInBrackets("e", s);
            if (pos != -1)
                return new Expression("e", 0);
            pos = FindSignNotInBrackets("pi", s);
            if (pos != -1)
                return new Expression("pi", 0);
            pos = FindSignNotInBrackets("a", s);
            if (pos != -1)
                return new Expression("a", 0);
            pos = FindSignNotInBrackets("b", s);
            if (pos != -1)
                return new Expression("b", 0);
            pos = FindSignNotInBrackets("c", s);
            if (pos != -1)
                return new Expression("c", 0);
            pos = FindSignNotInBrackets("d", s);
            if (pos != -1)
                return new Expression("d", 0);
            try
            {
                return new Expression("n", Convert.ToSingle(s));
            }
            catch
            {
                throw new System.ArithmeticException();
            }
        }

        public float Calculate(float x, float a, float b, float c, float d)
        {
            switch (sign)
            {
                case "+": return left.Calculate(x, a, b, c, d) + right.Calculate(x, a, b, c, d);
                case "-": return left.Calculate(x, a, b, c, d) - right.Calculate(x, a, b, c, d);
                case "*": return left.Calculate(x, a, b, c, d) * right.Calculate(x, a, b, c, d);
                case "/": return left.Calculate(x, a, b, c, d) / right.Calculate(x, a, b, c, d);
                case "^": return Convert.ToSingle(Math.Pow(Convert.ToDouble(left.Calculate(x, a, b, c, d)), Convert.ToDouble(right.Calculate(x, a, b, c, d))));
                case "sin": return Convert.ToSingle(Math.Sin(right.Calculate(x, a, b, c, d)));
                case "cos": return Convert.ToSingle(Math.Cos(right.Calculate(x, a, b, c, d)));
                case "tan": return Convert.ToSingle(Math.Tan(right.Calculate(x, a, b, c, d)));
                case "acos": return Convert.ToSingle(Math.Acos(right.Calculate(x, a, b, c, d)));
                case "asin": return Convert.ToSingle(Math.Asin(right.Calculate(x, a, b, c, d)));
                case "atan": return Convert.ToSingle(Math.Atan(right.Calculate(x, a, b, c, d)));
                case "ln": return Convert.ToSingle(Math.Log(right.Calculate(x, a, b, c, d)));
                case "log": return Convert.ToSingle(Math.Log(right.Calculate(x, a, b, c, d), left.Calculate(x, a, b, c, d)));
                case "sqrt": return Convert.ToSingle(Math.Sqrt(right.Calculate(x, a, b, c, d)));
                case "abs": return Convert.ToSingle(Math.Abs(right.Calculate(x, a, b, c, d)));
                case "n": return value;
                case "x": return x;
                case "e": return Convert.ToSingle(Math.E);
                case "pi": return Convert.ToSingle(Math.PI); 
                case "a": return a;
                case "b": return b;
                case "c": return c;
                case "d": return d;
                default: return 0;
            }
        }
        
    }
}
