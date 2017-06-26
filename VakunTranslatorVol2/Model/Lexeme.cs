using System;
using System.Collections.Generic;
using System.Linq;

namespace VakunTranslatorVol2.Model
{
    [Flags]
    public enum LexemeFlags
    {
        None = 0x0,
        Const = 0x1,
        Id = 0x2,
        InlineDelimiter = 0x4,
        EndlineDelimiter = 0x8,
        TypeDefinition = 0x10,
        Reserved = 0x20,
        ControlConstruction = 0x40,
        Operator = 0x80
    }

    public class Lexeme
    {
        public string Body { get; private set; }
        public LexemeFlags Flags { get; private set; }
        public string Type { get; set; }
        public int Code { get; set; }
        public int Line { get; set; }
        public int ConstCode { get; set; }

        public Lexeme(string body, int code)
        {
            Body = body;
            Code = code;
        }

        private Lexeme(string body, LexemeFlags flags, int code)
        {
            Body = body;
            Flags = flags;
            Code = code;
            Type = string.Empty;
            Line = -1;
            ConstCode = -1;
        }

        public static Lexeme Parse(string value)
        {
            var lexemeCode = GetCode(value);
            var flags = GetFlags(value);

            return CreateConstLexeme(value) ?? new Lexeme(value, flags, lexemeCode);
        }

        private static int GetCode(string value)
        {
            if (lexemTable.ContainsKey(value))
            {
                return lexemTable.Keys.ToList().IndexOf(value);
            }

            return (int)LexemeCodes.ID;
        }
        private static LexemeFlags GetFlags(string value)
        {
            if (lexemTable.ContainsKey(value))
            {
                return lexemTable[value];
            }

            return LexemeFlags.Id;
        }
        private static Lexeme CreateConstLexeme(string value)
        {
            var lexemCode = (int)LexemeCodes.CONSTANT;

            if (int.TryParse(value, out int iConst))
            {
                return new Lexeme(value, LexemeFlags.Const, lexemCode) { Type = "int" };
            }

            if (double.TryParse(value.Replace(".", ","), out double dConst))
            {
                return new Lexeme(value, LexemeFlags.Const, lexemCode) { Type = "float" };
            }

            return null;
        }

        public bool Is(LexemeFlags flag)
        {
            return (Flags & flag) != 0;
        }

        public bool Is(LexemeCodes code)
        {
            return (LexemeCodes)Code == code;
        }

        public override bool Equals(object obj)
        {
            return (obj as Lexeme)?.Body.Equals(Body) ?? false;
        }

        public override int GetHashCode()
        {
            return Code;
        }

        public override string ToString()
        {
            return $"{(LexemeCodes)Code} '{Body}'";
        }

        private static Dictionary<string, LexemeFlags> lexemTable = new Dictionary<string, LexemeFlags>
        {
            ["program"] = LexemeFlags.TypeDefinition,
            ["float"] = LexemeFlags.TypeDefinition,
            ["int"] = LexemeFlags.TypeDefinition,
            ["label"] = LexemeFlags.TypeDefinition,
            ["{"] = LexemeFlags.ControlConstruction,
            ["}"] = LexemeFlags.ControlConstruction,
            [";"] = LexemeFlags.EndlineDelimiter,
            [","] = LexemeFlags.InlineDelimiter,
            ["("] = LexemeFlags.ControlConstruction,
            [")"] = LexemeFlags.ControlConstruction,
            ["["] = LexemeFlags.ControlConstruction,
            ["]"] = LexemeFlags.ControlConstruction,
            ["if"] = LexemeFlags.Reserved,
            ["then"] = LexemeFlags.Reserved,
            ["while"] = LexemeFlags.Reserved,
            ["for"] = LexemeFlags.Reserved,
            ["="] = LexemeFlags.Operator,
            ["read"] = LexemeFlags.Reserved,
            ["write"] = LexemeFlags.Reserved,
            ["<"] = LexemeFlags.Operator,
            [">"] = LexemeFlags.Operator,
            ["<="] = LexemeFlags.Operator,
            [">="] = LexemeFlags.Operator,
            ["+"] = LexemeFlags.Operator,
            ["-"] = LexemeFlags.Operator,
            ["*"] = LexemeFlags.Operator,
            ["/"] = LexemeFlags.Operator,
            ["&"] = LexemeFlags.Operator,
            ["|"] = LexemeFlags.Operator,
            ["=="] = LexemeFlags.Operator,
            ["!="] = LexemeFlags.Operator,
            ["!"] = LexemeFlags.Operator,
            ["to"] = LexemeFlags.Reserved,
            ["by"] = LexemeFlags.Reserved,
            ["end"] = LexemeFlags.Reserved,
            ["goto"] = LexemeFlags.Reserved
        };
    }
}