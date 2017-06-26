using System;
using System.Collections.Generic;
using System.Linq;
using VakunTranslatorVol2.Extensions;

namespace VakunTranslatorVol2.Model.Modes
{
    public class Mode
    {
        static Mode()
        {
            Modes = new[]
            {
                new Mode
                {
                    ID = 0,
                    CanLoopWith = x=> x.IsLetter() || x.IsUnderscore() || x.IsDigit(),
                    StartsWith = x => x.IsLetter() || x.IsUnderscore()
                },
                new Mode
                {
                    ID = 1,
                    StartsWith = x=>x.IsSign(),
                    NextModes = new [] { 2, 6 }
                },
                new Mode
                {
                    ID = 2,
                    CanLoopWith = x=>x.IsDigit(),
                    StartsWith = x=>x.IsDigit(),
                    NextModes = new[] { 7,8 }
                },
                new Mode
                {
                    ID = 3,
                    StartsWith = x=>x.IsFullstop(),
                    NextModes = new[] { 7, 13 }
                },
                new Mode
                {
                    ID = 4,
                    StartsWith = x=>x.IsDelimiter()
                },
                new Mode
                {
                    ID = 5,
                    StartsWith = x => x.IsOperator(),
                    NextModes = new[] { 11 }
                },
                new Mode
                {
                    ID = 6,
                    StartsWith = x => x.IsFullstop(),
                    NextModes = new[] { 7 }
                },
                new Mode
                {
                    ID = 7,
                    StartsWith = x=>x.IsDigit() || x.IsFullstop(),
                    CanLoopWith = x=>x.IsDigit(),
                    NextModes = new[] { 8 }
                },
                new Mode
                {
                    ID = 8,
                    StartsWith = x=>x.IsExponent(),
                    NextModes = new[] { 9, 10 }
                },
                new Mode
                {
                    ID = 9,
                    StartsWith = x=>x.IsDigit(),
                    CanLoopWith = x=>x.IsDigit()
                },
                new Mode
                {
                    ID = 10,
                    StartsWith = x=>x.IsSign(),
                    NextModes = new[] { 9 }
                },
                new Mode
                {
                    ID = 11,
                    StartsWith = x=>x.IsEquality()
                },
                new Mode
                {
                    ID = 12,
                    StartsWith = x=>x.IsWhitespace()
                },
                new Mode
                {
                    ID = 13,
                    StartsWith = x=> { throw new ArgumentException($"Unexpected symbol {x}"); }
                },
            };
        }
        public static Mode Initial { get; } = new InitalMode();
        public static event Action<string> LexemeComplited;
        public static event Action NewLine;


        public int[] NextModes { get; protected set; }
        public Predicate<char> StartsWith { get; private set; }
        public int ID { get; private set; }
        public Predicate<char> CanLoopWith { get; private set; }


        public Mode()
        {
            NextModes = new int[] { };
            StartsWith = x => false;
            CanLoopWith = x => false;
        }
        public virtual Mode MoveNext(char c)
        {
            if(CanLoopWith(c))
            {
                Buffer += c;
                return this;
            }

            var next = this[NextModes].FirstOrDefault(x => x.StartsWith(c));
            if(next != null)
            {
                Buffer += c;
                return next;
            }

            PickLast();
            return Initial.MoveNext(c);
        }

        public void PickLast()
        {
            if(!string.IsNullOrWhiteSpace(Buffer))
            {
                LexemeComplited?.Invoke(Buffer);
            }

            Buffer = string.Empty;
        }

        protected static string Buffer { get; set; } = string.Empty;
        protected static Mode[] Modes { get; private set; }

        protected IEnumerable<Mode> this[params int[] modeNumbers]
        {
            get { return Modes.Where(x => modeNumbers.Contains(x.ID)); }
        }
        protected void OnNewLine()
        {
            NewLine?.Invoke();
        }
    }
}