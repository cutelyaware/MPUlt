using System;
using System.IO;

namespace _3dedit {
    /*
    internal class ImportMC4D {

        internal int Size;
        internal int LSeq;
        internal int LShuffle;
        internal short[] Seq;

        static short[] scvt=new short[]{
            31,23,0, 23,31,0, 23,13,0, 13,23,0, 32,31,0, 31,32,0, 13,32,0, 32,13,0, 
            13,21,32, 12,31,23, 12,13,32, 13,12,23, 21,32,13, 21,23,31, 12,23,13, 12,32,31, 31,12,32, 21,13,23, 21,31,32, 31,21,23,
            23,0,0, 31,0,0, 12,0,0, 21,0,0, 13,0,0, 32,0,0, 0,0,0
        };
        static int[,] fcvt=new int[,] { { 0,1,2,3 },{ 1,0,2,3 },{ 2,0,1,3 },{ 3,0,1,2 },{ 3,0,1,2 },{ 2,0,1,3 },{ 1,0,2,3 },{ 0,1,2,3 } };
        static bool[] revdir=new bool[] { false,true,false,true,false,true,false,true };


        internal bool ReadLog(StreamReader sw) {
            try {
                string line;
                line=sw.ReadLine();
                string[] ss;
                ss=line.Split(' ','\t');

                if(ss[0]!="MagicCube4D" || ss[4]!="{4,3,3}") return false;
                int ls=int.Parse(ss[3]);
                Size=int.Parse(ss[5]);
                if(Size<3 || Size>5) return false;
                Seq=new short[2*Math.Max(ls,100)];

                while(line!="*" && line!=null) line=sw.ReadLine();
                if(line==null) return false;

                int p=0;
                for(;;) {
                    line=sw.ReadLine();
                    if(line==null) break;
                    bool qlast=false;
                    int ii=line.IndexOf('.');
                    if(ii>=0) {
                        line=line.Substring(0,ii);
                        qlast=true;
                    }
                    ss=line.Split(' ');
                    foreach(string cc in ss) {
                        if(cc=="m|") {
                            LShuffle=p; continue;
                        } else if(cc=="m[") AddSeq(-2,ref p);
                        else if(cc=="m]") AddSeq(-1,ref p);
                        else {
                            string[] ccc=cc.Split(',');
                            int stik=int.Parse(ccc[0]);
                            bool dir=(ccc[1]=="1");
                            int mask=int.Parse(ccc[2]);
                            int ncub=stik/27,sstik=stik%27;
                            if(revdir[ncub]) dir=!dir;
                            if(ncub==0 || ncub==4 || ncub==5 || ncub==6) {
                                int mk=0;
                                for(int i=0;i<Size;i++) {
                                    mk=mk*2+(mask&1);
                                    mask>>=1;
                                }
                                mask=mk;
                            }
                            if(dir) {
                                for(int u=0;u<3;u++) {
                                    int mm=scvt[3*sstik+u];
                                    if(mm!=0) {
                                        int d0=fcvt[ncub,0];
                                        int d1=fcvt[ncub,mm/10];
                                        int d2=fcvt[ncub,mm%10];
                                        if(d1==0 || d2==0) { int dx=d1; d1=d2; d2=dx; }
                                        AddSeq(mask+((d0*16+d1*4+d2)<<Size),ref p);
                                    }
                                }
                            } else {
                                for(int u=3;--u>=0;) {
                                    int mm=scvt[3*sstik+u];
                                    if(mm!=0) {
                                        int d0=fcvt[ncub,0];
                                        int d1=fcvt[ncub,mm/10];
                                        int d2=fcvt[ncub,mm%10];
                                        if(d1==0 || d2==0) { int dx=d1; d1=d2; d2=dx; }
                                        AddSeq(mask+((d0*16+d2*4+d1)<<Size),ref p);
                                    }
                                }
                            }
                        }
                    }
                    if(qlast) break;
                }
                LSeq=p;
            } catch {
                return false;
            }
            return true;
        }

        private void AddSeq(int v,ref int p) {
            if(p==Seq.Length) {
                short[] sq=new short[2*p];
                for(int i=0;i<p;i++) sq[i]=Seq[i];
                Seq=sq;
            }
            Seq[p++]=(short)v;
        }
    }
         * */
}
