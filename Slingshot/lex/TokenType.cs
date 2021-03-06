﻿
namespace Slingshot
{
    namespace Compiler
    {

        public enum TokenType
        {
            Slingshot,
            Integer,
            Float,
            SciNumber,
            String,
            Char,
            //------------------ 16
            LeftBracket,        //[
            RightBracket,       //]
            LeftParentheses,	// (
            RightParenthese,	// )
            LeftCurlyBracket,   //{
            RightCurlyBracket,  //}

            Colon,				// :
            SemiColon,          // ;
            Comma,				// ,
            QMark,              // ?
            Tide,               // ~
            Caret,              // ^


            Add,				// +
            AddAsign,			// +=
            Inc,                //++

            Sub,				// -
            SubAsign,			// -=
            Dec,                //--

            Mul,				// *
            MulAsign,			// *=
            NPow,               // **

            Div,				// +
            DivAsign,			// /=
            Comment,			// // xxxx

            Mod,				// %
            ModAsign,			// %=

            LT,					// <
            LE,					// <=
            BitSL,              // <<

            GT,					// >
            GE,					// >=
            BitSR,              // >>

            //------------------
            EQ,					// ==
            Assign,				// =

            NE,					// !=

            //------------------------
            BitXOR,             // ^
            BitXORAsign,        // ^=
            BitAnd,             // &
            BitAndAsign,        // &=
            BitOr,              // |
            BitOrAsign,         // |=

            Break,
            Continue,
            Identifier,
            //
            True,
            False,
            And,				// and
            Or,					// or
            Not,				// not !
            Def,                // def
            UnDef,                // undef
            Func,               // func
            If,
            ElIf,
            //Begin,              // begin
            List,               // list
            //Else,
            //While,
            //For,
            //In,

            Note,               // @ abc

            Test,                // test{}
            Unknown
        }
    }
}
