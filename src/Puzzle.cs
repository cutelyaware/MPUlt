using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace _3dedit {
    class Puzzle {
        internal PuzzleStructure Str;
        internal int NStk;
        internal short[] Field,Fld2;
        internal const short CMask=0x3fff,HMask=0x4000;
        internal long[] Seq;    // ulong=axis:twist:angle:mask
        internal int LShuffle,LSeq,Ptr;
        internal int NTwists;
        int[,] SetupStack;
        int LStk;
        internal long CTime;
        const int START_MACRO=-2,STOP_MACRO=-1;

        internal static Puzzle LoadFromLog(string fn) {
            Puzzle res=new Puzzle();
            try {
                res.Load(fn);
            } catch(Exception e) {
//                System.Windows.Forms.MessageBox.Show("Cannot load file "+fn+": "+e.Message);
                res=new Puzzle(PuzzleStructure.Example);
            }
            return res;
        }

        Puzzle() { }

        internal Puzzle(PuzzleStructure str) {
            Str=str;
            NStk=Str.NStickers;
            Field=new short[NStk];
            Fld2=new short[NStk];
            Seq=new long[100000];
            LShuffle=LSeq=Ptr=0;
            NTwists=0;
            SetupStack=new int[100,2];
            LStk=0;
            Reset();
        }

        internal void Reset() {
            Str.SetColors(Field);
            LSeq=LShuffle=Ptr=0;
            NTwists=0;
        }

        internal bool Check() {
            return Str.Check(Field,CMask);
        }

        internal void SetTwist(int axis,int mask,Func<int,bool>F) {  // for animation
            PAxis A=Str.Axes[axis];
            mask=A.Base.Remask(mask);  // insert or clear zero layer if necessary
            int k=0;
            while(mask!=0) {
                if((mask&1)!=0) {
                    foreach(int s in A.Layers[k]) F(s);
                }
                mask>>=1; k++;
            }
        }

        internal void GetTwistGeom(int axis,int twist,int angle,out double[] tvec,out double rangle) {
            double[] v=Str.Axes[axis].Twists[twist];
            int h=Str.Axes[axis].Base.Twists[twist].Order;
            double a0=Math.PI/h;
            double c0=Math.Cos(a0),s0=Math.Sin(a0);
            int d=Str.Dim;
            double a=0,b=0;
            for(int i=0;i<d;i++) {
                a+=v[i]*v[i];
                b+=v[i+d]*v[i+d];
            }
            a=Math.Sqrt(a); b=Math.Sqrt(b);
            double[]v1=new double[2*d];
            for(int i=0;i<d;i++) {
                v1[i]=v[i]/a;
                v1[i+d]=(v[i+d]/b-v1[i]*c0)/s0;
            }
            tvec=v1;
            rangle=2*a0*angle;
        }

        internal void Twist(int axis,int twist,int angle,int mask) {
            mask=Str.Axes[axis].Base.Remask(mask);
            TwistNormMask(axis,twist,angle,mask);
        }

        void TwistNormMask(int axis,int twist,int angle,int mask) {
            angle=Str.Axes[axis].Base.Twists[twist].ReAngle(angle);
            long code=((long)((mask<<16)+angle)<<32)+((twist<<16)+axis);
            WriteCode(code);
            DoTwist(axis,twist,angle,mask);
        }

        private void WriteCode(long code) {
            CheckSeq();
            Seq[Ptr++]=code;
            if(code>=0) NTwists++;
            LSeq=Ptr;
        }

        private unsafe void CheckSeq() {
            if(Seq.Length==Ptr) {
                long[] s=new long[2*Ptr];
                Buffer.BlockCopy(Seq,0,s,0,Ptr*sizeof(long));
                Seq=s;
            }
        }
        private void TwistByCode(long code,int sgn,bool write) {
            int axis,twist,angle,mask;
            UnpackCode(code,out axis,out twist,out angle,out mask);
            if(write) TwistNormMask(axis,twist,angle*sgn,mask);
            else DoTwist(axis,twist,angle*sgn,mask);
        }

        private void UnpackCode(long code,out int axis,out int twist,out int angle,out int mask) {
            axis=(short)code; code>>=16;
            twist=(short)code; code>>=16;
            angle=(short)code; code>>=16;
            mask=(short)code;
        }


        private unsafe void DoTwist(int axis,int twist,int angle,int mask) {
            Buffer.BlockCopy(Field,0,Fld2,0,NStk*sizeof(short));
            PAxis A=Str.Axes[axis];
            angle=A.Base.Twists[twist].NormAngle(angle);  // symmetric class of remainders
            if(angle==0) return;
            int a=Math.Abs(angle);
            int k=0;
            while(mask!=0) {
                if((mask&1)!=0) {
                    int[] stks=A.Layers[k];
                    int[] map=angle<0 ? A.Base.Twists[twist].InvMap[k] : A.Base.Twists[twist].Map[k];
                    int lm=map.Length;
                    if(a==1) {
                        for(int i=0;i<lm;i++) Field[stks[map[i]]]=Fld2[stks[i]];
                    } else {
                        for(int i=0;i<lm;i++) {
                            int x=i;
                            for(int j=0;j<a;j++) x=map[x];
                            Field[stks[x]]=Fld2[stks[i]];
                        }
                    }
                }
                mask>>=1; k++;
            }
        }

        internal bool Undo() {
            if(Ptr==LShuffle) return false;
            int cm=0;
            do{
                if(Ptr==LShuffle) break;
                Ptr--;
                if(Seq[Ptr]==STOP_MACRO) cm++;
                else if(Seq[Ptr]==START_MACRO && cm>0) cm--;
                else {
                    TwistByCode(Seq[Ptr],-1,false);
                    NTwists--;
                }
            }while(cm>0);
            return true;
        }

        internal bool Redo() {
            if(Ptr==LSeq) return false;
            int cm=0;
            do {
                if(Ptr==LSeq) break;
                if(Seq[Ptr]==START_MACRO) cm++;
                else if(Seq[Ptr]==STOP_MACRO && cm>0) cm--;
                else {
                    TwistByCode(Seq[Ptr],1,false);
                    NTwists++;
                }
                Ptr++;
            } while(cm>0);
            return true;
        }

        internal bool GetUndo(out int ax,out int tw,out int ang,out int mask) {
            ax=tw=ang=mask=0;
            if(Ptr==LShuffle) return false;
            long code=Seq[Ptr-1];
            if(code<0) return false;
            UnpackCode(code,out ax,out tw,out ang,out mask);
            ang=Str.Axes[ax].Base.Twists[tw].NormAngle(-ang);
            return true;
        }

        internal bool GetRedo(out int ax,out int tw,out int ang,out int mask) {
            ax=tw=ang=mask=0;
            if(Ptr==LSeq) return false;
            long code=Seq[Ptr];
            if(code<0) return false;
            UnpackCode(code,out ax,out tw,out ang,out mask);
            ang=Str.Axes[ax].Base.Twists[tw].NormAngle(ang);
            return true;
        }


        internal void StartSetup() {
            SetupStack[LStk,0]=Ptr;
            SetupStack[LStk,1]=-1;
            LStk++;
        }

        internal void StopSetup() {
            if(LStk==0) return;
            if(SetupStack[LStk-1,1]>=0) { LStk--; return; }
            SetupStack[LStk-1,1]=Ptr;
        }

        internal void Conjugate() {
            if(LStk==0) return;
            LStk--;
            int start=SetupStack[LStk,0],stop=SetupStack[LStk,1];
            if(stop<0) return;
            if(Ptr<stop) stop=Ptr;
            WriteCode(START_MACRO);
            MakeInvCode(start,stop);
            WriteCode(STOP_MACRO);
        }

        internal void Commutator() {
            if(LStk==0) return;
            LStk--;
            int start=SetupStack[LStk,0],stop=SetupStack[LStk,1];
            if(stop<0) return;
            if(Ptr<stop) stop=Ptr;
            int end=Ptr;
            WriteCode(START_MACRO);
            MakeInvCode(start,stop);
            MakeInvCode(stop,end);
            WriteCode(STOP_MACRO);
        }

        private void MakeInvCode(int start,int stop) {
            for(int i=stop;--i>=start;) {
                long code=Seq[i];
                if(code<0) continue;
                TwistByCode(code,-1,true);
            }
        }

        internal void RepeatCode(int start,int stop) {
            WriteCode(START_MACRO);
            for(int i=start;i<stop;i++) {
                long code=Seq[i];
                if(code<0) continue;
                TwistByCode(code,1,true);
            }
            WriteCode(STOP_MACRO);
        }

        internal string GetRevStack() {
            int p=LShuffle;
            int q=Ptr;
            string l="";
            for(int i=0;i<LStk;i++) {
                int r=SetupStack[i,0];
                if(r!=p) l+=GetNTwists(p,r).ToString()+"[";
                else l+="[";
                p=r;
                r=SetupStack[i,1];
                if(r>=0) {
                    l+=GetNTwists(p,r).ToString()+"]";
                    p=r;
                }
            }
            if(p<q) l+=GetNTwists(p,q).ToString();
            return l;
        }

        internal int GetNTwists(int p,int q) {
            if(p==q) return 0;
            if(NTwists!=0 && p==0) return NTwists-GetNTwists(q,Ptr);
            int s=0;
            for(int i=p;i<q;i++) if(Seq[i]>=0) s++;
            return s;
        }
        int[] AxisMap;
        int[][] TwistMap;

        internal void ApplyMacro(long[] code,int lcode,double[,] matr,bool qrev) {
            if(AxisMap==null) AxisMap=new int[Str.Axes.Length];
            if(TwistMap==null) TwistMap=new int[Str.Axes.Length][];
            WriteCode(START_MACRO);
            if(qrev){
                for(int i=lcode;--i>=0;){
                    MakeMacroStep(code[i],matr,true);
                }
            }else{
                for(int i=0;i<lcode;i++){
                    MakeMacroStep(code[i],matr,false);
                }
            }
            WriteCode(STOP_MACRO);

            for(int i=0;i<AxisMap.Length;i++) if(AxisMap[i]!=0) {
                    AxisMap[i]=0;
                    if(TwistMap[i]!=null) {
                        for(int j=0;j<TwistMap[i].Length;j++) TwistMap[i][j]=0;
                    }
                }
        }

        void MakeMacroStep(long code,double[,] matr,bool qrev) {
            int ax,tw,an,ms;
            UnpackCode(code,out ax,out tw,out an,out ms);
            PAxis A=Str.Axes[ax];
            int ax1=AxisMap[ax];
            if(ax1==0) {
                AxisMap[ax]=ax1=Str.FindAxis(A.Dir,matr);
            }
            if(ax1<0) {
                ms=A.Base.ReverseMask(ms);
                ax1=-ax1;
            }
            ax1--;
            PAxis A1=Str.Axes[ax1];
            if(TwistMap[ax]==null) TwistMap[ax]=new int[A1.Twists.Length];
            int tw1=TwistMap[ax][tw];
            if(tw1==0) {
                bool qrev1;
                tw1=A1.FindTwist(A.Twists[tw],matr,out qrev1);
                tw1=qrev1 ? -1-tw1 : 1+tw1;
                TwistMap[ax][tw]=tw1;
            }
            if(tw1<0) {
                qrev=!qrev;
                tw1=-tw1;
            }
            tw1--;
            if(qrev) an=-an;
            TwistNormMask(ax1,tw1,an,ms);
        }

        ulong CSum;
        void AddCSum(long m) {
            CSum=CSum*0x12345675+(ulong)m;
        }
        ulong RevBit(ulong x) {
            ulong y=0;
            for(int i=0;i<64;i++) {
                y=(y<<1)+(x&1);
                x>>=1;
            }
            return y;
        }

        internal void Save(string fn) {
            try {
                StreamWriter sw=new StreamWriter(fn);
                sw.NewLine="\r\n";
                sw.WriteLine("MPUltimate v1 {0} {1} {2} {3}",Str.Name,LSeq,LShuffle,Ptr);
                string[] descr=Str.GetDescription();
                sw.WriteLine("Puzzle {0}",Str.Name);
                foreach(string s in descr) sw.WriteLine(s);
                sw.WriteLine("EndPuzzle");

                // application-specific code
                int nf=Str.Faces.Length;
                int nstk=Str.NStickers;
                int lb=nf<36 ? 1 : nf<1296 ? 2 : 3;
                int ll=256/lb;

                sw.WriteLine("{0} {1}",lb,nstk);
                char []str=new char[lb*ll];
                for(int i=0;i<nstk;i+=ll) {
                    int ll1=Math.Min(ll,nstk-i);
                    for(int j=0;j<ll1;j++) SetBytes36(str,j*lb,lb,Field[i+j]&CMask);
                    sw.WriteLine(new string(str,0,ll1*lb));
                }
                long T=CTime/10000;
                sw.WriteLine("#timer {0}",T);

                CSum=0;
                AddCSum(nstk); AddCSum(LSeq); AddCSum(LShuffle); AddCSum(Ptr);
                AddCSum(T);
                for(int i=0;i<LSeq;i++) AddCSum(Seq[i]);
                for(int i=0;i<nstk;i++) AddCSum(Field[i]&CMask);
                sw.WriteLine("#CRC {0}",RevBit(CSum));
                sw.WriteLine("*");

                for(int i=0;i<LSeq;i+=16) {
                    int l=Math.Min(16,LSeq-i);
                    for(int j=0;j<l;j++) {
                        if(i+j==LShuffle) sw.Write("m| ");
                        long d=Seq[i+j];
                        if(d==START_MACRO) sw.Write("m[ ");
                        else if(d==STOP_MACRO) sw.Write("m] ");
                        else {
                            int ax,tw,an,ms;
                            UnpackCode(d,out ax,out tw,out an,out ms);
                            sw.Write("{0}:{1}:{2}:{3} ",ax,tw,an,ms);
                        }
                    }
                    if(i+l==LSeq) sw.Write(".");
                    sw.WriteLine();
                }
                sw.Close();
            } catch {
                //System.Windows.Forms.MessageBox.Show("Cannot save file "+fn);
                return;
            }
        }

        private void SetBytes36(char []str,int ptr,int lb,int val) {
            for(int i=0;i<lb;i++) {
                int v=val%36; val/=36;
                if(v<10) str[ptr++]=(char)('0'+v);
                else str[ptr++]=(char)('A'+(v-10));
            }
        }

        internal void Load(string fn) {
            StreamReader sr=null;
            try {
                sr=new StreamReader(fn);
                string[] head=sr.ReadLine().Split(' ','\t');
                bool MyProg=head[0]=="MPUltimate";
                bool needGen=true;
                ulong crc=0;
                long T=0;
                string Name;
                if(MyProg) {
                    Name=head[2];
                    LSeq=int.Parse(head[3]);
                    LShuffle=int.Parse(head[4]);
                    Ptr=int.Parse(head[5]);
                    if(File.Exists(Name+".pzl")) {
                        try {
                            Str=PuzzleStructure.ReadCompiled(Name+".pzl");
                            needGen=false;
                        } catch { }
                    }
                }
                string[] ss=sr.ReadLine().Split(' ','\t');
                if(ss[0]!="Puzzle") throw new Exception("Missing 'Puzzle' tag");
                ArrayList dsc=new ArrayList();
                for(;;) {
                    string s0=sr.ReadLine();
                    if(s0==null) throw new Exception("Missing 'EndPuzzle' tag");
                    if(s0=="EndPuzzle") break;
                    dsc.Add(s0);
                }
                if(needGen) {
                    Str=PuzzleStructure.Create(ss[1],(string[])dsc.ToArray(typeof(string)));
                    Str.SaveCompiled(ss[1]+".pzl");
                }
                if(MyProg) {
                    ss=sr.ReadLine().Split(' ','\t');
                    int nstk=int.Parse(ss[1]),lb=int.Parse(ss[0]);
                    Field=new short[nstk];
                    Fld2=new short[nstk];
                    for(int k=0;k<nstk;) {
                        string s1=sr.ReadLine();
                        int nb=s1.Length/lb;
                        for(int i=0;i<nb;i++) Field[k++]=(short)GetBytes36(s1,i*lb,lb);
                    }
                    for(;;) {
                        string s2=sr.ReadLine();
                        if(s2=="*") break;
                        string[] ss2=s2.Split(' ');
                        if(ss2[0]=="#timer") T=long.Parse(ss2[1]);
                        if(ss2[0]=="#CRC") crc=ulong.Parse(ss2[1]);
                    }
                    NStk=nstk;
                } else {
                    for(;;) {
                        string s2=sr.ReadLine();
                        if(s2=="*" || s2==null) break;
                    }
                }
                Seq=new long[100000];
                int p=0,ps=0;
                for(;;) {
                    string s3=sr.ReadLine();
                    if(s3==null) break;
                    bool qlast=s3.EndsWith(".");
                    if(qlast) s3=s3.Substring(0,s3.Length-1);
                    string[] ss3=s3.Split(' ','\t');
                    foreach(string s4 in ss3) {
                        if(p==Seq.Length) {
                            long[] sq=new long[2*p];
                            Buffer.BlockCopy(Seq,0,sq,0,p*sizeof(long));
                            Seq=sq;
                        }
                        if(s4==null || s4=="") continue;
                        else if(s4=="m[") Seq[p++]=START_MACRO;
                        else if(s4=="m]") Seq[p++]=STOP_MACRO;
                        else if(s4=="m|") ps=p;
                        else {
                            string[] s5=s4.Split(':');
                            int ax=int.Parse(s5[0]);
                            int tw=int.Parse(s5[1]);
                            int an=int.Parse(s5[2]);
                            int ms=int.Parse(s5[3]);
                            an=Str.Axes[ax].Base.Twists[tw].ReAngle(an);  // -1 => 3
                            Seq[p++]=((long)((ms<<16)+an)<<32)+((tw<<16)+ax);
                        }
                    }
                }
                if(!MyProg) {
                    LSeq=Ptr=p; LShuffle=ps;
                    NStk=Str.NStickers;
                    Field=new short[NStk];
                    Fld2=new short[NStk];
                    Recalculate();
                } else {
#if false
                    if(crc!=1234567890123456789) {
                        CSum=0;
                        AddCSum(Str.NStickers); AddCSum(LSeq); AddCSum(LShuffle); AddCSum(Ptr);
                        AddCSum(T);
                        for(int i=0;i<LSeq;i++) AddCSum(Seq[i]);
                        for(int i=0;i<NStk;i++) AddCSum(Field[i]);
                        if(RevBit(CSum)!=crc) throw new Exception("Wrong CRC");
                    }
#endif
                    CTime=T*10000;
                }
                NTwists=0;
                NTwists=GetNTwists(0,Ptr);
                SetupStack=new int[100,2];
                LStk=0;
            } catch(Exception e) {
                if(sr!=null) sr.Close();
                throw e;
            }
            sr.Close();
        }

        private int GetBytes36(string str,int ptr,int lb) {
            int v=0;
            while(--lb>=0) {
                char c=str[ptr+lb];
                v*=36;
                if(c<='9') v+=(c-'0');
                else v+=(c-'A')+10;
            }
            return v;
        }

        internal void Recalculate() {
            Str.SetColors(Field);
            for(int i=0;i<Ptr;i++) {
                if(Seq[i]>=0) TwistByCode(Seq[i],1,false);
            }
        }

        internal void Scramble(int nt) {
            Reset();
            if(nt<0) nt=40*Str.Axes.Length;

            uint seed=(uint)(DateTime.Now.Ticks/10000000);
            int cc=0;
            while(cc<nt) {
                seed=(seed*0x1010005+1);
                long s=seed;

                s*=Str.Axes.Length;
                int ax=(int)(s>>32);
                s=(uint)s;
                s*=Str.Axes[ax].Twists.Length;
                int tw=(int)(s>>32);
                s=(uint)s;
                s*=(Str.Axes[ax].Base.Twists[tw].Order-1);
                int an=(int)(s>>32)+1;
                s=(uint)s;
                s<<=Str.Axes[ax].Base.NLayers;
                int ms=(int)(s>>32);
                ms=Str.Axes[ax].Base.Remask(ms);
                if(ms==0) continue;
                Twist(ax,tw,an,ms);
                cc++;
            }
            LShuffle=LSeq=Ptr;
        }

        internal void SaveField() {
            for(int i=0;i<NStk;i++) Fld2[i]=Field[i];
        }
        internal void RestoreField() {
            for(int i=0;i<NStk;i++) Field[i]=Fld2[i];
        }



        internal void Optimize() {
            const long LMASK=~0xffff00000000;
            for(;;) {
                bool qc=false;
                int p=LShuffle,q=LShuffle;
                while(p<Ptr) {
                    if(p==Ptr-1 || Seq[p]<0 || Seq[p+1]<0) {
                        Seq[q++]=Seq[p++];
                        continue;
                    }
                    long atw=Seq[p]&LMASK;
                    if(atw!=(Seq[p+1]&LMASK)) {
                        Seq[q++]=Seq[p++];
                        continue;
                    }
                    int ang=(int)(Seq[p]>>32)&0xffff;
                    int r1=p+1;
                    while(r1<Ptr) {
                        if((Seq[r1]&LMASK)!=atw) break;
                        ang+=(int)(Seq[r1]>>32)&0xffff;
                        r1++;
                    }
                    int tw=((int)atw>>16)&0xffff;
                    ang%=Str.Axes[(int)atw&0xffff].Base.Twists[tw].Order;
                    if(ang==0) qc=true;
                    else Seq[q++]=atw+((long)ang<<32);
                    p=r1;
                }
                Ptr=q;
                if(!qc) break;
            }
            LSeq=Ptr;
            NTwists=0;
            for(int i=0;i<Ptr;i++) if(Seq[i]>=0) NTwists++;
        }

        internal void ClearSelected() {
            for(int i=0;i<NStk;i++) Field[i]&=0x7fff;
        }
    }
}
