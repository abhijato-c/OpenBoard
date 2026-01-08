using UnityEngine;
using System;
using Bitboard = System.UInt64;

public class Chess {
    public Bitboard wp=0; public Bitboard wr=0; public Bitboard wn=0; public Bitboard wb=0; public Bitboard wq=0; public Bitboard wk=0;
    public Bitboard bp=0; public Bitboard br=0; public Bitboard bn=0; public Bitboard bb=0; public Bitboard bq=0; public Bitboard bk=0;

    public Bitboard bpcs=0; public Bitboard wpcs=0; public Bitboard pieces=0;

    public bool WCastleKing = true; public bool WCastleQueen = true;
    public bool BCastleKing = true; public bool BCastleQueen = true;

    public bool turn=true;

    public void ParseFEN(string FFEN){
        // Split the fen into segments
        string[] segments = FFEN.Split(' ');
        string fen = segments[0];
        char player = Char.Parse(segments[1]);
        string CastleRights = segments[2];
        string EPsquare = segments[3];

        if (player == 'w') turn = true;
        else turn = false;

        wp=0; wr=0; wn=0; wb=0; wq=0; wk=0;
        bp=0; br=0; bn=0; bb=0; bq=0; bk=0;
        foreach (char c in fen) {
            if (Char.IsDigit(c)) {
                wp<<=c-'0'; wr<<=c-'0'; wn<<=c-'0'; wb<<=c-'0'; wq<<=c-'0'; wk<<=c-'0';
                bp<<=c-'0'; br<<=c-'0'; bn<<=c-'0'; bb<<=c-'0'; bq<<=c-'0'; bk<<=c-'0';
            }
            else if (c!='/'){
                wp<<=1; wr<<=1; wn<<=1; wb<<=1; wq<<=1; wk<<=1;
                bp<<=1; br<<=1; bn<<=1; bb<<=1; bq<<=1; bk<<=1;
                switch(c){
                    case 'p':
                        bp+=1;
                        break;
                    case 'n':
                        bn+=1;
                        break;
                    case 'b':
                        bb+=1;
                        break;
                    case 'r':
                        br+=1;
                        break;
                    case 'q':
                        bq+=1;
                        break;
                    case 'P':
                        wp+=1;
                        break;
                    case 'N':
                        wn+=1;
                        break;
                    case 'B':
                        wb+=1;
                        break;
                    case 'R':
                        wr+=1;
                        break;
                    case 'Q':
                        wq+=1;
                        break;
                    case 'k':
                        bk+=1;
                        break;
                    case 'K':
                        wk+=1;
                        break;
                }
            }
        }
        bpcs=bp|br|bn|bb|bq|bk;
        wpcs=wp|wr|wn|wb|wq|wk;
        pieces=bpcs|wpcs;
    }
}