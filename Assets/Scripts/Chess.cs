using System;
using System.Collections.Generic;
using Unity.Mathematics;

using Bitboard = System.UInt64;
using Move = System.Int32;


public class Chess {
    Bitboard clear_a=9187201950435737471UL;
    Bitboard clear_b=13816973012072644543UL;
    //Bitboard clear_c=16131858542891098079UL;
    //Bitboard clear_d=17289301308300324847UL;
    //Bitboard clear_e=17868022691004938231UL;
    //Bitboard clear_f=18157383382357244923UL;
    Bitboard clear_g=18302063728033398269UL;
    Bitboard clear_h=18374403900871474942UL;

    //Bitboard mask_a=9259542123273814144UL;
    //Bitboard mask_b=4629771061636907072UL;
    //Bitboard mask_c=2314885530818453536UL;
    //Bitboard mask_d=1157442765409226768UL;
    //Bitboard mask_e=578721382704613384UL;
    //Bitboard mask_f=289360691352306692UL;
    //Bitboard mask_g=144680345676153346UL;
    //Bitboard mask_h=72340172838076673UL;

    //Bitboard clear_8=72057594037927935UL;
    //Bitboard clear_7=18374967954648334335UL;
    //Bitboard clear_6=18446463698244468735UL;
    //Bitboard clear_5=18446742978492891135UL;
    //Bitboard clear_4=18446744069431361535UL;
    //Bitboard clear_3=18446744073692839935UL;
    //Bitboard clear_2=18446744073709486335UL;
    //Bitboard clear_1=18446744073709551360UL;

    //Bitboard mask_8=18374686479671623680UL;
    Bitboard mask_7=71776119061217280UL;
    //Bitboard mask_6=280375465082880UL;
    //Bitboard mask_5=1095216660480UL;
    //Bitboard mask_4=4278190080UL;
    //Bitboard mask_3=16711680UL;
    Bitboard mask_2=65280UL;
    //Bitboard mask_1=255UL;
    public Bitboard wp=0; public Bitboard wr=0; public Bitboard wn=0; public Bitboard wb=0; public Bitboard wq=0; public Bitboard wk=0;
    public Bitboard bp=0; public Bitboard br=0; public Bitboard bn=0; public Bitboard bb=0; public Bitboard bq=0; public Bitboard bk=0;

    public Bitboard bpcs=0; public Bitboard wpcs=0; public Bitboard pieces=0;

    public bool WCastleKing = true; public bool WCastleQueen = true;
    public bool BCastleKing = true; public bool BCastleQueen = true;

    public bool turn=true;
    public int ctz(Bitboard brd) {return math.tzcnt(brd);}
    public int popcnt(Bitboard brd) {return math.countbits(brd);}

    public void ParseFEN(string FFEN){
        // Split the fen into segments
        string[] segments = FFEN.Split(' ');
        string fen = segments[0];
        char player = Char.Parse(segments[1]);
        string CastleRights = segments[2];
        string EPsquare = segments[3];

        // Reset board
        wp=0; wr=0; wn=0; wb=0; wq=0; wk=0;
        bp=0; br=0; bn=0; bb=0; bq=0; bk=0;
        WCastleKing = false; BCastleKing = false; WCastleQueen = false; BCastleQueen = false;

        // Set turn
        if (player == 'w') turn = true;
        else turn = false;

        // Castling rights
        foreach (char c in CastleRights) {
            if (c == 'K') WCastleKing = true;
            else if (c == 'Q') WCastleQueen = true;
            else if (c == 'k') BCastleKing = true;
            else if (c == 'q') BCastleQueen = true;
        }

        // Piece arrangement
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

    public string GenerateFen() {
        string fen = "";
        string t = turn ? "w":"b";
        string CastleRights = "";

        for (int rank = 7; rank >= 0; rank--) {
            int emptyCount = 0;

            for (int file = 0; file < 8; file++) {
                int shift = rank*8 + 7-file;
                
                // Check occupancy
                if (((pieces >> shift) & 1) == 0) emptyCount++;
                else {
                    if (emptyCount > 0) {
                        fen += emptyCount;
                        emptyCount = 0;
                    }
                    
                    // Identify and append the piece character
                    if (((wp >> shift) & 1) != 0) fen += 'P';
                    else if (((wr >> shift) & 1) != 0) fen += 'R';
                    else if (((wn >> shift) & 1) != 0) fen += 'N';
                    else if (((wb >> shift) & 1) != 0) fen += 'B';
                    else if (((wq >> shift) & 1) != 0) fen += 'Q';
                    else if (((wk >> shift) & 1) != 0) fen += 'K';
                    else if (((bp >> shift) & 1) != 0) fen += 'p';
                    else if (((br >> shift) & 1) != 0) fen += 'r';
                    else if (((bn >> shift) & 1) != 0) fen += 'n';
                    else if (((bb >> shift) & 1) != 0) fen += 'b';
                    else if (((bq >> shift) & 1) != 0) fen += 'q';
                    else if (((bk >> shift) & 1) != 0) fen += 'k';
                }
            }

            // Append remaining empty squares at the end of the rank
            if (emptyCount > 0) fen += emptyCount;
            // Add separator between ranks, but not after the last one
            if (rank > 0) fen += '/';
        }

        if(WCastleKing) CastleRights += 'K';
        if(WCastleQueen) CastleRights += 'Q';
        if(BCastleKing) CastleRights += 'k';
        if(BCastleQueen) CastleRights += 'q';
        if(CastleRights == "") CastleRights = "-";

        return fen + ' ' + t + ' '+ CastleRights + " - 1 1";
    }
    public void WKmoves(int i, ref List<Move> Moves){
        if (((((1UL << i) & clear_h) << 7) &~ wpcs) != 0)
            Moves.Add(ctz(1UL << i << 7) << 6 | i);
        if (((1UL << i << 8) &~ wpcs) != 0)
            Moves.Add(ctz((1UL << i) << 8) << 6 | i);
        if (((((1UL << i) & clear_a) << 9) &~ wpcs) != 0)
            Moves.Add(ctz((1UL << i) << 9) << 6 | i);
        if (((((1UL << i) & clear_a) << 1) &~ wpcs) != 0)
            Moves.Add(ctz((1UL << i) << 1) << 6 | i);
        if (((((1UL << i) & clear_a) >> 7) &~ wpcs) != 0)
            Moves.Add(ctz((1UL << i) >> 7) << 6 | i);
        if ((((1UL << i) >> 8) &~ wpcs) != 0)
            Moves.Add(ctz((1UL << i) >> 8) << 6 | i);
        if (((((1UL << i) & clear_h) >> 9) &~ wpcs) != 0)
            Moves.Add(ctz((1UL << i) >> 9) << 6 | i);
        if (((((1UL << i) & clear_h) >> 1) &~ wpcs) != 0)
            Moves.Add(ctz((1UL << i) >> 1) << 6 | i);
    }

    public void BKmoves(int i, ref List<Move> Moves){
        if (((((1UL << i) & clear_h) << 7) &~ bpcs) != 0)
            Moves.Add(ctz((1UL << i) << 7) << 6 | i);
        if ((((1UL << i) << 8) &~ bpcs) != 0)
            Moves.Add(ctz((1UL << i) << 8) << 6 | i);
        if (((((1UL << i) & clear_a) << 9) &~ bpcs) != 0)
            Moves.Add(ctz((1UL << i) << 9) << 6 | i);
        if (((((1UL << i) & clear_a) << 1) &~ bpcs) != 0)
            Moves.Add(ctz((1UL << i) << 1) << 6 | i);
        if (((((1UL << i) & clear_a) >> 7) &~ bpcs) != 0)
            Moves.Add(ctz((1UL << i) >> 7) << 6 | i);
        if ((((1UL << i) >> 8) &~ bpcs) != 0)
            Moves.Add(ctz((1UL << i) >> 8) << 6 | i);
        if (((((1UL << i) & clear_h) >> 9) &~ bpcs) != 0)
            Moves.Add(ctz((1UL << i) >> 9) << 6 | i);
        if (((((1UL << i) & clear_h) >> 1) &~ bpcs) != 0)
            Moves.Add(ctz((1UL << i) >> 1) << 6 | i);
    }

    public void WNmoves(int i, ref List<Move> Moves){
        if (((((1UL << i) & clear_a & clear_b) >> 6) &~ wpcs) != 0)
            Moves.Add(ctz((1UL << i) >> 6) << 6 | i);
        if (((((1UL << i) & clear_a) >> 15) &~ wpcs) != 0)
            Moves.Add(ctz((1UL << i) >> 15) << 6 | i);
        if (((((1UL << i) & clear_h) >> 17) &~ wpcs) != 0)
            Moves.Add(ctz((1UL << i) >> 17) << 6 | i);
        if (((((1UL << i) & clear_h & clear_g) >> 10) &~ wpcs) != 0)
            Moves.Add(ctz((1UL << i) >> 10) << 6 | i);
        if (((((1UL << i) & clear_h & clear_g) << 6) &~ wpcs) != 0)
            Moves.Add(ctz((1UL << i) << 6) << 6 | i);
        if (((((1UL << i) & clear_h) << 15) &~ wpcs) != 0)
            Moves.Add(ctz((1UL << i) << 15) << 6 | i);
        if (((((1UL << i) & clear_a) << 17) &~ wpcs) != 0)
            Moves.Add(ctz((1UL << i) << 17) << 6 | i);
        if (((((1UL << i) & clear_a & clear_b) << 10) &~ wpcs) != 0)
            Moves.Add(ctz((1UL << i) << 10) << 6 | i);
    }

    public void BNmoves(int i, ref List<Move> Moves){
        if (((((1UL << i) & clear_a & clear_b) >> 6) &~ bpcs) != 0)
            Moves.Add(ctz((1UL << i) >> 6) << 6 | i);
        if (((((1UL << i) & clear_a) >> 15) &~ bpcs) != 0)
            Moves.Add(ctz((1UL << i) >> 15) << 6 | i);
        if (((((1UL << i) & clear_h) >> 17) &~ bpcs) != 0)
            Moves.Add(ctz((1UL << i) >> 17) << 6 | i);
        if (((((1UL << i) & clear_h & clear_g) >> 10) &~ bpcs) != 0)
            Moves.Add(ctz((1UL << i) >> 10) << 6 | i);
        if (((((1UL << i) & clear_h & clear_g) << 6) &~ bpcs) != 0)
            Moves.Add(ctz((1UL << i) << 6) << 6 | i);
        if (((((1UL << i) & clear_h) << 15) &~ bpcs) != 0)
            Moves.Add(ctz((1UL << i) << 15) << 6 | i);
        if (((((1UL << i) & clear_a) << 17) &~ bpcs) != 0)
            Moves.Add(ctz((1UL << i) << 17) << 6 | i);
        if (((((1UL << i) & clear_a & clear_b) << 10) &~ bpcs) != 0)
            Moves.Add(ctz((1UL << i) << 10) << 6 | i);
    }

    public void WPmoves(int i, ref List<Move> Moves){
        if ((((1UL << i) << 8) &~ pieces) != 0)
            Moves.Add(ctz((1UL << i) << 8) << 6 | i);
        if (((((((1UL << i) & mask_2) << 8) &~ pieces) << 8) &~ pieces) != 0)
            Moves.Add(ctz((1UL << i) << 16) << 6 | i);
        if ((((1UL << i) << 7) & bpcs & clear_a) != 0)
            Moves.Add(ctz((1UL << i) << 7) << 6 | i);
        if ((((1UL << i) << 9) & bpcs & clear_h) != 0)
            Moves.Add(ctz((1UL << i) << 9) << 6 | i);
    }

    public void BPmoves(int i, ref List<Move> Moves){
        if ((((1UL << i) >> 8) &~ pieces) != 0)
            Moves.Add(ctz((1UL << i) >> 8) << 6 | i);
        if (((((((1UL << i) & mask_7) >> 8) &~ pieces) >> 8) &~ pieces) != 0)
            Moves.Add(ctz((1UL << i) >> 16) << 6 | i);
        if ((((1UL << i) >> 7) & wpcs & clear_h) != 0)
            Moves.Add(ctz((1UL << i) >> 7) << 6 | i);
        if ((((1UL << i) >> 9) & wpcs & clear_a) != 0)
            Moves.Add(ctz((1UL << i) >> 9) << 6 | i);
    }

    public void WBmoves(int i, ref List<Move> Moves){
        Bitboard piece = 1UL << i;
        Bitboard blockers=pieces;
        Bitboard moves=0;
        Bitboard loc=piece<<9;
        if((loc&clear_h)!=0){
            moves|=loc;
            while (((loc&blockers)==0) && (loc&clear_a)!=0){
                loc <<= 9;
                moves |= loc;
            }
        }
        loc=piece<<7;
        if((loc&clear_a)!=0){
            moves|=loc;
            while (((loc&blockers)==0) && (loc&clear_h)!=0){
                loc <<= 7;
                moves |= loc;
            }
        }
        loc=piece>>7;
        if((loc&clear_h)!=0){
            moves|=loc;
            while (((loc&blockers)==0) && ((loc&clear_a)!=0)){
                loc >>= 7;
                moves |= loc;
            }
        }
        loc=piece>>9;
        if((loc&clear_a)!=0){
            moves|=loc;
            while (((loc&blockers)==0) && ((loc&clear_h)!=0)){
                loc >>= 9;
                moves |= loc;
            }
        }
        moves &= ~wpcs;

        int j = -1;
        while(moves!=0){
            j += ctz(moves) + 1;
            moves >>= ctz(moves);
            moves >>= 1;

            Moves.Add((j << 6) | i);
        }
    }

    public void BBmoves(int i, ref List<Move> Moves){
        Bitboard piece = 1UL << i;
        Bitboard blockers=pieces;
        Bitboard moves=0;
        Bitboard loc=piece<<9;
        if((loc&clear_h)!=0){
            moves|=loc;
            while (((loc&blockers)==0) && (loc&clear_a)!=0){
                loc <<= 9;
                moves |= loc;
            }
        }
        loc=piece<<7;
        if((loc&clear_a)!=0){
            moves|=loc;
            while (((loc&blockers)==0) && (loc&clear_h)!=0){
                loc <<= 7;
                moves |= loc;
            }
        }
        loc=piece>>7;
        if((loc&clear_h)!=0){
            moves|=loc;
            while (((loc&blockers)==0) && ((loc&clear_a)!=0)){
                loc >>= 7;
                moves |= loc;
            }
        }
        loc=piece>>9;
        if((loc&clear_a)!=0){
            moves|=loc;
            while (((loc&blockers)==0) && ((loc&clear_h)!=0)){
                loc >>= 9;
                moves |= loc;
            }
        }
        moves &= ~bpcs;

        int j = -1;
        while(moves!=0){
            j += ctz(moves) + 1;
            moves >>= ctz(moves);
            moves >>= 1;

            Moves.Add((j << 6) | i);
        }
    }

    public void WRmoves(int i, ref List<Move> Moves){
        Bitboard piece = 1UL << i;
        Bitboard blockers = pieces;
        Bitboard moves=0;
        Bitboard loc = piece<<8;

        if(loc!=0){
            moves|=loc;
            while (((loc&blockers)==0) && (loc!=0)){
                loc <<= 8;
                moves |= loc;
            }
        }
        loc=piece>>8;
        if(loc!=0){
            moves|=loc;
            while (((loc&blockers)==0) && (loc!=0)){
                loc >>= 8;
                moves |= loc;
            }
        }
        loc=piece<<1;
        if((loc&clear_h)!=0){
            moves|=loc;
            while (((loc&blockers)==0) && ((loc&clear_a)!=0)){
                loc <<= 1;
                moves |= loc;
            }
        }
        loc=piece>>1;
        if((loc&clear_a)!=0){
            moves|=loc;
            while (((loc&blockers)==0) && ((loc&clear_h)!=0)){
                loc >>= 1;
                moves |= loc;
            }
        }
        moves &= ~wpcs;

        int j = -1;
        while(moves!=0){
            j += ctz(moves) + 1;
            moves >>= ctz(moves);
            moves >>= 1;

            Moves.Add((j << 6) | i);
        }
    }

    public void BRmoves(int i, ref List<Move> Moves){
        Bitboard piece = 1UL << i;
        Bitboard blockers = pieces;
        Bitboard moves=0;
        Bitboard loc = piece<<8;

        if(loc!=0){
            moves|=loc;
            while (((loc&blockers)==0) && (loc!=0)){
                loc <<= 8;
                moves |= loc;
            }
        }
        loc=piece>>8;
        if(loc!=0){
            moves|=loc;
            while (((loc&blockers)==0) && (loc!=0)){
                loc >>= 8;
                moves |= loc;
            }
        }
        loc=piece<<1;
        if((loc&clear_h)!=0){
            moves|=loc;
            while (((loc&blockers)==0) && ((loc&clear_a)!=0)){
                loc <<= 1;
                moves |= loc;
            }
        }
        loc=piece>>1;
        if((loc&clear_a)!=0){
            moves|=loc;
            while (((loc&blockers)==0) && ((loc&clear_h)!=0)){
                loc >>= 1;
                moves |= loc;
            }
        }
        moves &= ~ bpcs;
        int j = -1;
        while(moves!=0){
            j += ctz(moves) + 1;
            moves >>= ctz(moves);
            moves >>= 1;

            Moves.Add((j << 6) | i);
        }
    }

    public void WQmoves(int i, ref List<Move> Moves){
        WRmoves(i, ref Moves);
        WBmoves(i, ref Moves);
    }

    public void BQmoves(int i, ref List<Move> Moves){
        BRmoves(i, ref Moves);
        BBmoves(i, ref Moves);
    }
    void move_piece_white(Move mv){
        Bitboard t = ~(1UL << ((mv>>6) & 63));
        bb &= t;
        bk &= t;
        bn &= t;
        bp &= t;
        bq &= t;
        br &= t;

        if(((mv>>12) & 7) != 0){
            wp &= ~(1UL << (mv & 63));
            switch((mv>>12) & 7){
                case 1:
                    wn |= ~t; break;
                case 2:
                    wb |= ~t; break;
                case 3:
                    wr |= ~t; break;
                case 4:
                    wq |= ~t; break;
            }
        }
        else{
            Bitboard f = 1UL << (mv & 63);
            if((wb&f)!=0) wb=(wb&(~f))|~t;
            else if((wr&f)!=0){
                wr=(wr&(~f))|~t;
                if ((mv & 63) == 0) WCastleKing = false;
                else if ((mv & 63)==7) WCastleQueen = false;
            }
            else if((wn&f)!=0) wn=(wn&(~f))|~t;
            else if((wp&f)!=0) wp=(wp&(~f))|~t;
            else if((wq&f)!=0) wq=(wq&(~f))|~t;
            else{
                wk=(wk&(~f))|~t;
                WCastleKing = false;
                WCastleQueen = false;
                // Check if castling move, then move rook
                if (mv == 67) wr = (wr & ~(1UL << 0)) | (1UL << 2);
                else if (mv == 323) wr = (wr & ~(1UL << 7)) | (1UL << 4);
            }
        }
    }

    void move_piece_black(Move mv){
        Bitboard t = ~(1UL << ((mv>>6) & 63));
        wb &= t; wk &= t; wn &= t; wp &= t; wq &= t; wr &= t;

        if(((mv>>12) & 7) != 0){
            bp &= ~(1UL << (mv & 63));
            switch((mv>>12) & 7){
                case 1:
                    bn |= ~t; break;
                case 2:
                    bb |= ~t; break;
                case 3:
                    br |= ~t; break;
                case 4:
                    bq |= ~t; break;
            }
        }
        else{
            Bitboard f = 1UL << (mv & 63);
            if((bb&f)!=0) bb=(bb&(~f))|~t;
            else if((br&f)!=0){
                br=(br&(~f))|~t;
                if ((mv & 63) == 56) BCastleKing = false;
                else if ((mv & 63) == 63) BCastleQueen = false;
            }
            else if((bn&f)!=0) bn=(bn&(~f))|~t;
            else if((bp&f)!=0) bp=(bp&(~f))|~t;
            else if((bq&f)!=0) bq=(bq&(~f))|~t;
            else{
                bk=(bk&(~f))|~t;
                BCastleKing = false;
                BCastleQueen = false;
                // Check if castling move, then move rook
                if (mv == 3707) br = (br & ~(1UL << 56)) | (1UL << 58);
                else if (mv == 3963) br = (br & ~(1UL << 63)) | (1UL << 60);
            }
        }
    }

    void move_piece(Move mv){
        if(turn) move_piece_white(mv);
        else move_piece_black(mv);
        turn = !turn;
        bpcs=bp|br|bn|bb|bq|bk;
        wpcs=wp|wr|wn|wb|wq|wk;
        pieces=bpcs|wpcs;
    }
    public string MovePieceSAN(Move mv){
        // Also returns the move in san, a bodged fix for the move history in the GUI
        string san = "";
        char[] files = {'a','b','c','d','e','f','g','h'};

        int to = (mv>>6) & 63;
        int from = mv & 63;
        Bitboard fromBB = 1UL << from;
        Bitboard toBB = 1UL << to;

        // Add piece identifier
        if (((wk | bk) & fromBB) != 0) san += "K";
        else if (((wq | bq) & fromBB) != 0) san += "Q";
        else if (((wr | br) & fromBB) != 0) san += "R";
        else if (((wb | bb) & fromBB) != 0) san += "B";
        else if (((wn | bn) & fromBB) != 0) san += "N";

        // Disambiguation file/rank
        if (san == "") {
            if (from%8 != to%8)  san += files[7 - (from % 8)]; // Pawn disambiguation for capture
        }
        else if (san != "K"){ // No disambiguation for kings
            bool fileConflict = false;
            bool rankConflict = false;
            bool duplicateFound = false;

            List<Move> candidates = LegalMoves();

            foreach (Move m in candidates) {
                int mFrom = m & 63;
                int mTo = (m >> 6) & 63;

                if (mTo == to && mFrom != from) {
                    Bitboard mFromBB = 1UL << mFrom;

                    // Conflicting piece of same type?
                    if ((san == "Q" && ((wq | bq) & mFromBB) != 0) || (san == "R" && ((wr | br) & mFromBB) != 0) ||
                    (san == "B" && ((wb | bb) & mFromBB) != 0) || (san == "N" && ((wn | bn) & mFromBB) != 0)) {
                        duplicateFound = true;
                        if ((mFrom % 8) == (from % 8)) fileConflict = true; 
                        if ((mFrom / 8) == (from / 8)) rankConflict = true; 
                    }
                }
            }

            if (duplicateFound) {
                if (!fileConflict) san += files[7 - (from % 8)].ToString();
                else if (!rankConflict) san += ((from / 8) + 1).ToString();
                else san += files[7 - (from % 8)].ToString() + ((from / 8) + 1).ToString();
            }
        }

        // Capture identifier
        if ((toBB & pieces) != 0) san += "x";

        // To square
        san += files[7 - (to%8)];
        san += (to/8) + 1;

        // Move the piece
        if(turn) move_piece_white(mv);
        else move_piece_black(mv);
        turn = !turn;
        bpcs=bp|br|bn|bb|bq|bk;
        wpcs=wp|wr|wn|wb|wq|wk;
        pieces=bpcs|wpcs;

        // Castling handle
        if (mv == 67 || mv == 3707) san = "O-O";
        else if (mv == 323 || mv == 3963) san = "O-O-O";

        // Check, checkmate, and draw
        if (IsCheckmate()) san += "#";
        else if (IsStalemate() || IsInsufficientMaterial()) san += "½-½";
        else if (IsCheck()) san += "+";

        return san;
    }
    public List<Move> PseudoLegalMoves(){
        List<Move> Moves = new List<Move>();
        if (turn){
            WKmoves(ctz(wk), ref Moves);

            Bitboard pc = wn;
            int i = -1;
            while(pc!=0){
                i += ctz(pc) + 1;
                pc >>= ctz(pc);
                pc >>= 1;
                WNmoves(i, ref Moves);
            }

            pc = wb;
            i = -1;
            while(pc!=0){
                i += ctz(pc) + 1;
                pc >>= ctz(pc);
                pc >>= 1;
                WBmoves(i, ref Moves);
            }

            pc = wp;
            i = -1;
            while(pc!=0){
                i += ctz(pc) + 1;
                pc >>= ctz(pc);
                pc >>= 1;
                WPmoves(i, ref Moves);
            }

            pc = wr;
            i = -1;
            while(pc!=0){
                i += ctz(pc) + 1;
                pc >>= ctz(pc);
                pc >>= 1;
                WRmoves(i, ref Moves);
            }

            pc = wq;
            i = -1;
            while(pc!=0){
                i += ctz(pc) + 1;
                pc >>= ctz(pc);
                pc >>= 1;
                WQmoves(i, ref Moves);
            }
        }
        else{
            BKmoves(ctz(bk), ref Moves);

            Bitboard pc = bn;
            int i = -1;
            while(pc!=0){
                i += ctz(pc) + 1;
                pc >>= ctz(pc);
                pc >>= 1;
                BNmoves(i, ref Moves);
            }

            pc = bb;
            i = -1;
            while(pc!=0){
                i += ctz(pc) + 1;
                pc >>= ctz(pc);
                pc >>= 1;
                BBmoves(i, ref Moves);
            }

            pc = bp;
            i = -1;
            while(pc!=0){
                i += ctz(pc) + 1;
                pc >>= ctz(pc);
                pc >>= 1;
                BPmoves(i, ref Moves);
            }

            pc = br;
            i = -1;
            while(pc!=0){
                i += ctz(pc) + 1;
                pc >>= ctz(pc);
                pc >>= 1;
                BRmoves(i, ref Moves);
            }

            pc = bq;
            i = -1;
            while(pc!=0){
                i += ctz(pc) + 1;
                pc >>= ctz(pc);
                pc >>= 1;
                BQmoves(i, ref Moves);
            }
        }
        return Moves;
    }
    public List<Move> LegalMoves(){
        List<Move> PseudoMoves = PseudoLegalMoves();
        List<Move> Moves = new List<Move>();

        Chess TempBoard = new Chess();
        foreach (Move mv in PseudoMoves) {
            TempBoard.wp = wp; TempBoard.wr = wr; TempBoard.wn = wn; TempBoard.wb = wb; TempBoard.wq = wq; TempBoard.wk = wk;
            TempBoard.bp = bp; TempBoard.br = br; TempBoard.bn = bn; TempBoard.bb = bb; TempBoard.bq = bq; TempBoard.bk = bk;
            TempBoard.WCastleKing = WCastleKing; TempBoard.WCastleQueen = WCastleQueen;
            TempBoard.BCastleKing = BCastleKing; TempBoard.BCastleQueen = BCastleQueen;
            TempBoard.turn = turn;
            TempBoard.move_piece(mv);
            TempBoard.turn = turn;

            if (!TempBoard.IsCheck()) Moves.Add(mv);
        }
        if(IsCheck()) return Moves;
        if (turn){
            if(WCastleKing && ((pieces & (1UL << 1 | 1UL << 2)) == 0) && !IsSquareAttacked(2,false) && !IsSquareAttacked(1,false)) Moves.Add(1 << 6 | 3);
            if(WCastleQueen && ((pieces & (1UL << 4 | 1UL << 5 | 1UL << 6)) == 0) && !IsSquareAttacked(4,false) && !IsSquareAttacked(5,false)) Moves.Add(5 << 6 | 3);
        }
        else{
            if(BCastleKing && ((pieces & (1UL << 57 | 1UL << 58)) == 0) && !IsSquareAttacked(57,true) && !IsSquareAttacked(58,true)) Moves.Add(57 << 6 | 59);
            if(BCastleQueen && ((pieces & (1UL << 60 | 1UL << 61 | 1UL << 62)) == 0) && !IsSquareAttacked(60,true) && !IsSquareAttacked(61,true)) Moves.Add(61 << 6 | 59);
        }

        return Moves;
    }
    public bool IsCheck(){
        Bitboard king = turn ? wk : bk;

        turn = !turn;
        foreach (Move mv in PseudoLegalMoves()) {
            if ((1UL << ((mv >> 6) & 63)) == king) {
                turn = !turn;
                return true;
            }
        }
        turn = !turn;
        return false;
    }
    public bool IsSquareAttacked(int square, bool by){
        Bitboard sqr = 1UL << square;
        bool OriginalTurn = turn;

        turn = by;
        foreach (Move mv in PseudoLegalMoves()) {
            if ((1UL << ((mv >> 6) & 63)) == sqr) {
                turn = OriginalTurn;
                return true;
            }
        }
        turn = OriginalTurn;
        return false;
    }
    public bool IsCheckmate(){
        if (IsCheck() && LegalMoves().Count == 0) return true;
        return false;
    }
    public bool IsStalemate(){
        if (!IsCheck() && LegalMoves().Count == 0) return true;
        return false;
    }
    public bool IsInsufficientMaterial(){
        if (wp != 0 || bp != 0 || wr != 0 || br != 0 || wq != 0 || bq != 0) return false;
        if(popcnt(wn)+popcnt(bn) == 0 && popcnt(wb)+popcnt(bb) == 0) return true; // King v king
        if(popcnt(wn)+popcnt(bn) == 1 && popcnt(wb)+popcnt(bb) == 0) return true; // King and knight v king
        if(popcnt(wn)+popcnt(bn) == 0 && popcnt(wb)+popcnt(bb) == 1) return true; // King and bishop v king
        if(popcnt(wn)+popcnt(bn) == 2 && popcnt(wb)+popcnt(bb) == 0 && (popcnt(wn)==2 || popcnt(bn)==2)) return true; // King, 2 knight v king
        if(popcnt(wn)+popcnt(bn) == 0 && popcnt(wb)+popcnt(bb) == 2){
            Bitboard bishops = wb | bb;
            Bitboard AltSquares = 6172840429334713770UL;

            bool hasLight = (bishops & AltSquares) != 0;
            bool hasDark = (bishops & ~AltSquares) != 0;

            if (!(hasLight && hasDark)) return true; // If they are only on one color, it is insufficient.
        }
        return false;
    }
    public bool IsGameOver(){
        if (IsCheckmate() || IsStalemate() || IsInsufficientMaterial()) return true;
        return false;
    }
}